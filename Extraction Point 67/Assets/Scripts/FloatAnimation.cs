using UnityEngine;

public class FloatAnimation : MonoBehaviour
{
    [Header("Floating Motion")]
    public float amplitude = 0.25f; 
    public float frequency = 1f;   

    [Header("Rotation")]
    public Vector3 rotationSpeed = new Vector3(0, 45f, 0); 

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {

        float newY = startPos.y + Mathf.Sin(Time.time * frequency) * amplitude;

        transform.position = new Vector3(startPos.x, newY, startPos.z);


        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
