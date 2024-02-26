using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
   // public int i = 2;
    private static SettingsManager _instance;

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public static SettingsManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SettingsManager>();
                if (_instance == null)
                {
                    GameObject managerObject = new GameObject("SettingsManager");
                    _instance = managerObject.AddComponent<SettingsManager>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        // Ensure there's only one instance
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("MainSimulationScene");
    }

}
