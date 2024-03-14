using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomBuilding : MonoBehaviour
{
    public int numFloors, WidthInMetres;
    public enum BuildingType
    {
        None, Target, Culprit
    }
    public BuildingType typeofBuilding;
    public Quaternion originalRotation;
    public float AddedAngle = 0;

    
    private void Start()
    {
        originalRotation = transform.rotation;
    }
}