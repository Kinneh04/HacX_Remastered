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

    public TMP_Text FeedbackText;

    public List<JsonCulpritData> JSONBlock = new();
    public  string ExportCulpritDataToJSON()
    {
        FeedbackText.text = "Processing...";
        foreach (Culprit c in culpritsManager.SpawnedCulprits)
        {
            JsonCulpritData JSD = new JsonCulpritData()
            {
                ExactCulpritPosX = c.transform.position.x,
                ExactCulpritPosY = c.transform.position.y,
                ExactCulpritPosZ = c.transform.position.z,
                culpritAccuracy = c.averageProbability,
                WindowHits = c.windowHit
            };

            JSONBlock.Add(JSD);
        }

        return JsonConvert.SerializeObject(JSONBlock);
    }
    public List<JsonCulpritData> ExportCulpritData()
    {
        FeedbackText.text = "Processing...";
        foreach (Culprit c in culpritsManager.SpawnedCulprits)
        {
            JsonCulpritData JSD = new JsonCulpritData()
            {
                ExactCulpritPosX = c.transform.position.x,
                ExactCulpritPosY = c.transform.position.y,
                ExactCulpritPosZ = c.transform.position.z,
                culpritAccuracy = c.averageProbability,
                WindowHits = c.windowHit
            };

            JSONBlock.Add(JSD);
        }

        return JSONBlock;
    }
    public void OnClickExportToCSV()
    {
        List<JsonCulpritData> culpritData = ExportCulpritData();

        // Specify the path for the CSV file
        string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        string filePath = Path.Combine(desktopPath, "CulpritData.csv");

        // Create a new StreamWriter
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            // Write the header line
            writer.WriteLine("ExactCulpritPosX,ExactCulpritPosY,ExactCulpritPosZ,culpritAccuracy,WindowHits");

            // Write data rows
            foreach (var CD in culpritData)
            {
                // Convert List<bool> to string and join the values with ","
                string windowHitsString = string.Join(",", CD.WindowHits);

                // Write the data line
                writer.WriteLine($"{CD.ExactCulpritPosX},{CD.ExactCulpritPosY},{CD.ExactCulpritPosZ},{CD.culpritAccuracy},{windowHitsString}");
            }
        }
        FeedbackText.text = "CSV file exported to: " + filePath;
        Debug.Log("CSV file exported to: " + filePath);
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
        FeedbackText.text = "JSON file exported to: " + filePath;
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
