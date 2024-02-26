using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("HelpMenu")]
    public Image HelpImage;
    public GameObject HelpHeaderDisplay;
    public TMP_Text HelpDescText, HelpTitleText;

    [Header("Settings")]
    public Slider Slider_NumCulpritsPerRow;
    private DontDestroyOnLoadSettings settingsManager;

    public void StartGame()
    {
        settingsManager.StartGame();
    }

    private void Update()
    {
        if(!settingsManager)
        {
            settingsManager = GameObject.FindObjectOfType<DontDestroyOnLoadSettings>();
        }
    }

    public bool GetSettingsManager()
    {
        if (settingsManager) return true;
        else
        {
            settingsManager = GameObject.FindObjectOfType<DontDestroyOnLoadSettings>();
            if (settingsManager) return true;
        }
        return false;
    }
    private void Start()
    {
        if (GetSettingsManager())
        {
            AddListenersToSettingsSliders();
            ParseCurrentValuesIntoSliders();
        }
    }

    public void ParseCurrentValuesIntoSliders()
    {
        Slider_NumCulpritsPerRow.value = settingsManager.NumCulpritsPerRowValue;
    }

    public void ParseSlidersIntoCurrentValues()
    {
        OnChangeNumCulpritsPerRow();
    }
    public void OnChangeNumCulpritsPerRow()
    {
       settingsManager.NumCulpritsPerRowValue = (int)Slider_NumCulpritsPerRow.value;
    }

    private void AddListenersToSettingsSliders()
    {
        Slider_NumCulpritsPerRow.onValueChanged.AddListener(delegate { OnChangeNumCulpritsPerRow(); });
    }
    public void DisplayHelpMenu(string title, string desc, Sprite Image)
    {
        if (Image)
        {
            HelpImage.sprite = Image;
            HelpImage.gameObject.SetActive(true);
        }
        else HelpImage.gameObject.SetActive(false);
        HelpDescText.text = desc;
        HelpHeaderDisplay.SetActive(true);
        HelpTitleText.text = title;
    }
}
