using GameStudio.HunterGatherer.CameraUtility;
using GameStudio.HunterGatherer.Utilities;
using UnityEngine;

namespace GameStudio.HunterGatherer.Minimap
{
    /// <summary>Continuesly draw lines on the GUI to show the frustum of the player's camera on the minimap</summary>
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(MinimapReferences))]
    public class MinimapFrustum : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        [Range(1, 10)]
        private float frustumDrawWidth = 2;

        [Header("References")]
        [SerializeField]
        private CameraFrustumType frustumType = CameraFrustumType.Schematic;

        [SerializeField]
        private CameraFrustum frustum = null;

        [SerializeField]
        private Color frustumColor = Color.white;

        [SerializeField]
        private bool antiAliasingEnabled = false;

        private RectTransform minimapRectTransform;
        private Rect minimapRect;
        private Vector3[] worldCorners = new Vector3[4];
        private MinimapReferences references;

        private void Awake()
        {
            if (frustum == null)
            {
                Debug.LogError("The variable frustum in MinimapFrustum doesn't have a value assigned!");
            }
            minimapRectTransform = GetComponent<RectTransform>();
            references = GetComponent<MinimapReferences>();
        }

        private void OnGUI()
        {
            RaycastHit hit;
            float height;

            minimapRect = references.RectTransformToScreenSpace(minimapRectTransform);

            // Get the height of the terrain the camera is hovering over
            if (Physics.Raycast(references.PlayerCamera.transform.position, Vector3.down, out hit))
            {
                height = references.PlayerCamera.transform.position.y - hit.distance;
            }
            else
            {
                height = frustum.TerrainHeight;
            }
            // Get the info of the frustum and translate it to a screen space position
            Vector3[] frustumCorners = frustum.GetFrustumCorners(height, frustumType);
            Vector2[] frustumCornersGUI = new Vector2[frustumCorners.Length];

            for (int i = 0; i < frustumCornersGUI.Length; i++)
            {
                frustumCornersGUI[i] = references.NormalizeWorldSpacePosition(frustumCorners[i]) * minimapRect.size + minimapRect.position;
            }

            // Create frustum
            for (int i = 0; i < frustumCorners.Length; i++)
            {
                if (frustumCorners.Length > i + 1)
                {
                    Drawing.DrawLine(frustumCornersGUI[i], frustumCornersGUI[i + 1], frustumColor, frustumDrawWidth, antiAliasingEnabled);
                }
                else
                {
                    Drawing.DrawLine(frustumCornersGUI[i], frustumCornersGUI[0], frustumColor, frustumDrawWidth, antiAliasingEnabled);
                }
            }
        }
    }
}