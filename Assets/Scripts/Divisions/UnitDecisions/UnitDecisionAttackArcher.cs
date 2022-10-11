using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace GameStudio.HunterGatherer.Divisions.UnitBehaviours
{
    /// <summary>Unit decision that checks if the division has given the unit an attack goal, and then moves to the target and attacks when in range</summary>
    public class UnitDecisionAttackArcher : UnitDecision
    {
        [Header("Settings")]
        [SerializeField]
        private UnitState stateIfInRange = UnitState.Attack;

        [SerializeField]
        private UnitState stateIfOutOfRange = UnitState.Move;

        [SerializeField, Tooltip("Describes the difference in distance to the closest target and the indexed target at which the unit should prioritize the closest target")]
        private float distanceDifferenceAtWhichToPrioritizeClosest = 5f;

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
                if (Unit.Division.AttackTarget is null)
                {
                    yield return null;
                    continue;
                }
                // Get closest unit from Division.AttackTarget
                Unit closestUnit = Unit.Division.AttackTarget.GetClosestUnit(Unit.transform.position);

                // Get indexed unit from Division.AttackTarget
                Unit indexedUnit = Unit.Division.AttackTarget.GetIndexedUnit(Unit.Division.Units.IndexOf(Unit)); //TODO: Check if IndexOf won't give errors

                if (closestUnit == null || indexedUnit == null || Unit.Division == null)
                {
                    yield return null;
                    continue;
                }

                // If you are close enough to the indexed unit, attack that, otherwise attack the closest target
                if (Vector3.Distance(Unit.transform.position, indexedUnit.transform.position) < Unit.Division.TypeData.Range || Vector3.Distance(Unit.transform.position, closestUnit.transform.position) + distanceDifferenceAtWhichToPrioritizeClosest < Vector3.Distance(Unit.transform.position, indexedUnit.transform.position))
                {
                    Unit.AttackTarget = indexedUnit;
                }
                else
                {
                    Unit.AttackTarget = closestUnit;
                }

                // If in range, attack state, else movetotarget state
                if (InRange)
                {
                    if (Unit.State != stateIfInRange)
                    {
                        //todo
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