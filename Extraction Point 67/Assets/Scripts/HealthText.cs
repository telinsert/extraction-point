using UnityEngine;
using UnityEngine.UI;

public class HealthText : MonoBehaviour
{
    public Health health;      // Reference to the Health script
    public Text healthText;     // Reference to the UI Text component

    void Start()
    {
        if (health != null && healthText != null)
        {
            UpdateHealthText();
        }
    }

    void Update()
    {
        if (health != null && healthText != null)
        {
            UpdateHealthText();
        }
    }

    void UpdateHealthText()
    {
        healthText.text = $"Health: {health.GetCurrentHealth()} / {health.maxHealth}";
    }
}
