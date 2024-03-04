using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using AnotherFileBrowser.Windows;
using UnityEngine.Networking;
using System.IO;
using TMPro;
using Newtonsoft.Json;

public class MainGameManager : MonoBehaviour
{
    public static event Action OnStartGame;
    public static event Action OnNextWindow;
    public static Action<int> OnNextIteration = delegate { };

    public List<List<GameObject>> ListOfHitList = new();
    public List<List<GameObject>> ListOfNoHitList = new();
    public List<Culprit> CulpritsDone = new();

    [Header("SimulationUI")]
    public GameObject PreSimUI;
    public GameObject SimulationUI, PostSimUI;
   
    CulpritsManager culpritsManager;
    public PostUIManager postUIManager;
    public CameraManager cameraManager;

    [Header("Importing")]
    public TMP_Text ImportingFeedback;
    [TextArea(5, 5)]
    public string loadedJSONFromFile = "";
    public GameObject ImportingCloseButton, importingFeedbackGO;

    [Header("Screenshotting")]
    public GameObject MetadataTextGO;
    public ScenarioBuilder scenarioBuilder;

    public TMP_Text MD_Title, MD_Date, MD_Time, MD_TargetFloors, MD_CulpritFloors, MD_Distance;

    enum SimulationState
    {
        PRESIMULATE,
        SIMULATE,
        POSTSIMULATE
    }

    SimulationState simState = SimulationState.PRESIMULATE;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        Culprit.OnCantHit += HandleCantHit;
        Culprit.OnHit += HandleHit;
        Culprit.OnDone += HandleDone;

        SceneManager.sceneLoaded += OnSceneLoaded;

        culpritsManager = FindObjectOfType<CulpritsManager>();
    }

    private void OnDestroy()
    {
        Culprit.OnCantHit -= HandleCantHit;
        Culprit.OnHit -= HandleHit;
        Culprit.OnDone -= HandleDone;

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Main Menu")
        {
            Destroy(gameObject);
        }
    }
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void StartSimulation()
    {
        for (int i = 0; i < WindowsManager.Instance.SelectedWindows.Count; i++)
        {
            ListOfHitList.Add(new List<GameObject>());
            ListOfNoHitList.Add(new List<GameObject>());
        }
        simState = SimulationState.SIMULATE;
        OnStartGame?.Invoke();
        WindowsManager.Instance.canSelectWindow = false;

        Time.fixedDeltaTime = 1 / DontDestroyOnLoadSettings.Instance.timeStepAmt;
        Time.timeScale = DontDestroyOnLoadSettings.Instance.SimulationSpeedValue;

        Debug.Log("Starting SImulation");
    }

    IEnumerator TakeScreenshot()
    {
        // Check if the "P" key is pressed
            MetadataTextGO.SetActive(true);

            Scenario S = DontDestroyOnLoadSettings.Instance.LoadedBuilding;

            // Get the scenario name from your DontDestroyOnLoadSettings.Instance.LoadedBuilding
            string scenarioName = S.NameOfScenario;

            // Get the current date in YYYY_MM_DD format
            string currentDate = System.DateTime.Now.ToString("yyyy_MM_dd");
        string currentTime = System.DateTime.Now.ToString("HH-mm-ss");
        // Combine the scenario name, date, and a custom prefix for the screenshot file name
        string screenshotFileName = "SS_" + scenarioName + " " + currentDate + " " + currentTime + ".png";


            MD_Title.text = screenshotFileName + " data";
            MD_Date.text = "Date: " + currentDate;
          
            MD_Time.text = "Time: " + currentTime;
            MD_TargetFloors.text = "Target building floors: " + scenarioBuilder.ParsedBuildingDatablock[0].SavedNumFloors.ToString();
            MD_CulpritFloors.text = "Culprit building floors: " + scenarioBuilder.ParsedBuildingDatablock[1].SavedNumFloors.ToString();
            MD_Distance.text = "Distance: " + S.DistanceBetweenBuildings.ToString() + "m";
            // Get the desktop path
            string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);

            // Combine the desktop path with the screenshot file name
            string fullScreenshotPath = Path.Combine(desktopPath, screenshotFileName);

            // Capture and save the screenshot with the generated file name to the desktop
            ScreenCapture.CaptureScreenshot(fullScreenshotPath);
            yield return null;
            // Optionally, you can log the full file path or display a message
            Debug.Log($"Screenshot captured and saved to desktop: {fullScreenshotPath}");
            MetadataTextGO.SetActive(false);
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(TakeScreenshot());
        }
    }

    public void HandleCantHit(GameObject culprit, int target)
    {
        ListOfNoHitList[target].Add(culprit);
        StartNextWindow(target);
    }

    public void HandleHit(GameObject ball, int target)
    {
        ListOfHitList[target].Add(ball);
        StartNextWindow(target);
    }

    public void StartNextWindow(int target)
    {
        if (ListOfNoHitList[target].Count + ListOfHitList[target].Count == culpritsManager.SpawnedCulprits.Count)
        {
            OnNextWindow?.Invoke();
        }
    }

    public void HandleDone(Culprit culprit)
    {
        CulpritsDone.Add(culprit);

        // check if all done
        if(CulpritsDone.Count == culpritsManager.SpawnedCulprits.Count)
        { 
            SortCulpritsList();

            // post UI
            EnablePostUI();
        }
    }

    public void SortCulpritsList()
    {
        // sort by highest probability
        List<Culprit> sortedList = CulpritsDone.OrderByDescending(go => go.probability).ToList();
        CulpritsDone = sortedList;
    }

    public void EnablePostUI()
    {
        SimulationUI.SetActive(false);
        PostSimUI.SetActive(true);
        cameraManager.OnEndSimulation();
        postUIManager.OnEndSimulation();
    }


    // Redundant, but idw delete JIC we have use one day.
    public void ImportCulpritData()
    {
        var bp = new BrowserProperties();
        bp.filterIndex = 0;
        new FileBrowser().OpenFileBrowser(bp, path =>
        {
            StartCoroutine(LoadJSONSave(path));
        });

        IEnumerator LoadJSONSave(string path)
        {
            importingFeedbackGO.SetActive(true);
            using (UnityWebRequest uwr = UnityWebRequest.Get(path))
            {
                yield return uwr.SendWebRequest();
                if (uwr.isNetworkError || uwr.isHttpError)
                {
                    ImportingFeedback.text = "An error has occured: " + uwr.error;
                    ImportingCloseButton.SetActive(true);
                }
                else
                {
                    loadedJSONFromFile = uwr.downloadHandler.text;
                    Debug.Log(loadedJSONFromFile);



                    try
                    {


                        Scenario S = new Scenario();
                        S = JsonConvert.DeserializeObject<Scenario>(loadedJSONFromFile);

                        ImportingFeedback.text = "Scenario imported successfully!";
                        ImportingCloseButton.SetActive(true);
                    }
                    catch (Exception e)
                    {
                        ImportingFeedback.text = "Scenario import error: " + e.Message;
                        ImportingCloseButton.SetActive(true);
                    }
                }
            }
        }
    }
}
