using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace GameStudio.HunterGatherer.Divisions.UnitBehaviours
{
    /// <summary>Unit decision that checks if the division has given the unit an attack goal, and then moves to the target and attacks when in range</summary>
    public class UnitDecisionAttack : UnitDecision
    {
        [Header("Settings")]
        [SerializeField]
        private UnitState stateIfInRange = UnitState.Attack;

        [SerializeField]
        private UnitState stateIfOutOfRange = UnitState.Move;

        [SerializeField, Tooltip("Describes the difference in distance to the current target and the closest enemy at which the unit should switch to the closest enemy")]
        private float distanceDifferenceAtWhichToSwitchTargets = 1f;

        [Header("References")]
        [SerializeField]
        private NavMeshAgent navMeshAgent = null;

        private Coroutine moveToTargetRoutine;

        private bool InRange => Vector3.Distance(Unit.transform.position, Unit.AttackTarget.transform.position) < Unit.Division.TypeData.Range;

        /// <summary>Start the moveToTarget coroutine</summary>
        protected override void ExecuteDecision()
        {
            if (moveToTargetRoutine != null)
            {
                StopCoroutine(moveToTargetRoutine);
            }
            Unit.AttackTarget = null;
            navMeshAgent.stoppingDistance = Unit.Division.TypeData.Range;
            
            if (gameObject.activeInHierarchy)
                moveToTargetRoutine = StartCoroutine(MoveToTarget());
        }

        /// <summary>Coroutine that sets the unit's state to MOVE when out of range, and ATTACK when in range</summary>
        private IEnumerator MoveToTarget()
        {
            while (Goal.HasFlag(Unit.Division.Goal))
            {
                // Get closest unit from Division.AttackTarget
                //Debug.Log($"Unit.Division.AttackTarget:{Unit.Division.AttackTarget is null}");
                if (Unit.Division.AttackTarget is null)
                {
                    yield return null;
                    continue;
                }

                Unit closestUnit = Unit.Division.AttackTarget.GetClosestUnit(Unit.transform.position);

                if (closestUnit == null)
                {
                    yield return null;
                    continue;
                }

                // If Unit.AttackTarget is not targetable || distance to closest unit is way smaller than current target, set
                if (Unit.AttackTarget == null || !Unit.AttackTarget.IsVisible || !Unit.AttackTarget.IsTargetable ||
                    Vector3.Distance(Unit.transform.position, closestUnit.transform.position) + distanceDifferenceAtWhichToSwitchTargets < Vector3.Distance(Unit.transform.position, Unit.AttackTarget.transform.position))
                {
                    Unit.AttackTarget = closestUnit;
                }

                // If in range, attack state, else movetotarget state
                if (InRange)
                {
                    if (Unit.State != stateIfInRange)
                    {
                        Unit.MoveTarget = null;
                        Unit.State = stateIfInRange;
                    }
                }
                else
                {
                    if (Unit.State != stateIfOutOfRange || Unit.MoveTarget == null || Unit.MoveTarget.Position != Unit.AttackTarget.transform.position)
                    {
                        Unit.MoveTarget = new MoveTarget(Unit.AttackTarget.transform, Unit.AttackTarget.transform.position - Unit.transform.position);
                        Unit.State = stateIfOutOfRange;
                    }
                }

                yield return null;
            }
        }
    }
}