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

    public List<List<GameObject>> ListOfHitList = new();
    public List<List<GameObject>> ListOfNoHitList = new();
    public List<Culprit> CulpritsDone = new();
   
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
        Culprit.OnDone += HandleDone;

        culpritsManager = FindObjectOfType<CulpritsManager>();
    }

    private void OnDestroy()
    {
        Culprit.OnCantHit -= HandleCantHit;
        Culprit.OnHit -= HandleHit;
        Culprit.OnDone -= HandleDone;
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

    public void HandleDone(Culprit culprit)
    {
        CulpritsDone.Add(culprit);

        // check if all done
        if(CulpritsDone.Count == culpritsManager.SpawnedCulprits.Count)
        {
            // sort by highest probability
            List<Culprit> sortedList = CulpritsDone.OrderByDescending(go => go.probability).ToList();
            CulpritsDone = sortedList;
        }
    }
}
