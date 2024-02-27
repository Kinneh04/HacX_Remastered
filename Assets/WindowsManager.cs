using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WindowsManager : MonoBehaviour
{
    public bool canSelectWindow;
    [SerializeField] private Color originalColor; // Store the original color of the window

    private GameObject CurrentlyHoveredWindow;

    [Header("Colors")]
    public Color HighlightedColor, SelectedColor, UnavailableColor;

    [Header("SelectedWindows")]
    public List<GameObject> SelectedWindows = new();

    public GameObject StartButton;
    public TMP_Text WindowCounter;

    private void Awake()
    {
        StartButton.SetActive(false);
        WindowCounter.text = "Windows Selected: " + 0;
    }
    void ToggleWindow()
    {
        if (SelectedWindows.Contains(CurrentlyHoveredWindow))
        {
            DeselectWindow(CurrentlyHoveredWindow);
        }
        else SelectWindow(CurrentlyHoveredWindow);
    }

    void SelectWindow(GameObject GO)
    {
        GO.GetComponent<MeshRenderer>().material.color = SelectedColor;
        SelectedWindows.Add(GO);
    }

    void DeselectWindow(GameObject GO)
    {

        GO.GetComponent<MeshRenderer>().material.color = originalColor;
        SelectedWindows.Remove(GO);
    }
    private void Update()
    {
        if (!canSelectWindow) return;

        if (Input.GetMouseButtonDown(0) && CurrentlyHoveredWindow)
        {
            ToggleWindow();
            WindowCounter.text = "Windows Selected: " + SelectedWindows.Count;
            StartButton.SetActive(SelectedWindows.Count > 0);
        }
        // Create a ray from the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Declare a RaycastHit variable to store the hit information
        RaycastHit hit;

        // Perform the raycast
        if (Physics.Raycast(ray, out hit))
        {
            // Check if the hit object has the tag "window"
            if (hit.collider.CompareTag("Window"))
            {
                // Do something when a window is hit
                Debug.Log("Hit a window!");
                // Restore the color of the previously hovered window
                //if (hit.collider.TryGetComponent<Renderer>(out Renderer previousRenderer))
                //{
                //    previousRenderer.material.color = originalColor;
                //}

                CurrentlyHoveredWindow = hit.collider.gameObject;

                if (SelectedWindows.Contains(CurrentlyHoveredWindow)) return;
                Material M = CurrentlyHoveredWindow.GetComponent<MeshRenderer>().material;
              //  originalColor = M.color;
                M.color = HighlightedColor;

            }
            else
            {
                //Stop hover on window
                if(CurrentlyHoveredWindow)
                {
                    CurrentlyHoveredWindow.GetComponent<MeshRenderer>().material.color = originalColor;
                    CurrentlyHoveredWindow = null;
                }
            }
        }
    }
}
