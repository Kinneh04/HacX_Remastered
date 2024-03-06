using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using System.IO;
public class DataManager : MonoBehaviour
{

    public TMP_InputField NameinputField, CulpritNameInputField;

    public CulpritsManager culpritsManager;
    public MainGameManager mainGameManager;

  //  public TMP_Text FeedbackText;

    public List<JsonCulpritData> JSONBlock = new();
    public  string ExportCulpritDataToJSON()
    {
       // FeedbackText.text = "Processing...";
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
      //  FeedbackText.text = "Processing...";
        foreach (Culprit c in culpritsManager.SpawnedCulprits)
        {
            JsonCulpritData JSD = new JsonCulpritData()
            {
                ExactCulpritPosX = c.transform.position.x,
                ExactCulpritPosY = c.transform.position.y,
                ExactCulpritPosZ = c.transform.position.z,
                culpritAccuracy = c.averageProbability,
                WindowHits = c.windowHit,
                Floor = c.floor,
                Column = c.column,
                TotalBallsFired = c.totalBallsThrown,
                BallsHit = c.TotalBallsHit,
                BallsMissed = c.TotalBallsHit - c.TotalBallsHit,
                MissedToHitRatio = (float)c.TotalBallsHit / (c.totalBallsThrown - c.TotalBallsHit)
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
        string filePath = Path.Combine(desktopPath, NameinputField.text + ".csv");

        // Create a new StreamWriter
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            // Write the header line
            writer.WriteLine("ExactCulpritPosX,ExactCulpritPosY,ExactCulpritPosZ,culpritAccuracy,WindowHits,Floor,Column,TotalBallsFired,BallsHit,BallsMissed,MissedToHitRatio");

            // Write data rows
            foreach (var CD in culpritData)
            {
                // Convert List<bool> to string and join the values with ","
                string windowHitsString = string.Join(",", CD.WindowHits);

                // Write the data line
                writer.WriteLine($"{CD.ExactCulpritPosX},{CD.ExactCulpritPosY},{CD.ExactCulpritPosZ},{CD.culpritAccuracy},{windowHitsString},{CD.Floor}, {CD.Column},{CD.TotalBallsFired},{CD.BallsHit}, {CD.BallsMissed},{CD.MissedToHitRatio}");
            }
        }
        // FeedbackText.text = "CSV file exported to: " + filePath;
        PopupUIManager.Instance.ShowPopup("Export results", "CSV file exported to: " + filePath);
        Debug.Log("CSV file exported to: " + filePath);
    }

    public string ExportSingularCulpritDataJSON(Culprit c)
    {
       // FeedbackText.text = "Processing...";

        JsonCulpritData JSD = new JsonCulpritData()
        {
            ExactCulpritPosX = c.transform.position.x,
            ExactCulpritPosY = c.transform.position.y,
            ExactCulpritPosZ = c.transform.position.z,
            culpritAccuracy = c.averageProbability,
            WindowHits = c.windowHit,
            Floor = c.floor,
            Column = c.column,
            TotalBallsFired = c.totalBallsThrown,
            BallsHit = c.TotalBallsHit,
            BallsMissed = c.TotalBallsHit - c.TotalBallsHit,
            MissedToHitRatio = (float)c.TotalBallsHit / (c.totalBallsThrown - c.TotalBallsHit)
        };

        string JSOND = JsonConvert.SerializeObject(JSD);

        return JSOND;
    }

    public JsonCulpritData ExportSingularCulpritData(Culprit c)
    {
        JsonCulpritData JSD = new JsonCulpritData()
        {
            ExactCulpritPosX = c.transform.position.x,
            ExactCulpritPosY = c.transform.position.y,
            ExactCulpritPosZ = c.transform.position.z,
            culpritAccuracy = c.averageProbability,
            WindowHits = c.windowHit,
            Floor = c.floor,
            Column = c.column,
            TotalBallsFired = c.totalBallsThrown,
            BallsHit = c.TotalBallsHit,
            BallsMissed = c.TotalBallsHit - c.TotalBallsHit,
            MissedToHitRatio = (float)c.TotalBallsHit / (c.totalBallsThrown - c.TotalBallsHit)
        };
        return JSD;
    }

    
    public void OnClickExportSingularCulpritDataToCSV()
    {
        JsonCulpritData CD = ExportSingularCulpritData(culpritsManager.SelectedCulprit);
        // Specify the path for the CSV file
        string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        string filePath = Path.Combine(desktopPath, CulpritNameInputField.text + ".csv");

        // Create a new StreamWriter
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            // Write the header line
            writer.WriteLine("ExactCulpritPosX,ExactCulpritPosY,ExactCulpritPosZ,culpritAccuracy,WindowHits,Floor,Column,TotalBallsFired,BallsHit,BallsMissed,MissedToHitRatio");

            // Write data rows

    // Convert List<bool> to string and join the values with ","
    string windowHitsString = string.Join(",", CD.WindowHits);

            // Write the data line
            writer.WriteLine($"{CD.ExactCulpritPosX},{CD.ExactCulpritPosY},{CD.ExactCulpritPosZ},{CD.culpritAccuracy},{windowHitsString},{CD.Floor}, {CD.Column},{CD.TotalBallsFired},{CD.BallsHit}, {CD.BallsMissed},{CD.MissedToHitRatio}");
            
        }
        // FeedbackText.text = "CSV file exported to: " + filePath;
        PopupUIManager.Instance.ShowPopup("Export results", "CSV file exported to: " + filePath);
        Debug.Log("CSV file exported to: " + filePath);
    }

    public void OnClickExportSingularCulpritDataToJson()
    {
        string culpritData = ExportSingularCulpritDataJSON(culpritsManager.SelectedCulprit);
        string nameOfFile = CulpritNameInputField.text;
        // Get the desktop path
        string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);

        // Combine desktop path and file name to get the full path
        string filePath = Path.Combine(desktopPath, nameOfFile + ".json");

        // Write the JSON data to the file
        File.WriteAllText(filePath, culpritData);
        // FeedbackText.text = "JSON file exported to: " + filePath;
        PopupUIManager.Instance.ShowPopup("Export results", "JSON file exported to: " + filePath);
        Debug.Log("JSON data exported to: " + filePath);
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
        //FeedbackText.text = "JSON file exported to: " + filePath;
        PopupUIManager.Instance.ShowPopup("Export results", "JSON file exported to: " + filePath);
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

    [Header("OptionalStats")]
    public int Floor;
    public int Column, TotalBallsFired, BallsHit, BallsMissed;
    public float MissedToHitRatio;
}
