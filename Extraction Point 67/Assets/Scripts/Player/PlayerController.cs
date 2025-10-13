// In PlayerController.cs

using UnityEngine;
[RequireComponent(typeof(PlayerStats))]
public class PlayerController : MonoBehaviour
{

    public int playerNumber = 1;
    public float autoAimRange = 12f;

 
    public GameObject bulletPrefab;
    public Transform firePoint;

    private PlayerStats stats;
    private CharacterController controller;
    private Animator animator;
    private Vector3 moveDirection;
    private float nextFireTime = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        stats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        // ... (All your movement, animation, and auto-aim logic) ...
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
        // Use stats.moveSpeed instead of moveSpeed
        controller.Move(moveDirection * stats.moveSpeed * Time.deltaTime);

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

        if (closestEnemy != null && closestDistance <= autoAimRange)
        {
            Vector3 targetDirection = closestEnemy.transform.position - transform.position;
            targetDirection.y = 0;
            if (targetDirection.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.LookRotation(targetDirection);
            }

            if (Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + 1f / stats.fireRate;
                Fire();
            }
        }
        else
        {
            Vector3 moveDir = new Vector3(moveDirection.x, 0f, moveDirection.z);
            if (moveDir.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.LookRotation(moveDir.normalized);
            }
        }
    }

    void Fire()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            // 1. Instantiate the bullet and keep a reference to it
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            // 2. Get the BulletController component from the new bullet
            BulletController bulletCtrl = bullet.GetComponent<BulletController>();

            // 3. Set the bullet's damage to the player's current damage
            if (bulletCtrl != null)
            {
                bulletCtrl.damageAmount = stats.bulletDamage;
                bulletCtrl.speed = stats.bulletSpeed;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PowerUp"))
        {
            GetComponent<Health>()?.Heal(25);
            Destroy(other.gameObject);
        }
    }
}
