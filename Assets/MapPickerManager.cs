using Mapbox.Unity.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mapbox.Examples;

public class MapPickerManager : MonoBehaviour
{
    [Header("UIElements")]
    public GameObject EditorUI;
    public GameObject MappickerUI, MapPickerWarningUI; bool saidOnce = false;
    public GameObject EditorSceneObjects, MapPickerSceneObjects;
    public GameObject MainCamera, PickerCamera;
    public AbstractMap map;

    [Header("Selection and related UI")]
    public List<GameObject> SelectedBuildings = new();
    public TMP_Text Building1Text, Building2Text;
    public Image Building1, Building2;
    public TMP_Text DistanceCalculatedText;

    [Header("Colors")]
    public Color OriginalColor, HighlightedColor;

    [Header("Factors")]
    [Range(0.1f, 30f)]
    public float DistanceScaleFactor;

    public LineRenderer lineRenderer;

    [Header("Building")]
    public EditorManager editorManager;
    public float distanceCalculated;

    public void EraseCollisionLists()
    {
        HighlightFeature[] highlights = GameObject.FindObjectsOfType<HighlightFeature>();
        foreach(HighlightFeature f in highlights)
        {
            f.IntersectingObjects.Clear();
        }
    }

    public void CopyStencil()
    {
        foreach (GameObject GO in SelectedBuildings)
        {
            GO.GetComponent<HighlightFeature>().OnDeselectBuilding();
            Destroy(GO.GetComponent<FeatureSelectionDetector>());
            Destroy(GO.GetComponent<HighlightFeature>());
        }
        editorManager.CopyMapStencil(SelectedBuildings);
    }

    public void BuildMap()
    {

        for (int i = 0; i < SelectedBuildings.Count - 1; i++)
        {
            if (SelectedBuildings[i].transform.position.z > SelectedBuildings[i + 1].transform.position.z)
            {
                // Swap elements
                GameObject temp = SelectedBuildings[i];
                SelectedBuildings[i] = SelectedBuildings[i + 1];
                SelectedBuildings[i + 1] = temp;
            }
        }
        editorManager.BuildMapScenario(SelectedBuildings, distanceCalculated);
        CloseMapPicker();
    }

    public void ClearAllBuildings()
    {
        foreach(GameObject GO in SelectedBuildings)
        {
            GO.GetComponent<HighlightFeature>().OnDeselectBuilding(false);
        }

        SelectedBuildings.Clear();
        UpdateTexts();
    }

    public void ToggleBuildingSelect(GameObject GO)
    {
        if (SelectedBuildings.Contains(GO))
        {
            DeselectBuilding(GO);
        }
        else SelectBuilding(GO);

        UpdateTexts();
    }

    public void DeselectBuilding(GameObject GO)
    {
        HighlightFeature HF = GO.GetComponent<HighlightFeature>();
      //  HF.mapPicker = this;
        if (HF.isSelected)
        {
            SelectedBuildings.Remove(GO);
            HF.OnDeselectBuilding();
        }
    }

    public void SelectBuilding(GameObject GO)
    {
        HighlightFeature HF = GO.GetComponent<HighlightFeature>();
    //    HF.mapPicker = this;
        if (!HF.isSelected)
        {
            if (SelectedBuildings.Count == 2) return;
            SelectedBuildings.Add(GO);
        }
        HF.OnSelectBuilding();
    }

    public void UpdateTexts()
    {
        DistanceCalculatedText.text = "";
        lineRenderer.enabled = false;    
        if (SelectedBuildings.Count == 2) { Building1.color = HighlightedColor; Building2.color = HighlightedColor;
            DisplayTrail();
        }
        else if (SelectedBuildings.Count == 1) { Building2.color = OriginalColor; Building1.color = HighlightedColor; }
        else { Building2.color = OriginalColor; Building1.color = OriginalColor; }
    }

    public void DisplayTrail()
    {
        distanceCalculated = Vector3.Distance(SelectedBuildings[0].transform.position, SelectedBuildings[1].transform.position);
        DistanceCalculatedText.text = "Estimated distance: " + (distanceCalculated * DistanceScaleFactor).ToString("F2") + "m";
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, SelectedBuildings[0].transform.position);
        lineRenderer.SetPosition(1, SelectedBuildings[1].transform.position);
    }

    public void GotoMapPicker()
    {
        EditorUI.SetActive(false);
      //  MappickerUI.SetActive(true);
        if(!saidOnce)
        {
            saidOnce = true;
            MapPickerWarningUI.SetActive(true);
            MappickerUI.SetActive(false);
        }
        else
        {
            MappickerUI.SetActive(true);
        }
        MainCamera.SetActive(false);
        PickerCamera.SetActive(true);
        EditorSceneObjects.SetActive(false);
        MapPickerSceneObjects.SetActive(true);
    }

    public void CloseMapPicker()
    {
        EditorUI.SetActive(true);
        MapPickerWarningUI.SetActive(false);
        MapPickerSceneObjects.SetActive(false);
        MainCamera.SetActive(true);
        EditorSceneObjects.SetActive(true);
        PickerCamera.SetActive(false);
        MappickerUI.SetActive(false);
    }

    public void InitializeMap()
    {
        MappickerUI.SetActive(true);
        MapPickerWarningUI.SetActive(false);
        map.SetUpMap();
    }

}
