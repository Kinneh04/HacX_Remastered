using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cinemachine;
using UnityEngine.EventSystems;

public class WindowsManager : MonoBehaviour
{
    public static WindowsManager Instance;
    public bool canSelectWindow;
    [SerializeField] private Color originalColor; // Store the original color of the window

    private GameObject CurrentlyHoveredWindow;

    [Header("Colors")]
    public Color HighlightedColor, SelectedColor, UnavailableColor;

    [Header("SelectedWindows")]
    public List<GameObject> SelectedWindows = new();
    public List<Precise_Window> PreciseWindows = new();
    public Precise_Window CurrentlySelectedPreciseWindow;
    public Precise_Window CurrentlySelectedRicochetWindow;
    public GameObject StartButton;
    public TMP_Text WindowCounter;

    [Header("PrecisionSelection")]

    [Header("Components")]
    public CinemachineVirtualCamera MainVCamera;
    public bool isPrecisionMode = false;
    public float TargetOrthoSize;
    public float OverviewOrthoSize = 22f;
    Vector3 OriginalCamPosition;
    Quaternion OriginalCamRotation;
    public Transform VCamOverviewFollower;
    public GameObject CurrentlySelectedWIndow;

    [Header("Marker")]
    public GameObject PrecisionMarkerHighlighter;
    public GameObject PrecisionMarkerPrefab;

    public GameObject RicochetMarkerHighlighter;
    public GameObject RicochetMarkerPrefab;

    [Header("Ricochet Marker")]
    

    [Header("UI")]
    public GameObject MainUI, WindowPrecisionUI, PrecisionMarkerSettingsUI;
    public Slider ConfidenceSlider;
    Vector3 OriginalPrefabScale;

    public GraphicRaycaster raycaster;

    private void Start()
    {
        OriginalCamPosition = MainVCamera.transform.position;
        OriginalCamRotation = MainVCamera.transform.rotation;
        OriginalPrefabScale = PrecisionMarkerPrefab.transform.localScale;
    }

    public bool isRegisteredPreciseWindow(GameObject Target)
    {
        foreach (Precise_Window PW in PreciseWindows)
        {
            if (Target == PW.WindowGO)
            {
                CurrentlySelectedPreciseWindow = PW;
                return true;
            }
        }
        return false;
    }
    public void GotoPrecision(GameObject Target)
    {
        if (!isRegisteredPreciseWindow(Target))
        {
            Precise_Window PW = new Precise_Window()
            { 
                PrecisionMarker = null,
                WindowGO = Target
            };
            PreciseWindows.Add(PW);
            CurrentlySelectedPreciseWindow = PW;

        }
     
        MainUI.SetActive(false);
        WindowPrecisionUI.SetActive(true);
        canSelectWindow = false;
        MainVCamera.Follow = Target.transform;
        TargetOrthoSize = 2.5f;
         //  MainVCamera.m_Lens.OrthographicSize = 2.5f;
         Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Declare a RaycastHit variable to store the hit information
        RaycastHit hit;
        isPrecisionMode = true;
        if (Physics.Raycast(ray, out hit))
        {
            PrecisionMarkerHighlighter.transform.position = hit.point;
        }
        PlacePrecisionMarker();
    }

    public void RemoveFromPreciseWindows(GameObject Window)
    {
        foreach (Precise_Window PW in PreciseWindows)
        {
            if (Window == PW.WindowGO)
            {
                if (PW.PrecisionMarker != null) Destroy(PW.PrecisionMarker);
                PreciseWindows.Remove(PW);
                return;
            }
        }
    }

    public void OnRemovePrecision()
    {
        if (CurrentlySelectedPreciseWindow.PrecisionMarker != null) Destroy(CurrentlySelectedPreciseWindow.PrecisionMarker);
        if (CurrentlySelectedPreciseWindow.RicochetMarker != null) Destroy(CurrentlySelectedPreciseWindow.RicochetMarker);
        PreciseWindows.Remove(CurrentlySelectedPreciseWindow);
        DeselectWindow(CurrentlySelectedWIndow);
       // RemoveFromPreciseWindows(CurrentlySelectedPreciseWindow.WindowGO);
        EndPrecision();
    }

    public void OnSavePrecision()
    {
        EndPrecision();
    }

    public void EndPrecision()
    {
        MainUI.SetActive(true);
        WindowPrecisionUI.SetActive(false);
        canSelectWindow = true;
        MainVCamera.Follow = VCamOverviewFollower;
        TargetOrthoSize = OverviewOrthoSize;
        if (CurrentlySelectedWIndow) CurrentlySelectedWIndow = null;
        MainVCamera.transform.position = OriginalCamPosition;
        MainVCamera.transform.rotation = OriginalCamRotation;
        isPrecisionMode = false;
    }

    private void Awake()
    {
        StartButton.SetActive(false);
        WindowCounter.text = "Windows Selected: " + 0;
        
        Instance = this;
    }

    void ToggleWindow()
    {
        //if (SelectedWindows.Contains(CurrentlyHoveredWindow))
        //{
        //    DeselectWindow(CurrentlyHoveredWindow);
        //}
        //else SelectWindow(CurrentlyHoveredWindow);
        SelectWindow(CurrentlyHoveredWindow);
    }

    void SelectWindow(GameObject GO)
    {
        GotoPrecision(GO);
        CurrentlySelectedWIndow = CurrentlyHoveredWindow;
        if (SelectedWindows.Contains(GO)) return;
       
        GO.GetComponent<MeshRenderer>().material.color = SelectedColor;
        SelectedWindows.Add(GO);
    }

    void DeselectWindow(GameObject GO)
    {

        GO.GetComponent<MeshRenderer>().material.color = originalColor;
        isPrecisionMode = false;
        SelectedWindows.Remove(GO);
    }

    public void PlacePrecisionMarker()
    {
        if (CurrentlySelectedPreciseWindow.PrecisionMarker) Destroy(CurrentlySelectedPreciseWindow.PrecisionMarker);
        GameObject GO = Instantiate(PrecisionMarkerPrefab, PrecisionMarkerHighlighter.transform.position, Quaternion.identity);
        GO.transform.localScale = PrecisionMarkerPrefab.transform.localScale;
        CurrentlySelectedPreciseWindow.PrecisionMarker = GO;
        OnUpdateConfidenceScale();        
    }

    public void PlaceRicochetMarker(Vector3 normal)
    {
        if (CurrentlySelectedPreciseWindow.RicochetMarker) Destroy(CurrentlySelectedPreciseWindow.RicochetMarker);
        GameObject GO = Instantiate(RicochetMarkerPrefab, RicochetMarkerHighlighter.transform.position, Quaternion.identity);
        GO.transform.localScale = RicochetMarkerPrefab.transform.localScale;
        //GO.GetComponent<Precise_Window>().CalculateRequiredAngleofIncidence();
        CurrentlySelectedPreciseWindow.RicochetMarker = GO;
        CurrentlySelectedPreciseWindow.ricochetNormal = normal;
        OnUpdateConfidenceScale();

    }
    public void OnUpdateConfidenceScale()
    {
        CurrentlySelectedPreciseWindow.PrecisionMarker.transform.localScale = OriginalPrefabScale * 1 / ConfidenceSlider.value;
        //Vector3 newScale = OriginalPrefabScale * 1 / ConfidenceSlider.value;
        //CurrentlySelectedPreciseWindow.PrecisionMarker.transform.localScale = newScale;
        //CurrentlySelectedPreciseWindow.RicochetMarker.transform.localScale = newScale;
    }

    private void Update()
    {
       

        if (MainVCamera.m_Lens.OrthographicSize != TargetOrthoSize)
            MainVCamera.m_Lens.OrthographicSize = Mathf.Lerp(MainVCamera.m_Lens.OrthographicSize, TargetOrthoSize, Time.deltaTime * 3);
        //if (!canSelectWindow) return;

   
        // Create a ray from the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Declare a RaycastHit variable to store the hit information
        RaycastHit hit;
        if (CurrentlySelectedPreciseWindow != null && CurrentlySelectedPreciseWindow.PrecisionMarker != null)
        {
            PrecisionMarkerSettingsUI.SetActive(true);
        }
        else PrecisionMarkerSettingsUI.SetActive(false);
        // Perform the raycast
        PrecisionMarkerHighlighter.SetActive(false);
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        if (Physics.Raycast(ray, out hit))
        {
            // Check if the hit object has the tag "window"
            if (hit.collider.CompareTag("Window"))
            {
                RicochetMarkerHighlighter.SetActive(false);

                CurrentlyHoveredWindow = hit.collider.gameObject;
                if (isPrecisionMode && CurrentlyHoveredWindow == CurrentlySelectedPreciseWindow.WindowGO)
                {
                    PrecisionMarkerHighlighter.SetActive(true);
                    PrecisionMarkerHighlighter.transform.position = hit.point;

                    if(Input.GetMouseButtonDown(0))
                    {
                        PlacePrecisionMarker();
                    }
                }
                if (!SelectedWindows.Contains(CurrentlyHoveredWindow))
                {
                    Material M = CurrentlyHoveredWindow.GetComponent<MeshRenderer>().material;
                    //  originalColor = M.color;
                    M.color = HighlightedColor;
                }
               
            }
            else
            {
                PrecisionMarkerHighlighter.SetActive(false);
                //Stop hover on window
                if (CurrentlyHoveredWindow)
                {
                    if (!SelectedWindows.Contains(CurrentlyHoveredWindow))
                        CurrentlyHoveredWindow.GetComponent<MeshRenderer>().material.color = originalColor;
                    CurrentlyHoveredWindow = null;
                }

                if (CurrentlySelectedPreciseWindow != null && isPrecisionMode)
                {
                    RicochetMarkerHighlighter.SetActive(true);
                    RicochetMarkerHighlighter.transform.position = hit.point;

                    if (Input.GetMouseButtonDown(0))
                    {
                        PlaceRicochetMarker(hit.normal);
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(0) && CurrentlyHoveredWindow && canSelectWindow)
        {
            ToggleWindow();
          
            StartButton.SetActive(SelectedWindows.Count > 0);
        }
        WindowCounter.text = "Windows Selected: " + SelectedWindows.Count;
    }
}

[System.Serializable]
public class Precise_Window
{
    public float windowThickness = 10f; // window thickness in mm
    public float windowLength = 2500f;
    public float windowHeight = 1500f;

    public float breakingStress = 18f; // maximum allowable stress in MPa (N/m^2)

    public GameObject PrecisionMarker, WindowGO;

    public GameObject RicochetMarker;
    public Vector3 ricochetNormal;
    public float reflectionAngle = 0;
    public float estimatedAngleOfIncidence = 0;

    public void CalculateRequiredAngleofIncidence()
    {
        if(RicochetMarker == null)
        {
            Debug.Log("no ricochet point");
            return;
        }
        Vector3 reflectVector = Vector3.Reflect(PrecisionMarker.transform.position - RicochetMarker.transform.position, ricochetNormal);
        reflectionAngle = Vector3.Angle(ricochetNormal, reflectVector);
        estimatedAngleOfIncidence = reflectionAngle;
    }
}
