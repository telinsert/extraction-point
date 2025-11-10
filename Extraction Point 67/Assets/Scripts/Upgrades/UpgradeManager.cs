// In /Scripts/Upgrades/UpgradeManager.cs


using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class UpgradeManager : MonoBehaviour
{
    // --- NEW --- Singleton Pattern
    public static UpgradeManager Instance { get; private set; }

    [Tooltip("Drag ALL of your Upgrade ScriptableObject assets here. This is the master list.")]
    public List<Upgrade> masterUpgradeList;
    [Header("Player References")]
    public PlayerStats player1Stats; // Note: These will be updated by GameManager
    public PlayerStats player2Stats;

    private List<Upgrade> player1SoloPool;
    private List<Upgrade> player2SoloPool;
    private List<Upgrade> teamUpgradePool;

    // --- MODIFIED ---
    void Awake()
    {
        // --- Singleton Logic ---
        if (Instance != null && Instance != this)
        {
            // If another instance already exists, destroy this one.
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // --- Persistence ---
        DontDestroyOnLoad(gameObject);

        // --- Initialize Pools ---
        // This will now only run ONCE at the start of the entire game.
        InitializeUpgradePool();
    }

    // The rest of your UpgradeManager script remains exactly the same.
    // No other changes are needed in this file.

    public void InitializeUpgradePool()
    {
        // ... (This method is unchanged)
        player1SoloPool = new List<Upgrade>();
        player2SoloPool = new List<Upgrade>();
        teamUpgradePool = new List<Upgrade>();
        foreach (var upgrade in masterUpgradeList)
        {
            if (!upgrade.isUnlockable)
            {
                if (upgrade.isTeamUpgrade)
                {
                    teamUpgradePool.Add(upgrade);
                }
                else
                {
                    player1SoloPool.Add(upgrade);
                    player2SoloPool.Add(upgrade);
                }
            }
        }
    }

    public Upgrade GetRandomUpgrade(int choiceType)
    {
        // ... (This method is unchanged)
        List<Upgrade> sourcePool;
        if (choiceType == 1) sourcePool = player1SoloPool;
        else if (choiceType == 2) sourcePool = player2SoloPool;
        else sourcePool = teamUpgradePool;

        if (sourcePool.Count == 0)
        {
            Debug.LogWarning($"No upgrades found for choice type: {choiceType}");
            return null;
        }

        float totalWeight = sourcePool.Sum(upgrade => GetWeightForRarity(upgrade.rarity));
        float randomValue = Random.Range(0, totalWeight);
        float currentWeight = 0;

        foreach (var upgrade in sourcePool)
        {
            currentWeight += GetWeightForRarity(upgrade.rarity);
            if (randomValue <= currentWeight)
            {
                return upgrade;
            }
        }
        return null;
    }

    public void HandleUpgradeSelection(Upgrade chosenUpgrade, int choiceType)
    {
        // ... (This method is unchanged)
        if (chosenUpgrade.unlocksUpgrades != null && chosenUpgrade.unlocksUpgrades.Count > 0)
        {
            foreach (var unlockedUpgrade in chosenUpgrade.unlocksUpgrades)
            {
                if (unlockedUpgrade.isTeamUpgrade)
                {
                    if (!teamUpgradePool.Contains(unlockedUpgrade))
                    {
                        teamUpgradePool.Add(unlockedUpgrade);
                        Debug.Log($"Unlocked new TEAM upgrade: {unlockedUpgrade.upgradeName}");
                    }
                }
                else
                {
                    if (choiceType == 1)
                    {
                        if (!player1SoloPool.Contains(unlockedUpgrade))
                        {
                            player1SoloPool.Add(unlockedUpgrade);
                            Debug.Log($"Unlocked new SOLO upgrade for PLAYER 1: {unlockedUpgrade.upgradeName}");
                        }
                    }
                    else if (choiceType == 2)
                    {
                        if (!player2SoloPool.Contains(unlockedUpgrade))
                        {
                            player2SoloPool.Add(unlockedUpgrade);
                            Debug.Log($"Unlocked new SOLO upgrade for PLAYER 2: {unlockedUpgrade.upgradeName}");
                        }
                    }
                    else
                    {
                        if (!player1SoloPool.Contains(unlockedUpgrade)) player1SoloPool.Add(unlockedUpgrade);
                        if (!player2SoloPool.Contains(unlockedUpgrade)) player2SoloPool.Add(unlockedUpgrade);
                        Debug.Log($"Unlocked new SOLO upgrade for BOTH players (from a team choice): {unlockedUpgrade.upgradeName}");
                    }
                }
            }
        }

        if (chosenUpgrade.removeAfterSelection)
        {
            if (choiceType == 1) player1SoloPool.Remove(chosenUpgrade);
            else if (choiceType == 2) player2SoloPool.Remove(chosenUpgrade);
            else teamUpgradePool.Remove(chosenUpgrade);
        }
        CheckForSynergyUnlocks();
    }

    private void CheckForSynergyUnlocks()
    {
        // ... (This method is unchanged)
        foreach (var potentialSynergy in masterUpgradeList)
        {
            if (!potentialSynergy.isSynergy || teamUpgradePool.Contains(potentialSynergy)) continue;

            bool p1ConditionsMet = potentialSynergy.synergyRequirements.requiredUpgradesForP1.All(req => player1Stats.appliedUpgrades.Contains(req));
            if (!p1ConditionsMet) continue;

            bool p2ConditionsMet = potentialSynergy.synergyRequirements.requiredUpgradesForP2.All(req => player2Stats.appliedUpgrades.Contains(req));

            if (p1ConditionsMet && p2ConditionsMet)
            {
                teamUpgradePool.Add(potentialSynergy);
                Debug.Log($"SYNERGY UNLOCKED: {potentialSynergy.upgradeName} was added to the team pool!");
            }
        }
    }

    private float GetWeightForRarity(UpgradeRarity rarity)
    {
        // ... (This method is unchanged)
        switch (rarity)
        {
            case UpgradeRarity.Common: return 10.0f;
            case UpgradeRarity.Uncommon: return 5.0f;
            case UpgradeRarity.Rare: return 2.0f;
            case UpgradeRarity.Epic: return 1.0f;
            case UpgradeRarity.Legendary: return 0.5f;
            default: return 1.0f;
        }
    }
}
