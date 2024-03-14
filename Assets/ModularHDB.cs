using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModularHDB : MonoBehaviour
{
    [Range(1,30)]
    public int numfloors = 5;
    public GameObject ModularHDBTopPrefab;
    public Transform HDBFloorPivot;
    public float floorHeightAdjustment = 3.61f;
    public Vector3 OriginalHDBFloorPivot;
    public List<GameObject> InstantiatedFloors = new();
    [Range(4, 15)]
    public int NumWindowsPerFloor = 7;

    public bool instantiateOnStart = true;

    public List<WindowSpawnArea> WindowsSplits = new();

    private void Awake()
    {
        OriginalHDBFloorPivot = HDBFloorPivot.transform.position;
    }

    private void Start()
    {
      //  OriginalHDBFloorPivot = HDBFloorPivot.transform.position;

        if(instantiateOnStart)
        ChangeFloors(numfloors);
    }

    public void RefreshFloors()
    {
        ChangeFloors(numfloors);
    }

    public void ChangeFloors(int num)
    {
        numfloors = num;
        foreach(GameObject GO in InstantiatedFloors)
        {
            Destroy(GO);
        }
        InstantiatedFloors.Clear();

        HDBFloorPivot.position = new Vector3(HDBFloorPivot.position.x, OriginalHDBFloorPivot.y, HDBFloorPivot.position.z);


        for (int i = 0; i < numfloors; i++)
        {
            GameObject ModFloor = Instantiate(ModularHDBTopPrefab, HDBFloorPivot.position, HDBFloorPivot.rotation);
            ModFloor.transform.SetParent(transform);
            ModFloor.transform.localScale = new Vector3(1, 1, 1);
            Vector3 CurrentFLoorPivotPosition = HDBFloorPivot.position;
            CurrentFLoorPivotPosition.y += floorHeightAdjustment;
            InstantiatedFloors.Add(ModFloor);
            HDBFloorPivot.position = CurrentFLoorPivotPosition;
            HDBFloor floor = ModFloor.GetComponent<HDBFloor>();
            if (i == numfloors - 1)
            {
                floor.Roof.SetActive(true);
            }
            foreach (WindowSpawnArea WSA in floor.windowAreas)
            {
                WSA.ChangeWindowCount(NumWindowsPerFloor);
            }
        }

        foreach(WindowSpawnArea WSA in WindowsSplits)
        {
            WSA.ChangeWindowCount(NumWindowsPerFloor);
        }
    }

    public void RefreshAllWindowCount(int newFloor)
    {
        numfloors = newFloor;
        foreach (GameObject GO in InstantiatedFloors)
        {
            foreach(WindowSpawnArea WSA in GO.GetComponent<HDBFloor>().windowAreas)
            {
                WSA.ChangeWindowCount(newFloor);
            }

           
        }

        foreach (WindowSpawnArea WSA in WindowsSplits)
        {
            WSA.ChangeWindowCount(newFloor);
        }
    }

}
