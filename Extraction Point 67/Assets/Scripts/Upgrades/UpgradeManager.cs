// In /Scripts/Upgrades/UpgradeManager.cs

using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Needed for the filtering (Where)

public class UpgradeManager : MonoBehaviour
{
    // You will drag all of your Upgrade ScriptableObject assets into this list in the Inspector
    public List<Upgrade> allUpgrades;

    // This method gets a random upgrade, filtering by team status
    public Upgrade GetRandomUpgrade(bool forTeam)
    {
        // 1. Filter the list to get only the upgrades that match the team status
        var availableUpgrades = allUpgrades.Where(u => u.isTeamUpgrade == forTeam).ToList();

        if (availableUpgrades.Count == 0)
        {
            Debug.LogWarning($"No upgrades found for type: {(forTeam ? "Team" : "Solo")}");
            return null;
        }

        // 2. Weighted Random Selection based on Rarity
        // This makes common upgrades appear more often than rare ones
        float totalWeight = 0;
        foreach (var upgrade in availableUpgrades)
        {
            totalWeight += GetWeightForRarity(upgrade.rarity);
        }

        float randomValue = Random.Range(0, totalWeight);
        float currentWeight = 0;

        foreach (var upgrade in availableUpgrades)
        {
            currentWeight += GetWeightForRarity(upgrade.rarity);
            if (randomValue <= currentWeight)
            {
                return upgrade;
            }
        }

        return null; // Should not happen
    }

    // Assigns a "weight" to each rarity. Higher weight = more common.
    private float GetWeightForRarity(UpgradeRarity rarity)
    {
        switch (rarity)
        {
            case UpgradeRarity.Common: return 10.0f;
            case UpgradeRarity.Uncommon: return 5.0f;
            case UpgradeRarity.Rare: return 2.0f;
            case UpgradeRarity.Legendary: return 0.5f;
            default: return 1.0f;
        }
    }
}
