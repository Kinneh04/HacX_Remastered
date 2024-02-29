    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using UnityEngine.UI.Extensions;
using Cinemachine;

public class EditorManager : MonoBehaviour
{

    public bool isInEditMode = false;
    public Color OriginalCulpritBuildingColor, OriginalTargetBuildingColor;
    public List<CustomBuilding> CurrentBuildingsOnEditorDisplay = new();
    public float colorLerpSpeed;

    [Header("EditorSceneObjects")]
    public GameObject EditorObjects, MainMenuObjects;
    public bool canSelect = true;

    public void toggleCanSelect(bool b)
    {
        canSelect = b;
    }

    [Header("EditorUIMenu")]
    public GameObject EditorUI, BuildingDetailsUI, MainButtonsUI;
    public TMP_Text TitleMenu;
    public Slider NumFloorSlider, WidthSlider, AngleSlider;
    public TMP_Text NumFloorSliderValue, WidthValue, AngleValue;

    [Header("LastSavedDetails")]
    public int SavedFloors, SavedWidthInMetres, SavedAngleInDegrees;

    [Header("CurrentSelectedBuildingStats")]
    public CustomBuilding CurrentlySelectedBuilding;

    [Header("SavingOfBuilding")]
    [TextArea(5, 5)]
    public string PreviewSavedJsonString;
    public TMP_InputField ScenarioNameInput;
    public EditorSaveManager editorSave;

    [Header("Materials")]
    public Material TargetBuildingMaterial, CulpritBuildingMaterial;

    [Header("DistanceToggle")]

    public Camera OrthoCamera;
    public bool showDistance;
    public UILineRenderer CanvasLineRenderer;

    [Header("ScenarioSettings")]
    public Slider DistanceBetweenBuildingsSlider;
    public TMP_Text DistanceBetweenBuildingsValueText;
    public GameObject SceneSettingsUI;

    [Header("DefaultValues")]
    public float defaultDistanceFromBuildings = 40;
    public int DefaultFloorCount, DefaultBuildingWidth, DefaultAngle = 0;

    [Header("Environment")]
    public GameObject EnvironmentUI;
    public EditorEnvironmentManager editorEnvironmentManager;

    [Header("Cameras")]
    public List<GameObject> CameraPerspectives = new();
    public int camIndex = 0;
    private bool isMiddleMouseButtonHeld = false;
    public float Sensitivity = 1.0f;
    public float zoomSpeed = 5f;
    public void OnClickOpenEnvironmentUI()
    {
        EnvironmentUI.SetActive(true);
        MainButtonsUI.SetActive(false);
        canSelect = false;
    }
    void SwitchCamera(int direction)
    {
        // Set the current camera inactive
        CameraPerspectives[camIndex].SetActive(false);

        // Move to the next or previous camera
        camIndex = (camIndex + direction + CameraPerspectives.Count) % CameraPerspectives.Count;

        // Set the new camera active
        CameraPerspectives[camIndex].SetActive(true);
    }
    public void OnClickCloseEnvironmentUI()
    {
        EnvironmentUI.SetActive(false);
        MainButtonsUI.SetActive(true);
        canSelect = true;
    }

    private void Start()
    {
     //   ResetToDefaults();
    }

    public void ResetToDefaults()
    {
        foreach(CustomBuilding CB in CurrentBuildingsOnEditorDisplay)
        {
            OverrideBuildingFloors(CB, DefaultFloorCount);
            OverrideBuildingWidth(CB, DefaultBuildingWidth);
            OverrideBuildingRotation(CB, 0);
        }
        OverrideBuildingDistance(defaultDistanceFromBuildings);
    }

    public void OnChangeAngleOfCurrentBuilding()
    {
        AngleValue.text = AngleSlider.value.ToString() + "°";

        // Get the original rotation of the building
        Quaternion originalRotation = CurrentlySelectedBuilding.originalRotation;

        // Calculate the new rotation based on the slider value
        float angle = AngleSlider.value;
        Quaternion newRotation = Quaternion.Euler(0f, angle, 0f);
        CurrentlySelectedBuilding.AddedAngle = AngleSlider.value;
        // Apply the new rotation relative to the original rotation
        CurrentlySelectedBuilding.transform.rotation = originalRotation * newRotation;
    }

    public void OpenSceneSettingsUI()
    {
        SceneSettingsUI.SetActive(true);
        MainButtonsUI.SetActive(false);
        canSelect = false;
    }

    public void CloseSceneSettingsUI()
    {
        SceneSettingsUI.SetActive(false);
        MainButtonsUI.SetActive(true);
        canSelect = true;
    }

    public void OverrideBuildingRotation(int buildingIndex, int newAngle)
    {
        CurrentlySelectedBuilding = CurrentBuildingsOnEditorDisplay[buildingIndex];
        AngleSlider.value = newAngle;
        CurrentlySelectedBuilding.AddedAngle = newAngle;
        OnChangeAngleOfCurrentBuilding();
        OnSaveBuildingDetails();
    }
    public void OverrideBuildingRotation(CustomBuilding building, int newAngle)
    {
        CurrentlySelectedBuilding = building;
        AngleSlider.value = newAngle;
        OnChangeAngleOfCurrentBuilding();
        OnSaveBuildingDetails();
    }
    public void OverrideBuildingDistance(float dist)
    {
        DistanceBetweenBuildingsSlider.value = dist;
        OnChangeDistanceBetweenBuildings();
     //   OnSaveBuildingDetails();
    }

    public void OverrideBuildingFloors(int buildingIndex, int newFloors )
    {
        CurrentlySelectedBuilding = CurrentBuildingsOnEditorDisplay[buildingIndex];
        NumFloorSlider.value = newFloors;
        OnChangeSelectedBuildingFloors();
        OnSaveBuildingDetails();
    }
    public void OverrideBuildingFloors(CustomBuilding building, int newFloors)
    {
        CurrentlySelectedBuilding = building;
        NumFloorSlider.value = newFloors;
        OnChangeSelectedBuildingFloors();
        OnSaveBuildingDetails();
    }
    public void OverrideBuildingWidth(CustomBuilding building, int newWidth)
    {
        CurrentlySelectedBuilding = building;
        WidthSlider.value = newWidth;
        OnChangeSelectedBuildingWidth();
        OnSaveBuildingDetails();
    }
    public void OverrideBuildingWidth(int buildingIndex, int newWidth)
    {
        CurrentlySelectedBuilding = CurrentBuildingsOnEditorDisplay[buildingIndex];
        WidthSlider.value = newWidth;
        OnChangeSelectedBuildingWidth();
        OnSaveBuildingDetails();
    }

    public void OnChangeDistanceBetweenBuildings()
    {
        DistanceBetweenBuildingsValueText.text = DistanceBetweenBuildingsSlider.value.ToString() + "m";
        Vector3 pos = CurrentBuildingsOnEditorDisplay[0].transform.position;
        pos.x = DistanceBetweenBuildingsSlider.value + CurrentBuildingsOnEditorDisplay[1].transform.position.x;
        CurrentBuildingsOnEditorDisplay[0].transform.position = pos;

    }
    public void GoToEditorMenu()
    {
        isInEditMode = true;
        MainMenuObjects.SetActive(false);
        EditorObjects.SetActive(true);
        EditorUI.SetActive(true);
    }
    public void ExitFromEditorMenu()
    {
        isInEditMode = false;
        EditorUI.SetActive(false);
        MainMenuObjects.SetActive(true);
        EditorObjects.SetActive(false);

        CameraPerspectives[camIndex].SetActive(false);
        camIndex = 0;
        CameraPerspectives[camIndex].SetActive(true);
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
        if (Physics.Raycast(ray, out hit) && canSelect)
        {
            // Check if the hit object has the tag "window"
            if (hit.collider.CompareTag("EditorHDB"))
            {
                if (CurrentlySelectedBuilding == hit.collider.GetComponent<CustomBuilding>()) return;
                CurrentlySelectedBuilding = hit.collider.GetComponent<CustomBuilding>();
                if (CurrentlySelectedBuilding != null)
                {
                    BuildingDetailsUI.SetActive(true);
                    MainButtonsUI.SetActive(false);
                    TitleMenu.text =  "Selected building: " +CurrentlySelectedBuilding.typeofBuilding.ToString();

                    if (CurrentlySelectedBuilding.typeofBuilding == CustomBuilding.BuildingType.Target)
                        TargetBuildingMaterial.color = Color.white;
                    else if (CurrentlySelectedBuilding.typeofBuilding == CustomBuilding.BuildingType.Culprit)
                        CulpritBuildingMaterial.color = Color.white;
                        OverrideNumFloorSlider(CurrentlySelectedBuilding.numFloors);
                    OverrideWidthSlider(CurrentlySelectedBuilding.WidthInMetres);
                    OverrideAngleSlider(CurrentlySelectedBuilding.AddedAngle);
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
        CurrentlySelectedBuilding.GetComponent<ModularHDB>().ChangeFloors((int)NumFloorSlider.value);
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

    //public void OnRevertBuildingDetails()
    //{
    //    OnDeselectBuilding();
    //}

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

    public void OverrideAngleSlider(float newAngle)
    {
        AngleSlider.value = newAngle;
        AngleValue.text = newAngle.ToString() + "°";
    }

    public void SaveNewBuildingPreset()
    {
        List<SavableBuildingDetails> buildingDataBlock = new();
        foreach(CustomBuilding building in CurrentBuildingsOnEditorDisplay)
        {
            SavableBuildingDetails buildingData = new SavableBuildingDetails()
            {
                savedBuildingType = building.typeofBuilding,
                SavedNumFloors = building.numFloors,
                SavedWidthInMetres = building.WidthInMetres,
                SavedAngleInDegrees = (int)building.AddedAngle
            };
            buildingDataBlock.Add(buildingData);
        }
        PreviewSavedJsonString = JsonConvert.SerializeObject(buildingDataBlock);

        Scenario newScenario = new Scenario()
        {
            JsonSave = PreviewSavedJsonString,
            NameOfScenario = ScenarioNameInput.text,
            DistanceBetweenBuildings = DistanceBetweenBuildingsSlider.value
            
        };
        editorSave.CurrentlySavedScenarios.Add(newScenario);
        PopupUIManager.Instance.ShowPopup("Success", "Scenario saved successfully!");
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
                CulpritBuildingMaterial.color = Color.Lerp(CulpritBuildingMaterial.color, OriginalCulpritBuildingColor, Time.deltaTime * colorLerpSpeed);
            else if (customBuilding.typeofBuilding == CustomBuilding.BuildingType.Target)
                    TargetBuildingMaterial.color = Color.Lerp(TargetBuildingMaterial.color, OriginalTargetBuildingColor, Time.deltaTime * colorLerpSpeed);

         //   customBuilding.UpdateBuildingTransforms();
        }
        if (Input.GetMouseButtonDown(2))
        {
            isMiddleMouseButtonHeld = true;
        }
        else if (Input.GetMouseButtonUp(2))
        {
            isMiddleMouseButtonHeld = false;
        }

        // Move the GameObject if the middle mouse button is held down
        if (isMiddleMouseButtonHeld)
        {
            // Get mouse movement on the x and y axes
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            // Adjust the GameObject's position based on the mouse movement
            CameraPerspectives[camIndex].transform.Translate(new Vector3(-mouseX * Sensitivity, -mouseY * Sensitivity, 0) * Time.deltaTime);
        }
        // Check for left arrow key press
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            // Move to the previous camera
            SwitchCamera(-1);
        }
        // Check for right arrow key press
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            // Move to the next camera
            SwitchCamera(1);
        }

        // Check for scroll input
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        // Adjust orthographic size based on scroll input
        if (scroll != 0f)
        {
            CinemachineVirtualCamera vCam = CameraPerspectives[camIndex].GetComponent<CinemachineVirtualCamera>();
            // Get the current orthographic size
            float currentSize = vCam.m_Lens.OrthographicSize;

            // Calculate the new orthographic size after scrolling
            float newSize = Mathf.Clamp(currentSize - scroll * zoomSpeed, 1f, Mathf.Infinity);

            vCam.m_Lens.OrthographicSize = newSize;

            //// Set the new orthographic size
            //WindowsManager.Instance.OverviewOrthoSize = newSize;
            //if (!WindowsManager.Instance.isPrecisionMode)
            //{
            //    WindowsManager.Instance.TargetOrthoSize = WindowsManager.Instance.OverviewOrthoSize;
            //}
        }
        // [WIP] canvas line renderer for showing distance;
        //if (showDistance)
        //{
        //    CanvasLineRenderer.enabled = true;
        //    DisplayDots();
        //}
        //else CanvasLineRenderer.enabled = false;
    }

    public void DisplayDots()
    {
        int o = 0;
        foreach (CustomBuilding customBuilding in CurrentBuildingsOnEditorDisplay)
        {
            CanvasLineRenderer.Points[o].x = OrthoCamera.WorldToScreenPoint(customBuilding.transform.position).x;
            CanvasLineRenderer.Points[o].y = OrthoCamera.WorldToScreenPoint(customBuilding.transform.position).y;
            o++;
        }
    }

    public void ModifyPoint(float XValue, float YValue)
    {
        var point = new Vector2() { x = XValue, y = YValue};
        var pointlist = new List<Vector2>(CanvasLineRenderer.Points);
        pointlist.Add(point);
        CanvasLineRenderer.Points = pointlist.ToArray();
    }
}

[System.Serializable]
public class Scenario
{
    //[WIP]
    public string JsonSave;
    public string NameOfScenario;
    public float DistanceBetweenBuildings;

}


[System.Serializable]
public class SavableBuildingDetails
{
    public int SavedNumFloors, SavedWidthInMetres, SavedAngleInDegrees;
    public CustomBuilding.BuildingType savedBuildingType;
}

