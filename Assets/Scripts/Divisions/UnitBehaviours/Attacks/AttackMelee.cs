using GameStudio.HunterGatherer.Networking;
using UnityEngine;

namespace GameStudio.HunterGatherer.Divisions.UnitBehaviours.UnitAttacks
{
    /// <summary>Melee attack pattern, triggered by calling trigger attack</summary>
    /// Note by team Tech Shack; previous team(s) made the blocking mechanic really ugly, and we've managed to fix it only somewhat. Sorry!
    public class AttackMelee : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        public Unit unit = null;

        /// <summary>Check if we hit, miss or get blocked, and send that hit event</summary>
        public void TriggerAttack()
        {
            //if (!unit.IsMine) return;
            unit.AttackTarget.Hit(HitType.Hit, unit, unit.Division.TypeData.Damage);
        }
    }
}