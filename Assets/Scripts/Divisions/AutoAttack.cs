using System.Collections.Generic;
using UnityEngine;

namespace GameStudio.HunterGatherer.Divisions
{
    /// <summary>Handles auto attacking enemy divisions that come into range</summary>
    [RequireComponent(typeof(CapsuleCollider))]
    public class AutoAttack : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private Division division = null;

        private CapsuleCollider capsuleCollider;
        private List<Division> enemyDivisionsInRange = new List<Division>();

        private void Awake()
        {
            capsuleCollider = GetComponent<CapsuleCollider>();
        }

        private void OnEnable()
        {
            enemyDivisionsInRange.Clear();
            if (division.IsMine)
            {
                division.OnDivisionTypeChanged.AddListener(UpdateSize);
                division.OnDivisionStatsChanged.AddListener(UpdateSize);
                division.OnChangedGoal.AddListener(AttackClosestDivisionInRange);
            }
        }

        private void OnDisable()
        {
            if (division.IsMine)
            {
                division.OnDivisionTypeChanged.RemoveListener(UpdateSize);
                division.OnDivisionStatsChanged.RemoveListener(UpdateSize);
                division.OnChangedGoal.RemoveListener(AttackClosestDivisionInRange);
            }
        }

        /// <summary>Update the size of the auto attack collider, called by the OnDivisionTypeChanged event</summary>
        private void UpdateSize()
        {
            capsuleCollider.radius = division.TypeData.AutoAttackRange;
        }

        /// <summary>If we are idle and we have enemy divisions in range, attack the closest</summary>
        private void AttackClosestDivisionInRange(DivisionGoal _)
        {
            // Guard clause to exit if there is no divisions in range or we are not idle right now
            if (enemyDivisionsInRange.Count == 0 || division.Goal != DivisionGoal.Idle)
            {
                return;
            }

            List<Division> sortedListOfDivisions = new List<Division>(enemyDivisionsInRange);
            sortedListOfDivisions.Sort((d1, d2) => Vector3.Distance(transform.position, d1.transform.position).CompareTo(Vector3.Distance(transform.position, d2.transform.position)));
            division.DefendOrder(sortedListOfDivisions[0]);
        }

        private void OnTriggerEnter(Collider other)
        {
            // Guard clause to exit if this division isn't ours
            if (!division.IsMine)
            {
                return;
            }

            if (other.gameObject.layer == LayerMask.NameToLayer("Division"))
            {
                Division division = other.gameObject.GetComponentInParent<Division>();

                if (!division.IsMine)
                {
                    RangeEnter(division);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // Guard clause to exit if this division isn't ours
            if (!division.IsMine)
            {
                return;
            }

            if (other.gameObject.layer == LayerMask.NameToLayer("Division"))
            {
                Division division = other.gameObject.GetComponentInParent<Division>();
                if (!division.IsMine)
                {
                    RangeExit(division);
                }
            }
        }

        /// <summary>Handle a division entering the attack range</summary>
        private void RangeEnter(Division division)
        {
            division.OnDestroyDivision.AddListener(RangeExit);
            if (!enemyDivisionsInRange.Contains(division))
            {
                enemyDivisionsInRange.Add(division);
            }

            if (this.division.Goal == DivisionGoal.Idle)
            {
                this.division.DefendOrder(division);
            }
        }

        /// <summary>Handle a division exiting the attack range</summary>
        private void RangeExit(Division division)
        {
            division.OnDestroyDivision.RemoveListener(RangeExit);
            if (enemyDivisionsInRange.Contains(division))
            {
                enemyDivisionsInRange.Remove(division);
            }
        }
    }
}