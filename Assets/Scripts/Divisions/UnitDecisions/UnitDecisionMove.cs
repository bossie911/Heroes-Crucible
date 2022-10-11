using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GameStudio.HunterGatherer.Divisions.UnitBehaviours
{
    /// <summary>Unit decision that checks if the division has given the unit an idle goal</summary>
    public class UnitDecisionMove : UnitDecision
    {
        [Header("Settings")]
        [SerializeField]
        private UnitState stateWhenMoving = UnitState.Move;

        [Header("References")]
        [SerializeField]
        private NavMeshAgent navMeshAgent = null;

        protected override void ExecuteDecision()
        {
            navMeshAgent.stoppingDistance = 0f;
            Unit.State = stateWhenMoving;
        }
    }
}