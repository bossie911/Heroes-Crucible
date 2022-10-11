using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameStudio.HunterGatherer.Divisions.Upgrades
{
    /// <summary>
    /// Script that holds the functionality of the Upgrade system.
    /// It listens to an Event System for a Division and a specific Upgrade, then updates the Division's stats.
    /// </summary>
    public class UpgradeFunctionality : NetworkBehaviour
    {
        public int SwordsmenLevel = 1;
        public int ArcherLevel = 1;
        public int PikemenLevel = 1;

        public void Upgrade(Division division, UpgradeBase upgrade)
        {
            division.CommandNewMovementSpeed(upgrade.MovementSpeed);
            division.CommandNewCooldown(upgrade.Cooldown);
            division.CommandNewDamage(upgrade.Damage);
            division.CommandNewMaxHealth(upgrade.MaxHealth);
        }
        

    }
}