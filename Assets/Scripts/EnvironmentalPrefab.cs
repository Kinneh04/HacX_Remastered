using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentalPrefab : MonoBehaviour
{
    public Outline outline;
    // Set the speed at which you want to fade out
    public float fadeSpeed = 0.5f;
    public int propIndex = 0;
    private void Start()
    {
        outline = GetComponent<Outline>();
    }
    private void Update()
    {
        if (outline.OutlineColor.a > 0)
        {
            // Set the speed at which you want to fade out

            // Calculate the new alpha value using Lerp
            float newAlpha = Mathf.Lerp(outline.OutlineColor.a, 0f, fadeSpeed * Time.deltaTime);

            // Update the alpha value of the outline color
            outline.OutlineColor = new Color(outline.OutlineColor.r, outline.OutlineColor.g, outline.OutlineColor.b, newAlpha);
        }
    }
}
