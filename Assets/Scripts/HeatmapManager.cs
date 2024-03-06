using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HeatmapManager : MonoBehaviour
{
    public MainGameManager mainGameManager;
    public List<float> accs = new();
    public float AdditionalAccuracy = 0.0f;
    public void UpdateHeatmap()
    {
        mainGameManager.SortCulpritsList();
        float min = 0;
        for (int i = mainGameManager.CulpritsDone.Count - 1; i >= 0; i--)
        {
            Culprit currentCulprit = mainGameManager.CulpritsDone[i];

            // Check if the probability is not equal to 0
            if (currentCulprit.probability != 0f)
            {
                // Found the next culprit with a probability != 0
                min = currentCulprit.probability * (1 + AdditionalAccuracy);

                break; // Break the loop since we found the next culprit
            }
        }

        float max = mainGameManager.CulpritsDone[0].probability;
        if (min > max) min = max * 0.99f;
        foreach (Culprit C in mainGameManager.CulpritsDone)
        {
            float normalizedAccuracy = Mathf.InverseLerp(min, max, C.probability);
            float clampedNormalizedAccuracy = Mathf.Clamp01(normalizedAccuracy);
            accs.Add(clampedNormalizedAccuracy);
            Color heatmapColor = Color.Lerp(Color.red, Color.green, clampedNormalizedAccuracy);
            C.culpritMeshRenderer.material.color = heatmapColor;
        }

    }

    public void OnChangeHeatmapPrecision(float newPrecision)
    {
        AdditionalAccuracy = newPrecision;
        UpdateHeatmap();
    }

    public void DisableHeatmap()
    {
        foreach (Culprit C in mainGameManager.CulpritsDone)
        {

            C.culpritMeshRenderer.material.color = Color.red;
        }
    }
}
