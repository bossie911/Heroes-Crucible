using UnityEngine;

namespace GameStudio.HunterGatherer.Divisions
{
    /// <summary>Abstract class holding a unit behaviour which gets triggered when the state of the unit changes</summary>
    public abstract class UnitBehaviour : MonoBehaviour
    {
        [Header("References UnitBehaviour")]
        [SerializeField]
        private Unit unit = null;

        protected Unit Unit { get { return unit; } }

        public virtual void Init()
        {
            unit.OnStateChanged.AddListener(StateChanged); // Needs to be executed BEFORE the unit script is enabled
        }

        /// <summary>Check if behaviour should start or stop when unit state is changed</summary>
        private void StateChanged(UnitState state, bool stateStarting)
        {
            // Guard clause
            if (!ShouldBeActiveDuringState(state))
            {
                return;
            }

            if (stateStarting)
            {
                StartBehaviour();
            }
            else
            {
                StopBehaviour();
            }
        }

        /// <summary>Abstract function that returns if this behaviour should be active during the given state</summary>
        protected abstract bool ShouldBeActiveDuringState(UnitState state);

        /// <summary>Abstract function that runs when the behaviour start is triggered</summary>
        protected abstract void StartBehaviour();

        /// <summary>Abstract function that runs when the behaviour stop is triggered</summary>
        protected abstract void StopBehaviour();
    }
}