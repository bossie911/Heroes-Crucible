using DG.Tweening;
using GameStudio.HunterGatherer.Networking;
using UnityEngine;
using UnityEngine.AI;

namespace GameStudio.HunterGatherer.Divisions
{
    /// <summary>Handles what happens to a unit when it dies, called using StartDeath</summary>
    public class UnitDeath : UnitBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private UnitState activeState = UnitState.Death;

        [SerializeField]
        private float dropDuration = 0.5f;

        [SerializeField]
        private float sinkDuration = 2f;

        [SerializeField]
        private Vector3 sinkPosition = new Vector3(0f, -2f, 0f);

        [Header("References")]
        [SerializeField]
        private GameObject unitCharacter = null;

        [SerializeField]
        private NavMeshAgent navMeshAgent = null;

        private Quaternion defaultLocalRotation;
        private Vector3 defaultLocalPosition;
        private Sequence tween;

        private void Awake()
        {
            defaultLocalRotation = unitCharacter.transform.localRotation;
            defaultLocalPosition = unitCharacter.transform.localPosition;
        }

        public override void Init()
        {
            base.Init();
            unitCharacter.transform.localRotation = defaultLocalRotation;
            unitCharacter.transform.localPosition = defaultLocalPosition;
        }

        private void OnDisable()
        {
            tween.Kill();
            unitCharacter.transform.localRotation = defaultLocalRotation;
            unitCharacter.transform.localPosition = defaultLocalPosition;
            //todo Unit.NetworkedMovingObject.ShouldSynchronize = true;
        }

        protected override bool ShouldBeActiveDuringState(UnitState state)
        {
            return activeState == state;
        }

        protected override void StartBehaviour()
        {
            StartDeath();
        }

        protected override void StopBehaviour()
        {
            // YOU CAN'T COME BACK FROM DEATH BAAAHAHAHAHAHAHAHAHA
        }

        /// <summary>Start the sequence of this unit dying</summary>
        public void StartDeath()
        {
            Unit.IsTargetable = false;
            if (navMeshAgent.isOnNavMesh && !navMeshAgent.isStopped)
            {
                navMeshAgent.isStopped = true;
            }

            // If the last thing that did damage was a Unit, or if it was a Godpower
            var dropDirection = Vector3.forward;
            dropDirection.y = 0;
            dropDirection.Normalize();
            
            tween = DOTween.Sequence();
            tween.Append(unitCharacter.transform.DORotateQuaternion(Quaternion.FromToRotation(Vector3.up, dropDirection), dropDuration));
            tween.Append(unitCharacter.transform.DOLocalMove(sinkPosition, sinkDuration));
            tween.AppendCallback(() => DestroyUnit(Unit));
        }

        /// <summary>Destroy this unit using networking if it's owned by this client</summary>
        private void DestroyUnit(Unit unit)
        {
            // Only destroy if owned unit, and if it's not destroyed already
            if (Unit.IsMine && unit.gameObject.activeInHierarchy)
            {
                Unit.Division.RemoveUnitFromDivision(Unit);
            }
        }
    }
}