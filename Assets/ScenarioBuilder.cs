using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.UI;
using TMPro;
public class ScenarioBuilder : MonoBehaviour
{
    [Header("Datablocks")]
    [SerializeField] List<SavableBuildingDetails> ParsedBuildingDatablock;
    [SerializeField] Scenario SavedScenario;
    [Header("Scenario builder")]
    public List<Transform> Buildings = new();
    [Header("UI")]
    public TMP_Text NameOfScenario;
    public TMP_Text ScenarioDetails;
    private void BuildScenario()
    {
        for(int i = 0; i < ParsedBuildingDatablock.Count; i++)
        {

            // Change the angle of the building
            Vector3 OriginalBuildingPosition = Buildings[i].transform.position;
            Quaternion OriginalBuildingRotation = Buildings[i].transform.rotation;
            float angle = ParsedBuildingDatablock[i].SavedAngleInDegrees;
            Quaternion newRotation = Quaternion.Euler(0f, angle, 0f);
            Buildings[i].transform.rotation = OriginalBuildingRotation * newRotation;

            // TODO: Change the floors and width of the current building

            //int numFloors = ParsedBuildingDatablock[i].SavedNumFloors;
            //int width = ParsedBuildingDatablock[i].SavedWidthInMetres;
        }
        // First building is always the target building with the distance applied to.
        Vector3 position = Buildings[0].transform.position;
        position.z = Buildings[1].position.x - SavedScenario.DistanceBetweenBuildings;
        Buildings[0].position = position;
        NameOfScenario.text = "Loaded: " + SavedScenario.NameOfScenario;
        ScenarioDetails.text = "Distance: " + SavedScenario.DistanceBetweenBuildings.ToString() + "m";
    }

    public void ParseScenario(Scenario scenario)
    {
        SavedScenario = scenario;
        ParsedBuildingDatablock = JsonConvert.DeserializeObject<List<SavableBuildingDetails>>(SavedScenario.JsonSave);
        BuildScenario();
    }
}
