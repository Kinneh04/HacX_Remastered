using UnityEngine;

public class BillboardText : MonoBehaviour
{
    private Transform mainCamera;

    void Start()
    {
        mainCamera = Camera.main.transform;
    }

    void LateUpdate()
    {
        // Look at the main camera, but only rotate on the Y axis
        transform.LookAt(mainCamera.position, Vector3.up);
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
    }
}
