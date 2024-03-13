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
    public Slider HeatmapPrecisionSlider;

    [Header("Managers")]
    public HeatmapManager heatmapManager;
    public CulpritsManager culpritsManager;
    public MainGameManager mainGameManager;

    [Header("Values")]
    public float AccuracyLimit;
    public TMP_Text AccuracyLimitValueText;
    public TMP_Text HeatmapPrecisionText;

    public List<int> WindowIndexesSelected = new();
    public List<WindowToggle> windowsToggles = new();

    [Header("Windows")]
    public GameObject WindowSelectionTogglePrefab;
    public Transform WindowSelectionPrefabParent;
    // TODO: Assign these and instantiate them;

    [Header("Answer")]
    public TMP_Text AnswerText;


    public void OnClearOutliersButton()
    {
        culpritsManager.RemoveOutliers();
        heatmapManager.UpdateHeatmap();

    }

    public void OnChangeHeatmapPrecision()
    {
        HeatmapPrecisionText.text = (HeatmapPrecisionSlider.value * 100).ToString("F2") + "%";
        heatmapManager.OnChangeHeatmapPrecision(HeatmapPrecisionSlider.value);
    }

    public void OnClearAllWindows()
    {
        foreach(WindowToggle WT in windowsToggles)
        {
            WT.toggle.isOn = false;
            OnSelectWindow(WT.windowSelectedIndex, WT.toggle);
        }
    }

    public void InstantiateWindowToggles()
    {
        int PWIndex = 0;
        foreach(Precise_Window PW in WindowsManager.Instance.PreciseWindows)
        {
            GameObject GO = Instantiate(WindowSelectionTogglePrefab);
            GO.transform.SetParent(WindowSelectionPrefabParent, false);
            WindowToggle WT = GO.GetComponent<WindowToggle>();
            windowsToggles.Add(WT);
            WT.WindowText.text = "Window " + (PWIndex+1).ToString();
            WT.windowSelectedIndex = PWIndex;
            WT.toggle.onValueChanged.AddListener(delegate { OnSelectWindow(WT.windowSelectedIndex, GO.GetComponent<Toggle>()); });
            WindowIndexesSelected.Add(PWIndex);
            PWIndex++;

            

        }
    }
    public void OnSelectWindow(int windowIndex, Toggle T)
    {
        if (T.isOn)
            WindowIndexesSelected.Add(windowIndex);
        else WindowIndexesSelected.Remove(windowIndex);
        RecalculateWindows();

        ChangeAccLimitSlider.maxValue = returnCulpritMaxAccuracy();
        ChangeAccLimitSlider.minValue = returnCulpritMinAccuracy();
    }

    public void RecalculateWindows()
    {
        foreach(Culprit C in culpritsManager.SpawnedCulprits)
        {
            C.CalculateProbabilityWithWindows(WindowIndexesSelected);
        }
        heatmapManager.UpdateHeatmap();
    }

    public void OnEndSimulation()
    {
        heatmapManager.UpdateHeatmap();
        ChangeAccLimitSlider.maxValue = returnCulpritMaxAccuracy();
        ChangeAccLimitSlider.minValue = returnCulpritMinAccuracy();
        ChangeAccLimitSlider.value = ChangeAccLimitSlider.minValue;

        InstantiateWindowToggles();

        Culprit AnswerCulprit = mainGameManager.CulpritsDone[0];

        AnswerText.text = $"Most likely culprit on Floor {AnswerCulprit.floor}, Column {AnswerCulprit.column}, with an accuracy of {AnswerCulprit.averageProbability.ToString("F2")}%";
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

    public void OnHitMostLikelyCulprit()
    {
        //Most likely culprit
        mainGameManager.CulpritsDone[0].outline.OutlineColor = Color.cyan;

        //if(showMostLikelyCulpritToggle.isOn)
        //{
        //    mainGameManager.CulpritsDone[0].culpritMeshRenderer.material.color = Color.cyan;
        //}
        //else
        //{
        //    if(ShowHeatmapToggle.isOn)
        //    {
        //        mainGameManager.CulpritsDone[0].culpritMeshRenderer.material.color = Color.green;
        //    }
        //    else
        //    {
        //        mainGameManager.CulpritsDone[0].culpritMeshRenderer.material.color = Color.red;
        //    }
        //}
    }

    public float returnCulpritMaxAccuracy()
    {
        float max = mainGameManager.CulpritsDone[0].probability;
        return max;
    }

    public void ReplayScenario()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

        ChangeAccLimitSlider.interactable = !ShowAllCulpritsToggle.isOn;
    }

    public void OnToggleTracers()
    {
        foreach(HitList HList in mainGameManager.ListOfHitList)
        {
            foreach (GameObject GO in HList.Hit)
            {
                GO.GetComponent<TrailRenderer>().enabled = ShowTracersToggle.isOn;
            }
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

        //if (showMostLikelyCulpritToggle.isOn)
        //{
        //    mainGameManager.CulpritsDone[0].culpritMeshRenderer.material.color = Color.cyan;
        //}
    }
}
