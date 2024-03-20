using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyCarTypes : MonoBehaviour
{
    public List<Car> Cars = new();
    private static DontDestroyCarTypes _instance;
    public static DontDestroyCarTypes Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<DontDestroyCarTypes>();
                if (_instance == null)
                {
                    GameObject managerObject = new GameObject("SettingsManager");
                    _instance = managerObject.AddComponent<DontDestroyCarTypes>();
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
}

[System.Serializable]
public class Car
{
    public string carName;
    [TextArea(5,5)]
    public string carDesc;
    public GameObject CarModel;
}
