using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;

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

    void Update()
    {

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
}
