using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameStudio.HunterGatherer.Minimap
{
    ///<summary>References to common variables and functions for minimap scripts</summary>
    [RequireComponent(typeof(MinimapReferences))]
    public class MinimapReferences : MonoBehaviour, IMinimapReferences
    {
        [Header("Settings")]
        [SerializeField]
        private bool minimapZeroInvertX = false;

        [SerializeField]
        private bool minimapZeroInvertY = false;

        [Header("References")]
        [SerializeField]
        private Camera playerCamera = null;

        [SerializeField]
        private Transform worldCenterTransform = null;

        [SerializeField]
        private Camera depthCamera = null;

        public bool MinimapZeroInvertX { get; private set; }
        public bool MinimapZeroInvertY { get; private set; }
        public Camera PlayerCamera { get; private set; }
        public Transform WorldCenterTransform { get; private set; }
        public Camera DepthCamera { get; private set; }

        private Vector3[] worldCorners = new Vector3[4];

        private void Awake()
        {
            MinimapZeroInvertX = minimapZeroInvertX;
            MinimapZeroInvertY = MinimapZeroInvertY;
            PlayerCamera = playerCamera;
            WorldCenterTransform = worldCenterTransform;
            DepthCamera = depthCamera;

            if (playerCamera == null)
            {
                Debug.LogError("The variable playerCamera in MinimapReferences doesn't have a value assigned!");
            }
            if (worldCenterTransform == null)
            {
                Debug.LogError("The variable worldCenterTransform in MinimapReferences doesn't have a value assigned!");
            }
        }

        /// <summary>Normalize a worldposition to be used for the GUI</summary>
        public Vector2 NormalizeWorldSpacePosition(Vector3 position)
        {
            Vector2 normalizedPosition;

            normalizedPosition.x = position.x - worldCenterTransform.transform.position.x;
            normalizedPosition.y = position.z - worldCenterTransform.transform.position.z;

            normalizedPosition /= depthCamera.orthographicSize * 2;

            if (minimapZeroInvertX)
            {
                normalizedPosition.x *= -1;
            }
            if (minimapZeroInvertY)
            {
                normalizedPosition.y *= -1;
            }

            normalizedPosition.x += 0.5f;
            normalizedPosition.y += 0.5f;

            normalizedPosition.x = Mathf.Clamp(normalizedPosition.x, 0, 1);
            normalizedPosition.y = Mathf.Clamp(normalizedPosition.y, 0, 1);

            return normalizedPosition;
        }

        /// <summary>Translate a rect transform to a screen space rect</summary>
        public Rect RectTransformToScreenSpace(RectTransform transform)
        {
            transform.GetWorldCorners(worldCorners);
            Bounds bounds = new Bounds(worldCorners[0], Vector3.zero);
            for (int i = 1; i < 4; ++i)
            {
                bounds.Encapsulate(worldCorners[i]);
            }
            Rect rect = new Rect(bounds.min, bounds.size);
            rect.position = new Vector2(rect.position.x, Screen.height - rect.position.y - rect.size.y);

            return rect;
        }
    }
}