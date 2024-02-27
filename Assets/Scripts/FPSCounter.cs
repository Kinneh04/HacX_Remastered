using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    public TMP_Text fpsText; // Reference to your TMP_Text component
    float deltaTime = 0.0f;

    private void Start()
    {
        //DontDestroyOnLoad(gameObject);
    }
    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        UpdateFPSDisplay();
    }

    void UpdateFPSDisplay()
    {
        if (fpsText != null)
        {
            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;

            string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
            fpsText.text = text;
        }
    }
}
