using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using System.IO;
public class DataManager : MonoBehaviour
{

    public TMP_InputField NameinputField;

    public CulpritsManager culpritsManager;
    public MainGameManager mainGameManager;

    public List<JsonCulpritData> JSONBlock = new();
    public  string ExportCulpritDataToJSON()
    {
        foreach(Culprit c in culpritsManager.SpawnedCulprits)
        {
            JsonCulpritData JSD = new JsonCulpritData()
            {
                ExactCulpritPosX = c.transform.position.x,
                ExactCulpritPosY = c.transform.position.y,
                ExactCulpritPosZ = c.transform.position.z,
                culpritAccuracy = c.TotalProbability,
                WindowHits = c.windowHit
            };

            JSONBlock.Add(JSD);
        }

        return JsonConvert.SerializeObject(JSONBlock);
    }

    public void OnClickExportDataToJson()
    {
        string JSONData = ExportCulpritDataToJSON();
        string nameOfFile = NameinputField.text;
        // Get the desktop path
        string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);

        // Combine desktop path and file name to get the full path
        string filePath = Path.Combine(desktopPath, nameOfFile + ".json");

        // Write the JSON data to the file
        File.WriteAllText(filePath, JSONData);

        Debug.Log("JSON data exported to: " + filePath);
        // save a text file containing the JsonData to desktop and name it "nameOfFile"
    }
}

[System.Serializable]
public class JsonCulpritData
{
    public float ExactCulpritPosX, ExactCulpritPosY, ExactCulpritPosZ;
    public float culpritAccuracy;
    public List<bool> WindowHits = new();
}
