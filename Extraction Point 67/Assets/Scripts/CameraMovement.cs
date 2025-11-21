using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Target")]
    public Transform player1;
    public Transform player2;

    [Header("Smoothing")]
    public float smoothSpeed = 0.125f;

    [Header("Zoom")]
    public float minZoom = 10f; 
    public float maxZoom = 25f;
    public float zoomLimiter = 50f; 

    private Vector3 offset;
    private Camera cam;

    void Start()
    {
        offset = transform.position - GetMidpoint();
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (player1 == null && player2 == null)
            return;

        Vector3 midpoint = GetMidpoint();
        float newZoom = Mathf.Lerp(minZoom, maxZoom, GetDistanceBetweenPlayers() / zoomLimiter);

        Vector3 desiredPosition = midpoint + offset.normalized * newZoom;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.position = smoothedPosition;
    }

    float GetDistanceBetweenPlayers()
    {
        if (player1 != null && player2 != null)
            return Vector3.Distance(player1.position, player2.position);
        else if (player1 != null)
            return 0f; 
        else if (player2 != null)
            return 0f;
        return 0f;
    }

    Vector3 GetMidpoint()
    {
        if (player1 != null && player2 != null)
            return (player1.position + player2.position) / 2f;
        else if (player1 != null)
            return player1.position; 
        else if (player2 != null)
            return player2.position; 
        return Vector3.zero;
    }
}