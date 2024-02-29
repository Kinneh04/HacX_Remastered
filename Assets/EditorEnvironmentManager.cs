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

    [Header("UI")]
    public GameObject PropNamePrefab;
    public Transform PropNamePrefabParent;

    public List<GameObject> InstantiatedPropButtons, InstantiatedProps;
    
    public void ClearAllProps()
    {
        foreach(GameObject GO in InstantiatedPropButtons)
        {
            DeleteTiedProp(GO.GetComponent<PropButtonPrefab>().TiedProp, GO, false);
        }
        InstantiatedPropButtons.Clear();
    }

    public void OnClickHelperButton(PropButtonPrefab prefab)
    {
        Color originalColor = prefab.TiedProp.GetComponent<EnvironmentalPrefab>().outline.OutlineColor;
        originalColor.a = 1;
        prefab.TiedProp.GetComponent<EnvironmentalPrefab>().outline.OutlineColor = originalColor;
    }

    public void SpawnEnvironmentProp(int itemIndex)
    {
        GameObject NewProp = Instantiate(DontDestroyOnLoadSettings.Instance.EnvironmentalPrefabs[itemIndex], EnvironmentSpawnPoint.transform.position, Quaternion.identity);
        GameObject NewPropButton = Instantiate(PropNamePrefab);
        NewPropButton.transform.SetParent(PropNamePrefabParent,false);
        PropButtonPrefab script = NewPropButton.GetComponent<PropButtonPrefab>();
        NewPropButton.transform.SetAsFirstSibling();
        
        script.propNameText.text = DontDestroyOnLoadSettings.Instance.EnvironmentalPrefabs[itemIndex].name;
        script.TiedProp = NewProp;
      
        script.propHelperButton.onClick.AddListener(delegate { OnClickHelperButton(script); });
        script.propDeleteButton.onClick.AddListener(delegate { DeleteTiedProp(NewProp, NewPropButton); });
        InstantiatedPropButtons.Add(NewPropButton);
        InstantiatedProps.Add(NewProp);

        NewProp.GetComponent<EnvironmentalPrefab>().propIndex = itemIndex;

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

    public void DeleteTiedProp(GameObject GO, GameObject self, bool removeFromList = true)
    {
        Destroy(GO);
        if(removeFromList)
        InstantiatedPropButtons.Remove(self);
        InstantiatedProps.Remove(GO);
        Destroy(self);
    }
}
