using UnityEngine;

namespace GameStudio.HunterGatherer.Divisions
{
    /// <summary>Movetarget holding a position or transform and a direction, serving as a waypoint for units and divisions</summary>
    public class MoveTarget
    {
        public Vector3 Position
        {
            get
            {
                return targetTransform == null ? position : targetTransform.position;
            }
        }
        public Vector3 Direction { get; private set; }
        public bool HasTargetTransform => targetTransform != null;

        public Transform targetTransform;
        public Vector3 position;

        public MoveTarget(Transform targetTransform, Vector3 direction)
        {
            this.targetTransform = targetTransform;
            direction.y = 0;
            Direction = direction.normalized;
        }

        public MoveTarget(Vector3 position, Vector3 direction)
        {
            this.position = position;
            direction.y = 0;
            Direction = direction.normalized;
        }

        public MoveTarget()
        {
            Direction = Vector3.forward.normalized;
        }
    }
}