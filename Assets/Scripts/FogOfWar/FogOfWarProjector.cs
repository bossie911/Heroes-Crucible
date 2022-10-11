using GameStudio.HunterGatherer.CameraUtility;
using GameStudio.HunterGatherer.Minimap;
using UnityEngine;

namespace GameStudio.HunterGatherer.FogOfWar
{
    /// <summary>Handle the projection of fog of war on the main camera frustrum</summary>
    [ExecuteInEditMode, RequireComponent(typeof(Projector), typeof(Camera))]
    public class FogOfWarProjector : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private float projectorMaxSize = 0;

        [Header("References")]
        [SerializeField]
        private CameraFrustum frustum = null;

        [SerializeField]
        private Transform floodTransform = null;

        private Projector fogProjector;
        private Camera fogCamera;
        private float width;

        private void Awake()
        {
            if (frustum == null)
            {
                Debug.LogError("The variable frustum in FogOfWarProjector doesn't have a value assigned!", frustum);
            }
        }

        private void Start()
        {
            fogProjector = GetComponent<Projector>();
            fogCamera = GetComponent<Camera>();
        }

        private void Update()
        {
            UpdateTransform();
        }

        /// <summary>Update the position and size of the fogProjector and fogCamera</summary>
        private void UpdateTransform()
        {
            Vector3[] frustumCorners = frustum.GetFrustumCornersSchematic(floodTransform.position.y);

            float radius = 0f;
            foreach (Vector3 corner in frustumCorners)
            {
                float distance = (frustum.Center - corner).magnitude;
                if (distance > radius)
                {
                    radius = distance;
                }
            }
            width = Mathf.Min(radius * 2, projectorMaxSize);

            // Set position and size of fogProjector and fogCamera
            Vector3 newPos = frustum.Center;
            newPos.y = transform.position.y;

            transform.position = newPos;
            fogProjector.orthographicSize = width / 2;
            fogCamera.orthographicSize = width / 2;
        }
    }
}