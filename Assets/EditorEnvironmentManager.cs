using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnotherFileBrowser.Windows;
using UnityEngine.Networking;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using TMPro;
using Dummiesman; //Load OBJ Model

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

    public Transform EditorParentTransform;

    [Header("CustomOBJ")]
    public GameObject model;
    public Transform ParentPlatform;
    public GameObject ModelViewScreen, ImportScreen;
    public TMP_InputField CustomPropNameInput;
    public EditorManager editorManager;
    
    public void ClearAllProps()
    {
        foreach(GameObject GO in InstantiatedPropButtons)
        {
            DeleteTiedProp(GO.GetComponent<PropButtonPrefab>().TiedProp, GO, false);
        }
        InstantiatedPropButtons.Clear();
    }

    public void AddCustomPropToEditor()
    {

    }
    public void OpenFileBrowser()
    {
        var bp = new BrowserProperties();
        bp.filterIndex = 0;

        new FileBrowser().OpenFileBrowser(bp, path =>
        {
            //Load image from local path with UWR
            StartCoroutine(OutputModel(path));
        });
    }

    IEnumerator OutputModel(string path)
    {

        if(model)
        {
            Destroy(model);
        }
        UnityWebRequest www = UnityWebRequest.Get(path);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("WWW ERROR: " + www.error);

            PopupUIManager.Instance.ShowPopup("Error!", www.error);
        }
        else
        {
            ModelViewScreen.SetActive(true);
            ImportScreen.SetActive(false);
            //textMeshPro.text = www.downloadHandler.text;

            //Load OBJ Model
            MemoryStream textStream = new MemoryStream(Encoding.UTF8.GetBytes(www.downloadHandler.text));
            if (model != null)
            {
                Destroy(model);
            }
            model = new OBJLoader().Load(textStream);
            model.transform.localScale = new Vector3(-1, 1, 1); // set the position of parent model. Reverse X to show properly 
            FitOnScreen();
            DoublicateFaces();
            AddMeshColliderToModel();

            model.transform.SetParent(ParentPlatform);
        }
    }
    private Bounds GetBound(GameObject gameObj)
    {
        Bounds bound = new Bounds(gameObj.transform.position, Vector3.zero);
        var rList = gameObj.GetComponentsInChildren(typeof(Renderer));
        foreach (Renderer r in rList)
        {
            bound.Encapsulate(r.bounds);
        }
        return bound;
    }

    public void AddMeshColliderToModel()
    {
        // Iterate through all children of the model
        foreach (Transform child in model.transform)
        {
            // Check if the child has a MeshRenderer component
            MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                // Add a MeshCollider to the child
                MeshCollider meshCollider = child.gameObject.GetComponent<MeshCollider>();
                if (meshCollider == null)
                {
                    meshCollider = child.gameObject.AddComponent<MeshCollider>();
                    meshCollider.convex = true;
                    // Optionally, you can set other properties of the MeshCollider here
                }

                child.gameObject.layer = LayerMask.NameToLayer("CustomEditorProp");
               
            }
        }

        BoxCollider BC = model.AddComponent<BoxCollider>();
        BC.size = new Vector3(2, 2, 2);
        BC.isTrigger = true;
        model.tag = "EditorEnvironmentProp";
    }

    public void FitOnScreen()
    {
        Bounds bound = GetBound(model);
        Vector3 boundSize = bound.size;
        float diagonal = Mathf.Sqrt((boundSize.x * boundSize.x) + (boundSize.y * boundSize.y) + (boundSize.z * boundSize.z)); //Get box diagonal
        Camera.main.orthographicSize = diagonal / 2.0f;
        Camera.main.transform.position = bound.center;
    }



    // Doublicate the size of mesh components, in which the second half of the tringles winding order and normals are reverse of the first half to enable displaying front and back faces
    //https://answers.unity.com/questions/280741/how-make-visible-the-back-face-of-a-mesh.html
    public void DoublicateFaces()
    {
        for (int i = 0; i < model.GetComponentsInChildren<Renderer>().Length; i++) //Loop through the model children
        {
            // Get oringal mesh components: vertices, normals triangles and texture coordinates 
            Mesh mesh = model.GetComponentsInChildren<MeshFilter>()[i].mesh;
            Vector3[] vertices = mesh.vertices;
            int numOfVertices = vertices.Length;
            Vector3[] normals = mesh.normals;
            int[] triangles = mesh.triangles;
            int numOfTriangles = triangles.Length;
            Vector2[] textureCoordinates = mesh.uv;
            if (textureCoordinates.Length < numOfTriangles) //Check if mesh doesn't have texture coordinates 
            {
                textureCoordinates = new Vector2[numOfVertices * 2];
            }

            // Create a new mesh component, double the size of the original 
            Vector3[] newVertices = new Vector3[numOfVertices * 2];
            Vector3[] newNormals = new Vector3[numOfVertices * 2];
            int[] newTriangle = new int[numOfTriangles * 2];
            Vector2[] newTextureCoordinates = new Vector2[numOfVertices * 2];

            for (int j = 0; j < numOfVertices; j++)
            {
                newVertices[j] = newVertices[j + numOfVertices] = vertices[j]; //Copy original vertices to make the second half of the mew vertices array
                newTextureCoordinates[j] = newTextureCoordinates[j + numOfVertices] = textureCoordinates[j]; //Copy original texture coordinates to make the second half of the mew texture coordinates array  
                newNormals[j] = normals[j]; //First half of the new normals array is a copy original normals
                newNormals[j + numOfVertices] = -normals[j]; //Second half of the new normals array reverse the original normals
            }

            for (int x = 0; x < numOfTriangles; x += 3)
            {
                // copy the original triangle for the first half of array
                newTriangle[x] = triangles[x];
                newTriangle[x + 1] = triangles[x + 1];
                newTriangle[x + 2] = triangles[x + 2];
                // Reversed triangles for the second half of array
                int j = x + numOfTriangles;
                newTriangle[j] = triangles[x] + numOfVertices;
                newTriangle[j + 2] = triangles[x + 1] + numOfVertices;
                newTriangle[j + 1] = triangles[x + 2] + numOfVertices;
            }
            mesh.vertices = newVertices;
            mesh.uv = newTextureCoordinates;
            mesh.normals = newNormals;
            mesh.triangles = newTriangle;
        }
    }


    public void ImportCustomEditorProp()
    {
        OpenFileBrowser();
    }

    public void OnClickHelperButton(PropButtonPrefab prefab)
    {
        Color originalColor = prefab.TiedProp.GetComponent<EnvironmentalPrefab>().outline.OutlineColor;
        originalColor.a = 1;
        prefab.TiedProp.GetComponent<EnvironmentalPrefab>().outline.OutlineColor = originalColor;
        editorManager.runtimeTransformGameObj.SetActive(true);
        editorManager.runtimeTransformHandle.target = prefab.TiedProp.transform;
    }

    public void OnClickAddCustomProp()
    {
        SpawnCustomEnvironmentProp(model);
    }

    public void SpawnCustomEnvironmentProp(GameObject model)
    {
        if(string.IsNullOrEmpty(CustomPropNameInput.text))
        {

            PopupUIManager.Instance.ShowPopup("Error!", "Enter a valid name for your custom prop!");
            return;
        }

        GameObject NewProp = Instantiate(model, EnvironmentSpawnPoint.transform.position, Quaternion.identity);
        GameObject NewPropButton = Instantiate(PropNamePrefab);
        NewPropButton.transform.SetParent(PropNamePrefabParent, false);
        PropButtonPrefab script = NewPropButton.GetComponent<PropButtonPrefab>();
        NewPropButton.transform.SetAsFirstSibling();

        script.propNameText.text = CustomPropNameInput.text;
        script.TiedProp = NewProp;

        script.propHelperButton.onClick.AddListener(delegate { OnClickHelperButton(script); });
        script.propDeleteButton.onClick.AddListener(delegate { DeleteTiedProp(NewProp, NewPropButton); });
        InstantiatedPropButtons.Add(NewPropButton);
        InstantiatedProps.Add(NewProp);
        EnvironmentalPrefab EP = NewProp.GetComponent<EnvironmentalPrefab>();
        if (!EP)
        {
            EP = NewProp.AddComponent<EnvironmentalPrefab>();
            Outline O =  NewProp.AddComponent<Outline>();
            O.OutlineColor = Color.cyan;
            EP.outline = O;
        }
        EP.propIndex = -1;
        NewProp.transform.SetParent(EditorParentTransform);
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
        NewProp.transform.SetParent(EditorParentTransform);

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
        editorManager.runtimeTransformGameObj.SetActive(false);
        Destroy(GO);
        if(removeFromList)
        InstantiatedPropButtons.Remove(self);
        InstantiatedProps.Remove(GO);
        Destroy(self);
    }
}
