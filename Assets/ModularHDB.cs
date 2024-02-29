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
    Vector3 OriginalHDBFloorPivot;
    public List<GameObject> InstantiatedFloors = new();

    private void Start()
    {
        OriginalHDBFloorPivot = HDBFloorPivot.transform.position;
        ChangeFloors(numfloors);
    }

    public void ChangeFloors(int num)
    {
        numfloors = num;
        foreach(GameObject GO in InstantiatedFloors)
        {
            Destroy(GO);
        }

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
            if(i == numfloors - 1)
            {
                ModFloor.GetComponent<HDBFloor>().Roof.SetActive(true);
            }
        }
    }
   
}
