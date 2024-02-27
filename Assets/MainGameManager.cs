using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
public class MainGameManager : MonoBehaviour
{
    public static event Action OnStartGame;
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void StartSimulation()
    {
        OnStartGame?.Invoke();
        WindowsManager.Instance.canSelectWindow = false;
        Debug.Log("Starting SImulation");
    }
}
