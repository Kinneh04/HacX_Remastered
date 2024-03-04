using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoBuildingsManager : MonoBehaviour
{
    public List<ModularHDB> modularBuildings = new();

    public float maxInterval;
    public int minFloors, maxFloors;
    float currentinterval;
    public void RandomizeAllBuildings()
    {
        foreach(ModularHDB MHDB in modularBuildings)
        {
            int RandomFLoor = Random.Range(minFloors, maxFloors);
            MHDB.ChangeFloors(RandomFLoor);
        }

        currentinterval = maxInterval;
    }

    private void Update()
    {
        currentinterval -= Time.deltaTime;
        if (currentinterval < 0) RandomizeAllBuildings();
    }
}
