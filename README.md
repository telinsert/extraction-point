# 🧟 Extraction Point 67  

Two marines killing hordes of zombies with their weapons, collecting power-ups and upgrades in order to help you advance through areas and fight stronger zombies  

---

## 📖 Overview  
Extraction Point 67 is a top-down, two-player couch co-op auto-shooter set in a zombie-infested Los Angeles. Players assume the roles of two elite marines, Frank and Castle, tasked with recovering cure components from overrun zones ranging from forests to military bases.

Blending the horde-survival intensity of Vampire Survivors with the tactical cooperation of Call of Duty: Zombies, the game emphasizes positioning over aiming. Players must coordinate their builds to unlock powerful team synergies, manage high-stakes revives, and survive increasingly difficult waves of undead to reach the final extraction point.  

---

## 🎮 Gameplay Features  
- **Co-op Focused Survival:** A tethered camera system and a high-risk revive mechanic—where players must stand still to save a downed partner—force teammates to cover each other constantly.
- **Synergy Upgrade System:** Upgrades are not just individual; if Player 1 builds "Fire" and Player 2 builds "Explosions," they unlock unique Team Synergies that combine these effects.  
- **Auto-Shooter Combat:** Characters automatically target the nearest threat, shifting the player's focus to movement, crowd control, and ability management.  
- **Roguelite Progression:** Features a run-based structure with permadeath. Utilizing varied weapon upgrades and stat boosts found in chests, no two runs are the same.
- **Dynamic AI & Bosses:** Enemies utilize NavMesh pathfinding to swarm players, culminating in a multi-stage Final Boss fight with enrage mechanics and summon abilities.

---

## 💻 Technical Highlights
**Synergy System Architecture**
The upgrade system utilizes **ScriptableObjects** and **LINQ** to dynamically check player stats during runtime. If specific conditions are met across *both* players, rare upgrades are injected into the loot pool.

```csharp
// Snippet from UpgradeManager.cs
private void CheckForSynergyUnlocks()
{
  foreach (var potentialSynergy in masterUpgradeList)
  {
    // Check if synergy is already unlocked or not applicable
    if (!potentialSynergy.isSynergy || teamUpgradePool.Contains(potentialSynergy)) continue;

    // Check conditions using LINQ
    bool p1Met = potentialSynergy.synergyRequirements.requiredUpgradesForP1.All(req => player1Stats.appliedUpgrades.Contains(req));
    bool p2Met = potentialSynergy.synergyRequirements.requiredUpgradesForP2.All(req => player2Stats.appliedUpgrades.Contains(req));

    if (p1Met && p2Met)
    {
       // Unlock the powerful team upgrade
       teamUpgradePool.Add(potentialSynergy);
       Debug.Log($"SYNERGY UNLOCKED: {potentialSynergy.upgradeName}");
    }
  }
}
```

---

## 🛠️ Technology
- **Engine:** Unity 2022+
- **Language:** C#
- **Architecture:** ScriptableObject-based Data, Singleton Managers, Object Pooling (Audio)
- **AI:** Unity NavMesh Agents & State Machines
- **Platform:** PC (Windows)

---

## 🚧 Development Roadmap
- [x] **Core Loop:** Auto-shooting, movement, and wave completion triggers.
- [x] **Co-op Systems:** Shared camera logic, revive UI, and split input handling.
- [x] **Data Persistence:** State saving between scenes (Forest -> City -> Base).
- [x] **Polished Audio:** Audio Manager with Object Pooling for high-performance SFX.
- [x] **Content:** 4 Playable Levels, 30+ Upgrades, Boss Encounter.
- [ ] Online Multiplayer Support (Future Scope).

---

## 🎨 Inspirations
- *Vampire Survivors* (Auto-shooter mechanics, roguelite progression)
- *Call of Duty: Zombies* (Theme, power-up logic)
- *Dead Ops Arcade* (Top-down perspective) 

---

## 📂 Repository Structure
- ├── Assets/
- │   ├── Scripts/
- │   │   ├── Core/        # Managers (Game, Audio, Dialogue)
- │   │   ├── Player/      # Controllers, Stats, Revive Logic
- │   │   ├── Upgrades/    # ScriptableObject Logic & Managers
- │   │   ├── Enemies/     # AI, Spawners, Boss Logic
- │   │   └── UI/          # HUD, Menus, Upgrade Selection
- │   ├── Prefabs/         # configured GameObjects
- │   └── Scenes/          # Main Menu & Game Levels
- └── README.md
---

## 👥 Team  

**Team Name:** Grave Studios  

**Team Members:**  
- Edward Garcia – egarci288@calstatela.edu 
- Daniel Herrera – danielh7555@calstatela.edu  
- Edwin Rojas – erojas55@calstatela.edu  

---
