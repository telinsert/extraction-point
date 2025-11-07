// In /Scripts/Upgrades/Upgrade.cs

using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SynergyDefinition
{
    [Tooltip("Which upgrades does Player 1 need to have for this synergy to unlock?")]
    public List<Upgrade> requiredUpgradesForP1;
    [Tooltip("Which upgrades does Player 2 need to have for this synergy to unlock?")]
    public List<Upgrade> requiredUpgradesForP2;
}
public enum UpgradeRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}
// This attribute allows you to create instances of this script as assets in the Unity Editor
// menuName is the path you will use to create a new Upgrade asset
[CreateAssetMenu(fileName = "New Upgrade", menuName = "Roguelike/Upgrade")]
public class Upgrade : ScriptableObject
{
    [Header("Upgrade Info")]
    public string upgradeName;
    [TextArea(3, 5)]
    public string description;
    public Sprite icon; // For the UI later

    [Header("Upgrade Stats")]
    public UpgradeRarity rarity;
    public bool isTeamUpgrade = false; // Is this a smaller boost for both players?
    [Header("Unlock & Progression Logic")]
    [Tooltip("If checked, this upgrade will NOT appear in the pool until another upgrade unlocks it.")]
    public bool isUnlockable = false;

    [Tooltip("If checked, this upgrade is removed from the pool for the rest of the run after being selected.")]
    public bool removeAfterSelection = false;

    [Tooltip("Which upgrades should be added to the pool after this one is selected?")]
    public List<Upgrade> unlocksUpgrades;
    [Header("Synergy Logic")]
    [Tooltip("If checked, this upgrade will only be unlocked if the Synergy Requirements are met.")]
    public bool isSynergy = false;
    // This will only appear in the Inspector if isSynergy is checked.
    public SynergyDefinition synergyRequirements;

    // This is the core function. Each specific upgrade type will override this
    // to implement its own logic.
    public virtual void ApplyUpgrade(PlayerStats targetStats)
    {
        // Base implementation does nothing.
        Debug.Log($"Applying base upgrade '{upgradeName}' to {targetStats.gameObject.name}. This should be overridden!");
    }
}
