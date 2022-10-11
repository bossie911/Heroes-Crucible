using UnityEditor;
using UnityEngine;

namespace GameStudio.HunterGatherer.Divisions
{
    /// <summary>ScriptableObject holding all data that could be related to a divisionType</summary>
    [CreateAssetMenu(fileName = "DivisionTypeData", menuName = "ScriptableObjects/DivisionTypeData")]
    public class DivisionTypeData : ScriptableObject
    {
        [Header("General")]
        [SerializeField]
        private string typeName = string.Empty;

        [SerializeField]
        private DivisionType type = DivisionType.Swordsmen;

        [SerializeField] private DivisionType isWeakToType = DivisionType.Swordsmen;

        //TODO remove
        [SerializeField]
        private Sprite icon = null;

        //TODO remove
        [SerializeField]
        private GameObject prefabUnit = null;

        [SerializeField]
        private string iconPath;

        [SerializeField]
        private string prefabUnitPath;

        [SerializeField, Min(1)]
        private int maxUnitCount = 24;

        [Header("Movement")]
        [SerializeField, Min(1)]
        private float movementSpeed = 4f;

        [SerializeField, Min(1)]
        private float movementSpeedRunning = 8f;

        [SerializeField, Min(0), Tooltip("Distance from enemy target from which to start running at the target rather than walking")]
        private float distanceFromEnemiesToStartRunning = 0f;

        [SerializeField]
        private float moveSpeedUpTime = 10f;
        [SerializeField]
        private float moveSpeedUpFinishedTime = 15f;
        [SerializeField]
        private float moveSpeedIncreasePercent = 0.5f;

        [Header("Attack outgoing")]
        [SerializeField, Min(0)]
        private float damage = 1;

        [SerializeField/*, Range(0f, 3f)*/]
        private float weaknessHitMultiplier = 2f;

        [SerializeField, Range(0f, 1f), Tooltip("Percentage of attacks that should result in a hit")]
        private float hitChance = 0.5f;

        [SerializeField, Min(1f)]
        private float range = 3f;

        [SerializeField, Min(0f)]
        private float autoAttackRange = 10f;

        [SerializeField, Min(0f)]
        private float cooldown = 2f;

        [SerializeField, Min(0f)]
        private float chargeUpTime = 0.5f;

        [SerializeField] 
        private float favorGainedOnHit = 0.5f;
        
        [Header("Attack incoming")]
        [SerializeField, Min(1)]
        private int maxHealth = 1;

        public string TypeName => typeName;
        public DivisionType Type => type;
        public DivisionType IsWeakToType => isWeakToType;
        public Sprite Icon => icon;

        public float MaxHealth => maxHealth;
        public GameObject PrefabUnit => prefabUnit;
        public int MaxUnitCount => maxUnitCount;
        public float HitChance => hitChance;
        public float Cooldown => cooldown;
        public float Range => range;
        public float AutoAttackRange => autoAttackRange;
        public float Damage => damage;
        public float WeaknessHitMultiplier => weaknessHitMultiplier;
        public float ChargeUpTime => chargeUpTime;
        public float MovementSpeed => movementSpeed;
        public float MovementSpeedRunning => movementSpeedRunning;
        public float DistanceFromEnemiesToStartRunning => distanceFromEnemiesToStartRunning;
        public float MoveSpeedUpTime => moveSpeedUpTime;
        public float MoveSpeedUpFinishedTime => moveSpeedUpFinishedTime;
        public float MoveSpeedIncreasePercent => moveSpeedIncreasePercent;
        public string IconPath => iconPath;
        public string PrefabUnitPath => prefabUnitPath;
        public float FavorGainedOnHit => favorGainedOnHit;

    }
}