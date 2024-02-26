    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class EditorManager : MonoBehaviour
{

    public bool isInEditMode = false;
    public Color OriginalCulpritBuildingColor, OriginalTargetBuildingColor;
    public List<CustomBuilding> CurrentBuildingsOnEditorDisplay = new();
    public float colorLerpSpeed;

    [Header("EditorSceneObjects")]
    public GameObject EditorObjects, MainCamera;

    [Header("EditorUIMenu")]
    public GameObject EditorUI, BuildingDetailsUI, MainButtonsUI;
    public TMP_Text TitleMenu;
    public Slider NumFloorSlider, WidthSlider;
    public TMP_Text NumFloorSliderValue, WidthValue;

    [Header("LastSavedDetails")]
    public int SavedFloors, SavedWidthInMetres;

    [Header("CurrentSelectedBuildingStats")]
    public CustomBuilding CurrentlySelectedBuilding;

    public void GoToEditorMenu()
    {
        isInEditMode = true;
        MainCamera.SetActive(false);
        EditorObjects.SetActive(true);
        EditorUI.SetActive(true);
    }
    public void ExitFromEditorMenu()
    {
        isInEditMode = false;
        EditorUI.SetActive(false);
        MainCamera.SetActive(true);
        EditorObjects.SetActive(false);
    }

    public void ToggleEditMode(bool t)
    {
        isInEditMode = t;
    }
    public void OnSelectBuilding()
    {
        // Create a ray from the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Declare a RaycastHit variable to store the hit information
        RaycastHit hit;

        // Perform the raycast
        if (Physics.Raycast(ray, out hit))
        {
            // Check if the hit object has the tag "window"
            if (hit.collider.CompareTag("EditorHDB"))
            {
                if (CurrentlySelectedBuilding == hit.collider.GetComponent<CustomBuilding>()) return;
                CurrentlySelectedBuilding = hit.collider.GetComponent<CustomBuilding>();
                if (CurrentlySelectedBuilding)
                {
                    BuildingDetailsUI.SetActive(true);
                    MainButtonsUI.SetActive(false);
                    TitleMenu.text =  "Selected building: " +CurrentlySelectedBuilding.typeofBuilding.ToString();
                    CurrentlySelectedBuilding.HighlightMaterial.color = Color.white;
                    OverrideNumFloorSlider(CurrentlySelectedBuilding.numFloors);
                    OverrideWidthSlider(CurrentlySelectedBuilding.WidthInMetres);
                    SavedFloors = CurrentlySelectedBuilding.numFloors;
                    SavedWidthInMetres = CurrentlySelectedBuilding.WidthInMetres;
                    //NumFloorSlider.value = CurrentlySelectedBuilding.numFloors;
                    //WidthSlider.value = CurrentlySelectedBuilding.
                }
            }
        }
    }

    // TODO: Change building height and width according to slider values.
    // Update the savedWidth and SavedFloors. 

    public void OnChangeSelectedBuildingFloors()
    {
        NumFloorSliderValue.text = NumFloorSlider.value.ToString();
        SavedFloors = (int)NumFloorSlider.value;
    }
    public void OnChangeSelectedBuildingWidth()
    {
        WidthValue.text = WidthSlider.value.ToString();
        SavedWidthInMetres = (int)WidthSlider.value;
    }
    public void OnSaveBuildingDetails()
    {
        CurrentlySelectedBuilding.WidthInMetres = SavedWidthInMetres;
        CurrentlySelectedBuilding.numFloors = SavedFloors;

        OnDeselectBuilding();
    }

    public void OnRevertBuildingDetails()
    {
        OnDeselectBuilding();
    }

    public void OnDeselectBuilding()
    {
        CurrentlySelectedBuilding = null;
        BuildingDetailsUI.SetActive(false);
        MainButtonsUI.SetActive(true);
    }

    public void OverrideNumFloorSlider(int value)
    {
        NumFloorSlider.value = value;
        NumFloorSliderValue.text = value.ToString();
    }

    public void OverrideWidthSlider(int value)
    {
        WidthSlider.value = value;
        WidthValue.text = value.ToString();
    }

   

    private void Update()
    {
        if (!isInEditMode) return;

        if(Input.GetMouseButtonDown(0))
        {
            OnSelectBuilding();
        }
        foreach(CustomBuilding customBuilding in CurrentBuildingsOnEditorDisplay)
        {
            if (customBuilding.typeofBuilding == CustomBuilding.BuildingType.Culprit)
                customBuilding.HighlightMaterial.color = Color.Lerp(customBuilding.HighlightMaterial.color, OriginalCulpritBuildingColor, Time.deltaTime * colorLerpSpeed);
            else if (customBuilding.typeofBuilding == CustomBuilding.BuildingType.Target)
                    customBuilding.HighlightMaterial.color = Color.Lerp(customBuilding.HighlightMaterial.color, OriginalTargetBuildingColor, Time.deltaTime * colorLerpSpeed);
        }
    }
}
