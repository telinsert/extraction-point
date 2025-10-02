using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public Health health;
    public Slider healthSlider;

    void Start()
    {
        if (health != null)
        {
            healthSlider.maxValue = health.maxHealth;
            healthSlider.value = health.GetCurrentHealth();
        }
    }

    void Update()
    {
        if (health != null)
        {
            healthSlider.value = health.GetCurrentHealth();
        }
    }
}
