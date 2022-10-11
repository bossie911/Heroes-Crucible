using System;
using GameStudio.HunterGatherer.CustomEvents;
using GameStudio.HunterGatherer.GodFavor.UI;
using GameStudio.HunterGatherer.Divisions.UnitBehaviours;
using Mirror;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace GameStudio.HunterGatherer.Divisions
{
    /// <summary>Unit used for one unit in a division</summary>
    public class Unit : NetworkBehaviour
    {
        [Header("References")]

        [SerializeField] private Renderer[] renderersToApplyDivisionTexture = new Renderer[0];

        private MaterialPropertyBlock _materialPropertyBlock;

        private MaterialPropertyBlock materialPropertyBlock
        {
            get
            {
                if (_materialPropertyBlock == null)
                    _materialPropertyBlock = new MaterialPropertyBlock();
                return _materialPropertyBlock;
            }
            set { _materialPropertyBlock = value; }
        }
        [SyncVar(hook = nameof(HealthHook))]
        private float health;
        [SerializeField, SyncVar(hook = nameof(SetStateVar))]
        private UnitState _state;
        [SyncVar(hook = nameof(SetDivision))]
        private Division _division = null;
        [SyncVar]
        private MoveTarget _moveTarget = null;
        [SyncVar]
        private Unit _attackTarget = null;
        private bool _isVisible;
        public UnityEvent onHit;
        public UnityEvent OnMiss;
        public UnityEvent OnBlock;
        public UnityEvent onHeal;
        public UnityEvent<float> onHealthChanged;

        private UnitState _oldState = UnitState.Idle;

        [SyncVar]
        private Unit _lastAttacker = null;

        public GameObject lastDamagingObject { get; private set; }
      
        public float Health => health;

        public UnitState State
        {
            get { return _state; }
            set
            {
                // Guard clause to not change state if it's from death to non-idle, that's ILLEGAL
                if (State == UnitState.Death && value != UnitState.Idle)
                {
                    return;
                }

                if (NetworkServer.active)
                {
                    OnStateChanged?.Invoke(_state, false);
                    _state = value;
                    OnStateChanged?.Invoke(_state, true);
                }
                else
                {
                    SetState(value);               
                }
            }
        }

        [Command(requiresAuthority = false)]
        private void SetState(UnitState value)
        {
            _state = value;
        }

        private void SetStateVar(UnitState v1, UnitState v2)
        {
            OnStateChanged?.Invoke(_oldState, false);
            OnStateChanged?.Invoke(_state, true);
            _oldState = _state;
        }

        public UnityEventDivisionGoal OnDivisionChangedGoal { get; } = new UnityEventDivisionGoal();
        public UnityEventUnitState OnStateChanged { get; } = new UnityEventUnitState();

        public Division Division
        {
            get => _division;
            set
            {
                _division = value;
                if(_division != null && NetworkServer.active)
                {
                    ApplyDivisionType();
                    _division.AddUnitToDivision(this);
                    _division.OnChangedGoal.AddListener(DivisionChangedGoal);
                }
            }
        }

        public bool IsTargetable { get; set; }

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                Division?.CheckSelectable(_isVisible);
            }
        }

        public bool HasArrived => State == UnitState.Idle;
        public bool IsMine = false;
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
                    SetMoveTargetCMD(value);
                }
            }
        }

        [Command(requiresAuthority = false)]
        private void SetMoveTargetCMD(MoveTarget value)
        {
            _moveTarget = value;
        }

        public Unit AttackTarget
        {
            get => _attackTarget;
            set
            {
                if (NetworkServer.active)
                {
                    _attackTarget = value;
                }
                else if (NetworkClient.active)
                {
                    _attackTarget = value;
                    SetAttackTargetCMD(value);
                }
            }
        }

        [Command(requiresAuthority = false)]
        private void SetAttackTargetCMD(Unit value)
        {
            _attackTarget = value;
        }

        public Unit LastAttacker
        {
            get => _lastAttacker;
            set
            {
                if (NetworkServer.active)
                {
                    _lastAttacker = value;
                }
                else if (NetworkClient.active)
                {
                    _lastAttacker = value;
                    SetLastAttackerCMD(value);
                }
            }
        }

        [Command(requiresAuthority = false)]
        private void SetLastAttackerCMD(Unit value)
        {
            _lastAttacker = value;
        }

        private void OnEnable()
        {
            var unitBehaviours = GetComponents<UnitBehaviour>();
            foreach (var unitBehaviour in unitBehaviours)
            {
                unitBehaviour.Init();
            }
            //Division = null;
            IsTargetable = true;
            MoveTarget = new MoveTarget(transform.position, Vector3.forward);
            State = UnitState.Move;
        }

        private void OnDrawGizmos()
        {
            if (State == UnitState.Move)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(MoveTarget.Position, 1f);
            }
        }

        /// <summary>Change the division of the unit, received from the UnitSetDivision event</summary>
        private void SetDivision(Division oldDivision, Division newDivision)
        {
            ApplyDivisionType();
            _division.AddUnitToDivision(this);
            _division.OnChangedGoal.AddListener(DivisionChangedGoal);
        }

        /// <summary>Invoke the changed goal event</summary>
        private void DivisionChangedGoal(DivisionGoal goal)
        {
            OnDivisionChangedGoal.Invoke(goal);
        }

        /// <summary>
        /// Sets the materials of unit to given materials
        /// </summary>
        /// <param name="material"></param>
        /// <param name="divisionTexture"></param>
        public void SetMaterial(Material material, Texture divisionTexture)
        {
            foreach (Renderer renderer in renderersToApplyDivisionTexture)
            {
                if (renderer == null)
                    continue;

                if (material && divisionTexture)
                {
                    renderer.sharedMaterial = material;
                    renderer.GetPropertyBlock(materialPropertyBlock);
                    materialPropertyBlock.SetTexture("_MainTex", divisionTexture);
                    renderer.SetPropertyBlock(materialPropertyBlock);
                }
            }
        }

        /// <summary>Apply division type by when unit switches division or the unit's division changes it's type</summary>
        private void ApplyDivisionType()
        {
            SetHealth(_division.TypeData.MaxHealth);
            //health = _division.TypeData.MaxHealth;
            GetComponent<UnitMovement>().navMeshAgent.speed = _division.DivisionSpeed;
        }

        /// <summary>Raise the event to get hit by a given attacker for the given damage</summary>
        public void Hit(HitType hitType, Unit attacker, float damage = 0)
        {
            Debug.Log($"hit");
            switch (hitType)
            {
                case HitType.Hit:

                    //This if statement acts as the check to see if the attacking Unit is of a type that this Unit is weak to
                    //For now this'll be the housing of any ROCK PAPER SCISSORS mechanic, but it could maybe be done in a cleaner way if it gets too big

                    float attackDamage = damage;

                    if (attacker.Division.Type == Division.TypeData.IsWeakToType)
                    {
                        //Logic regarding RPS should be added in this if statement. 
                        attackDamage *= Division.TypeData.WeaknessHitMultiplier;
                    }

                    //The player received favor points on hit, akin to getting super charge in smash or something. Adjustable in scriptable typedata.
                    GodFavorUI.Instance.AddGodFavor(Division.TypeData.FavorGainedOnHit);

                    //health -= attackDamage;
                    //onHit.Invoke();
                    //onHealthChanged?.Invoke(health);
                    //LastAttacker = attacker;
                    TakeDamage(attackDamage, attacker.gameObject);
                    break;

                case HitType.Miss:
                    OnMiss.Invoke();
                    break;
                case HitType.Block:
	                break;
                default:
	                throw new ArgumentOutOfRangeException(nameof(hitType), hitType, null);
            }
        }

        public void Heal(float percentageHeal)
        {
            float maxHealth = Division.TypeData.MaxHealth;
            float hp = health;
            hp += maxHealth * percentageHeal;
            hp = Mathf.Clamp(hp, 0, Division.TypeData.MaxHealth);
            health = hp;
            onHealthChanged?.Invoke(health);
            onHeal?.Invoke();
        }

        public void TakeDamage(float damageAmount, GameObject damagingObject)
        {
            if (State == UnitState.Death) return;
            lastDamagingObject = damagingObject;
            float newHealth = health - damageAmount;
            if (NetworkServer.active)
            {
                health = newHealth;
            }
            else if(NetworkClient.active)
            {
                SetHealth(newHealth);
            }
        }

        [Command(requiresAuthority = false)]
        public void SetHealth(float health)
        {
            this.health = health;
        }

        private void HealthHook(float oldHealth, float newHealth)
        {
            onHit.Invoke();
            onHealthChanged?.Invoke(health);

            //Checking death
            if (health <= 0 && State != UnitState.Death)
            {
                State = UnitState.Death;
            }
        }

        [Command]
        public void DestroyUnit()
        {
            NetworkServer.Destroy(gameObject);
        }
	}
}