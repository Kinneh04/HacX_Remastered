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

    [Header("DragCoefficient")]
    public Slider Slider_Drag;
    public TMP_Text Text_drag;

    [Header("InitVelocity")]
    public Slider Slider_StartVelocity;
    public TMP_Text Text_StartVelocity;


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

        Slider_StartVelocity.value = settingsManager.initialVelocity;
        Text_StartVelocity.text = Slider_StartVelocity.value.ToString("F2") + "m/s";

        Slider_Drag.value = settingsManager.dragCoefficient;
        Text_drag.text = Slider_Drag.value.ToString("F1");
    }

    public void ParseSlidersIntoCurrentValues()
    {
        OnChangeNumCulpritsPerRow();
        OnChangeMaxIterations();
        OnChangeSimulationSpeed();
        OnChangeDragCoefficient();
        OnChangeInitialVelocity();
    }

    public void OnChangeDragCoefficient()
    {
        settingsManager.dragCoefficient = Slider_Drag.value;
        Text_drag.text = Slider_Drag.value.ToString("F1");
    }

    public void OnChangeInitialVelocity()
    {
        settingsManager.initialVelocity = Slider_StartVelocity.value;
        Text_StartVelocity.text = Slider_StartVelocity.value.ToString("F1") + "m/s";
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

        Slider_Drag.onValueChanged.AddListener(delegate { OnChangeDragCoefficient(); });

        Slider_StartVelocity.onValueChanged.AddListener(delegate { OnChangeInitialVelocity(); });
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
