using GameStudio.HunterGatherer.Divisions;
using System.Collections.Generic;
using UnityEngine;

namespace GameStudio.HunterGatherer.Selection
{
    /// <summary>Can be setup using a selectable object and will display its selection state</summary>
    public class SelectionDecal : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private float interactTweenLength = 0.1f;

        [SerializeField]
        private float interactTweenSizeIncrease = 0.2f;

        [Header("References")]
        [SerializeField]
        private Projector projectorHoverDecal = null;

        [SerializeField]
        private Projector projectorSelectDecal = null;

        private SelectableObject selectableObject;
        private float hoverDecalSize;
        private LTSeq interactTweenSequence;

        private void Update()
        {
            UpdateDecals();
        }

        /// <summary>Set up the decal with the given selectable object and color</summary>
        public void Setup(SelectableObject selectableObject, Color color)
        {
            this.selectableObject = selectableObject;
            projectorHoverDecal.orthographicSize = this.selectableObject.HoverDecalSize;
            projectorSelectDecal.orthographicSize = this.selectableObject.SelectDecalSize;
            hoverDecalSize = projectorHoverDecal.orthographicSize;
            projectorHoverDecal.material.color = color;
            projectorSelectDecal.material.color = color;

            UpdateDecals();
            LeanTween.rotateAround(projectorHoverDecal.gameObject, Vector3.up, 360, 4).setRepeat(-1);
            this.selectableObject.OnInteract.AddListener(Interact);
            gameObject.SetActive(true);
        }

        /// <summary>Update the decal's position and the active states of the specific decals</summary>
        private void UpdateDecals()
        {
            if (selectableObject != null)
            {
                transform.position = selectableObject.transform.position;
                projectorHoverDecal.gameObject.SetActive(selectableObject.IsHovered);
                projectorSelectDecal.gameObject.SetActive(selectableObject.IsSelected);
            }
            else
            {
                projectorHoverDecal.gameObject.SetActive(false);
                projectorSelectDecal.gameObject.SetActive(false);
            }
        }

        /// <summary>Reset the decal and set it to inactive</summary>
        public void Deactivate()
        {
            projectorHoverDecal.orthographicSize = hoverDecalSize;
            LeanTween.cancel(projectorHoverDecal.gameObject);
            selectableObject.OnInteract.RemoveListener(Interact);
            gameObject.SetActive(false);
        }

        /// <summary>Tween the size of the hover decal to show interaction</summary>
        private void Interact(List<Division> divisions)
        {
            projectorHoverDecal.orthographicSize = hoverDecalSize;

            if (interactTweenSequence != null)
            {
                LeanTween.cancel(interactTweenSequence.id);
            }

            interactTweenSequence = LeanTween.sequence();
            interactTweenSequence.append(LeanTween.value(hoverDecalSize, hoverDecalSize * (1 + interactTweenSizeIncrease), interactTweenLength).setOnUpdate((float val) => projectorHoverDecal.orthographicSize = val));
            interactTweenSequence.append(() =>
            {
                LeanTween.value(hoverDecalSize * (1 + interactTweenSizeIncrease), hoverDecalSize, interactTweenLength).setOnUpdate((float val) => projectorHoverDecal.orthographicSize = val);
            });
        }
    }
}