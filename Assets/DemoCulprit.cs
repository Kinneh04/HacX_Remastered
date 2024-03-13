using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoCulprit : MonoBehaviour
{
    public ObjectPooler objectPooler;
    public float spawnInterval = 2f;
    public float spawnRadius = 5f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnObject();
            timer = 0f;
        }
    }

    void SpawnObject()
    {
        if (!objectPooler) return;
        GameObject obj = objectPooler.GetPooledObject();

        if (obj != null)
        {
            // Calculate a random rotation
            Quaternion randomRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

            // Calculate a random position within spawn radius
            Vector3 spawnPosition = transform.position + Random.insideUnitSphere * spawnRadius;

            // Set the position and rotation of the object
            obj.transform.position = spawnPosition;
            obj.transform.rotation = randomRotation;

            // Activate the object
            obj.SetActive(true);
        }
    }
}
