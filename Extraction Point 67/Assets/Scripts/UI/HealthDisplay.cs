// New script: HealthDisplay.cs
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    public Health health;       // Reference to the Health script
    public Slider healthSlider; // Optional: drag the slider here
    public Text healthText;     // Optional: drag the text here

    // Subscribe to events when this object is enabled
    void OnEnable()
    {
        if (health != null)
        {
            health.OnHealthChanged += UpdateHealthUI;
            // Also update the UI immediately on start
            UpdateHealthUI(health.GetCurrentHealth(), health.maxHealth);
        }
    }

    // Unsubscribe when the object is disabled to prevent errors
    void OnDisable()
    {
        if (health != null)
        {
            health.OnHealthChanged -= UpdateHealthUI;
        }
    }

    // This function is now only called when the health actually changes
    private void UpdateHealthUI(int currentHealth, int maxHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        if (healthText != null)
        {
            healthText.text = $"Health: {currentHealth} / {maxHealth}";
        }
    }
}