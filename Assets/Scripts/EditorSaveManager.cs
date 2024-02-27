using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
public class EditorSaveManager : MonoBehaviour
{

    [Header("ScenarioSaveList")]
    public List<Scenario> CurrentlySavedScenarios = new();
    List<GameObject> InstantiatedScenarioPrefabs = new();
    public GameObject NoScenariosText;

    [Header("Prefabs")]
    public GameObject ScenarioListUI;
    public GameObject ScenarioUIPrefab;
    public Transform ScenarioUIPrefabParentTransform;

    [Header("SelectScenarioUI")]
    public EditorManager editorManager;
    public GameObject ScenarioSettingsUI;
    public Scenario SelectedScenario;

    public void CloseScenarioSettings()
    {
        ScenarioSettingsUI.SetActive(false);
        SelectedScenario = null;
    }

    public void SelectScenario(Scenario scenario)
    {
        SelectedScenario = scenario;
        ScenarioSettingsUI.SetActive(true);
    }

    public void LoadCurrentScene()
    {

        //Load selected scene

        List<SavableBuildingDetails> BuildingDatablock = new();

        BuildingDatablock = JsonConvert.DeserializeObject<List<SavableBuildingDetails>>(SelectedScenario.JsonSave);

        for(int i = 0; i < BuildingDatablock.Count; i++)
        {
            editorManager.OverrideBuildingFloors(i, BuildingDatablock[i].SavedNumFloors);
            editorManager.OverrideBuildingWidth(i, BuildingDatablock[i].SavedWidthInMetres);
        }

        editorManager.OverrideBuildingDistance(SelectedScenario.DistanceBetweenBuildings);

        CloseScenarioSettings();
        ScenarioListUI.SetActive(false);
    }

    public void DeleteCurrentScene()
    {
        CurrentlySavedScenarios.Remove(SelectedScenario);
        CloseScenarioSettings();
        OnRefreshScenarioList();
    }

    public void OnOpenScenarioList()
    {
        foreach(GameObject gameObject in InstantiatedScenarioPrefabs)
        {
            Destroy(gameObject);
        }
        InstantiatedScenarioPrefabs.Clear();
        ScenarioListUI.SetActive(true);
        OnRefreshScenarioList();
    }

    private void OnRefreshScenarioList()
    {

        if (CurrentlySavedScenarios.Count > 0)
        {
            NoScenariosText.SetActive(false);
            foreach (Scenario scenario in CurrentlySavedScenarios)
            {
                GameObject GO = Instantiate(ScenarioUIPrefab);
                GO.transform.SetParent(ScenarioUIPrefabParentTransform, false);
                ScenarioPrefab scenarioPrefab = GO.GetComponent<ScenarioPrefab>();
                scenarioPrefab.NameText.text = scenario.NameOfScenario;
                scenarioPrefab.OnClickButton.onClick.AddListener(delegate { SelectScenario(scenario); });
            }
        }
        else
        {
            NoScenariosText.SetActive(true);
        }
    }
}
