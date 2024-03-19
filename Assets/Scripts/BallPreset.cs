using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Resources/BallPresets")]
public class BallPreset : ScriptableObject
{
    public float drag = 0f;
    public float restitution = 0f;
    public float density = 0f;
    public float diameter = 0f;
}
