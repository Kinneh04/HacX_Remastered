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
    [Header("CalculationDensity")]
    public Slider Slider_NumCulpritsPerRow;
    public TMP_Text Text_NumCulpritePerRow;

    [Header("SimulationSpeed")]
    public Slider Slider_SimulationSpeed;
    public TMP_Text Text_SimulationSpeed;

    [Header("Maxiterations")]
    public Slider Slider_MaxIterations;
    public TMP_Text Text_MaxIterations;
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
        Slider_MaxIterations.value = settingsManager.MaxIterationsValue;
        Text_MaxIterations.text = Slider_MaxIterations.value.ToString();

        Slider_SimulationSpeed.value = settingsManager.SimulationSpeedValue;
        Text_SimulationSpeed.text = Slider_SimulationSpeed.value.ToString("F1") + "x";

        Slider_NumCulpritsPerRow.value= settingsManager.NumCulpritsPerRowValue;
        Text_NumCulpritePerRow.text = Slider_NumCulpritsPerRow.value.ToString();
    }

    public void ParseSlidersIntoCurrentValues()
    {
        OnChangeNumCulpritsPerRow();
        OnChangeMaxIterations();
        OnChangeSimulationSpeed();
    }

    public void OnChangeMaxIterations()
    {
        settingsManager.MaxIterationsValue = (int)Slider_MaxIterations.value;
        Text_MaxIterations.text = Slider_MaxIterations.value.ToString();
    }

    public void OnChangeSimulationSpeed()
    {
        settingsManager.SimulationSpeedValue = Slider_SimulationSpeed.value;
        Text_SimulationSpeed.text = Slider_SimulationSpeed.value.ToString("F1") + "x";
    }
    public void OnChangeNumCulpritsPerRow()
    {
       settingsManager.NumCulpritsPerRowValue = (int)Slider_NumCulpritsPerRow.value;
        Text_NumCulpritePerRow.text = Slider_NumCulpritsPerRow.value.ToString();
    }

    private void AddListenersToSettingsSliders()
    {
        Slider_NumCulpritsPerRow.onValueChanged.AddListener(delegate { OnChangeNumCulpritsPerRow(); });

        Slider_MaxIterations.onValueChanged.AddListener(delegate { OnChangeMaxIterations(); });

        Slider_SimulationSpeed.onValueChanged.AddListener(delegate { OnChangeSimulationSpeed(); });
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
