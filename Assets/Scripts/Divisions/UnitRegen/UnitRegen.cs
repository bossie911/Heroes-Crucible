using System.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace GameStudio.HunterGatherer.Divisions
{
    public class UnitRegen : MonoBehaviour
    {
        [SerializeField] private Unit Unit;
        [SerializeField, Range(0, 1)] private float percentageRegen = 0.03f;
        [SerializeField] private float delayBetweenHeals = 1;

        [SerializeField] private bool canRegenHealth;

        void Start()
        {
            //todo StartCoroutine(Regen());
        }

        /// <summary>
        /// Enables or disables the health regen based on the input
        /// </summary>
        /// <param name="canRegen"></param>
        public void SetCanRegen(bool canRegen)
        {
            canRegenHealth = canRegen;
        }

        private IEnumerator Regen()
        {
            while (true)
            {
                if (Unit.IsMine && Unit.State == UnitState.Idle &&
                    Unit.Health < Unit.Division.TypeData.MaxHealth)
                {
                    Unit.Heal(percentageRegen);
                }

                yield return new WaitForSeconds(delayBetweenHeals);
            }
        }
    }
}