using Mirror;
using UnityEngine;

namespace GameStudio.HunterGatherer.Divisions.Upgrades
{
    /// <summary>
    /// ScriptableObject Holding all modifiers an Upgrade could have.
    /// TODO: When implementing more buffs: Make a ScriptableObject that inherits from this class, but with duration and cost variables.
    /// </summary>
    [CreateAssetMenu(fileName = "Upgrade", menuName = "ScriptableObjects/Upgrade", order = 1)]
    public class UpgradeBase : ScriptableObject
    {
        //Attribute Targets are used for cleanliness and to add public accessors to previously private fields.
        [field: Header("Movement Speed Modifiers")]
        [field: SerializeField, Tooltip("Movement Speed")]
        public Sprite icon { get; private set; }
        [field: SerializeField, Tooltip("Movement Speed")]
        public float MovementSpeed { get; private set; }

        [field: SerializeField, Tooltip("Movement Speed while running")]
        public float MovementSpeedRunning { get; private set; }

        [field: SerializeField, Tooltip("How far the division has to be from enemies to start running")]
        public float DistanceFromEnemiesToStartRunning { get; private set; }

        [field: SerializeField, Tooltip("Duration you have to run before accelerating")]
        public float MoveSpeedUpTime { get; private set; }

        [field: SerializeField, Tooltip("Duration you have to run to reach maximum movement speed")]
        public float MoveSpeedUpFinishedTime { get; private set; }

        [field: SerializeField, Tooltip("How much movement speed increases while running")]
        public float MoveSpeedIncreasePercent { get; private set; }

        [field: Header("Outgoing Attacks")]
        [field: SerializeField, Tooltip("Damage dealt by units in the division")]
        public float Damage { get; private set; }

        [field: SerializeField, Range(-1f, 1f), Tooltip("Percentage of attacks that should result in a hit")]
        public float HitChance { get; private set; }

        [field: SerializeField, Tooltip("Attack Range")]
        public float Range { get; private set; }

        [field: SerializeField, Tooltip("Range from which units attack by themselves")]
        public float AutoAttackRange { get; private set; }

        [field: SerializeField, Tooltip("Time between attacks")]
        public float Cooldown { get; private set; }

        [field: SerializeField, Tooltip("Distance foes are knocked back")]
        public float KnockbackDistance { get; private set; }

        [field: SerializeField, Tooltip("How long foes are on the ground after a knockback")]
        public float KnockbackDuration { get; private set; }

        [field: SerializeField, Tooltip("Time it takes to attack")]
        public float ChargeUpTime { get; private set; }

        [field: Header("Incoming Attacks")]
        [field: SerializeField, Tooltip("Hitpoints of units in the division")]
        public float MaxHealth { get; private set; }

        [field: SerializeField, Range(-1f, 1f), Tooltip("Percentage of how much enemy's knockback affects this unit")]
        public float KnockbackTenacity { get; private set; }

        [field: SerializeField, Range(-1f, 1f), Tooltip("Percentage of enemy melee hits that should be blocked")]
        public float BlockChanceMelee { get; private set; }

        [field: SerializeField, Range(-1f, 1f), Tooltip("Percentage of enemy projectile hits that should be blocked")]
        public float BlockChanceProjectile { get; private set; }

        [field: SerializeField, Tooltip("Image for upgrade")]
        public Sprite UpgradeImage { get; set; }

        [field: SerializeField, Range(-3, 0), Tooltip("The amount of hits a unit must take before blocking once")]
        public int HitsPerBlock { get; set; }
    }
}