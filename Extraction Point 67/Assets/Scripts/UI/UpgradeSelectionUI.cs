
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeSelectionUI : MonoBehaviour
{
    [Header("Dependencies")]
    public PlayerStats player1Stats;
    public PlayerStats player2Stats; 
    private UpgradeManager upgradeManager;

    [Header("UI Structure")]
    public GameObject columnsContainer;

    [Header("Column 1 UI (Player 1)")]
    public TextMeshProUGUI p1_upgradeNameText; 
    public TextMeshProUGUI p1_descriptionText;
    public Image p1_iconImage;
    public Button p1_selectButton; 
    private Upgrade p1_upgrade;

    [Header("Column 2 UI (Player 2)")]
    public TextMeshProUGUI p2_upgradeNameText; 
    public TextMeshProUGUI p2_descriptionText; 
    public Image p2_iconImage;
    public Button p2_selectButton; 
    private Upgrade p2_upgrade;

    [Header("Column 3 UI (Team)")]
    public TextMeshProUGUI team_upgradeNameText; 
    public TextMeshProUGUI team_descriptionText; 
    public Image team_iconImage;
    public Button team_selectButton; 
    private Upgrade team_upgrade;
    [Header("Rarity Sounds")]
    public string commonSound = "UpgradeCommon";
    public string uncommonSound = "UpgradeUncommon";
    public string rareSound = "UpgradeRare";
    public string epicSound = "UpgradeEpic";
    public string legendarySound = "UpgradeLegendary";
    void Start()
    {
        upgradeManager = UpgradeManager.Instance;
        if (upgradeManager == null)
        {
            Debug.LogError("UpgradeManager instance not found!");
            return;
        }

        p1_selectButton.onClick.AddListener(() => OnUpgradeSelected(p1_upgrade, 1));
        p2_selectButton.onClick.AddListener(() => OnUpgradeSelected(p2_upgrade, 2));
        team_selectButton.onClick.AddListener(() => OnUpgradeSelected(team_upgrade, 3));
    }

    public void ShowUpgradeChoices()
    {
        GameManager.Instance.PauseGameForUI();
        if (columnsContainer != null) columnsContainer.SetActive(true);
        p1_upgrade = upgradeManager.GetRandomUpgrade(1); 
        p2_upgrade = upgradeManager.GetRandomUpgrade(2); 
        team_upgrade = upgradeManager.GetRandomUpgrade(3);

        p1_upgradeNameText.text = p1_upgrade.upgradeName;
        p1_descriptionText.text = p1_upgrade.description;
        p1_iconImage.sprite = p1_upgrade.icon;

        p2_upgradeNameText.text = p2_upgrade.upgradeName;
        p2_descriptionText.text = p2_upgrade.description;
        p2_iconImage.sprite = p2_upgrade.icon;  

        team_upgradeNameText.text = team_upgrade.upgradeName;
        team_descriptionText.text = team_upgrade.description;
        team_iconImage.sprite = team_upgrade.icon;


    }

    void OnUpgradeSelected(Upgrade chosenUpgrade, int choiceType)
    {
        if (choiceType == 1 && player1Stats != null) 
        {
            player1Stats.Apply(chosenUpgrade);
        }
        else if (choiceType == 2 && player2Stats != null) 
        {
            player2Stats.Apply(chosenUpgrade);
        }
        else if (choiceType == 3) 
        {
            if (player1Stats != null) player1Stats.Apply(chosenUpgrade);
            if (player2Stats != null) player2Stats.Apply(chosenUpgrade);
        }
        upgradeManager.HandleUpgradeSelection(chosenUpgrade, choiceType);
        PlayRaritySound(chosenUpgrade.rarity);

        if (columnsContainer != null) columnsContainer.SetActive(false);
        GameManager.Instance.ResumeGameFromUI();
    }
    void PlayRaritySound(UpgradeRarity rarity)
    {
        string soundToPlay = commonSound;

        switch (rarity)
        {
            case UpgradeRarity.Uncommon:
                soundToPlay = uncommonSound;
                break;
            case UpgradeRarity.Rare:
                soundToPlay = rareSound;
                break;
            case UpgradeRarity.Epic:
                soundToPlay = epicSound;
                break;
            case UpgradeRarity.Legendary:
                soundToPlay = legendarySound;
                break;
            case UpgradeRarity.Common:
            default:
                soundToPlay = commonSound;
                break;
        }

        AudioManager.Instance.PlaySFX(soundToPlay);
    }
}
