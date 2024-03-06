using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.UI;
using TMPro;
public class ScenarioBuilder : MonoBehaviour
{
    [Header("Datablocks")]
    [SerializeField] public List<SavableBuildingDetails> ParsedBuildingDatablock;
    [SerializeField] List<SavableEnvironmentDetails> ParsedEnvDatablock;
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
            Vector3 OriginalBuildingPosition = new Vector3(ParsedBuildingDatablock[i].PosZ, ParsedBuildingDatablock[i].PosY - 0.7f, ParsedBuildingDatablock[i].PosX);
            Quaternion newRotation = Quaternion.Euler(ParsedBuildingDatablock[i].RotX, ParsedBuildingDatablock[i].RotY + 90, ParsedBuildingDatablock[i].RotZ);
            if (newRotation.y < 0) newRotation.y *= -1;
            Buildings[i].transform.rotation = newRotation;
            Buildings[i].transform.position = OriginalBuildingPosition;

            //TODO: Change the floors and width of the current building

            int numFloors = ParsedBuildingDatablock[i].SavedNumFloors;
            Buildings[i].GetComponent<ModularHDB>().ChangeFloors(numFloors);
            //int width = ParsedBuildingDatablock[i].SavedWidthInMetres;
        }
        if(ParsedEnvDatablock != null && ParsedEnvDatablock.Count > 0)
        for(int i = 0; i < ParsedEnvDatablock.Count; i++)
        {
            GameObject GO = Instantiate(DontDestroyOnLoadSettings.Instance.EnvironmentalPrefabs[ParsedEnvDatablock[i].savedItemIndex]);
            GO.transform.position = new Vector3(ParsedEnvDatablock[i].PosX, ParsedEnvDatablock[i].PosY, ParsedEnvDatablock[i].PosZ);
            GO.transform.rotation = Quaternion.Euler(new Vector3(ParsedEnvDatablock[i].RotX, ParsedEnvDatablock[i].RotY, ParsedEnvDatablock[i].RotZ));
        }
        // First building is always the target building with the distance applied to.
        Vector3 position = Buildings[0].transform.position;
        //position.z = Buildings[1].position.x - SavedScenario.DistanceBetweenBuildings;
        //Buildings[0].position = position;
        NameOfScenario.text = "Loaded: " + SavedScenario.NameOfScenario;
        ScenarioDetails.text = "Distance: " + SavedScenario.DistanceBetweenBuildings.ToString() + "m";
    }

    public void ParseScenario(Scenario scenario)
    {
        SavedScenario = scenario;
        ParsedBuildingDatablock = JsonConvert.DeserializeObject<List<SavableBuildingDetails>>(SavedScenario.JsonSave);
        ParsedEnvDatablock = JsonConvert.DeserializeObject<List<SavableEnvironmentDetails>>(SavedScenario.EnvironmentJSON);
        BuildScenario();
    }
}
