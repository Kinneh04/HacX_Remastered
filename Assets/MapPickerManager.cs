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

    public void ClearAllBuildings()
    {
        foreach(GameObject GO in SelectedBuildings)
        {
            GO.GetComponent<HighlightFeature>().OnDeselectBuilding();
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
        SelectedBuildings.Remove(GO);
        GO.GetComponent<HighlightFeature>().OnDeselectBuilding();
    }

    public void SelectBuilding(GameObject GO)
    {
        if (SelectedBuildings.Count == 2) return;
        SelectedBuildings.Add(GO);
        GO.GetComponent<HighlightFeature>().OnSelectBuilding();

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
        float distanceCalculated = Vector3.Distance(SelectedBuildings[0].transform.position, SelectedBuildings[1].transform.position);
        DistanceCalculatedText.text = "Calculated distance: " + (distanceCalculated * DistanceScaleFactor).ToString("F2") + "m";
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
