using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int playerNumber = 1; // Set this to 1 or 2 in the Inspector for each player
    public float moveSpeed = 5f;

    private CharacterController controller;
    private Animator animator;
    private Vector3 moveDirection;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // --- Input ---
        float horizontal = 0f;
        float vertical = 0f;

        if (playerNumber == 1)
        {
            horizontal = Input.GetAxis("Horizontal"); // A/D keys
            vertical = Input.GetAxis("Vertical");     // W/S keys
        }
        else // Player 2 uses arrow keys
        {
            horizontal = Input.GetAxis("Horizontal2");
            vertical = Input.GetAxis("Vertical2");
        }

        // --- Movement ---
        moveDirection = new Vector3(horizontal, 0, vertical);
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);

        // --- Animation ---
        animator.SetFloat("Speed", moveDirection.magnitude);

        // --- Auto-Aim Rotation ---
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        if (closestEnemy != null)
        {
            Vector3 targetDirection = closestEnemy.transform.position - transform.position;
            targetDirection.y = 0; // Keep the character upright
            transform.rotation = Quaternion.LookRotation(targetDirection);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PowerUp"))
        {
            Debug.Log("Player " + playerNumber + " collected a power-up!");
            Destroy(other.gameObject); // Make the power-up disappear
        }
    }
}
