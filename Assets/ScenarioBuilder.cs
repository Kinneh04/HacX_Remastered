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
    public CulpritsManager culpritsManager;
    public GameObject BuildingScenarioScreen;

    public IEnumerator buildSCenarioCoroutine()
    {
        for (int i = 0; i < ParsedBuildingDatablock.Count; i++)
        {
           //Change floors of buildings
            int numFloors = ParsedBuildingDatablock[i].SavedNumFloors;
            Buildings[i].GetComponent<ModularHDB>().ChangeFloors(numFloors);

        }
        yield return new WaitForSeconds(0.5f);
        // Add Culprits into the buildings
        CulpritsManager culpritsManager = FindObjectOfType<CulpritsManager>();
        culpritsManager.NumCulpritsPerRow = DontDestroyOnLoadSettings.Instance.NumCulpritsPerRowValue;
        culpritsManager.InitFloors();

        for (int i = 0; i < ParsedBuildingDatablock.Count; i++)
        {
            //Change transforms of buildings

            Vector3 OriginalBuildingPosition = new Vector3(ParsedBuildingDatablock[i].PosZ, ParsedBuildingDatablock[i].PosY - 0.7f, ParsedBuildingDatablock[i].PosX);
            ParsedBuildingDatablock[i].RotY += 270;
           // if (ParsedBuildingDatablock[i].RotY < 0) ParsedBuildingDatablock[i].RotY *= -1;
            Quaternion newRotation = Quaternion.Euler(ParsedBuildingDatablock[i].RotZ, ParsedBuildingDatablock[i].RotY, ParsedBuildingDatablock[i].RotX);
            Buildings[i].transform.rotation = newRotation;
            Buildings[i].transform.position = OriginalBuildingPosition;

        }

        if (ParsedEnvDatablock != null && ParsedEnvDatablock.Count > 0)
            for (int i = 0; i < ParsedEnvDatablock.Count; i++)
            {
                // Add environmentals
                GameObject GO = Instantiate(DontDestroyOnLoadSettings.Instance.EnvironmentalPrefabs[ParsedEnvDatablock[i].savedItemIndex]);
                GO.transform.position = new Vector3(ParsedEnvDatablock[i].PosZ, ParsedEnvDatablock[i].PosY, ParsedEnvDatablock[i].PosY);
                GO.transform.rotation = Quaternion.Euler(new Vector3(ParsedEnvDatablock[i].RotX, ParsedEnvDatablock[i].RotY, ParsedEnvDatablock[i].RotZ));
                GO.transform.localScale = new Vector3(ParsedEnvDatablock[i].ScaleX, ParsedEnvDatablock[i].ScaleY, ParsedEnvDatablock[i].ScaleZ);
            }

        // First building is always the target building with the distance applied to.
        Vector3 position = Buildings[0].transform.position;
        NameOfScenario.text = "Loaded: " + SavedScenario.NameOfScenario;
        ScenarioDetails.text = "Distance: " + SavedScenario.DistanceBetweenBuildings.ToString() + "m";
        BuildingScenarioScreen.SetActive(false);
    }
    private void BuildScenario()
    {
        StartCoroutine(buildSCenarioCoroutine());
    }

    public void ParseScenario(Scenario scenario)
    {
        BuildingScenarioScreen.SetActive(true);
        SavedScenario = scenario;
        ParsedBuildingDatablock = JsonConvert.DeserializeObject<List<SavableBuildingDetails>>(SavedScenario.JsonSave);
        ParsedEnvDatablock = JsonConvert.DeserializeObject<List<SavableEnvironmentDetails>>(SavedScenario.EnvironmentJSON);
        BuildScenario();
    }
}
