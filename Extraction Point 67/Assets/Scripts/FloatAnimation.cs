using UnityEngine;

public class FloatAnimation : MonoBehaviour
{
    // --- Floating Settings ---
    [Header("Floating Motion")]
    public float amplitude = 0.25f; // How high and low it goes from the center
    public float frequency = 1f;   // How fast it bobs up and down

    // --- Rotation Settings ---
    [Header("Rotation")]
    public Vector3 rotationSpeed = new Vector3(0, 45f, 0); // Degrees per second on each axis

    // Private variables
    private Vector3 startPos;

    void Start()
    {
        // Store the starting position when the game begins
        startPos = transform.position;
    }

    void Update()
    {
        // --- Handle Floating ---
        // Calculate the new Y position using a Sine wave for smooth motion
        float newY = startPos.y + Mathf.Sin(Time.time * frequency) * amplitude;

        // Apply the new Y position, keeping the original X and Z
        transform.position = new Vector3(startPos.x, newY, startPos.z);


        // --- Handle Rotation ---
        // Rotate the object around its local axes
        // Time.deltaTime makes the rotation smooth and frame-rate independent
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
