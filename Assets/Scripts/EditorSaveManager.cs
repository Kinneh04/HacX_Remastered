using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using TMPro;
using AnotherFileBrowser.Windows;
using UnityEngine.Networking;
using System;

public class EditorSaveManager : MonoBehaviour
{

    [Header("ScenarioSaveList")]
    public List<Scenario> CurrentlySavedScenarios = new();
    List<GameObject> InstantiatedScenarioPrefabs = new();
    public GameObject NoScenariosText;

    [Header("Prefabs")]
    public GameObject ScenarioListUI;
    public GameObject ScenarioUIPrefab;
    public Transform ScenarioUIPrefabParentTransform;

    [Header("SelectScenarioUI")]
    public EditorManager editorManager;
    public GameObject ScenarioSettingsUI;
    public Scenario SelectedScenario;

    [Header("Exporting And Importing")]
    public TMP_Text ExportingFeedback;
    public TMP_Text ImportingFeedback;
    [TextArea(5,5)]
    public string loadedJSONFromFile = "";
    public GameObject ImportingCloseButton, importingFeedbackGO;
    public EditorEnvironmentManager environmentManager;

    public List<GameObject> CustomEnvironmentThings = new();
    public void FetchCustomThings()
    {
        int childCount = DontDestroyOnLoadSettings.Instance.transform.childCount;
        GameObject[] children = new GameObject[childCount];

        for (int i = 0; i < childCount; i++)
        {
            CustomEnvironmentThings.Add(DontDestroyOnLoadSettings.Instance.transform.GetChild(i).gameObject);
        }
    }
    public void DeleteCurrentlyselectedScenario()
    {
        CurrentlySavedScenarios.Remove(SelectedScenario);
        OnRefreshScenarioList();
        CloseScenarioSettings();
    }

    public bool AlreadyHasSaveWithName(string name, bool forceDelete = false)
    {
        foreach(Scenario S in CurrentlySavedScenarios)
        {
            if (S.NameOfScenario == name)
            {
                if (forceDelete) CurrentlySavedScenarios.Remove(S);
                return true;
            }
        }
        return false;
    }

    private void Awake()
    {
        LoadCurrentScenariosFromPlayerPrefs();
    }

    private void OnDestroy()
    {
        SaveCurrentScenariosToPlayerPrefs();
    }

    public void SaveCurrentScenariosToPlayerPrefs()
    {
        string JSONSave = JsonConvert.SerializeObject(CurrentlySavedScenarios);
        PlayerPrefs.SetString("SavedScenarios", JSONSave);
    }

    public void LoadCurrentScenariosFromPlayerPrefs()
    {
        List<Scenario> Temp = JsonConvert.DeserializeObject<List<Scenario>>(PlayerPrefs.GetString("SavedScenarios"));

        if (Temp.Count > 0)
            CurrentlySavedScenarios = Temp;
    }

    public void CloseScenarioSettings()
    {
        ScenarioSettingsUI.SetActive(false);
        SelectedScenario = null;
    }

    public void SelectScenario(Scenario scenario)
    {
        SelectedScenario = scenario;
        ScenarioSettingsUI.SetActive(true);
    }
    public void ExportScenario()
    {
        string JSONData = JsonConvert.SerializeObject(SelectedScenario);
        string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);

        // Combine desktop path and file name to get the full path
        string filePath = Path.Combine(desktopPath, SelectedScenario.NameOfScenario + ".json");

        // Write the JSON data to the file
        File.WriteAllText(filePath, JSONData);
        ExportingFeedback.text = "JSON file exported to: " + filePath;
        Debug.Log("JSON data exported to: " + filePath);
    }

    public void ImportScenario()
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
                if(uwr.isNetworkError || uwr.isHttpError)
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
                        CurrentlySavedScenarios.Add(S);

                        ImportingFeedback.text = "Scenario imported successfully! Saved as " + S.NameOfScenario;
                        ImportingCloseButton.SetActive(true);
                    }
                    catch(Exception e)
                    {
                        ImportingFeedback.text = "Scenario import error: " + e.Message;
                        ImportingCloseButton.SetActive(true);
                    }
                }
            }
        }
    }

    public GameObject ReturnCustomPropWithIndex(int index)
    {
        foreach (GameObject GO in CustomEnvironmentThings)
        {
            EnvironmentalPrefab EP = GO.GetComponent<EnvironmentalPrefab>();
            if (EP.propIndex == index) return GO;
        }
        return null;
    }
    public void LoadCurrentlySelectedScene()
    {

        //Load selected scene

        LoadCurrentScene();
    }

    public void LoadCurrentScene(Scenario S = null)
    {

        //Load selected scene
        FetchCustomThings();
        List<SavableBuildingDetails> BuildingDatablock = new();
        List<SavableEnvironmentDetails> EnvironmentDatablock = new();
        if (S == null)
        {
            BuildingDatablock = JsonConvert.DeserializeObject<List<SavableBuildingDetails>>(SelectedScenario.JsonSave);
            EnvironmentDatablock = JsonConvert.DeserializeObject<List<SavableEnvironmentDetails>>(SelectedScenario.EnvironmentJSON);
        }
        else
        {
            BuildingDatablock = JsonConvert.DeserializeObject<List<SavableBuildingDetails>>(S.JsonSave);
            EnvironmentDatablock = JsonConvert.DeserializeObject<List<SavableEnvironmentDetails>>(S.EnvironmentJSON);
        }
        for (int i = 0; i < BuildingDatablock.Count; i++)
        {
            
            editorManager.OverrideBuildingFloors(i, BuildingDatablock[i].SavedNumFloors);
            editorManager.OverrideBuildingTransforms(i, new Vector3(BuildingDatablock[i].PosX, BuildingDatablock[i].PosY, BuildingDatablock[i].PosZ), new Vector3(BuildingDatablock[i].RotX, BuildingDatablock[i].RotY, BuildingDatablock[i].RotZ));
            //  editorManager.OverrideBuildingWidth(i, BuildingDatablock[i].SavedWidthInMetres);
            //  editorManager.OverrideBuildingRotation(i, BuildingDatablock[i].SavedAngleInDegrees);

            editorManager.RefreshCurrentBuildingFloors(i);
        }
        for (int i = 0; i < EnvironmentDatablock.Count; i++)
        {
            // Add environmentals
            int index = EnvironmentDatablock[i].savedItemIndex;
            GameObject GO = null;
                // Prop is a custom env;
                GO = ReturnCustomPropWithIndex(index);
            GO = Instantiate(GO, GO.transform.position, Quaternion.identity);
            environmentManager.AddEnvironmentProp(GO);
            GO.transform.position = new Vector3(EnvironmentDatablock[i].PosX, EnvironmentDatablock[i].PosY, EnvironmentDatablock[i].PosZ);
            GO.transform.rotation = Quaternion.Euler(new Vector3(EnvironmentDatablock[i].RotX, EnvironmentDatablock[i].RotY, EnvironmentDatablock[i].RotZ));
            GO.name = GO.name.Replace("(Clone)", "");
            GO.transform.localScale = new Vector3(EnvironmentDatablock[i].ScaleX, EnvironmentDatablock[i].ScaleY, EnvironmentDatablock[i].ScaleZ);

            GO.transform.SetParent(DontDestroyOnLoadSettings.Instance.transform);
        }

        //  editorManager.OverrideBuildingDistance(SelectedScenario.DistanceBetweenBuildings);


        foreach (GameObject GO in CustomEnvironmentThings)
        {
            Destroy(GO);
        }
        CloseScenarioSettings();
        ScenarioListUI.SetActive(false);
    }

    public void OnOpenScenarioList()
    {
        foreach(GameObject gameObject in InstantiatedScenarioPrefabs)
        {
            Destroy(gameObject);
        }
        InstantiatedScenarioPrefabs.Clear();
        ScenarioListUI.SetActive(true);
        OnRefreshScenarioList();
    }

    private void OnRefreshScenarioList()
    {

        foreach(GameObject GO in InstantiatedScenarioPrefabs)
        {
            Destroy(GO);
        }
        InstantiatedScenarioPrefabs.Clear();

        if (CurrentlySavedScenarios.Count > 0)
        {
            NoScenariosText.SetActive(false);
            foreach (Scenario scenario in CurrentlySavedScenarios)
            {
                GameObject GO = Instantiate(ScenarioUIPrefab);
                GO.transform.SetParent(ScenarioUIPrefabParentTransform, false);
                ScenarioPrefab scenarioPrefab = GO.GetComponent<ScenarioPrefab>();
                scenarioPrefab.NameText.text = scenario.NameOfScenario;
                scenarioPrefab.OnClickButton.onClick.AddListener(delegate { SelectScenario(scenario); });

                InstantiatedScenarioPrefabs.Add(GO);
            }
        }
        else
        {
            NoScenariosText.SetActive(true);
        }
    }
}
