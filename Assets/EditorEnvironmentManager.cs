using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorEnvironmentManager : MonoBehaviour
{
    // All environment items must come with the EnvironmentProp class and a mesh collider for handling

    public Transform EnvironmentSpawnPoint;
    public bool canSelectProp = true;
    public LayerMask editorFloorLayer;
    public GameObject CurrentlyMovingObject;

    public void SpawnEnvironmentProp(GameObject ItemPrefab)
    {
        GameObject NewProp = Instantiate(ItemPrefab, EnvironmentSpawnPoint.transform.position, Quaternion.identity);
    }

    public void OnPickupProp(GameObject ObjectToPickup)
    {
        CurrentlyMovingObject = ObjectToPickup;
    }

    public void OnSetDownProp()
    {
        CurrentlyMovingObject = null;
    }

    private void Update()
    {

        if (Input.GetMouseButtonDown(0) && CurrentlyMovingObject == null)
        {
            // Create a ray from the mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Check if the ray hits something
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // Check if the hit object has the tag "Window"
                if (hit.collider.CompareTag("EditorEnvironmentProp"))
                {
                    OnPickupProp(hit.collider.gameObject);
                }
            }
        }
        if(Input.GetMouseButtonUp(0))
        {
            OnSetDownProp();
        }

        if(CurrentlyMovingObject)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Check if the ray hits something on the "EditorFloor" layer
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, editorFloorLayer))
            {
                // Instantiate the object at the hit point
                CurrentlyMovingObject.transform.position = hit.point;
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                // Rotate the instantiated object by 45 degrees in the Y-axis
                CurrentlyMovingObject.transform.Rotate(0f, 45f, 0f);
            }
        }
    }
}
