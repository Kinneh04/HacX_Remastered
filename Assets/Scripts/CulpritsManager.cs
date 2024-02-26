using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CulpritsManager : MonoBehaviour
{
    public List<Culprit> SpawnedCulprits = new();

    public AreaSplitManager[] Floors;

    public GameObject CulpritPrefab;

    public int NumCulpritsPerRow;

    private void Start()
    {
      //  InitFloors();
    }

    public void InitFloors()
    {
        Floors = GameObject.FindObjectsOfType<AreaSplitManager>();
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
