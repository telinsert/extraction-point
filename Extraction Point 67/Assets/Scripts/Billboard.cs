using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        // Cache the main camera reference
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCamera != null)
        {
            // Make the health bar face the camera
            transform.LookAt(transform.position + mainCamera.transform.forward);
        }
    }
}
