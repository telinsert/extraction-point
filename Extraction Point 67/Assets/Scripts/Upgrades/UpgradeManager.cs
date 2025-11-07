// In /Scripts/Upgrades/UpgradeManager.cs

using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Needed for the filtering (Where)

public class UpgradeManager : MonoBehaviour
{
    [Tooltip("Drag ALL of your Upgrade ScriptableObject assets here. This is the master list.")]
    public List<Upgrade> masterUpgradeList;
    [Header("Player References")]
    public PlayerStats player1Stats;
    public PlayerStats player2Stats;
    // --- MODIFIED ---
    // We now have three distinct pools instead of one.
    private List<Upgrade> player1SoloPool;
    private List<Upgrade> player2SoloPool;
    private List<Upgrade> teamUpgradePool;

    void Awake()
    {
        InitializeUpgradePool();
    }

    public void InitializeUpgradePool()
    {
        // --- MODIFIED ---
        // Create new instances for all three lists.
        player1SoloPool = new List<Upgrade>();
        player2SoloPool = new List<Upgrade>();
        teamUpgradePool = new List<Upgrade>();

        foreach (var upgrade in masterUpgradeList)
        {
            if (!upgrade.isUnlockable)
            {
                if (upgrade.isTeamUpgrade)
                {
                    // Add team upgrades to the shared team pool.
                    teamUpgradePool.Add(upgrade);
                }
                else
                {
                    // Add solo upgrades to BOTH player pools at the start.
                    player1SoloPool.Add(upgrade);
                    player2SoloPool.Add(upgrade);
                }
            }
        }
    }

    // --- MODIFIED ---
    // The signature has changed. Instead of a boolean, we use an integer
    // to specify which column (P1, P2, or Team) we need an upgrade for.
    public Upgrade GetRandomUpgrade(int choiceType)
    {
        List<Upgrade> sourcePool;

        // Select the correct pool based on the choice type
        if (choiceType == 1)
        {
            sourcePool = player1SoloPool;
        }
        else if (choiceType == 2)
        {
            sourcePool = player2SoloPool;
        }
        else // choiceType == 3
        {
            sourcePool = teamUpgradePool;
        }

        if (sourcePool.Count == 0)
        {
            Debug.LogWarning($"No upgrades found for choice type: {choiceType}");
            return null;
        }

        // The weighted random selection logic itself is unchanged.
        // It now correctly operates on the specified player's pool.
        float totalWeight = 0;
        foreach (var upgrade in sourcePool)
        {
            totalWeight += GetWeightForRarity(upgrade.rarity);
        }

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

    // --- MODIFIED ---
    // This method now also takes the choiceType to know which pool to modify.
    public void HandleUpgradeSelection(Upgrade chosenUpgrade, int choiceType)
    {
        // Handle UNLOCKS first.
        if (chosenUpgrade.unlocksUpgrades != null && chosenUpgrade.unlocksUpgrades.Count > 0)
        {
            foreach (var unlockedUpgrade in chosenUpgrade.unlocksUpgrades)
            {
                if (unlockedUpgrade.isTeamUpgrade)
                {
                    // If a team upgrade is unlocked, add it to the team pool.
                    if (!teamUpgradePool.Contains(unlockedUpgrade))
                    {
                        teamUpgradePool.Add(unlockedUpgrade);
                        Debug.Log($"Unlocked new TEAM upgrade: {unlockedUpgrade.upgradeName}");
                    }
                }
                else
                {
                    if (choiceType == 1) // Player 1 made the choice.
                    {
                        // Add the unlocked solo upgrade ONLY to Player 1's pool.
                        if (!player1SoloPool.Contains(unlockedUpgrade))
                        {
                            player1SoloPool.Add(unlockedUpgrade);
                            // Updated log for clarity
                            Debug.Log($"Unlocked new SOLO upgrade for PLAYER 1: {unlockedUpgrade.upgradeName}");
                        }
                    }
                    else if (choiceType == 2) // Player 2 made the choice.
                    {
                        // Add the unlocked solo upgrade ONLY to Player 2's pool.
                        if (!player2SoloPool.Contains(unlockedUpgrade))
                        {
                            player2SoloPool.Add(unlockedUpgrade);
                            Debug.Log($"Unlocked new SOLO upgrade for PLAYER 2: {unlockedUpgrade.upgradeName}");
                        }
                    }
                    else // choiceType == 3 (A team upgrade unlocked a solo upgrade)
                    {
                        // If a team choice unlocks a solo upgrade, it's fair to add it to both pools.
                        if (!player1SoloPool.Contains(unlockedUpgrade)) player1SoloPool.Add(unlockedUpgrade);
                        if (!player2SoloPool.Contains(unlockedUpgrade)) player2SoloPool.Add(unlockedUpgrade);
                        Debug.Log($"Unlocked new SOLO upgrade for BOTH players (from a team choice): {unlockedUpgrade.upgradeName}");
                    }
                }
            }
        }


        // Handle REMOVAL second.
        if (chosenUpgrade.removeAfterSelection)
        {
            if (choiceType == 1)
            {
                player1SoloPool.Remove(chosenUpgrade);
                Debug.Log($"Removed {chosenUpgrade.upgradeName} from Player 1's pool.");
            }
            else if (choiceType == 2)
            {
                player2SoloPool.Remove(chosenUpgrade);
                Debug.Log($"Removed {chosenUpgrade.upgradeName} from Player 2's pool.");
            }
            else // choiceType == 3
            {
                teamUpgradePool.Remove(chosenUpgrade);
                Debug.Log($"Removed TEAM upgrade {chosenUpgrade.upgradeName} from the shared pool.");
            }
        }
        CheckForSynergyUnlocks();

    }
    private void CheckForSynergyUnlocks()
    {
        // We need to check every upgrade in our master list.
        foreach (var potentialSynergy in masterUpgradeList)
        {
            // Skip if it's not a synergy, or if it's already in the team pool.
            if (!potentialSynergy.isSynergy || teamUpgradePool.Contains(potentialSynergy))
            {
                continue;
            }

            // Assume the conditions are met until proven otherwise.
            bool p1ConditionsMet = true;
            foreach (var requiredUpg in potentialSynergy.synergyRequirements.requiredUpgradesForP1)
            {
                // If P1 is missing even one required upgrade, the condition fails.
                if (!player1Stats.appliedUpgrades.Contains(requiredUpg))
                {
                    p1ConditionsMet = false;
                    break;
                }
            }

            // If P1 failed, no need to check P2.
            if (!p1ConditionsMet) continue;

            bool p2ConditionsMet = true;
            foreach (var requiredUpg in potentialSynergy.synergyRequirements.requiredUpgradesForP2)
            {
                if (!player2Stats.appliedUpgrades.Contains(requiredUpg))
                {
                    p2ConditionsMet = false;
                    break;
                }
            }

            // If both players meet all their requirements...
            if (p1ConditionsMet && p2ConditionsMet)
            {
                // ...add the synergy upgrade to the team pool!
                teamUpgradePool.Add(potentialSynergy);
                Debug.Log($"SYNERGY UNLOCKED: {potentialSynergy.upgradeName} was added to the team pool!");
            }
        }
    }
    private float GetWeightForRarity(UpgradeRarity rarity)
    {
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
