using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthUI : MonoBehaviour
{
    public static BossHealthUI Instance { get; private set; }

    [Header("UI Structure")]
    [Tooltip("Drag the child GameObject (Panel Container) here.")]
    public GameObject contentHolder;

    [Header("UI Elements")]
    public Slider healthSlider;
    public TextMeshProUGUI bossNameText;
    public TextMeshProUGUI healthAmountText;

    // Keep track of the original color to reset it if needed
    private Color originalNameColor;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (bossNameText != null) originalNameColor = bossNameText.color;
        HideUI();
    }

    public void InitializeBoss(Health bossHealth, string name)
    {
        if (contentHolder != null) contentHolder.SetActive(true);

        // Reset visual style (in case a previous boss was enraged)
        if (bossNameText != null)
        {
            bossNameText.text = name;
            bossNameText.color = originalNameColor;
        }

        // Initialize Slider & Text
        if (healthSlider != null)
        {
            healthSlider.maxValue = bossHealth.MaxHealth;
            healthSlider.value = bossHealth.GetCurrentHealth();
        }
        UpdateHealthText(bossHealth.GetCurrentHealth(), bossHealth.MaxHealth);

        // Subscribe
        bossHealth.OnHealthChanged += (current, max) =>
        {
            if (healthSlider != null) healthSlider.value = current;
            UpdateHealthText(current, max);
        };

        bossHealth.OnDeath += HideUI;
    }

    // --- NEW METHOD ---
    public void EnableEnragedVisuals(string originalName)
    {
        if (bossNameText != null)
        {
            bossNameText.text = $"{originalName} <size=80%>(ENRAGED)</size>";
            bossNameText.color = Color.red;
        }
    }

    private void UpdateHealthText(int current, int max)
    {
        if (healthAmountText != null)
        {
            healthAmountText.text = $"{current} / {max}";
        }
    }

    public void HideUI()
    {
        if (contentHolder != null) contentHolder.SetActive(false);
    }
}