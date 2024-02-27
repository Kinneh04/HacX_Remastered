using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SimulationSelectionManager : MonoBehaviour
{
    public GameObject SimulationMenuUI;
    public GameObject SimulationPrefab;
    List<GameObject> InstantiatedSimualtionPrefabs = new();
    public Transform SimulationPrefabParent;
    public ScenarioPrefab SelectedScenarioPrefab;
    public Button RunButton;
    Color originalColor;
    public Color SelectedColor = Color.green;
    public EditorSaveManager editorSaveManager;

    private void Start()
    {
        originalColor = SimulationPrefab.GetComponent<ScenarioPrefab>().backgroundImage.color;
    }

    public void OnSelectScenario(ScenarioPrefab scenario)
    {
        if (SelectedScenarioPrefab)
            DeselectCurrentScenario();

        SelectedScenarioPrefab = scenario;
        SelectedScenarioPrefab.backgroundImage.color = SelectedColor;
        RunButton.interactable = true;
    }
    public void DeselectCurrentScenario()
    {
        if (!SelectedScenarioPrefab) return;
        SelectedScenarioPrefab.backgroundImage.color = originalColor;
        SelectedScenarioPrefab = null;
        RunButton.interactable = false;
    }

    public void OnClickStartButton()
    {
        SimulationMenuUI.SetActive(true);

        foreach(GameObject SimulationPrefab in InstantiatedSimualtionPrefabs)
        {
            Destroy(SimulationPrefab);
        }
        InstantiatedSimualtionPrefabs.Clear();
        foreach(Scenario scenario in editorSaveManager.CurrentlySavedScenarios)
        {
            GameObject GO = Instantiate(SimulationPrefab);
            ScenarioPrefab SP = GO.GetComponent<ScenarioPrefab>();
            SP.OnClickButton.onClick.AddListener(delegate { OnSelectScenario(SP); });
            SP.NameText.text = scenario.NameOfScenario;
            GO.transform.SetParent(SimulationPrefabParent, false);
        }
    }

    public void OnClickBackButton()
    {
        DeselectCurrentScenario();
        SimulationMenuUI.SetActive(false);
    }
}
