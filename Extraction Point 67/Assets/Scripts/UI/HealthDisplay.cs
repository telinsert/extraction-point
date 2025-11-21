using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    public Health health;       
    public Slider healthSlider; 
    public Text healthText;    

    void OnEnable()
    {
        if (health != null)
        {
            health.OnHealthChanged += UpdateHealthUI;

            UpdateHealthUI(health.GetCurrentHealth(), health.MaxHealth);
        }
    }

    void OnDisable()
    {
        if (health != null)
        {
            health.OnHealthChanged -= UpdateHealthUI;
        }
    }

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