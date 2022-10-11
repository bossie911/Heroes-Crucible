using UnityEngine;

namespace GameStudio.HunterGatherer.Utilities
{
    /// <summary>Utility for drawing gizmos on object</summary>
    public class ShowGizmo : MonoBehaviour
    {
        private enum GizmoType { Sphere, Cube }

        [Header("Settings")]
        [SerializeField]
        private GizmoType type = GizmoType.Sphere;

        [SerializeField]
        private float radius = 5f;

        [SerializeField]
        private Color color = Color.black;

        private void OnDrawGizmos()
        {
            Gizmos.color = color;

            switch (type)
            {
                case GizmoType.Sphere:
                    Gizmos.DrawSphere(transform.position, radius);
                    break;

                case GizmoType.Cube:
                    float s = radius * 2f;
                    Vector3 size = new Vector3(s, s, s);
                    Gizmos.DrawCube(transform.position, size);
                    break;
            }
        }

        /// <summary>Updates shown gizmo's color</summary>
        public void UpdateColor(Color newColor)
        {
            color = newColor;
        }
    }
}