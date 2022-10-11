using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace GameStudio.HunterGatherer.Divisions.UnitBehaviours
{
    /// <summary>Unit behaviour that handles attacking another unit when in range</summary>
    public class UnitAttack : UnitBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private UnitState activeState = UnitState.Attack;

        [SerializeField, Tooltip("How long to do over tweening the units rotation when it arrives, regardless of current rotation")]
        private float rotationDurationFlat = 0.25f;

        [SerializeField, Tooltip("How long to do over tweening the units rotation when it arrives, scaled with difference in rotation, on top of flat rotation")]
        private float rotationDurationScaled = 0.5f;

        private Coroutine attackRoutine;
        private float attackCooldownCurrent;
        private Tween tween;
        public UnityEvent onAttack;

        private void Update()
        {
            // Reduce attackCooldown in update, because it should also happen if this behaviour isn't running
            if (attackCooldownCurrent > 0)
            {
                attackCooldownCurrent -= Time.deltaTime;
            }
        }

        /// <summary>This behaviour is active during the attack state</summary>
        protected override bool ShouldBeActiveDuringState(UnitState state)
        {
            return activeState == state;
        }

        /// <summary>Start the attack routine</summary>
        protected override void StartBehaviour()
        {
            if (attackRoutine != null)
            {
                StopCoroutine(attackRoutine);
            }
            attackRoutine = StartCoroutine(Attack());
        }

        /// <summary>Stop the attack routine</summary>
        protected override void StopBehaviour()
        {
            if (attackRoutine != null)
            {
                StopCoroutine(attackRoutine);
            }
        }

        /// <summary>Strike when attack cooldown is done</summary>
        private IEnumerator Attack()
        {
            while (Unit.State == activeState)
            {
                if (Unit.AttackTarget == null || !Unit.AttackTarget.IsTargetable)
                {
                    yield return null;
                    continue;
                }

                if (attackCooldownCurrent <= 0)
                {
                    yield return new WaitForSeconds(Unit.Division.TypeData.ChargeUpTime);
                    attackCooldownCurrent = Unit.Division.TypeData.Cooldown;
                    onAttack.Invoke();
                }

                // Rotate towards Attacktarget
                Vector3 direction = Unit.AttackTarget.transform.position - Unit.transform.position;
                direction.y = 0;
                float tweenLength = rotationDurationFlat + rotationDurationScaled * Vector3.Angle(Unit.transform.rotation * Vector3.forward, direction) / 360f;
                if (tween != null)
                {
                    tween.Kill();
                }
                tween = Unit.transform.DORotateQuaternion(Quaternion.LookRotation(direction), tweenLength);

                yield return null;
            }
        }
    }
}