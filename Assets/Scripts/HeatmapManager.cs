using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatmapManager : MonoBehaviour
{
    public MainGameManager mainGameManager;
    public void UpdateHeatmap()
    {
        float min = mainGameManager.CulpritsDone[-1].probability;
        float max = mainGameManager.CulpritsDone[0].probability;

        foreach(Culprit C in mainGameManager.CulpritsDone)
        {
            float normalizedAccuracy = Mathf.InverseLerp(min, max, C.probability);
            Color heatmapColor = Color.Lerp(Color.red, Color.green, normalizedAccuracy);
            C.culpritMaterial.color = heatmapColor;
        }
    }
}
