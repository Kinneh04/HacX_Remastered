using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupUIManager : MonoBehaviour
{
    // Singleton instance
    public static PopupUIManager Instance { get; private set; }

    // Reference to the popup prefab
    public GameObject popupPrefab;

    private void Awake()
    {
        // Ensure there is only one instance of the PopupUIManager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Function to show a popup with the given message
    public void ShowPopup(string message)
    {
        
    }
}
