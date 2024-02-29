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
    public Toggle ShowAccuracyToggle, ShowTracersToggle, ShowAllCulpritsToggle;

    [Header("Managers")]
    public HeatmapManager heatmapManager;

  public void ReplayScenario()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void exportData()
    {

    }

    public void OnToggleHeatmap()
    {
        if (ShowHeatmapToggle) heatmapManager.UpdateHeatmap();
        else heatmapManager.DisableHeatmap();
    }
}
