using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
public class MainGameManager : MonoBehaviour
{
    public static event Action OnStartGame;
    public static event Action OnNextWindow;

    public int hi = 0;
    public List<List<GameObject>> ListOfHitList = new();
    public List<List<GameObject>> ListOfNoHitList = new();

    CulpritsManager culpritsManager;

    enum SimulationState
    {
        PRESIMULATE,
        SIMULATE,
        POSTSIMULATE
    }

    SimulationState simState = SimulationState.PRESIMULATE;



    void Awake()
    {
        Culprit.OnCantHit += HandleCantHit;
        Culprit.OnHit += HandleHit;

        culpritsManager = FindObjectOfType<CulpritsManager>();
    }

    private void OnDestroy()
    {
        Culprit.OnCantHit -= HandleCantHit;
        Culprit.OnHit -= HandleHit;
    }
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void StartSimulation()
    {
        for (int i = 0; i < WindowsManager.Instance.SelectedWindows.Count; i++)
        {
            Debug.Log("the");
            ListOfHitList.Add(new List<GameObject>());
            ListOfNoHitList.Add(new List<GameObject>());
        }
        simState = SimulationState.SIMULATE;
        OnStartGame?.Invoke();
        WindowsManager.Instance.canSelectWindow = false;


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
}
