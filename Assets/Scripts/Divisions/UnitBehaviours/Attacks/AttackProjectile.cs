using GameStudio.HunterGatherer.Networking;
using GameStudio.HunterGatherer.Networking.Events;
using Mirror;
using UnityEngine;

namespace GameStudio.HunterGatherer.Divisions.UnitBehaviours.UnitAttacks
{
    /// <summary>Ranged projectile attack pattern, triggered by calling trigger attack</summary>
    /// Note by team Tech Shack; previous team(s) made the blocking mechanic really ugly, and we've managed to fix it only somewhat. Sorry!
    public class AttackProjectile : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField]
        public Unit Unit = null;

        [SerializeField]
        private Projectile prefabProjectile = null;

        /// <summary>Check if we hit, miss or block and send a projectile event that will be handled locally in FireProjectile</summary>
        public void TriggerAttack()
        {
            //Unit.AttackTarget.Hit(HitType.Hit, Unit, Unit.Division.TypeData.Damage);
            FireProjectile(HitType.Hit, Unit);
        }

        /// <summary>Received locally by event from TriggerAttack, making the projectile and letting it fly</summary>
        [Command(requiresAuthority = false)]
        private void FireProjectile(HitType hitType, Unit unit)
        {
            if (unit.AttackTarget == null) return;
            
            var projectile = Instantiate(prefabProjectile, transform.position, Quaternion.identity).GetComponent<Projectile>();
            projectile.attackerUnit = unit;
            projectile.targetUnit = unit.AttackTarget;
            projectile.hitType = hitType;

            projectile.startPos = unit.transform.position + projectile.unitOffset;
            projectile.endPos = unit.AttackTarget.transform.position;
            projectile.flyTimeTotal = Vector3.Distance(projectile.startPos, projectile.endPos) / projectile.distanceToTimeRatio;
            projectile.height = Vector3.Distance(projectile.startPos, projectile.endPos) / projectile.distanceToHeightRatio;
            projectile.isFlying = true;
            NetworkServer.Spawn(projectile.gameObject);

        }
    }
}