using GameStudio.HunterGatherer.Networking.Events;
using GameStudio.HunterGatherer.Utilities;
using Mirror;
using UnityEngine;

namespace GameStudio.HunterGatherer.Divisions
{
    /// <summary>Handles a projectile flying from a start position to an end position, with the option of tracking the target</summary>
    public class Projectile : NetworkBehaviour
    {
        [SerializeField]
        public Vector3 unitOffset = Vector3.up;

        [SerializeField]
        public float distanceToTimeRatio = 15f;

        [SerializeField]
        public float distanceToHeightRatio = 3f;

        public Unit attackerUnit;
        public Unit targetUnit;
        public HitType hitType;
        public Vector3 startPos;
        public Vector3 endPos;
        public Vector3 lastPos;
        public bool isFlying;
        public float flyTimeTotal;
        public float flyTimeCurrent;
        public float height;

        private void Update()
        {
            // Return if the projectile isn't flying yet/anymore
            if (!isFlying || targetUnit == null || !isServer)
            {
                return;
            }

            // Keep updating endPos if the projectile will hit (= target tracking)
            if (hitType != HitType.Miss)
            {
                endPos = targetUnit.transform.position + unitOffset;
            }
            // Move and rotate
            lastPos = transform.position;
            transform.position = Parabola.Lerp(startPos, endPos, height, flyTimeCurrent / flyTimeTotal);
            transform.LookAt(transform.position + transform.position - lastPos);

            // Increase time, check if arrived
            flyTimeCurrent += Time.deltaTime;
            if (flyTimeCurrent >= flyTimeTotal)
            {
                int damage = 1;
                Debug.Log("Damage:" + damage);
                targetUnit.Hit(hitType, attackerUnit, damage);
                isFlying = false;
                NetworkServer.Destroy(gameObject);
            }
        }
    }
}