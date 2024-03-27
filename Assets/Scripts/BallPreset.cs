using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using TMPro;
#if !UNITY_WEBGL
using AnotherFileBrowser.Windows;
#endif
using UnityEngine.Networking;
using System;
[CreateAssetMenu(menuName = "Resources/BallPresets")]
public class BallPreset {
    public string name = " ";
    public float drag = 0f;
    public float restitution = 0f;
    public float density = 0f;
    public float diameter = 0f;
    public static string SerializeToJson(BallPreset preset)
    {
        string jsonString = JsonConvert.SerializeObject(preset, Formatting.Indented);
        return jsonString;
    }

    public static BallPreset DeserializeFromJson(string jsonString)
    {
        BallPreset preset = JsonConvert.DeserializeObject<BallPreset>(jsonString);
        return preset;
    }

    public static void SavePreset(BallPreset preset, string key)
    {
        string jsonString = SerializeToJson(preset);
        PlayerPrefs.SetString(key, jsonString);
    }

    public static BallPreset LoadPreset(string key)
    {
        string jsonString = PlayerPrefs.GetString(key, null); // Handle potential null value
        if (string.IsNullOrEmpty(jsonString))
        {
            Debug.Log("none");
            return null; // Return null if no preset found for the key
        }
        return DeserializeFromJson(jsonString);
    }

}
