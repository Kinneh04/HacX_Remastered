using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    public TMP_Text fpsText; // Reference to your TMP_Text component

    private const float updateRate = 0.1f;
    private float deltaTime = 0.0f;

    private void LateUpdate()
    {
        UpdateDeltaTime();
        UpdateFPSDisplay();
    }

    private void UpdateDeltaTime()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * updateRate;
    }

    private void UpdateFPSDisplay()
    {
        if (fpsText == null) return;

        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;

        string text = $"{msec:0.0} ms ({fps:0.} fps)";
        fpsText.text = text;
    }
}
