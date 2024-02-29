using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class DontDestroyOnLoadSettings : MonoBehaviour
{
    [Header("Settings")]
    public int NumCulpritsPerRowValue;
    public float SimulationSpeedValue;
    public int MaxIterationsValue;
    public float dragCoefficient;
    public float initialVelocity;

    [Header("BuildingToLoad")]
    public Scenario LoadedBuilding;

    [Header("LoadingScenario")]
    private ScenarioBuilder scenarioBuilder;

    private static DontDestroyOnLoadSettings _instance;
    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadBuilding()
    {
        if (!scenarioBuilder) scenarioBuilder = GameObject.FindObjectOfType<ScenarioBuilder>();
        scenarioBuilder.ParseScenario(LoadedBuilding);
    }

    public void LoadSettingsIntoMainGame()
    {
        CulpritsManager culpritsManager = GameObject.FindObjectOfType<CulpritsManager>();
        culpritsManager.NumCulpritsPerRow = NumCulpritsPerRowValue;
        culpritsManager.InitFloors();
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
        yield return new WaitForSeconds(0.2f);
        LoadBuilding();
        yield return new WaitForSeconds(0.2f);
        // Call the method to load settings into the main game
        LoadSettingsIntoMainGame();

    }

}