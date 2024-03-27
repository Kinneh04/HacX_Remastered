using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;
public class SettingsMenu : MonoBehaviour
{
    [Header("HelpMenu")]
    public Image HelpImage;
    public GameObject HelpHeaderDisplay;
    public TMP_Text HelpDescText, HelpTitleText;

    [Header("Settings")]
    [Header("CalculationDensity")]
    public Slider Slider_NumCulpritsPerRow;
    public TMP_InputField numCulpritsPerRowInputField;

    [Header("SimulationSpeed")]
    public Slider Slider_SimulationSpeed;
    public TMP_InputField simulationSpeedInputField;
    

    [Header("Maxiterations")]
    public Slider Slider_MaxIterations;
    public TMP_InputField maxIterationsInputField;
    private DontDestroyOnLoadSettings settingsManager;

    [Header("DragCoefficient")]
    public Slider Slider_Drag;
    public TMP_InputField dragInputField;

    [Header("Coefficient")]
    public Slider Slider_Bounce;
    public TMP_InputField bounceInputField;

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

    [Header("AngleThreshold")]
    public TMP_InputField angleThresholdInputField;

    [Header("Ball Presets")]
    public TMP_Dropdown ballDropdown;
    public List<BallPreset> ballPresets;
    public TMP_InputField presetName;
    public PresetManager presetManager;

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
            AddListenersToSettings();
            PopulateBallPresets();
            ParseSettingsIntoCurrentValues();
        }
    }

    public void ParseSettingsIntoCurrentValues()
    {
        Slider_MaxIterations.value = settingsManager.MaxIterationsValue;
        maxIterationsInputField.text = Slider_MaxIterations.value.ToString();

        Slider_SimulationSpeed.value = settingsManager.SimulationSpeedValue;
        simulationSpeedInputField.text = Slider_SimulationSpeed.value.ToString("F1");

        Slider_NumCulpritsPerRow.value= settingsManager.NumCulpritsPerRowValue;
        numCulpritsPerRowInputField.text = Slider_NumCulpritsPerRow.value.ToString();

        Slider_Drag.value = settingsManager.dragCoefficient;
        dragInputField.text = Slider_Drag.value.ToString("F2");

        Slider_Bounce.value = settingsManager.coefficientOfRestitution;
        bounceInputField.text = Slider_Bounce.value.ToString("F2");

        minVelocityInputField.text = settingsManager.minVelocity.ToString();
        maxVelocityInputField.text = settingsManager.maxVelocity.ToString();
        velocityIncrementInputField.text = settingsManager.velocityIncrement.ToString();
        densityInputField.text = settingsManager.density.ToString();
        diameterInputField.text = settingsManager.diameter.ToString();
        positionInputField.text = settingsManager.positionRange.ToString();
        angleThresholdInputField.text = settingsManager.angleThreshold.ToString("F1");
    }

    public void ParseCurrentValuesIntoSettings()
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
        dragInputField.text = Slider_Drag.value.ToString("F2");
    }

    private void OnDragChanged(string value)
    {
        if (float.TryParse(value, out var newDrag))
        {
            newDrag = Mathf.Round(newDrag * 100.0f) * 0.01f;
            newDrag = Mathf.Clamp(newDrag, Slider_Drag.minValue, Slider_Drag.maxValue);
            settingsManager.dragCoefficient = newDrag;
        }
        else
        {
            settingsManager.dragCoefficient = 0.5f;
            Debug.LogError("Invalid input for drag. Please enter a valid number.");
        }
        ParseSettingsIntoCurrentValues();
    }

    public void OnChangeBounceCoefficient()
    {
        settingsManager.coefficientOfRestitution = Slider_Bounce.value;
        bounceInputField.text = Slider_Bounce.value.ToString("F2");
    }
    private void OnBounceChanged(string value)
    {
        if (float.TryParse(value, out var newBounce))
        {
            newBounce = Mathf.Round(newBounce * 100.0f) * 0.01f;
            newBounce = Mathf.Clamp(newBounce, Slider_Bounce.minValue, Slider_Bounce.maxValue);
            settingsManager.coefficientOfRestitution = newBounce;
        }
        else
        {
            settingsManager.coefficientOfRestitution = 0.85f;
            Debug.LogError("Invalid input for CoefficientOfRestitution. Please enter a valid number.");
        }
        ParseSettingsIntoCurrentValues();
    }

    public void OnChangeMaxIterations()
    {
        settingsManager.MaxIterationsValue = (int)Slider_MaxIterations.value;
        maxIterationsInputField.text = Slider_MaxIterations.value.ToString();
    }
    private void OnMaxIterationChanged(string value)
    {
        if (int.TryParse(value, out var newMax))
        {
            newMax = Mathf.Clamp(newMax, (int)Slider_MaxIterations.minValue, (int)Slider_MaxIterations.maxValue);
            settingsManager.MaxIterationsValue = newMax;
        }
        else
        {
            settingsManager.MaxIterationsValue = 1;
            Debug.LogError("Invalid input for MaxIteration. Please enter a valid number.");
        }
        ParseSettingsIntoCurrentValues();
    }

    public void OnChangeSimulationSpeed()
    {
        settingsManager.SimulationSpeedValue = Slider_SimulationSpeed.value;
        simulationSpeedInputField.text = Slider_SimulationSpeed.value.ToString("F1");
    }

    private void OnSimSpeedChanged(string value)
    {
        if (float.TryParse(value, out var newSpeed))
        {
            newSpeed = Mathf.Round(newSpeed * 10.0f) * 0.1f;
            newSpeed = Mathf.Clamp(newSpeed, Slider_SimulationSpeed.minValue, Slider_SimulationSpeed.maxValue);
            settingsManager.SimulationSpeedValue = newSpeed;
        }
        else
        {
            settingsManager.SimulationSpeedValue = 1;
            Debug.LogError("Invalid input for SimulationSpeed. Please enter a valid number.");
        }
        ParseSettingsIntoCurrentValues();
    }

    public void OnChangeNumCulpritsPerRow()
    {
       settingsManager.NumCulpritsPerRowValue = (int)Slider_NumCulpritsPerRow.value;
       numCulpritsPerRowInputField.text = Slider_NumCulpritsPerRow.value.ToString();
    }

    private void OnNumCulpritsChanged(string value)
    {
        if (int.TryParse(value, out var newCulprits))
        {
            newCulprits = Mathf.Clamp(newCulprits, (int)Slider_NumCulpritsPerRow.minValue, (int)Slider_NumCulpritsPerRow.maxValue);
            settingsManager.NumCulpritsPerRowValue = newCulprits;
        }
        else
        {
            settingsManager.NumCulpritsPerRowValue = 1;
            Debug.LogError("Invalid input for NumCulprits. Please enter a valid number.");
        }
        ParseSettingsIntoCurrentValues();
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
            ParseSettingsIntoCurrentValues();
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
            ParseSettingsIntoCurrentValues();
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
        ParseSettingsIntoCurrentValues();
    }

    private void OnDensityChanged(string value)
    {
        if (float.TryParse(value, out var newDensity) && newDensity > 0f)
        {
            settingsManager.density = newDensity;
        }
        else
        {
            settingsManager.density = 7750f;
            Debug.LogError("Invalid input for density. Please enter a valid number.");
        }
        ParseSettingsIntoCurrentValues();
    }

    private void OnDiameterChanged(string value)
    {
        if (float.TryParse(value, out var newDiameter) && newDiameter > 0f)
        {
            settingsManager.diameter = newDiameter;
        }
        else
        {
            settingsManager.diameter = 0.01f;
            Debug.LogError("Invalid input for diameter. Please enter a valid number.");
        }
        ParseSettingsIntoCurrentValues();
    }

    private void OnPositionRangeChanged(string value)
    {
        if (float.TryParse(value, out var newRange) && newRange >= 0f)
        {
            settingsManager.positionRange= newRange;
        }
        else
        {
            settingsManager.positionRange = 0;
            Debug.LogError("Invalid input for positionRange. Please enter a valid number.");
        }
        ParseSettingsIntoCurrentValues();
    }

    private void OnAngleThresholdChanged(string value)
    {
        if (float.TryParse(value, out var newAngle) && newAngle >= 0f)
        {
            newAngle = Mathf.Round(newAngle * 10.0f) * 0.1f;
            settingsManager.angleThreshold = newAngle;
        }
        else
        {
            settingsManager.angleThreshold = 0;
            Debug.LogError("Invalid input for angleThreshold. Please enter a valid number.");
        }
        ParseSettingsIntoCurrentValues();
    }

    public void OnTimestepDropdownChanged(int value)
    {
        settingsManager.timeStepAmt = settingsManager.timeSteps[value];
    }

    public void OnBallDropdownChanged(int value)
    {
        BallPreset b = ballPresets[value];
        settingsManager.dragCoefficient = b.drag;
        settingsManager.coefficientOfRestitution = b.restitution;
        settingsManager.density = b.density;
        settingsManager.diameter = b.diameter;

        ParseSettingsIntoCurrentValues();
    }


    private void AddListenersToSettings()
    {
        Slider_NumCulpritsPerRow.onValueChanged.AddListener(delegate { OnChangeNumCulpritsPerRow(); });
        numCulpritsPerRowInputField.onEndEdit.AddListener(OnNumCulpritsChanged);

        Slider_MaxIterations.onValueChanged.AddListener(delegate { OnChangeMaxIterations(); });
        maxIterationsInputField.onEndEdit.AddListener(OnMaxIterationChanged);

        Slider_SimulationSpeed.onValueChanged.AddListener(delegate { OnChangeSimulationSpeed(); });
        simulationSpeedInputField.onEndEdit.AddListener(OnSimSpeedChanged);

        Slider_Drag.onValueChanged.AddListener(delegate { OnChangeDragCoefficient(); });
        dragInputField.onEndEdit.AddListener(OnDragChanged);

        Slider_Bounce.onValueChanged.AddListener(delegate { OnChangeBounceCoefficient(); });
        bounceInputField.onEndEdit.AddListener(OnBounceChanged);

        minVelocityInputField.onEndEdit.AddListener(OnMinVelocityChanged);
        maxVelocityInputField.onEndEdit.AddListener(OnMaxVelocityChanged);
        velocityIncrementInputField.onEndEdit.AddListener(OnVelocityIncrementChanged);

        densityInputField.onEndEdit.AddListener(OnDensityChanged);
        diameterInputField.onEndEdit.AddListener(OnDiameterChanged);
        positionInputField.onEndEdit.AddListener(OnPositionRangeChanged);

        angleThresholdInputField.onEndEdit.AddListener(OnAngleThresholdChanged);

        timeStepDropdown.onValueChanged.AddListener(OnTimestepDropdownChanged);
        ballDropdown.onValueChanged.AddListener(OnBallDropdownChanged);
    }

    public void PopulateBallPresets()
    {
        ballPresets = PresetManager.LoadAllPresets();
        ballDropdown.ClearOptions();

        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        foreach (BallPreset preset in ballPresets)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(preset.name);
            options.Add(option);
        }
        ballDropdown.AddOptions(options);

        bool set = false;
        for (int i = 0; i < ballDropdown.options.Count; i++) // set steel as the default
        {
            if (ballDropdown.options[i].text == "Steel")
            {
                ballDropdown.value = i;
                set = true;
                break; 
            }
        }
    }

    public void CreateNewPreset()
    {
        BallPreset b = new();
        b.name = presetName.text;
        b.drag = settingsManager.dragCoefficient;
        b.restitution = settingsManager.coefficientOfRestitution;
        b.density = settingsManager.density;
        b.diameter = settingsManager.diameter;

        PresetManager.SavePreset(b, 0);
        PopulateBallPresets();

        for (int i = 0; i < ballDropdown.options.Count; i++) // set steel as the default
        {
            if (ballDropdown.options[i].text == b.name)
            {
                ballDropdown.value = i;
                break; 
            }
        }
    }

    public void OverridePreset()
    {
        BallPreset b = ballPresets[ballDropdown.value];
        b.drag = settingsManager.dragCoefficient;
        b.restitution = settingsManager.coefficientOfRestitution;
        b.density = settingsManager.density;
        b.diameter = settingsManager.diameter;
        PresetManager.SavePreset(b, ballDropdown.value);
    }

    public void DestroyPreset()
    {
        BallPreset b = ballPresets[ballDropdown.value];
        //string assetPath = "Assets/Resources/BallPresets/" + b.name + ".asset";
        //AssetDatabase.DeleteAsset(assetPath);
        //AssetDatabase.SaveAssets();
        //PopulateBallPresets();

 
        PresetManager.DeletePreset(b, ballDropdown.value);
        // Update dropdown menu after deletion
        PopulateBallPresets();
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
