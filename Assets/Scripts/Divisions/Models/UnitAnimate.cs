using Unity.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace GameStudio.HunterGatherer.Divisions
{
    [RequireComponent(typeof(NavMeshAgent), typeof(Unit))]
    public class UnitAnimate : MonoBehaviour
    {
        private Animator animator;
        private NavMeshAgent agent;
        private Unit unit;
        
        [Header("Animation Settings")]
        [SerializeField, Tooltip("Movement speed of the walk animation in m/s")]
        private float animationWalkSpeed = 1;
        [SerializeField, ReadOnly, Tooltip("Movement speed of the walk animation in m/s")]
        private float currentSpeed = 0;

        private float smoothedCurrentSpeed = 0;

        [SerializeField, Tooltip("Amount of available idle animations")]
        private int amountOfIdleAnimations = 1;

        [SerializeField, Tooltip("Amount of available walk animations")]
        private int amountOfWalkAnimations = 1;

        [SerializeField, Min(0), Tooltip("Attack animation speed multiplier")]
        private float attackAnimationSpeed = 1;

        [SerializeField, Tooltip("Amount of available attack animations")]
        private int amountOfAttackAnimations = 1;

        [SerializeField, Tooltip("Amount of available death animations")]
        private int amountOfDeathAnimations = 1;

        private Vector3 previousPosition = Vector3.zero;

        void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            agent = GetComponent<NavMeshAgent>();
            unit = GetComponent<Unit>();
        }

        void Update()
        {
            currentSpeed = (transform.position - previousPosition).magnitude / Time.deltaTime;

            if (smoothedCurrentSpeed <= 0.1f)
                smoothedCurrentSpeed = currentSpeed;
            else
            {
                smoothedCurrentSpeed = Mathf.Lerp(smoothedCurrentSpeed, currentSpeed, 0.5f);
            }
            
            // Update unit animations based on Unit state
            if (unit.State == UnitState.Attack)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
                {
                    animator.speed = 1;
                    animator.SetInteger("Attack anim", Random.Range(1, amountOfAttackAnimations));
                    animator.SetTrigger("AttackTrigger");
                }
            }
            else if (unit.State == UnitState.Death)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("Die"))
                {
                    animator.speed = attackAnimationSpeed;
                    animator.SetInteger("Die anim", Random.Range(1, amountOfDeathAnimations));
                    animator.SetTrigger("DeathTrigger");
                }
            }
            else if (smoothedCurrentSpeed > 0.1f)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("Move"))
                {
                    animator.SetInteger("Walk anim", Random.Range(1, amountOfWalkAnimations));
                    animator.SetTrigger("MoveTrigger");
                }

                animator.speed = smoothedCurrentSpeed / animationWalkSpeed;
            }
            else
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("Idle"))
                {
                    animator.speed = 1;
                    animator.SetInteger("Idle anim", Random.Range(1, amountOfIdleAnimations));
                    animator.SetTrigger("IdleTrigger");
                }
            }

            previousPosition = transform.position;
        }
    }
}
