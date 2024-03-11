    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using UnityEngine.UI.Extensions;
using Cinemachine;
using RuntimeHandle;

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
    public bool isEnvironmentalMode;
    public EditorEnvironmentManager editorEnvironmentManager;

    [Header("Cameras")]
    public List<GameObject> CameraPerspectives = new();
    public int camIndex = 0;
    private bool isMiddleMouseButtonHeld = false;
    public float Sensitivity = 1.0f;
    public float zoomSpeed = 5f;

    [Header("Stencils")]
    public List<GameObject> stencils = new();

    [Header("Gizmos")]
    public GameObject runtimeTransformGameObj;
    public RuntimeTransformHandle runtimeTransformHandle;
    private int runtimeTransformLayer = 6;
    private int runtimeTransformLayerMask;

    [Header("Distance")]
    float currentDistance;
    public TMP_Text DistanceText;
    public LineRenderer DistanceLineRenderer;

    [Header("OverrideSave")]
    public GameObject OverrideSaveUI;
    public TMP_Text NameOfSaveText;
    

    public void UpdateDistance()
    {
        Vector3 PosA = CurrentBuildingsOnEditorDisplay[0].transform.position;
        PosA.y = 1;
        Vector3 PosB = CurrentBuildingsOnEditorDisplay[1].transform.position;
        PosB.y = 1;


        DistanceLineRenderer.SetPosition(0, PosA);
        DistanceLineRenderer.SetPosition(1, PosB);
        currentDistance = Vector3.Distance(CurrentBuildingsOnEditorDisplay[0].transform.position, CurrentBuildingsOnEditorDisplay[1].transform.position);
        
        DistanceText.text = "Distance: " + currentDistance.ToString("F1") + "m";
    }

    public GameObject HelperInstructions, GizmosInstructions;
    private void Start()
    {
        runtimeTransformGameObj = new GameObject();
        runtimeTransformHandle = runtimeTransformGameObj.AddComponent<RuntimeTransformHandle>();
        runtimeTransformGameObj.layer = runtimeTransformLayer;
        runtimeTransformLayerMask = 1 << runtimeTransformLayer; //Layer number represented by a single bit in the 32-bit integer using bit shift
        runtimeTransformHandle.type = HandleType.POSITION;
        runtimeTransformHandle.autoScale = true;
        runtimeTransformHandle.autoScaleFactor = 1f;
        runtimeTransformHandle.rotationSnap = 5f;
        runtimeTransformHandle.positionSnap = new Vector3(0.1f, 0.1f, 0.1f);
        runtimeTransformHandle.scaleSnap = new Vector3(0.1f, 0.1f, 0.1f);
        runtimeTransformGameObj.SetActive(false);
    }

    public void BuildMapScenario(List<GameObject> BuildingPositions, float calculatedDistance)
    {
        Vector3 Pivot = -BuildingPositions[0].transform.position;
        CurrentBuildingsOnEditorDisplay[0].transform.position = Vector3.zero;
        for(int i = 1; i < BuildingPositions.Count; i++)
        {
            CurrentBuildingsOnEditorDisplay[i].transform.position = BuildingPositions[i].transform.position + Pivot;
        }


//        Vector3 Pivot = BuildingPositions[0].transform.position;
//        bool Pivotal = false;
//        for (int i = 0; i < BuildingPositions.Count; i++)
//        {
//            if (!Pivotal)
//            {
//                CurrentBuildingsOnEditorDisplay[i].transform.position = Vector3.zero;

//               // Pivot = BuildingPositions[i].transform.position;
//                Pivotal = true;

//            }
//            else
//            {
//                CurrentBuildingsOnEditorDisplay[i].transform.position = BuildingPositions[i].transform.position + Pivot;
//                Vector3 CurrentBuildingPosition = CurrentBuildingsOnEditorDisplay[i].transform.position;
//                CurrentBuildingPosition.x = -calculatedDistance;
//                CurrentBuildingsOnEditorDisplay[i].transform.position = CurrentBuildingPosition;
//;            }
//        }
    }


    // Unused. Who knows if i might use this agin
    public void CopyMapStencil(List<GameObject> Buildings)
    {
        Vector3 Pivot = Buildings[0].transform.position;
        bool Pivotal = false;
       foreach(GameObject GO in Buildings)
        {
            GameObject GOCopy = Instantiate(GO, GO.transform.position, GO.transform.rotation);
            GOCopy.transform.localScale = new Vector3(GO.transform.localScale.x, 50, GO.transform.localScale.z);
            if (!Pivotal)
            {
                GOCopy.transform.position = Vector3.zero;
                Pivotal = true;

            }
            else
            {
                GOCopy.transform.position -= Pivot;
            }
            stencils.Add(GO);
        }
    }

   
    public void OnClickOpenEnvironmentUI()
    {
        EnvironmentUI.SetActive(true);
        MainButtonsUI.SetActive(false);
        canSelect = false;
        isEnvironmentalMode = true;
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
        isEnvironmentalMode = false;
        runtimeTransformGameObj.SetActive(false);
    }

    public void ResetToDefaults()
    {
        int i = 0;
        float rot = 0;
        foreach(CustomBuilding CB in CurrentBuildingsOnEditorDisplay)
        {
            Vector3 Distance = new Vector3(i, 0, 0);
            Vector3 rota = new Vector3(0, rot, 0);
            OverrideBuildingFloors(CB, DefaultFloorCount);
            OverrideBuildingTransforms(CB, Distance, rota);
           
            i -= 40;
            rot += 180;

            CB.GetComponent<ModularHDB>().RefreshFloors();
        }
     
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
    public void OverrideBuildingTransforms(int buildingIndex, Vector3? position = null, Vector3? rotation = null)
    {
        CurrentlySelectedBuilding = CurrentBuildingsOnEditorDisplay[buildingIndex];

        if (position != null)
        {
            CurrentlySelectedBuilding.transform.position = position.Value;
        }

        if (rotation != null)
        {
            Quaternion quaternionRotation = Quaternion.Euler(rotation.Value);
            CurrentlySelectedBuilding.transform.rotation = quaternionRotation;
        }
    }
    public void OverrideBuildingTransforms(CustomBuilding CB, Vector3? position = null, Vector3? rotation = null)
    {
        CurrentlySelectedBuilding = CB;

        if (position != null)
        {
            CurrentlySelectedBuilding.transform.position = position.Value;
        }

        if (rotation != null)
        {
            Quaternion quaternionRotation = Quaternion.Euler(rotation.Value);
            CurrentlySelectedBuilding.transform.rotation = quaternionRotation;
        }
    }

    public void RefreshCurrentBuildingFloors(int buildingIndex)
    {
        CurrentlySelectedBuilding = CurrentBuildingsOnEditorDisplay[buildingIndex];
        CurrentlySelectedBuilding.GetComponent<ModularHDB>().RefreshFloors();
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
    public void GoToEditorMenu()
    {
        isInEditMode = true;
        MainMenuObjects.SetActive(false);
        EditorObjects.SetActive(true);
        EditorUI.SetActive(true);
    }
    public void ExitFromEditorMenu()
    {

        CameraPerspectives[camIndex].SetActive(false);
        camIndex = 0;
        CameraPerspectives[camIndex].SetActive(true);

        isInEditMode = false;
        EditorUI.SetActive(false);
        MainMenuObjects.SetActive(true);
        EditorObjects.SetActive(false);
        runtimeTransformGameObj.SetActive(false);

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
            if (hit.collider.CompareTag("EditorHDB") && canSelect)
            {
                if (CurrentlySelectedBuilding == hit.collider.GetComponent<CustomBuilding>()) return;
                CurrentlySelectedBuilding = hit.collider.GetComponent<CustomBuilding>();
                if (CurrentlySelectedBuilding != null)
                {
                    BuildingDetailsUI.SetActive(true);
                    MainButtonsUI.SetActive(false);
                    TitleMenu.text = "Selected building: " + CurrentlySelectedBuilding.typeofBuilding.ToString();

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

                runtimeTransformHandle.target = CurrentlySelectedBuilding.transform;
                runtimeTransformGameObj.SetActive(true);
            }
            else if (hit.collider.CompareTag("EditorEnvironmentProp") && isEnvironmentalMode)
            {
                runtimeTransformHandle.target = hit.collider.transform;
                
                runtimeTransformGameObj.SetActive(true);
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
        runtimeTransformGameObj.SetActive(false);
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

    public void SaveNewBuildingPreset(bool Override = false)
    {
        if(string.IsNullOrEmpty(ScenarioNameInput.text))
        {
            PopupUIManager.Instance.ShowPopup("Error!", "Save name cannot be blank!");
            return;
        }
        if (!Override && editorSave.AlreadyHasSaveWithName(ScenarioNameInput.text, Override))
        {
            OverrideSaveUI.SetActive(true);
            NameOfSaveText.text = ScenarioNameInput.text;
            return;
        }
        else if(Override)
        {
            editorSave.AlreadyHasSaveWithName(ScenarioNameInput.text, Override);
        }
        List<SavableBuildingDetails> buildingDataBlock = new();
        foreach(CustomBuilding building in CurrentBuildingsOnEditorDisplay)
        {
            SavableBuildingDetails buildingData = new SavableBuildingDetails()
            {
                savedBuildingType = building.typeofBuilding,
                SavedNumFloors = building.numFloors,
                PosX = building.transform.position.x,
                PosY = building.transform.position.y,
                PosZ = building.transform.position.z,

                // Quaternion to vector3, then put into RotX...

                RotX = building.transform.rotation.eulerAngles.x,
                RotY = building.transform.rotation.eulerAngles.y,
                RotZ = building.transform.rotation.eulerAngles.z

        };
            buildingDataBlock.Add(buildingData);
        }
        PreviewSavedJsonString = JsonConvert.SerializeObject(buildingDataBlock);

        List<SavableEnvironmentDetails> EnvironmentDatablock = new();
        foreach(GameObject GO in editorEnvironmentManager.InstantiatedProps)
        {
            EnvironmentalPrefab EnvP = GO.GetComponent<EnvironmentalPrefab>();
            SavableEnvironmentDetails savableEnv = new SavableEnvironmentDetails()
            {
                savedItemIndex = EnvP.propIndex,
                PosX = GO.transform.position.x,
                PosY = GO.transform.position.y,
                PosZ = GO.transform.position.z,
                RotX = GO.transform.rotation.x,
                RotY = GO.transform.rotation.y,
                RotZ = GO.transform.rotation.z,
                ScaleX = GO.transform.localScale.x,
                ScaleY = GO.transform.localScale.y,
                ScaleZ = GO.transform.localScale.z
            };
            EnvironmentDatablock.Add(savableEnv);
        }
        //string s = JsonConvert.SerializeObject(editorEnvironmentManager.InstantiatedProps);
        string env = JsonConvert.SerializeObject(EnvironmentDatablock);
        Scenario newScenario = new Scenario()
        {
            JsonSave = PreviewSavedJsonString,
            EnvironmentJSON = env,
            NameOfScenario = ScenarioNameInput.text,
            DistanceBetweenBuildings = currentDistance

        };
        editorSave.CurrentlySavedScenarios.Add(newScenario);
        PopupUIManager.Instance.ShowPopup("Success", "Scenario saved successfully!");
    }
   

    private void Update()
    {
        if (!isInEditMode) return;

        UpdateDistance();
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

        if (runtimeTransformGameObj.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                runtimeTransformHandle.type = HandleType.POSITION;
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                runtimeTransformHandle.type = HandleType.ROTATION;
            }
            if (Input.GetKeyDown(KeyCode.R) && isEnvironmentalMode)
            {
                runtimeTransformHandle.type = HandleType.SCALE;
            }
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                if (Input.GetKeyDown(KeyCode.G))
                {
                    runtimeTransformHandle.space = HandleSpace.WORLD;
                }
                if (Input.GetKeyDown(KeyCode.L))
                {
                    runtimeTransformHandle.space = HandleSpace.LOCAL;
                }
            }
            HelperInstructions.SetActive(false);
            GizmosInstructions.SetActive(true);
        }
        else
        {
            // Check for left arrow key press
            if (Input.GetKeyDown(KeyCode.A))
            {
                // Move to the previous camera
                SwitchCamera(-1);
            }
            // Check for right arrow key press
            else if (Input.GetKeyDown(KeyCode.D))
            {
                // Move to the next camera
                SwitchCamera(1);
            }
            HelperInstructions.SetActive(true);
            GizmosInstructions.SetActive(false);
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
    public string JsonSave, EnvironmentJSON;
    public string NameOfScenario;
    public float DistanceBetweenBuildings;

}


[System.Serializable]
public class SavableBuildingDetails
{
    public int SavedNumFloors, SavedWidthInMetres, SavedAngleInDegrees;
    public CustomBuilding.BuildingType savedBuildingType;
    public float PosX, PosY, PosZ;
    public float RotX, RotY, RotZ;
}

[System.Serializable]
public class SavableEnvironmentDetails
{
    public int savedItemIndex;
    public float PosX, PosY, PosZ;
    public float RotX, RotY, RotZ;
    public float ScaleX, ScaleY, ScaleZ;
}

