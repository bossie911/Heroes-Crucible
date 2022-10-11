using GameStudio.HunterGatherer.CustomEvents;
using GameStudio.HunterGatherer.Divisions.UnitBehaviours;
using GameStudio.HunterGatherer.Networking;
using GameStudio.HunterGatherer.Networking.Events;
using GameStudio.HunterGatherer.Selection;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.Events;
using System.Collections;
using NetworkRoomManager = GameStudio.HunterGatherer.Networking.NetworkRoomManager;


namespace GameStudio.HunterGatherer.Divisions
{
    /// <summary>Division that serves as a group of units that the player can command</summary>
    public class Division : NetworkBehaviour
    {
        #region Settings
        [Header("Settings")]
        [SerializeField, Tooltip("Distance between units when they stand in formation")]
        private float unitSpacing = 2f;

        [SerializeField, Min(1), Tooltip("The ratio the division uses for formation")]
        public float formationRatio = 2f;

        [SerializeField, Range(1, 100), Tooltip("The percentage of units that have to arrive at a location before idle state is set")]
        private int unitsArrivedTillIdle = 90;

        [SerializeField, Min(0), Tooltip("Distance within which the division will look for a new target after killing a previous one")]
        private float autoFindNextTargetDistance = 20f;
        #endregion

        #region References
        [Header("References")]
        [SerializeField] private DivisionFlag divisionFlag = null;
        [SerializeField] private SelectableObject selectableObject = null;
        [SerializeField] private DivisionColorPalette divisionColorPalette = null;
        [SerializeField] private DivisionTypeDatas divisionTypeDatas = null;
        [SerializeField] public UI.DivisionOverviewItem overviewItem = null;
        [SerializeField] private GameObject flag;
        [SerializeField] private Material UnitMaterial = null;
        #endregion 

        #region Synced Fields
        [SyncVar]
        private int m_OwnerID = -1;

        [SyncVar]
        private MoveTarget _moveTarget;

        [SyncVar(hook = nameof(SetGoalVar))]
        private DivisionGoal _goal;

        [SyncVar(hook = nameof(SetAttackTargetVar))]
        private Division _attackTarget;

        [SyncVar(hook = nameof(SetTypeData))]
        public SingleDivisionTypeData TypeData = new SingleDivisionTypeData();

        [SyncVar(hook = nameof(SetMovementSpeed))]
        public float tempMovementSpeed;

        [SyncVar(hook = nameof(SetCooldown))]
        public float tempCooldown;

        [SyncVar(hook = nameof(SetDamage))]
        public float tempDamage;

        [SyncVar(hook = nameof(SetMaxHealth))]
        public float tempMaxHealth;

        public readonly SyncList<Unit> Units = new SyncList<Unit>();
        #endregion

        #region Public Fields
        public bool hadUnits;
        public bool formationAdjusted = false;
        public GameObject FogOfWarMask;
        #endregion

        #region Private Fields
        private Unit flagBearer;
        private Division _oldAttackTarget;
        private SelectionManager selectionManager = new SelectionManager();
        private float currentFormation;
        private float _divisionSpeed;
        #endregion

        #region UnityEvents
        public UnityEvent<float> onHealthChanged;

        public UnityEvent onHeal;
        public UnityEvent OnDivisionTypeChanged { get; } = new UnityEvent();
        public UnityEvent OnDivisionStatsChanged { get; } = new UnityEvent();
        public UnityEvent OnUnitAdded { get; } = new UnityEvent();
        public UnityEventInt OnUnitCountChanged { get; } = new UnityEventInt();
        public UnityEventDivision OnDestroyDivision { get; } = new UnityEventDivision();
        public UnityEventDivisionGoal OnChangedGoal { get; } = new UnityEventDivisionGoal();
        #endregion

        #region Public Properties
        public string PlayerName => OwnerID >= 0 && OwnerID < NetworkRoomManager.Instance.roomSlots.Count ? NetworkRoomManager.Instance.roomSlots[OwnerID].GetComponent<RoomPlayer>().playerName : "";
        public DivisionTypeDatas DivisionTypeDatas => divisionTypeDatas;

        public Color DivisionColor { get; private set; }
        public Texture UnitTexture { get; private set; }
        public Texture FlagTexture { get; private set; }

        public SelectableObject SelectableObject => selectableObject;
        public bool IsSelected => SelectableObject.IsSelected;
        public bool IsMine
        {
            get
            {
                if (!NetworkClient.active) return false;
                return m_OwnerID == NetworkRoomManager.LocalPlayerID;
            }
        }

        public List<Unit> VisibleUnits { get; private set; } = new List<Unit>();
        public DivisionType Type => TypeData.Type;
        public DivisionType isWeakToType => TypeData.IsWeakToType;
        public float UnitSpacing => unitSpacing;
        public float DivisionSpeed
        {
            get { return _divisionSpeed; }
            set
            {
                if (value == _divisionSpeed)
                    return;

                _divisionSpeed = value;
                foreach (Unit unit in Units)
                {
                    unit.GetComponent<UnitMovement>().navMeshAgent.speed = _divisionSpeed;
                    //todo unit.MovementSpeed = _divisionSpeed;
                }

                //makes sure the flag always keeps up with the division 
                //TODO NetworkedMovingObject.MovementSpeed = float.MaxValue;
            }
        }

        public Unit FirstUnit
        {
            get
            {
                if (Units.Count != 0)
                {
                    return Units[0];
                }

                return null;
            }
        }
        public float FormationRatio => formationRatio;
        public float GetDivisionHealth(bool maxHealth = false)
        {
            float health = 0;
            if (!maxHealth)
            {
                for (int i = 0; i < Units.Count; i++)
                {
                    health += Units[i].Health;
                }
            }

            if (maxHealth)
            {
                health = TypeData.MaxHealth * TypeData.MaxUnitCount;
            }

            return health;
        }
        #endregion

        #region SyncVar Properties
        public int OwnerID
        {
            get => m_OwnerID;
            set
            {
                if(NetworkServer.active)
                {
                    m_OwnerID = value;
                }
                else if (NetworkClient.active)
                {
                    SetOwnerID_Command(value);
                }
            }
        }
        public DivisionGoal Goal
        {
            get { return _goal; }
            set
            {
                if (NetworkServer.active)
                {
                    _goal = value;
                    OnChangedGoal.Invoke(_goal);
                }
                else if(NetworkClient.active)
                {
                    _goal = value;
                    SetGoal(value);
                    OnChangedGoal.Invoke(_goal);
                }
            }
        }
        public MoveTarget MoveTarget
        {
            get { return _moveTarget; }
            set
            {
                if (NetworkServer.active)
                {
                    _moveTarget = value;
                }
                else if (NetworkClient.active)
                {
                    _moveTarget = value;
                    SetMoveTarget(value);
                }
            }
        }
        public Division AttackTarget
        {
            get => _attackTarget;
            set
            {
                if (NetworkServer.active)
                {
                    if (_attackTarget != null)
                    {
                        _attackTarget.OnDestroyDivision.RemoveListener(AttackTargetDies);
                    }

                    _attackTarget = value;
                    if (_attackTarget != null)
                    {
                        _attackTarget.OnDestroyDivision.AddListener(AttackTargetDies);
                    }
                    _oldAttackTarget = _attackTarget;
                }
                else
                {
                    _attackTarget = value;
                    SetAttackTarget(value);
                }         
            }
        }
        #endregion

        #region SyncVar Commands
        [Command]
        private void SetOwnerID_Command(int value)
        {
            m_OwnerID = value;
        }
        [Command(requiresAuthority = false)]
        private void SetMoveTarget(MoveTarget value)
        {
            _moveTarget = value;
        }

        [Command(requiresAuthority = false)]
        private void SetAttackTarget(Division value)
        {
            _attackTarget = value;
        }

        [Command(requiresAuthority = false)]
        private void SetGoal(DivisionGoal value)
        {
            _goal = value;
        }

        [Command]
        public void CommandNewMovementSpeed(float MovementSpeed)
        {
            tempMovementSpeed += MovementSpeed;
        }
        [Command]
        public void CommandNewCooldown(float Cooldown)
        {
            tempCooldown += Cooldown;
        }
        [Command]
        public void CommandNewDamage(float Damage)
        {
            tempDamage += Damage;
        }
        [Command]
        public void CommandNewMaxHealth(float MaxHealth)
        {
            tempMaxHealth += MaxHealth;
        }       
        #endregion

        #region SyncVar Hooks
        private void SetGoalVar(DivisionGoal v1, DivisionGoal v2)
        {
            OnChangedGoal.Invoke(_goal);
        }
        private void SetAttackTargetVar(Division v1, Division v2)
        {
            if (_oldAttackTarget != null)
            {
                _oldAttackTarget.OnDestroyDivision.RemoveListener(AttackTargetDies);
            }
            if (_attackTarget != null)
            {
                _attackTarget.OnDestroyDivision.AddListener(AttackTargetDies);
            }
            _oldAttackTarget = _attackTarget;
        }
        public void SetMovementSpeed(float oldMovementSpeed, float newMovementSpeed)
        {
            TypeData.MovementSpeed = tempMovementSpeed;
            DivisionSpeed = TypeData.MovementSpeed;
        }
        public void SetCooldown(float oldCoolDown, float newCooldown)
        {
            TypeData.Cooldown = tempCooldown;
        }
        public void SetDamage(float oldDamage, float newDamage)
        {
            TypeData.Damage = tempDamage;
        }
        public void SetMaxHealth(float oldMaxHealth, float newMaxhealth)
        {
            TypeData.MaxHealth = tempMaxHealth;
            for (int i = 0; i < Units.Count; i++)
            {
                Units[i].SetHealth(Units[i].Health + (newMaxhealth - oldMaxHealth));
            }
            OnDivisionTypeChanged.Invoke();
        }
        #endregion

        #region Setup Methods
        private void Awake()
        {
            OnUnitCountChanged.AddListener((_) => VisibleUnits = Units.Where(x => x.IsVisible).ToList());
        }

        private void OnEnable()
        {
            MoveTarget = new MoveTarget(transform.position, Vector3.forward);
            SelectableObject.SetGroup(IsMine ? SelectableObjectGroup.Friendly : SelectableObjectGroup.Enemy);

            if (!NetworkServer.active) StartCoroutine(Setup());
        }

        /// <summary>
        /// Setup for after the division is spawned and has all its units
        /// </summary>
        public IEnumerator Setup()
        {
            while (OwnerID == -1 || Units.Count < TypeData.MaxUnitCount)
            {
                yield return null;
            }

            if (IsMine)
            {
                FogOfWarMask.SetActive(true);
            }

            VisibilitySetup();
            SetupColorsAndTextures();
            HeroHealthBarSetup();
            OnDivisionTypeChanged.Invoke();
        }

        /// <summary>
        /// Setsup the visibility of the fowobjects in the units in this division
        /// </summary>
        private void VisibilitySetup()
        {
            foreach (Unit unit in Units)
            {
                unit.gameObject.GetComponentInChildren<FogOfWar.FogOfWarObject>().SetupVisibility();
            }
        }

        /// <summary>
        /// Setsup the colors for all division related objects
        /// </summary>
        private void SetupColorsAndTextures()
        {
            //order of these assignments is important
            DivisionColor = divisionColorPalette.GetDivisionColor(OwnerID);

            UnitTexture = divisionColorPalette.GetDivisionTexture(OwnerID);
            FlagTexture = divisionColorPalette.GetFlagTexture(Type);

            divisionFlag.SetMaterial(UnitMaterial, FlagTexture);

            flag.GetComponent<Image>().color = DivisionColor;
            foreach (var unit in Units)
            {
                unit.SetMaterial(UnitMaterial, UnitTexture);
            }
            //Needs to be done last
            if (IsMine)
            {
                overviewItem.imgBackground.color = DivisionColor;
            }
        }

        private void HeroHealthBarSetup()
        {
            if (IsMine && Type == DivisionType.Hero)
            {
                GameObject.FindGameObjectWithTag("HeroHealthBar").GetComponent<HeroHealthTracker>().Perp(this);
            }
        }
        #endregion

        private void Update()
        {
            if (!IsMine) return;

            if(NetworkRoomManager.Instance.CurrentPlayer.GetPlayerDivisionOfType(DivisionType.Hero) is null)
            {
	            foreach (var unit in Units)
	            {
                    unit.State = UnitState.Death;
	            }
            }

            // Destroy division if no units left
            if (Units.Count == 0 && hadUnits)
            {
                if (selectableObject == null)
                {
                    Debug.LogWarning("It's null! We can't destroy!");
                    return;
                }
                OnDestroyDivision.Invoke(this);
                Destroy();
                return;
            }

            UpdatePosition();
        }

        /// <summary>Update flag bearer and set position to that of the flag bearer</summary>
        private void UpdatePosition()
        {
            // Pick a new flag bearer if we don't have one currently (unset or died)
            if (flagBearer == null || !flagBearer.IsTargetable)
            {
                flagBearer = FindFirstTargetableUnit();

                // Guard clause to not update position if flag bearer is still null
                if (flagBearer == null)
                {
                    return;
                }
            }

            // Set position to flag bearers position
            transform.position = flagBearer.transform.position;
        }


        /// <summary>Return first unit belonging to the division that is targetable</summary>
        private Unit FindFirstTargetableUnit()
        {
            foreach (Unit unit in Units)
            {
                if (unit.IsTargetable)
                {
                    return unit;
                }
            }

            return null;
        }

        /// <summary>Handle interaction between given divisions and this division</summary>
        public void Interact(List<Division> divisions)
        {
            // Guard clause to not attack when friendly
            if (IsMine)
            {
                return;
            }

            divisions.ForEach(x => x.AttackOrder(this));
        }

        #region Division Orders
        /// <summary>Give division order to move to given position and end with the given rotation</summary>
        public void MoveOrder(Vector3 position, Vector3 direction)
        {
            if (direction == Vector3.zero)
            {
                direction = (position - transform.position);
            }

            direction.y = 0;

            MoveTarget = new MoveTarget(position, direction);
            AssignClosestMoveTarget(position, direction);

            AttackTarget = null;
            Goal = DivisionGoal.Move;
        }

        /// <summary>Give division order to attack given division</summary>
        public void AttackOrder(Division division)
        {
            // Guard clause to exit if division has no units to attack
            if (division.Units.Count == 0)
            {
                return;
            }

            AttackTarget = division;
            MoveTarget = new MoveTarget(division.transform, division.transform.position - transform.position);
            Goal = DivisionGoal.Attack;
        }

        /// <summary>Give division order to defend their current location against the given division</summary>
        public void DefendOrder(Division division)
        {
            // Guard clause to exit if division has no units to attack
            if (division.Units.Count == 0)
            {
                return;
            }

            AttackTarget = division;
            MoveTarget = new MoveTarget(division.transform, division.transform.position - transform.position);

            Goal = DivisionGoal.Defend;
        }

        /// <summary>Give division order to idle on current position</summary>
        private void IdleOrder()
        {
            AttackTarget = null;
            Goal = DivisionGoal.Idle;
        }
        #endregion

        /// <summary>Handle what needs to happen when the current attack target of the division dies</summary>
        private void AttackTargetDies(Division _)
        {
            MoveOrder(transform.position, MoveTarget.Direction);
        }

        /// <summary>Raise the event to set the type of this division to the given type</summary>
        [Server]
        public void SetType(DivisionType type)
        {
            TypeData = new SingleDivisionTypeData(divisionTypeDatas.GetDivisionTypeData(type));
            DivisionSpeed = TypeData.MovementSpeed;
            SelectableObject.ObjectName = TypeData.TypeName;

            tempMovementSpeed = TypeData.MovementSpeed;
            tempDamage = TypeData.Damage;
            tempCooldown = TypeData.Cooldown;
            tempMaxHealth = TypeData.MaxHealth;
        }

        private void SetTypeData(SingleDivisionTypeData oldData, SingleDivisionTypeData newData)
        {
            DivisionSpeed = newData.MovementSpeed;
            SelectableObject.ObjectName = newData.TypeName;
        }

        /// <summary>Return unit from division closest to given position</summary>
        public Unit GetClosestUnit(Vector3 position)
        {
            // Guard clause if there is no units
            if (Units.Count == 0)
            {
                return null;
            }

            // Sort and pick closest
            List<Unit> sorted = new List<Unit>();
            foreach (var unit in Units)
            {
                if (unit.IsVisible)
                {
                    sorted.Add(unit);
                }
            }

            // Guard clause if there is no visible units
            if (sorted.Count == 0)
            {
                return null;
            }

            sorted.Sort((u1, u2) => Vector3.Distance(position, u1.transform.position)
                .CompareTo(Vector3.Distance(position, u2.transform.position)));
            return sorted[0];
        }

        /// <summary>Return unit of division picked based on given index, to better divide attack targets</summary>
        public Unit GetIndexedUnit(int index)
        {
            // Guard clause if there is no units
            if (Units.Count == 0 || index < 0)
            {
                return null;
            }

            // Sort and pick closest
            List<Unit> visibleUnits = new List<Unit>();
            foreach (var unit in Units)
            {
                if (unit.IsVisible)
                {
                    visibleUnits.Add(unit);
                }
            }

            // Guard clause if there is no visible units
            if (visibleUnits.Count == 0)
            {
                return null;
            }

            int continuedIndex = index % visibleUnits.Count;

            return visibleUnits[continuedIndex];
        }

        /// <summary> Assigns each units MoveTarget, starting with the furthest, getting the closest position. </summary>
        private void AssignClosestMoveTarget(Vector3 position, Vector3 direction)
        {
            List<Vector3> formationPositions = new List<Vector3>();

            if (formationAdjusted || (Units.Count < 12))
            {
                formationPositions = FormationLayout.GeneratePositions(Units.Count, position, direction, FormationRatio, UnitSpacing);
                currentFormation = formationRatio;
            }
            else
            {
                formationRatio = currentFormation;
                formationPositions = FormationLayout.GeneratePositions(Units.Count, position, direction, FormationRatio, UnitSpacing);
            }

            Unit[] sortedUnits = Units.OrderByDescending(u => Vector3.Distance(u.transform.position, position)).ToArray();

            foreach (var unit in sortedUnits)
            {
                int closestIndex = 0;
                float closestDistance = float.MaxValue;

                for (var iLayout = 0; iLayout < formationPositions.Count; iLayout++)
                {
                    var distance = Vector3.Distance(unit.transform.position, formationPositions[iLayout]);
                    if (!(distance < closestDistance)) continue;
                    closestIndex = iLayout;
                    closestDistance = distance;
                }
                unit.MoveTarget = new MoveTarget(formationPositions[closestIndex], direction);
                formationPositions.RemoveAt(closestIndex);
            }

            formationAdjusted = false;
        }

        /// <summary>Spawn new unit and set its division</summary>
        [Command]
        public void SpawnUnit(NetworkConnectionToClient conn = null)
        {
            if(conn is null) return;

            if (Units.Count == TypeData.MaxUnitCount)
            {
                return;
            }

            var spawnPos = transform.position;
            if (NavMesh.SamplePosition(transform.position, out var closestHit, Mathf.Infinity, NavMesh.AllAreas))
            {
                spawnPos = closestHit.position;
            }

            if(TypeData.PrefabUnit is null)
            {
                return;
            }
            var newUnit = Instantiate(TypeData.PrefabUnit, spawnPos, Quaternion.identity);
            newUnit.gameObject.SetActive(true);
            newUnit.Division = this;

            NetworkServer.Spawn(newUnit.gameObject, conn);
            Units.Add(newUnit);
        }

        /// <summary>Add unit to the division's list of units</summary>
        public void AddUnitToDivision(Unit unit)
        {
            unit.GetComponent<UnitMovement>().division = this;
           
            OnUnitCountChanged.Invoke(Units.Count);

            unit.OnStateChanged.AddListener(HasArrived);
            unit.onHeal.AddListener(onHeal.Invoke);
            unit.onHealthChanged.AddListener(OnHealthChanged);

            //Setting fowobject reference in units
            unit.GetComponentInChildren<FogOfWar.FogOfWarObject>().fogOfWarMask = FogOfWarMask.GetComponent<FogOfWar.FogOfWarMask>();

            // Reassign idle order
            var moveTarget = MoveTarget ?? new MoveTarget(transform.position, Vector3.forward);
            MoveOrder(moveTarget.Position, moveTarget.Direction);
        }

        private void OnHealthChanged(float f)
        {
            onHealthChanged.Invoke(GetDivisionHealth());
        }

        /// <summary>Destroy last unit in division's list of units</summary>
        public void DestroyUnit()
        {
            // Guard clause if there is no units
            if (Units.Count <= 0)
            {
                return;
            }

            // Select last unit of units if no unit is passed
            Unit unit = Units[Units.Count - 1];

            unit.onHeal.RemoveListener(onHeal.Invoke);
            unit.onHealthChanged.RemoveListener(OnHealthChanged);

            // Destroy unit
            unit.DestroyUnit();
        }

        /// <summary>Remove given unit from division's list of units</summary>
        public void RemoveUnitFromDivision(Unit unit)
        {
            // Guard clause if units doesn't contain unit
            if (!Units.Contains(unit))
            {
                return;
            }

            unit.OnStateChanged.RemoveListener(HasArrived);
            RemoveUnit(unit);
            OnUnitCountChanged.Invoke(Units.Count);

            unit.onHeal.RemoveListener(onHeal.Invoke);
            unit.onHealthChanged.RemoveListener(OnHealthChanged);
            
            unit.DestroyUnit();
        }

        [Command]
        public void RemoveUnit(Unit unit)
        {
            Units.Remove(unit);
        }

        [Command]
        private void Destroy()
        {
            Goal = DivisionGoal.Idle;
            AttackTarget = null;
            InvokeDeathEvent(this);
            if (TypeData.Type == DivisionType.Hero) NetworkEvents.HeroDeath(OwnerID);
            NetworkRoomManager.Instance.roomSlots.FirstOrDefault(x => x.index == OwnerID)?.GetComponent<RoomPlayer>().PlayerDivisions.Remove(this);
            NetworkServer.Destroy(gameObject);
        }

        [ClientRpc(includeOwner = true)]
        private void InvokeDeathEvent(Division div)
        {
            OnDestroyDivision.Invoke(div);
        }

        /// <summary>Check if the division needs to be selectable, called by a unit when it's visibility changes</summary>
        public void CheckSelectable(bool unitVisible)
        {
            VisibleUnits = Units.Where(x => x.IsVisible).ToList();

            // Guard clause to exit if incoming unit change in unit visibility already equals the current status of selectableObject or if the division has not had units (e.g. when switching type)
            if (unitVisible == selectableObject.IsSelectable || !hadUnits)
            {
                return;
            }

            foreach (Unit unit in Units)
            {
                if (unit.IsVisible)
                {
                    SelectableObject.IsSelectable = true;
                    return;
                }
            }

            SelectableObject.IsSelectable = false;
        }

        /// <summary> Checks if a certain amount of units have arrived at the MoveTarget </summary>
        public void HasArrived(UnitState unitState, bool enabled)
        {
            // Guard clause to exit if the unitstate change is irrelevant (disable or not idle) or if the division goal is already idle
            if (!enabled || unitState != UnitState.Idle || Goal == DivisionGoal.Idle)
            {
                return;
            }

            int arrivedUnits = 0;
            foreach (Unit unit in Units)
            {
                if (unit.HasArrived)
                {
                    arrivedUnits++;
                }
            }

            // Switch to idle state once a percentage of the units have arrived
            if ((arrivedUnits / (float)Units.Count) * 100f >= unitsArrivedTillIdle)
            {
                IdleOrder();
            }
        }

        public bool IsDivisionVisible()
        {
            foreach (var unit in Units)
            {
                if (unit.IsVisible)
                {
                    return true;
                }
            }

            return false;
        }

    }
}