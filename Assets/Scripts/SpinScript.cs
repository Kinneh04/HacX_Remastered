using UnityEngine;

public class SpinScript : MonoBehaviour
{
    public float spinSpeed = 100.0f;
    public Vector3 spinAxis = Vector3.up;

    void Update()
    {
        // Rotate the object around the specified axis at the specified speed
        transform.Rotate(spinAxis, spinSpeed * Time.deltaTime);
    }
}
