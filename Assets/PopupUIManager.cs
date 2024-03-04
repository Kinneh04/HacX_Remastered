using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class PopupUIManager : MonoBehaviour
{
    // Singleton instance
    public static PopupUIManager Instance { get; private set; }

    // Reference to the popup prefab
    public GameObject PopUpWindow;

    public TMP_Text TitleText, DescriptionText;

    public Button popUpButton;

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
    public void ShowPopup(string title, string desc, UnityAction delegateAction = null)
    {
        PopUpWindow.SetActive(true);
        TitleText.text = title;
        DescriptionText.text = desc;

        popUpButton.onClick.RemoveAllListeners();
        popUpButton.onClick.AddListener(delegate { PopUpWindow.SetActive(false); });
        popUpButton.onClick.AddListener(delegateAction);
        
    }
}
