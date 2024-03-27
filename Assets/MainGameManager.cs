using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;

#if !UNITY_WEBGL
using AnotherFileBrowser.Windows;
#endif
using UnityEngine.Networking;
using System.IO;
using TMPro;
using Newtonsoft.Json;
using UnityEngine.UI;

public class MainGameManager : MonoBehaviour
{
    public static Action<int, int> OnStartGame = delegate { };
    public static event Action OnNextWindow;
    public static Action<int> OnNextIteration = delegate { };

    public List<HitList> ListOfHitList = new();
    public List<HitList> ListOfNoHitList = new();
    public List<Culprit> CulpritsDone = new();

    public List<List<float>> ListOfProbabilityList = new();

    [Header("SimulationUI")]
    public GameObject PreSimUI;
    public GameObject SimulationUI, PostSimUI;
   
    CulpritsManager culpritsManager;
    public PostUIManager postUIManager;
    public CameraManager cameraManager;

    [Header("Multiple Iterations")]
    DontDestroyOnLoadSettings settings;
    int startVel, maxVel;
    int increment;
    int currentIteration;
    int potentialIterations;


    [Header("Importing")]

    public TMP_Text ImportingFeedback;

    [TextArea(5, 5)]

    public string loadedJSONFromFile = "";

    public GameObject ImportingCloseButton, importingFeedbackGO;



    [Header("Screenshotting")]

    public GameObject MetadataTextGO;

    public ScenarioBuilder scenarioBuilder;

    [Header("EditorMode")]
    public GameObject EditorModeUI;

    [Header("ProgressBar")]
    public GameObject ProgressUI;
    public Slider Slider_Progress;
    public TMP_Text Text_Progress;
    public TMP_Text Text_WindowIter;
    public TMP_Text Text_CurrWindow;
    int currWindow = 0;

    public TMP_Text MD_Title, MD_Date, MD_Time, MD_TargetFloors, MD_CulpritFloors, MD_Distance;


    private void Start()
    {
        if(DontDestroyOnLoadSettings.Instance.isEditorMode)
        {
            EditorModeUI.SetActive(true);
        }
    }
    public enum SimulationState
    {
        PRESIMULATE,
        SIMULATE,
        POSTSIMULATE
    }

   public SimulationState simState = SimulationState.PRESIMULATE;

    public CameraManager camManager;

    void Awake()
    {
        Culprit.OnCantHit += HandleCantHit;
        Culprit.OnHit += HandleHit;
        Culprit.OnDone += HandleDone;

        SceneManager.sceneLoaded += OnSceneLoaded;

        culpritsManager = FindObjectOfType<CulpritsManager>();
        settings = FindObjectOfType<DontDestroyOnLoadSettings>();

        startVel = settings.minVelocity;
        maxVel = settings.maxVelocity;
        increment = settings.velocityIncrement;

        potentialIterations = (maxVel - startVel) / increment + 1;
        currentIteration = 0;

        //init the amount of probabilities to average
        for(int i = 0; i < potentialIterations; i++)
        {
            ListOfProbabilityList.Add(new());
        }

        currWindow = 0;
        Slider_Progress.value = 0;
        Text_Progress.text = Slider_Progress.value.ToString("F1") + "%";
        Text_WindowIter.text = (currentIteration + 1).ToString() + "/" + potentialIterations.ToString();
        ProgressUI.SetActive(false);
    }

    private void OnDestroy()
    {
        Culprit.OnCantHit -= HandleCantHit;
        Culprit.OnHit -= HandleHit;
        Culprit.OnDone -= HandleDone;

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    public void RefreshScene()
    {
        //int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        //// Load the current scene again
        //SceneManager.LoadScene(currentSceneIndex);

        //StartCoroutine(DontDestroyOnLoadSettings.Instance.LoadSettingsCoroutine());

        DontDestroyOnLoadSettings.Instance.StartGame();
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
        DontDestroyOnLoadSettings.Instance.ReturnToMainMenu();
    }

    public void Reiterate()
    {
        PostSimUI.SetActive(false);
   //     StartSimulation();
    }

    public void StartSimulation()
    {
        SimulationUI.SetActive(true);
        if (WindowsManager.Instance.PreciseWindows.Count <= 0)
        {
            PopupUIManager.Instance.ShowPopup("Error!", " Simulation cannot start. Please select at least one window.");
            return;
        }
        else if (WindowsManager.Instance.PreciseWindows.Count >= 4)
        {
            PopupUIManager.Instance.ShowPopup("Warning!", "(for the sake of the demo) Please dont select so many windows!");
            return;
        }
        PreSimUI.SetActive(false);
        ProgressUI.SetActive(true);
        Text_CurrWindow.text = "Window: " + 1.ToString() + "/" + WindowsManager.Instance.SelectedWindows.Count;
        for (int i = 0; i < WindowsManager.Instance.SelectedWindows.Count; i++)
        {
            ListOfHitList.Add(new HitList()
            {
                Hit = new()
            });
            ListOfNoHitList.Add(new HitList()
            {
                Hit = new()
            });
        }
        simState = SimulationState.SIMULATE;
        OnStartGame?.Invoke(startVel, currentIteration);
        WindowsManager.Instance.canSelectWindow = false;

        Time.fixedDeltaTime = 1 / DontDestroyOnLoadSettings.Instance.timeStepAmt;
        Time.timeScale = DontDestroyOnLoadSettings.Instance.SimulationSpeedValue;

        Debug.Log("Starting Simulation");
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
            MD_Distance.text = "Distance: " + S.DistanceBetweenBuildings.ToString("F1") + "m";
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

        if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R) && DontDestroyOnLoadSettings.Instance.isEditorMode)
        {
            DontDestroyOnLoadSettings.Instance.ReturnToMainMenu();
        }

        if(Input.GetKeyDown(KeyCode.F) && !WindowsManager.Instance.isPrecisionMode)
        {
            camManager.SwitchToFreecam(!camManager.freecamMode, false);
           // WindowsManager.Instance.canSelectWindow = !camManager.freecamMode;
        }
    }

    public void HandleCantHit(GameObject culprit, int target)
    {
        ListOfNoHitList[target].Hit.Add(culprit);
        StartNextWindow(target);
    }

    public void HandleHit(GameObject ball, int target)
    {
        ListOfHitList[target].Hit.Add(ball);
        HandleProgressBar(target);
        StartNextWindow(target);
    }
    public void HandleProgressBar(int target)
    {
        float done = ListOfHitList[target].Hit.Count / (float)(culpritsManager.SpawnedCulprits.Count - ListOfNoHitList[target].Hit.Count);
        Slider_Progress.value = done * 100;
        Text_Progress.text = Slider_Progress.value.ToString("F1") + "%";
    }
    public void StartNextWindow(int target)
    {
        if (ListOfNoHitList[target].Hit.Count + ListOfHitList[target].Hit.Count == culpritsManager.SpawnedCulprits.Count)
        {
            OnNextWindow?.Invoke();
            currWindow++;
            if (currWindow > WindowsManager.Instance.SelectedWindows.Count)
                currWindow = WindowsManager.Instance.SelectedWindows.Count - 1;
            Text_CurrWindow.text = "Window: " + (currWindow + 1).ToString() + "/" + WindowsManager.Instance.SelectedWindows.Count;
        }
    }

    public void HandleDone(Culprit culprit)
    {
        CulpritsDone.Add(culprit);

        // check if all done
        if (CulpritsDone.Count == culpritsManager.SpawnedCulprits.Count)
        {
            if(startVel != maxVel)
            {
                startVel = CalculateIterations(startVel);
                currentIteration++;
                Text_WindowIter.text = (currentIteration + 1).ToString() + "/" + potentialIterations.ToString();
                ResetListsForNextIteration();
                return;
            }
            else // end the simulation
            {
                //CalculateAvgProbability(ListOfProbabilityList);
                SortCulpritsList();

                // post UI
                EnablePostUI();
            }
        }
    }

    public void HandleEndSimulation()
    {
        for(int i = 0; i < culpritsManager.SpawnedCulprits.Count; i++)
        {
            culpritsManager.SpawnedCulprits[i].done = true;
            culpritsManager.SpawnedCulprits[i].CalculateProbability();
            if (!CulpritsDone.Contains(culpritsManager.SpawnedCulprits[i]))
                CulpritsDone.Add(culpritsManager.SpawnedCulprits[i]);
        }
        //CalculateAvgProbability(ListOfProbabilityList);
        SortCulpritsList();

        // post UI
        EnablePostUI();
    }
    public void ResetListsForNextIteration()
    {
        //settings.StartGame();
        ListOfHitList.Clear();
        ListOfNoHitList.Clear();
        CulpritsDone.Clear();
        currWindow = 0;
        Text_CurrWindow.text = "Window: " + (currWindow + 1).ToString() + "/" + WindowsManager.Instance.SelectedWindows.Count;
        StartSimulation();
    }
    public int CalculateIterations(int currentVel)
    {
        // Calculate the potential final value without incrementing
        int potentialFinalValue = currentVel + (int)Math.Ceiling((maxVel - currentVel) / (float)increment) * increment;

        // Check for overflow using a comparison with the max value
        if (potentialFinalValue > maxVel)
        {
            // Set the final value to the max if it overflows
            Debug.Log("Final value would overflow, set to max: " + maxVel);
            return maxVel;

        }
        else
        {
            return currentVel + increment;
        }
    }

    //public void CalculateAvgProbability(List<List<float>> probabilityArray)
    //{
    //    for(int i = 0; i < probabilityArray.Count; i++)
    //    {
    //        Debug.Log(probabilityArray[i].Count);
    //    }    
    //    // Validate input (ensure all sub-lists have the same length)
    //    if (!probabilityArray.All(subList => subList.Count == probabilityArray[0].Count))
    //    {
    //        throw new ArgumentException("All sub-lists in the probability array must have the same length.");
    //    }

    //    int numElements = probabilityArray[0].Count; // Assuming all sub-lists have the same length

    //    // Initialize a list to store the average probabilities
    //    //List<float> averageProbabilities = new List<float>(numElements);

    //    // Calculate the sum of probabilities for each index
    //    for (int i = 0; i < numElements; i++)
    //    {
    //        float sumOfProbabilities = 0f;
    //        foreach (var subList in probabilityArray)
    //        {
    //            sumOfProbabilities += subList[i];
    //        }

    //        // Calculate the average probability for the current index
    //        CulpritsDone[i].probability = (sumOfProbabilities / probabilityArray.Count);
    //    }
    //}    
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
        ProgressUI.SetActive(false);
        cameraManager.OnEndSimulation();
        postUIManager.OnEndSimulation();
        culpritsManager.OnEndSimulation();

        camManager.SwitchToFreecam(false, false);
        //WindowsManager.Instance.canSelectWindow = !camManager.freecamMode;
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
[System.Serializable]
public class HitList
{
    public List<GameObject> Hit;
}
