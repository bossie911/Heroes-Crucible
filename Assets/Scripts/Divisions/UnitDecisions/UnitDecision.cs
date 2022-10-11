using System.Collections.Generic;
using UnityEngine;

namespace GameStudio.HunterGatherer.Divisions
{
    /// <summary>Abstract class holding a unit decision which gets triggered when the goal of the unit's division changes</summary>
    public abstract class UnitDecision : MonoBehaviour
    {
        [Header("References UnitDecision")]
        [SerializeField]
        private Unit unit = null;

        [SerializeField]
        private DivisionGoal goal = DivisionGoal.Idle;

        protected Unit Unit => unit;
        protected DivisionGoal Goal => goal;

        private void OnEnable() => Unit.OnDivisionChangedGoal.AddListener(DivisionGoalChanged);

        private void OnDisable() => Unit.OnDivisionChangedGoal.RemoveListener(DivisionGoalChanged);

        /// <summary>Check if decision should be executed when unit's division's goal is changed</summary>
        private void DivisionGoalChanged(DivisionGoal newGoal)
        {
            if (goal.HasFlag(newGoal))
            {
                ExecuteDecision();
            }
        }

        /// <summary>Abstract function that runs when the decision is executed</summary>
        protected abstract void ExecuteDecision();
    }
}