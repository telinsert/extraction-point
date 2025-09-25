using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Target")]
    public Transform player1;
    public Transform player2;

    [Header("Smoothing")]
    public float smoothSpeed = 0.125f;

    [Header("Zoom")]
    public float minZoom = 10f; // Min distance from midpoint
    public float maxZoom = 25f; // Max distance from midpoint
    public float zoomLimiter = 50f; // Controls how "zoomed in" the view is

    private Vector3 offset;
    private Camera cam;

    void Start()
    {
        offset = transform.position - GetMidpoint();
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (player1 == null || player2 == null)
            return;

        // Find the midpoint and the new zoom level
        Vector3 midpoint = GetMidpoint();
        float newZoom = Mathf.Lerp(minZoom, maxZoom, GetDistanceBetweenPlayers() / zoomLimiter);

        // Calculate the camera's new position
        Vector3 desiredPosition = midpoint + offset.normalized * newZoom;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Apply the new position
        transform.position = smoothedPosition;
    }

    // A helper function to get the distance between players
    float GetDistanceBetweenPlayers()
    {
        return Vector3.Distance(player1.position, player2.position);
    }

    // A helper function to find the midpoint
    Vector3 GetMidpoint()
    {
        return (player1.position + player2.position) / 2f;
    }
}