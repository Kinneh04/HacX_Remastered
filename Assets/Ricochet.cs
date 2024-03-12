using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ricochet : MonoBehaviour
{
    Collider collider;
    private void Awake()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        other.GetComponent<Ball>().canRico = true;
    }

    public void OnTriggerExit(Collider other)
    {
        other.GetComponent<Ball>().canRico = false;
    }
}
