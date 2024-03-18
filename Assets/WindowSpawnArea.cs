using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowSpawnArea : MonoBehaviour
{
    public GameObject WindowPrefab;
    [Range(4, 15)]
    public int NumberOfWindows = 7;
    public float DeadzoneRadius = 0.5f;
    public List<GameObject> spawnedWindows = new();

    public Transform Parent;
    public bool inverse = false;

    public Vector3 MinSpawnScale, MaxSpawnScale;

    public bool CulpritWindowType = false;

  //  public LayerMask layers;

    private void Start()
    {
       // RespawnWindows();
    }
    Vector3 GenerateRandomVector()
    {
        float x = Random.Range(MinSpawnScale.x, MaxSpawnScale.x);
        float y = Random.Range(MinSpawnScale.y, MaxSpawnScale.y);
        float z = Random.Range(MinSpawnScale.z, MaxSpawnScale.z);

        return new Vector3(x, y, z);
    }
    public void ChangeWindowCount(int n)
    {
        NumberOfWindows = n;
        RespawnWindows();
    }
    public void RespawnWindows()
    {
        foreach (GameObject C in spawnedWindows)
        {
            Destroy(C);
        }
        spawnedWindows.Clear();
        Vector3 boxSize = GetComponent<Renderer>().bounds.size;
        float areaWidth = boxSize.x / NumberOfWindows;
        int s = 1;
        for (int i = 0; i < NumberOfWindows; i++)
        {

            float spawnPositionX = transform.position.x - (boxSize.x / 2) + (areaWidth * i) + (areaWidth / 2);
            Vector3 spawnPosition = new Vector3(spawnPositionX, transform.position.y, transform.position.z);
          //  if (!CheckCollision(spawnPosition))
            {
               
                GameObject GO = Instantiate(WindowPrefab, spawnPosition, transform.rotation);
                GO.transform.SetParent(Parent, true);
                if (inverse)
                {
                    GO.transform.Rotate(0, 180, 0);
                }
                // GO.transform.rotation = transform.rotation;
                spawnedWindows.Add(GO);
                GO.transform.localScale = GenerateRandomVector();
                s++;
                GO.layer = gameObject.layer;
                if (CulpritWindowType) GO.tag = "Untagged";
                //  mainGameManager.CulpritPositions.Add(culprit.ShootPosition.position);
            }
        }
    }
    bool CheckCollision(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, DeadzoneRadius); // Adjust the radius as needed

        return colliders.Length > 0;
    }
}
