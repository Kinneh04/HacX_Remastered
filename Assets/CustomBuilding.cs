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

    // Have to use these shitty vars instead of Vec3 coz 
    public float PositionX, PositionY, PositionZ;
    public float RotationX, RotationY, RotationZ;

    public void UpdateBuildingTransforms()
    {
        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;

        PositionX = pos.x;
        PositionY = pos.y;
        PositionZ = pos.z;

        RotationX = rot.x;
        RotationY = rot.y;
        RotationZ = rot.z;
    }
}
