using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    private bool isMiddleMouseButtonHeld = false;
    public Transform CameraPivot;
    public float Sensitivity = 1.0f;
    public CinemachineVirtualCamera cam;
    public float zoomSpeed = 5f;
    void Update()
    {
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
        if (isMiddleMouseButtonHeld)
        {
            // Get mouse movement on the x and y axes
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            // Adjust the GameObject's position based on the mouse movement
            CameraPivot.Translate(new Vector3(-mouseX * Sensitivity, -mouseY * Sensitivity, 0) * Time.deltaTime);
        }

        // Check for scroll input
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        // Adjust orthographic size based on scroll input
        if (scroll != 0f)
        {
            // Get the current orthographic size
            float currentSize = WindowsManager.Instance.OverviewOrthoSize;

            // Calculate the new orthographic size after scrolling
            float newSize = Mathf.Clamp(currentSize - scroll * zoomSpeed, 1f, Mathf.Infinity);

            // Set the new orthographic size
            WindowsManager.Instance.OverviewOrthoSize = newSize;
            if(!WindowsManager.Instance.isPrecisionMode)
            {
                WindowsManager.Instance.TargetOrthoSize = WindowsManager.Instance.OverviewOrthoSize;
            }
        }
    }
}
