using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class PostUIManager : MonoBehaviour
{

    [Header("Toggles")]
    public Toggle ShowHeatmapToggle;
    public Toggle ShowAccuracyToggle, ShowTracersToggle, ShowAllCulpritsToggle, showMostLikelyCulpritToggle;

    [Header("Sliders")]
    public Slider ChangeAccLimitSlider;

    [Header("Managers")]
    public HeatmapManager heatmapManager;
    public CulpritsManager culpritsManager;
    public MainGameManager mainGameManager;

    [Header("Values")]
    public float AccuracyLimit;
    public TMP_Text AccuracyLimitValueText;

    public List<int> WindowIndexesSelected = new();

    [Header("Windows")]
    public GameObject WindowSelectionTogglePrefab;
    public Transform WindowSelectionPrefabParent;
    // TODO: Assign these and instantiate them;

    public void InstantiateWindowToggles()
    {
        int PWIndex = 0;
        foreach(Precise_Window PW in WindowsManager.Instance.PreciseWindows)
        {
            GameObject GO = Instantiate(WindowSelectionTogglePrefab);
            GO.transform.SetParent(WindowSelectionPrefabParent, false);
            GO.GetComponent<Toggle>().onValueChanged.AddListener(delegate { OnSelectWindow(PWIndex, GO.GetComponent<Toggle>()); });
            PWIndex++;

        }
    }
    public void OnSelectWindow(int windowIndex, Toggle T)
    {
        if (T.isOn)
            WindowIndexesSelected.Add(windowIndex);
        else WindowIndexesSelected.Remove(windowIndex);
        RecalculateWindows();
    }

    public void RecalculateWindows()
    {
        foreach(Culprit C in culpritsManager.SpawnedCulprits)
        {
            C.CalculateProbabilityWithWindows(WindowIndexesSelected);
        }
    }

    public void OnEndSimulation()
    {
        heatmapManager.UpdateHeatmap();
        ChangeAccLimitSlider.maxValue = returnCulpritMaxAccuracy();
        ChangeAccLimitSlider.minValue = returnCulpritMinAccuracy();
        ChangeAccLimitSlider.value = ChangeAccLimitSlider.minValue;
    }

    public float returnCulpritMinAccuracy()
    {
        float min = 0;
        for (int i = mainGameManager.CulpritsDone.Count - 1; i >= 0; i--)
        {
            Culprit currentCulprit = mainGameManager.CulpritsDone[i];

            // Check if the probability is not equal to 0
            if (currentCulprit.probability != 0f)
            {
                // Found the next culprit with a probability != 0
                min = currentCulprit.probability;

                break; // Break the loop since we found the next culprit
            }
        }
        return min;
    }

    public void OnToggleMostLikelyCulprit()
    {

        if(showMostLikelyCulpritToggle.isOn)
        {
            mainGameManager.CulpritsDone[0].culpritMeshRenderer.material.color = Color.cyan;
        }
        else
        {
            if(ShowHeatmapToggle.isOn)
            {
                mainGameManager.CulpritsDone[0].culpritMeshRenderer.material.color = Color.green;
            }
            else
            {
                mainGameManager.CulpritsDone[0].culpritMeshRenderer.material.color = Color.red;
            }
        }
    }

    public float returnCulpritMaxAccuracy()
    {
        float max = mainGameManager.CulpritsDone[0].probability;
        return max;
    }

    public void ReplayScenario()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void exportData()
    {

    }

    public void OnChangeAccuracyLimitSlider()
    {
        AccuracyLimit = ChangeAccLimitSlider.value;
        AccuracyLimitValueText.text = AccuracyLimit.ToString("F1") + "%";
        if(!ShowAllCulpritsToggle.isOn)
        foreach (Culprit c in culpritsManager.SpawnedCulprits)
        {
                if (c.probability < AccuracyLimit) c.gameObject.SetActive(false);
                else c.gameObject.SetActive(true);
        }
    }

    public void OnToggleAllCulprits()
    {
        if(ShowAllCulpritsToggle.isOn)
        {
            foreach(Culprit c in culpritsManager.SpawnedCulprits)
            {
                c.gameObject.SetActive(true);
            }
        }
        else
        {
            foreach (Culprit c in culpritsManager.SpawnedCulprits)
            {
                if(c.probability == 0)
                    c.gameObject.SetActive(false);
            }
        }
    }

    public void OnToggleTracers()
    {
        List<GameObject> BallObjects = mainGameManager.ListOfHitList[1];
        foreach(GameObject GO in BallObjects)
        {
            GO.GetComponent<TrailRenderer>().enabled = ShowTracersToggle.isOn;
        }
    }

    public void OnToggleAccuracy()
    {
        foreach(Culprit c in culpritsManager.SpawnedCulprits)
        {
            c.probabilityText.gameObject.SetActive(ShowAccuracyToggle.isOn);
        }
    }

    public void OnToggleHeatmap()
    {
        if (ShowHeatmapToggle.isOn) heatmapManager.UpdateHeatmap();
        else heatmapManager.DisableHeatmap();

        if (showMostLikelyCulpritToggle.isOn)
        {
            mainGameManager.CulpritsDone[0].culpritMeshRenderer.material.color = Color.cyan;
        }
    }
}
