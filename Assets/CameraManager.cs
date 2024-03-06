using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;

public class CameraManager : MonoBehaviour
{
    [Header("OverviewCamera")]
    private bool isMiddleMouseButtonHeld = false;
    public Transform CameraPivot;
    public float Sensitivity = 1.0f;
    public CinemachineVirtualCamera cam;
    public float zoomSpeed = 5f;
    public bool canMoveAndZoom = true;

    [Header("Cameras")]
    public List<VCam> vCamList = new();
    private int vCamIndex = 0;
    public int CulpritCameraIndex = 2;
    public TMP_Text CurrentCameraText;
    public Camera mainCam;

    [Header("FreeCamPersp")]
    public bool freecamMode = false;
    public CinemachineVirtualCamera freeCam;

    public float moveSpeed = 5f;
    public float lookSpeed = 2f;
    public float scrollSpeed = 5f;

    public bool isDragging = false;
    private Vector3 lastMousePosition;

    public GameObject PostUI, FreecamUI;

    public VCam currentlyUsingCamera;

    private void Start()
    {
        ForceCameraToIndex(0);
    }

    public void ForceCameraToIndex(int index)
    {
        vCamList[vCamIndex].vCam.SetActive(false);

        // Move to the next or previous camera
        vCamIndex = index;

        // Set the new camera active
        vCamList[vCamIndex].vCam.SetActive(true);
        CurrentCameraText.text = "Current camera: " + vCamList[vCamIndex].CamName;

        ChangeCurrentCameraPerspective();
    }

    public void SwitchToFreecam(bool toggle)
    {
        freecamMode = toggle;
        freeCam.transform.position = mainCam.transform.position;
        freeCam.transform.rotation = mainCam.transform.rotation;

        foreach (VCam cam in vCamList)
        {
            cam.vCam.SetActive(false);
        }
        freeCam.gameObject.SetActive(toggle);
        mainCam.orthographic = !toggle;

        if(!WindowsManager.Instance.isPrecisionMode)
        {
            PostUI.SetActive(!toggle);
        }
        else
        {

        }
        FreecamUI.SetActive(toggle);
        if (!toggle) ForceCameraToIndex(0);
    }

    public void ChangeCurrentCameraPerspective()
    {
         mainCam.orthographic = vCamList[vCamIndex].perspective == VCam.Pers.Orthographic;
        mainCam.cullingMask = vCamList[vCamIndex].RenderedLayers;

    }

    public void OnEndSimulation()
    {
        ForceCameraToIndex(CulpritCameraIndex);
    }
    void SwitchCamera(int direction)
    {
        // Set the current camera inactive
        vCamList[vCamIndex].vCam.SetActive(false);

        // Move to the next or previous camera
        vCamIndex = (vCamIndex + direction + vCamList.Count) % vCamList.Count;

        // Set the new camera active
        vCamList[vCamIndex].vCam.SetActive(true);

        CurrentCameraText.text ="Current camera: " + vCamList[vCamIndex].CamName;

        ChangeCurrentCameraPerspective();
    }
    void Update()
    {
        if (canMoveAndZoom && !freecamMode)
        {

            // Check for left arrow key press
            if (Input.GetKeyDown(KeyCode.A))
            {
                // Move to the previous camera
                SwitchCamera(-1);
            }
            // Check for right arrow key press
            else if (Input.GetKeyDown(KeyCode.D))
            {
                // Move to the next camera
                SwitchCamera(1);
            }

            // Check if the middle mouse button is held down
            if (Input.GetMouseButtonDown(2))
            {
                isMiddleMouseButtonHeld = true;
            }
            else if (Input.GetMouseButtonUp(2))
            {
                isMiddleMouseButtonHeld = false;
            }

            // Move the GameObject if the middle mouse button is held down
            if (isMiddleMouseButtonHeld && vCamList[vCamIndex].transformable)
            {
                // Get mouse movement on the x and y axes
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");

                // Adjust the GameObject's position based on the mouse movement
                vCamList[vCamIndex].vCam.transform.Translate(new Vector3(-mouseX * Sensitivity, -mouseY * Sensitivity, 0) * Time.deltaTime);
            }

            // Check for scroll input
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            // Adjust orthographic size based on scroll input
            if (scroll != 0f)
            {

             
                if (vCamList[vCamIndex].virtualCamera == WindowsManager.Instance.MainVCamera)
                { 
                    // Get the current orthographic size
                    float currentSize = WindowsManager.Instance.OverviewOrthoSize;

                    // Calculate the new orthographic size after scrolling
                    float newSize = Mathf.Clamp(currentSize - scroll * zoomSpeed, 1f, Mathf.Infinity);

                    // Set the new orthographic size
                    WindowsManager.Instance.OverviewOrthoSize = newSize;
                    if (!WindowsManager.Instance.isPrecisionMode)
                    {
                        WindowsManager.Instance.TargetOrthoSize = WindowsManager.Instance.OverviewOrthoSize;
                    }
                }
                else
                {
                    float addtoZoom = scroll * zoomSpeed;

                    vCamList[vCamIndex].virtualCamera.m_Lens.OrthographicSize += addtoZoom;

                }
            }
        }
        else if(freecamMode)
        {
            //HandleLookAround();

            HandleMovement();
            HandleScroll();

            if (Input.GetKeyDown(KeyCode.E))
            {
                SwitchToFreecam(false);
            }
        }

        if(WindowsManager.Instance.isPrecisionMode)
        {
            if (Input.GetMouseButtonDown(2))
                SwitchToFreecam(!freecamMode);

            if (!freecamMode)
                return;
            
        }

        if (Input.GetKeyDown(KeyCode.Y))
            Time.timeScale = 3;
        else if (Input.GetKeyUp(KeyCode.Y))
            Time.timeScale = 1.0f;
    }

    //private void HandleMovement()
    //{
    //    float horizontal = Input.GetAxis("Horizontal");
    //    float vertical = Input.GetAxis("Vertical");

    //    Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;
    //    Vector3 moveAmount = moveDirection * moveSpeed * Time.deltaTime;
    //    freeCam.transform.Translate(moveAmount, Space.Self);
    //}

    private void HandleMovement()
    {
        //if (Input.GetMouseButton(1)) // Right mouse button
        //{
        //    // Get mouse input
        //    float mouseX = Input.GetAxis("Mouse X");
        //    float mouseY = Input.GetAxis("Mouse Y");

        //    // Rotate the camera based on mouse input
        //    transform.Rotate(Vector3.up * mouseX * lookSpeed, Space.World);
        //    transform.Rotate(Vector3.left * mouseY * lookSpeed, Space.Self);
        //}
        if (Input.GetMouseButtonDown(1)) // Check for right mouse button press
        {
            isDragging = true;

            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(1)) // Check for right mouse button release
        {
            isDragging = false;
        }


        if (isDragging)
        {

            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            freeCam.transform.Translate(Vector3.right * horizontalInput * 10 * Time.deltaTime);
            freeCam.transform.Translate(Vector3.forward * verticalInput * 10 * Time.deltaTime);

            Vector3 deltaMousePosition = Input.mousePosition - lastMousePosition;
            lastMousePosition = Input.mousePosition;

            // Rotate the camera around its own axes, simulating free rotation
            freeCam.transform.Rotate(Vector3.up, deltaMousePosition.x * 10 * Time.deltaTime);
            freeCam.transform.Rotate(Vector3.right, -deltaMousePosition.y * 10 * Time.deltaTime);
            Vector3 currentRotation = freeCam.transform.localEulerAngles;
            currentRotation.z = 0;
            freeCam.transform.localEulerAngles = currentRotation;
            // Optional: Add zoom functionality (scroll wheel)
            //float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
            //transform.Translate(Vector3.forward * scrollWheel * 10 * Time.deltaTime);
        }
    }

    private void HandleScroll()
    {
        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
        float scrollAmount = scrollDelta * scrollSpeed * Time.deltaTime;
        moveSpeed += scrollAmount;
        moveSpeed = Mathf.Clamp(moveSpeed, 1f, 20f); // Adjust the min and max speed as needed
    }

    public void HandlePrecisionMovement()
    {
        //if (!WindowsManager.Instance.isPrecisionMode)
        //    return;

        //if (Input.GetMouseButtonDown(1)) // Check for right mouse button press
        //{
        //    isDragging = true;

        //    if (vcam.Follow)
        //        vcam.Follow = null;

        //    lastMousePosition = Input.mousePosition;
        //}
        //else if (Input.GetMouseButtonUp(1)) // Check for right mouse button release
        //{
        //    isDragging = false;
        //}

        //if (Input.GetMouseButtonDown(2)) // Check for middle mouse button click
        //{
        //    Camera.main.orthographic = !Camera.main.orthographic; // Toggle orthographic view
        //}


        //float horizontalInput = Input.GetAxis("Horizontal");
        //float verticalInput = Input.GetAxis("Vertical");
        ////float forwardInput = Input.GetAxis("Forward");

        //transform.Translate(Vector3.right * horizontalInput * movementSpeed * Time.deltaTime);
        //transform.Translate(Vector3.forward * verticalInput * movementSpeed * Time.deltaTime);
        ////transform.Translate(transform.up * verticalInput * movementSpeed * Time.deltaTime);

        //if (isDragging)
        //{
        //    Vector3 deltaMousePosition = Input.mousePosition - lastMousePosition;
        //    lastMousePosition = Input.mousePosition;

        //    // Rotate the camera around its own axes, simulating free rotation
        //    transform.Rotate(Vector3.up, deltaMousePosition.x * rotationSpeed * Time.deltaTime);
        //    transform.Rotate(Vector3.right, -deltaMousePosition.y * rotationSpeed * Time.deltaTime);
        //    Vector3 currentRotation = transform.localEulerAngles;
        //    currentRotation.z = 0;
        //    transform.localEulerAngles = currentRotation;
        //    // Optional: Add zoom functionality (scroll wheel)
        //    float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        //    transform.Translate(Vector3.forward * scrollWheel * 10 * Time.deltaTime);
        //}
    }
}

[System.Serializable]
public class VCam
{
    public GameObject vCam;
    public CinemachineVirtualCamera virtualCamera;
    public string CamName;
    public enum Pers
    {
        Perspective, Orthographic
    }
    public Pers perspective = Pers.Orthographic;
    public bool transformable;

    public LayerMask RenderedLayers;
}
