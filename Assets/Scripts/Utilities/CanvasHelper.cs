using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameStudio.HunterGatherer.Utilities
{
    /// <summary>Aligns camera rect transform with the world rect transform. Only call in LateUpdate </summary>
    public static class CanvasHelper
    {
        public static void AlignnUIInWorldSpace(RectTransform UIElement, RectTransform canvas, Transform target, float offset)
        {
            // Offset position above object bbox (in world space)
            float offsetPosY = target.position.y + offset;

            // Final position of marker above GO in world space
            Vector3 offsetPos = new Vector3(target.transform.position.x, offsetPosY, target.transform.position.z);

            // Calculate *screen* position (note, not a canvas/recttransform position)
            Vector2 canvasPos;
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(offsetPos);

            // Convert screen position to Canvas / RectTransform space <- leave camera null if Screen Space Overlay
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, screenPoint, null, out canvasPos);

            // Set
            UIElement.localPosition = canvasPos;
        }
    }
}
