using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraOrbit : MonoBehaviour
{
    public float movementSpeed = 10.0f; // Adjust this value to control movement speed
    public float rotationSpeed = 10.0f; // Adjust this value to control camera rotation speed
    public float zoomSpeed = 10.0f; // Adjust this value to control zoom speed

    public bool isDragging = false;
    private Vector3 lastMousePosition;

    public CinemachineVirtualCamera vcam;

    private void Awake()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
    }
    void Update()
    {
        if (!WindowsManager.Instance.isPrecisionMode)
            return;

        if (Input.GetMouseButtonDown(1)) // Check for right mouse button press
        {
            isDragging = true;

            if(vcam.Follow)
                vcam.Follow = null;

            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(1)) // Check for right mouse button release
        {
            isDragging = false;
        }

        if (Input.GetMouseButtonDown(2)) // Check for middle mouse button click
        {
            Camera.main.orthographic = !Camera.main.orthographic; // Toggle orthographic view
        }


        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        //float forwardInput = Input.GetAxis("Forward");

        transform.Translate(Vector3.right * horizontalInput * movementSpeed * Time.deltaTime);
        transform.Translate(Vector3.forward * verticalInput * movementSpeed * Time.deltaTime);
        //transform.Translate(transform.up * verticalInput * movementSpeed * Time.deltaTime);

        if (isDragging)
        {
            Vector3 deltaMousePosition = Input.mousePosition - lastMousePosition;
            lastMousePosition = Input.mousePosition;

            // Rotate the camera around its own axes, simulating free rotation
            transform.Rotate(Vector3.up, deltaMousePosition.x * rotationSpeed * Time.deltaTime);
            transform.Rotate(Vector3.right, -deltaMousePosition.y * rotationSpeed * Time.deltaTime);
            Vector3 currentRotation = transform.localEulerAngles;
            currentRotation.z = 0;
            transform.localEulerAngles = currentRotation;
            // Optional: Add zoom functionality (scroll wheel)
            float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
            transform.Translate(Vector3.forward * scrollWheel * 10 * Time.deltaTime);
        }
    }
}
