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
public class PresetManager : MonoBehaviour
{
    public const string PresetKeyPrefix = "BallPreset_"; // Prefix for keys in PlayerPrefs

    public static void SavePreset(BallPreset preset, int index)
    {
        int currentCount = PlayerPrefs.GetInt("PresetCount");
        string key = PresetKeyPrefix + index;
        string jsonString = BallPreset.SerializeToJson(preset);


        if(!PlayerPrefs.HasKey(key) || index == 0)
        {
            // Save as a new preset (original behavior)
            PlayerPrefs.SetInt("PresetCount", currentCount + 1);
            key = PresetKeyPrefix + currentCount;

            PlayerPrefs.SetString(key, jsonString);
            // Update PresetCount if you're using it
        }
        else
            PlayerPrefs.SetString(key, jsonString);
    }

    public static void OverridePreset(BallPreset b)
    {
        string key = FindKey(b.name);
        if(PlayerPrefs.HasKey(key))
        {
            string jsonString = BallPreset.SerializeToJson(b);
            PlayerPrefs.SetString(key, jsonString);
        }
    }
    public static void DeletePreset(BallPreset preset, int index)
    {
        string key = PresetKeyPrefix + index;
        PlayerPrefs.DeleteKey(key);
        // Update PresetCount if you're using it
        int currentCount = PlayerPrefs.GetInt("PresetCount", 0);
        if (currentCount > 0)
        {
            PlayerPrefs.SetInt("PresetCount", currentCount - 1);
        }
    }
    public static List<BallPreset> LoadAllPresets()
    {
        List<BallPreset> presets = new List<BallPreset>();
        int check = PlayerPrefs.GetInt("PresetCount");
        int found = 0;
        int i = 0;
        while(found < check)
        {
            string key = PresetKeyPrefix + i;

            if (!PlayerPrefs.HasKey(key))
            {
                i++;
                continue;
            }
            else
            {

                BallPreset preset = BallPreset.LoadPreset(key);
                presets.Add(preset);
                found++;
                i++;
            }
        }
        return presets;
    }

    public static string FindKey(string name)
    {
        if (name == null)
            return null;
        string key;
        int check = PlayerPrefs.GetInt("PresetCount");
        int found = 0;
        int i = 0;
        while (i < 20)
        {
            key = PresetKeyPrefix + i;

            if (!PlayerPrefs.HasKey(key))
            {
                i++;
                continue;
            }
            else
            {

                BallPreset preset = BallPreset.LoadPreset(key);
                found++;
                i++;
                if (preset.name == name)
                    return key;
            }
        }
        return null;
    }
}
