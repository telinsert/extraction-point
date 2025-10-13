// In /Scripts/Upgrades/Upgrade.cs

using UnityEngine;
public enum UpgradeRarity
{
    Common,
    Uncommon,
    Rare,
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

    // This is the core function. Each specific upgrade type will override this
    // to implement its own logic.
    public virtual void ApplyUpgrade(PlayerStats targetStats)
    {
        // Base implementation does nothing.
        Debug.Log($"Applying base upgrade '{upgradeName}' to {targetStats.gameObject.name}. This should be overridden!");
    }
}
