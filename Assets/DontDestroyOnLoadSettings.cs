using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Windows.Forms;

public class DontDestroyOnLoadSettings : MonoBehaviour
{
    [Header("Simulation")]
    public float SimulationSpeedValue;
    public int MaxIterationsValue;
    public float timeStepAmt;
    public float[] timeSteps = { 50, 100, 200, 500, 1000 };
    public float positionRange;

    [Header("Building")]
    public int NumCulpritsPerRowValue;

    [Header("Ball")]
    public float diameter;
    public float dragCoefficient;
    public float coefficientOfRestitution;
    public float density;
    public int minVelocity;
    public int maxVelocity;
    public int velocityIncrement;
    public float angleThreshold;

    [Header("BuildingToLoad")]
    public Scenario LoadedBuilding;

    [Header("LoadingScenario")]
    private ScenarioBuilder scenarioBuilder;

    private static DontDestroyOnLoadSettings _instance;

    public List<GameObject> EnvironmentalPrefabs;

    [Header("Temporary")]
    public Scenario TempScenario;
    public bool isEditorMode;
    public void CheckForEditorPreset()
    {
        if (isEditorMode)
        {
            EditorManager EM = GameObject.FindObjectOfType<EditorManager>();
            EM.GoToEditorMenu();
            EM.editorSave.LoadCurrentScene(TempScenario);
            isEditorMode = false;
            TempScenario = null;
        }
    }
    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadBuilding()
    {
        if (!scenarioBuilder) scenarioBuilder = FindObjectOfType<ScenarioBuilder>();
        scenarioBuilder.ParseScenario(LoadedBuilding);
    }

    public void LoadSettingsIntoMainGame()
    {
        CulpritsManager culpritsManager = FindObjectOfType<CulpritsManager>();
        culpritsManager.NumCulpritsPerRow = NumCulpritsPerRowValue;
       // culpritsManager.InitFloors();
    }
    public static DontDestroyOnLoadSettings Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<DontDestroyOnLoadSettings>();
                if (_instance == null)
                {
                    GameObject managerObject = new GameObject("SettingsManager");
                    _instance = managerObject.AddComponent<DontDestroyOnLoadSettings>();
                }
            }
            
            return _instance;
        }
    }


    private void Awake()
    {
        // Ensure there's only one instance
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("MainGame");

        // Wait for the next frame before calling LoadSettingsIntoMainGame
        StartCoroutine(LoadSettingsCoroutine());
    }


    public IEnumerator LoadSettingsCoroutine()
    {
        // Wait for the next frame
        yield return null;
        
        // Wait for 0.2 seconds more
        yield return new WaitForSeconds(0.1f);
        LoadBuilding();
        yield return new WaitForSeconds(0.2f);
        // Call the method to load settings into the main game
       // LoadSettingsIntoMainGame();

    }

    public void ReturnToMainMenu()
    {
        StartCoroutine(ReturnToMainMenuCoroutine());
    }

    public IEnumerator ReturnToMainMenuCoroutine()
    {
        SceneManager.LoadScene("MainMenu");
        yield return null;
        CheckForEditorPreset();
    }

}