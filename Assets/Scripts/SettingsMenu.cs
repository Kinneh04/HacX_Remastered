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

    [Header("Coefficient")]
    public Slider Slider_Bounce;
    public TMP_Text Text_Bounce;

    [Header("Velocity")]
    public TMP_InputField minVelocityInputField;
    public TMP_InputField maxVelocityInputField;
    public TMP_InputField velocityIncrementInputField;

    [Header("TimeStep")]
    public TMP_Dropdown timeStepDropdown;

    [Header("Density")]
    public TMP_InputField densityInputField;

    [Header("Diameter")]
    public TMP_InputField diameterInputField;

    [Header("Position Range")]
    public TMP_InputField positionInputField;


    public void OpenLink(string s)
    {
        Application.OpenURL(s);
    }


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
            UpdateUI();
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

        Slider_Drag.value = settingsManager.dragCoefficient;
        Text_drag.text = Slider_Drag.value.ToString("F1");
    }

    public void ParseSlidersIntoCurrentValues()
    {
        OnChangeNumCulpritsPerRow();
        OnChangeMaxIterations();
        OnChangeSimulationSpeed();
        OnChangeDragCoefficient();
        OnChangeBounceCoefficient();
    }

    public void OnChangeDragCoefficient()
    {
        settingsManager.dragCoefficient = Slider_Drag.value;
        Text_drag.text = Slider_Drag.value.ToString("F1");
    }

    public void OnChangeBounceCoefficient()
    {
        settingsManager.coefficientOfRestitution = Slider_Bounce.value;
        Text_Bounce.text = Slider_Bounce.value.ToString("F1");
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

    private void OnMinVelocityChanged(string value)
    {
        if (int.TryParse(value, out var newMinVelocity) && newMinVelocity > 0f)
        {
            // Validation: Ensure minVelocity is less than maxVelocity
            if (newMinVelocity <= settingsManager.maxVelocity)
            {
                settingsManager.minVelocity = newMinVelocity;
            }
            else
            {
                settingsManager.minVelocity = settingsManager.maxVelocity;
                Debug.LogError("Min velocity cannot be greater than or equal to max velocity.");
            }
            UpdateUI();
        }
        else
        {
            settingsManager.minVelocity = 0;
            Debug.LogError("Invalid input for min velocity. Please enter a valid number.");
        }
    }

    private void OnMaxVelocityChanged(string value)
    {
        if (int.TryParse(value, out var newMaxVelocity) && newMaxVelocity > 0f)
        {
            // Validation: Ensure maxVelocity is greater than minVelocity
            if (newMaxVelocity >= settingsManager.minVelocity)
            {
                settingsManager.maxVelocity = newMaxVelocity;
            }
            else
            {
                settingsManager.maxVelocity = settingsManager.minVelocity;
                Debug.LogError("Max velocity cannot be less than or equal to min velocity.");
            }
            UpdateUI();
        }
        else
        {
            settingsManager.maxVelocity = 0;
            Debug.LogError("Invalid input for max velocity. Please enter a valid number.");
        }
    }

    private void OnVelocityIncrementChanged(string value)
    {
        if (int.TryParse(value, out var newVelocityIncrement) && newVelocityIncrement >= 1)
        {
            settingsManager.velocityIncrement = newVelocityIncrement;
        }
        else
        {
            settingsManager.velocityIncrement = 1;
            Debug.LogError("Invalid input for velocity increment. Please enter a valid number.");
        }
        UpdateUI();
    }

    private void OnDensityChanged(string value)
    {
        if (float.TryParse(value, out var newDensity) && newDensity > 0f)
        {
            // Add any validation or logic for density if needed
            settingsManager.density = newDensity;
            UpdateUI();
        }
        else
        {
            settingsManager.density = 7750f;
            Debug.LogError("Invalid input for density. Please enter a valid number.");
        }
    }

    private void OnDiameterChanged(string value)
    {
        if (float.TryParse(value, out var newDiameter) && newDiameter > 0f)
        {
            // Add any validation or logic for density if needed
            settingsManager.diameter = newDiameter;
            UpdateUI();
        }
        else
        {
            settingsManager.diameter = 0.01f;
            Debug.LogError("Invalid input for density. Please enter a valid number.");
        }
    }

    private void OnPositionRangeChanged(string value)
    {
        if (float.TryParse(value, out var newRange) && newRange >= 0f)
        {
            // Add any validation or logic for density if needed
            settingsManager.positionRange= newRange;
            UpdateUI();
        }
        else
        {
            settingsManager.positionRange = 0;
            Debug.LogError("Invalid input for density. Please enter a valid number.");
        }
    }

    public void OnTimestepDropdownChanged(int value)
    {
        settingsManager.timeStepAmt = settingsManager.timeSteps[value];
    }

    private void UpdateUI()
    {
        minVelocityInputField.text = settingsManager.minVelocity.ToString();
        maxVelocityInputField.text = settingsManager.maxVelocity.ToString();
        velocityIncrementInputField.text = settingsManager.velocityIncrement.ToString();
        densityInputField.text = settingsManager.density.ToString();
        diameterInputField.text = settingsManager.diameter.ToString();
        positionInputField.text = settingsManager.positionRange.ToString();
    }

    private void AddListenersToSettingsSliders()
    {
        Slider_NumCulpritsPerRow.onValueChanged.AddListener(delegate { OnChangeNumCulpritsPerRow(); });

        Slider_MaxIterations.onValueChanged.AddListener(delegate { OnChangeMaxIterations(); });

        Slider_SimulationSpeed.onValueChanged.AddListener(delegate { OnChangeSimulationSpeed(); });

        Slider_Drag.onValueChanged.AddListener(delegate { OnChangeDragCoefficient(); });

        Slider_Bounce.onValueChanged.AddListener(delegate { OnChangeBounceCoefficient(); });


        minVelocityInputField.onValueChanged.AddListener(OnMinVelocityChanged);
        maxVelocityInputField.onValueChanged.AddListener(OnMaxVelocityChanged);
        velocityIncrementInputField.onValueChanged.AddListener(OnVelocityIncrementChanged);

        densityInputField.onValueChanged.AddListener(OnDensityChanged);
        diameterInputField.onValueChanged.AddListener(OnDiameterChanged);
        positionInputField.onValueChanged.AddListener(OnPositionRangeChanged);

        timeStepDropdown.onValueChanged.AddListener(OnTimestepDropdownChanged);
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
