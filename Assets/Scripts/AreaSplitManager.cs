using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSplitManager : MonoBehaviour
{
    public GameObject CulpritPrefab;
    public int NumberOfCulprits = 5;
    public float DeadzoneRadius = 0.5f;
    public List<GameObject> SpawnedCulprits = new();
    public int Floor = 0;
    
    public CulpritsManager culpritManager;
    public Transform Parent;

    public bool spawnOnStart = false;
    private void Start()
    {
        if (spawnOnStart) RespawnCulprits();
    }

    public bool inverse = false;
    public void RespawnCulprits()
    {
        // if (!mainGameManager) mainGameManager = MainGameManager.instance;

        foreach(GameObject C in SpawnedCulprits)
        {
            Destroy(C);
        }
        SpawnedCulprits.Clear();
        Vector3 boxSize = GetComponent<Renderer>().bounds.size;
        float areaWidth = boxSize.x / NumberOfCulprits;
        int s = 1;
        for (int i = 0; i < NumberOfCulprits; i++)
        {

            float spawnPositionX = transform.position.x - (boxSize.x / 2) + (areaWidth * i) + (areaWidth / 2);
            Vector3 spawnPosition = new Vector3(spawnPositionX, transform.position.y, transform.position.z);

            if (!CheckCollision(spawnPosition))
            {
                GameObject GO = Instantiate(CulpritPrefab, spawnPosition, transform.rotation);

                GO.transform.SetParent(Parent);
                if(inverse)
                {
                    GO.transform.Rotate(0, 180, 0);
                }
                // GO.transform.rotation = transform.rotation;
                if (culpritManager)
                { 
                    culpritManager.AddCulprit(GO.GetComponent<Culprit>());
                    GO.name = "Culprit " + culpritManager.SpawnedCulprits.Count;
                    Culprit culprit = GO.GetComponent<Culprit>();
                    culprit.column = s;
                    culprit.floor = Floor;
                }
             
                SpawnedCulprits.Add(GO);
                s++;
              //  mainGameManager.CulpritPositions.Add(culprit.ShootPosition.position);
            }
        }
    }
    bool CheckCollision(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, DeadzoneRadius); // Adjust the radius as needed

        return colliders.Length > 0;
    }


    // DEPRECIATED
    //public int numberOfAreas = 5; // Set the number of equally distanced areas here
    //[Tooltip("How far the area has to be from a collidable object to actually spawn properly")]
    //public float DeadzoneRadius = 0.5f;
    //public GameObject objectToSpawn; // The object you want to spawn
    //public MainGameManager mainGameManager;
    //public int row;
    //private void Start()
    //{
    //    SpawnCulprits();
    //}
    //bool CheckCollision(Vector3 position)
    //{
    //    Collider[] colliders = Physics.OverlapSphere(position, DeadzoneRadius); // Adjust the radius as needed

    //    return colliders.Length > 0;
    //}

    //public void SpawnCulprits()
    //{
    //    if (!mainGameManager) mainGameManager = MainGameManager.instance;
    //    Vector3 boxSize = GetComponent<Renderer>().bounds.size;
    //    float areaWidth = boxSize.x / numberOfAreas;
    //    int s = 1;
    //    for (int i = 0; i < numberOfAreas; i++)
    //    {

    //        float spawnPositionX = transform.position.x - (boxSize.x / 2) + (areaWidth * i) + (areaWidth / 2);
    //        Vector3 spawnPosition = new Vector3(spawnPositionX, transform.position.y, transform.position.z);

    //        if (!CheckCollision(spawnPosition))
    //        {
    //            GameObject GO =Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
    //            mainGameManager.SpawnedCulprits.Add(GO);
    //            GO.name = "Culprit" + mainGameManager.SpawnedCulprits.Count;
    //            Culprit culprit = GO.GetComponent<Culprit>();
    //            culprit.column = s;
    //            culprit.row = row;
    //            s++;
    //            mainGameManager.CulpritPositions.Add(culprit.ShootPosition.position);
    //        }
    //    }
    //}
}
