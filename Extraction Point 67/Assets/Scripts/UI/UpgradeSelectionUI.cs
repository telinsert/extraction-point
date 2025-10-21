// In /Scripts/UI/UpgradeSelectionUI.cs

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeSelectionUI : MonoBehaviour
{
    [Header("Dependencies")]
    public UpgradeManager upgradeManager; // Drag the GameManager object here
    public PlayerStats player1Stats; // Drag Player 1 here
    public PlayerStats player2Stats; // Drag Player 2 here
    [Header("UI Structure")]
    public GameObject columnsContainer;

    [Header("Column 1 UI (Player 1)")]
    public TextMeshProUGUI p1_upgradeNameText; // <-- Changed
    public TextMeshProUGUI p1_descriptionText;
    public Image p1_iconImage;// <-- Changed
    public Button p1_selectButton; // Button stays the same
    private Upgrade p1_upgrade;

    [Header("Column 2 UI (Player 2)")]
    public TextMeshProUGUI p2_upgradeNameText; // <-- Changed
    public TextMeshProUGUI p2_descriptionText; // <-- Changed
    public Image p2_iconImage;// <-- Changed
    public Button p2_selectButton; // Button stays the same
    private Upgrade p2_upgrade;

    [Header("Column 3 UI (Team)")]
    public TextMeshProUGUI team_upgradeNameText; // <-- Changed
    public TextMeshProUGUI team_descriptionText; // <-- Changed
    public Image team_iconImage;// <-- Changed
    public Button team_selectButton; // Button stays the same
    private Upgrade team_upgrade;

    void Start()
    {
        // Wire up the button clicks to call our selection methods
        p1_selectButton.onClick.AddListener(() => OnUpgradeSelected(p1_upgrade, 1));
        p2_selectButton.onClick.AddListener(() => OnUpgradeSelected(p2_upgrade, 2));
        team_selectButton.onClick.AddListener(() => OnUpgradeSelected(team_upgrade, 3)); // 3 for team

    }

    public void ShowUpgradeChoices()
    {
        // 1. Pause the game
        Time.timeScale = 0f;
        if (columnsContainer != null) columnsContainer.SetActive(true);
        // 2. Get random upgrades from the manager
        p1_upgrade = upgradeManager.GetRandomUpgrade(false); // false = not a team upgrade
        p2_upgrade = upgradeManager.GetRandomUpgrade(false);
        team_upgrade = upgradeManager.GetRandomUpgrade(true); // true = team upgrade

        // 3. Populate the UI with the upgrade data
        p1_upgradeNameText.text = p1_upgrade.upgradeName;
        p1_descriptionText.text = p1_upgrade.description;
        p1_iconImage.sprite = p1_upgrade.icon;

        p2_upgradeNameText.text = p2_upgrade.upgradeName;
        p2_descriptionText.text = p2_upgrade.description;
        p2_iconImage.sprite = p2_upgrade.icon;  

        team_upgradeNameText.text = team_upgrade.upgradeName;
        team_descriptionText.text = team_upgrade.description;
        team_iconImage.sprite = team_upgrade.icon;

        // 4. Show the UI panel

    }

    void OnUpgradeSelected(Upgrade chosenUpgrade, int choiceType)
    {
        // Apply the upgrade to the correct player(s)
        if (choiceType == 1 && player1Stats != null) // Player 1
        {
            player1Stats.Apply(chosenUpgrade);
        }
        else if (choiceType == 2 && player2Stats != null) // Player 2
        {
            player2Stats.Apply(chosenUpgrade);
        }
        else if (choiceType == 3) // Team
        {
            if (player1Stats != null) player1Stats.Apply(chosenUpgrade);
            if (player2Stats != null) player2Stats.Apply(chosenUpgrade);
        }

        // Hide the UI and resume the game
        if (columnsContainer != null) columnsContainer.SetActive(false);
        Time.timeScale = 1f;
    }
}
