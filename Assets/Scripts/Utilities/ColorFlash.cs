using DG.Tweening;
using UnityEngine;

namespace GameStudio.HunterGatherer.Utilities
{
    /// <summary>Tween set renderer's color from a set color back to it's own color, and safely put it's original material back</summary>
    public class ColorFlash : MonoBehaviour
    {
        [SerializeField]
        private Color colorToFlash = Color.white;

        [SerializeField]
        private float flashDuration = 0.5f;

        [SerializeField]
        private MeshRenderer rendererToFlash = null;

        private Material originalMaterial;
        private Tween tween;

        /// <summary>Make a copy of the renderer's material, tween it's color, then put back the old material</summary>
        public void FlashColor()
        {
            // Complete existing flash tween if there is one
            if (tween != null)
            {
                tween.Complete();
            }

            // Save old material and make a flashed copy
            originalMaterial = rendererToFlash.material;
            rendererToFlash.material = new Material(rendererToFlash.material);
            rendererToFlash.material.color = colorToFlash;

            // Start flash tween
            tween = rendererToFlash.material.DOColor(originalMaterial.color, flashDuration).OnComplete(() => rendererToFlash.material = originalMaterial);
        }
    }
}