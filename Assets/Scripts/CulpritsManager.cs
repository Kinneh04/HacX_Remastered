using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cinemachine;

public class CulpritsManager : MonoBehaviour
{
    public List<Culprit> SpawnedCulprits = new();

    public AreaSplitManager[] Floors;

    public GameObject CulpritPrefab;

    public int NumCulpritsPerRow;

    public CinemachineVirtualCamera culpritVCam;
    public Culprit SelectedCulprit;
    public bool isSelectingCulprit;
    public bool canSelectCulprit = true;

    public PostUIManager postUIManager;
    public CameraManager cameraManager;
    public MainGameManager mainGameManager;
    public GameObject CulpritUI;
    Vector3 OriginalPosition;

    [Header("CulpritStats")]
    public TMP_Text FloorText;
    public TMP_Text ColumnText, OverallAccuracyText, TotalBallsFiredText, BallsHitText, BallsMissedText, MtHRatioText;

    [Header("LookAt")]
    public Transform TargetBuilding;

    private void Start()
    {
        InitFloors();
        OriginalPosition = culpritVCam.transform.position;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && canSelectCulprit && !SelectedCulprit)
        {
            // Get mouse position
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);

            // Perform raycast
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                // Check if the hit object has the "Culprit" tag
                if (hitInfo.collider.CompareTag("Culprit"))
                {
                    SelectCulprit(hitInfo.collider.GetComponent<Culprit>());
                }
            }
        }
    }

    public void SelectCulprit(Culprit C)
    {
        mainGameManager.PostSimUI.SetActive(false);
        CulpritUI.SetActive(true);
        cameraManager.canMoveAndZoom = false;
        isSelectingCulprit = true;
        SelectedCulprit = C;
        culpritVCam.Follow = C.transform;
        culpritVCam.m_Lens.OrthographicSize = 3.0f;

        FloorText.text = "Floor: " + C.floor.ToString();
        ColumnText.text = "Column: " + C.column.ToString();
        OverallAccuracyText.text = "Overall Accuracy: " + C.averageProbability.ToString("F2") + "%";
        TotalBallsFiredText.text = "Total balls fired: " + C.totalBallsThrown.ToString();
        BallsHitText.text = "Balls hit: " + C.TotalBallsHit.ToString();
        BallsMissedText.text = "Balls missed: " + (C.totalBallsThrown - C.TotalBallsHit).ToString();
        float ratio = (float)C.TotalBallsHit / (C.totalBallsThrown - C.TotalBallsHit);
        MtHRatioText.text = "Ratio: " + ratio.ToString("F2"); // Format to two decimal places
    }

    public void DeselectCulprit()
    {
        mainGameManager.PostSimUI.SetActive(true);
        CulpritUI.SetActive(false);
        cameraManager.canMoveAndZoom = true;
        isSelectingCulprit = false;
        SelectedCulprit = null;
        culpritVCam.m_Lens.OrthographicSize = 22.0f;
        culpritVCam.Follow = null;
        culpritVCam.transform.position = OriginalPosition;
    }
    public void InitFloors()
    {

        AreaSplitManager[] Floors = GameObject.FindObjectsOfType<AreaSplitManager>();

        SpawnedCulprits.Clear();
        for (int i = 0; i < Floors.Length; i++)
        {
            Floors[i].Floor = Floors.Length - i;
            Floors[i].CulpritPrefab = CulpritPrefab;
            Floors[i].culpritManager = this;
            Floors[i].NumberOfCulprits = NumCulpritsPerRow;
            Floors[i].RespawnCulprits(); 
        }
    }

    public void ClearCulprits()
    {
        foreach(Culprit c in SpawnedCulprits)
        {
            Destroy(c.gameObject);
        }
    }

    public void AddCulprit(Culprit C)
    {
        SpawnedCulprits.Add(C);
    }
}
