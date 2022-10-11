using System;
using GameStudio.HunterGatherer.Selection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameStudio.HunterGatherer.FogOfWar
{
    /// <summary>Can be attached to any object to make it react to fog of war by going invisible in the fog of war</summary>
    public class FogOfWarObject : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        public FogOfWarMask fogOfWarMask = null;

        [SerializeField, Tooltip("Renderers to make invisible when object is not visible")]
        private Renderer[] renderers = null;

        [SerializeField, Tooltip("Non-renderers to make invisible when object is not visible")]
        private GameObject[] nonRenderers = null;

        [Header("Events")]
        public UnityEvent OnSetVisible;

        public UnityEvent OnSetInvisible;

        private bool isVisible;
        private bool isBeingSpectated;
        private List<FogOfWarMask> masksOverlapping = new List<FogOfWarMask>();

        private bool HasFogOfWarMaskEnabled => (isBeingSpectated || fogOfWarMask) && fogOfWarMask.MaskReferenceEnabledInHierarchy;

        /// <summary>Property to get isvisible value and set to handle a change in visibility of the object</summary>
        public bool IsVisible
        {
            get { return isVisible; }
            private set
            {
                isVisible = value;
                if (isVisible)
                {
                    OnSetVisible?.Invoke();
                }
                else
                {
                    OnSetInvisible?.Invoke();
                }
            }
        }

        private void Awake()
        {
            IsVisible = false;
            OnSetVisible.AddListener(SetVisible);
            OnSetInvisible.AddListener(SetInvisible);
        }

        private void OnEnable()
        {
            //IsVisible = false;
        }

        private void OnDisable()
        {
            masksOverlapping.Clear();
            isBeingSpectated = false;
            IsVisible = false;
        }

        /// <summary>Makes this fog of war object visible to client and activates it's fogofwar mask</summary>
        public void Spectate()
        {
            IsVisible = true;
            isBeingSpectated = true;
            fogOfWarMask.SetMaskState(true);
        }

        private void OnTriggerEnter(Collider other)
        {
            // Exit if I have a mask myself
            if (HasFogOfWarMaskEnabled)
            {
                return;
            }

            if (other.gameObject.layer == LayerMask.NameToLayer("FogOfWarMask"))
            {
                FogOfWarMask mask = other.gameObject.GetComponentInParent<FogOfWarMask>();
                MaskEnter(mask);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // Exit if I have a mask myself
            if (HasFogOfWarMaskEnabled)
            {
                return;
            }

            if (other.gameObject.layer == LayerMask.NameToLayer("FogOfWarMask"))
            {
                FogOfWarMask mask = other.gameObject.GetComponentInParent<FogOfWarMask>();
                MaskExit(mask);
            }
        }

        public void SetupVisibility()
        {
            IsVisible = HasFogOfWarMaskEnabled;
        }

        /// <summary>Make the FogOfWarObject visible</summary>
        private void SetVisible()
        {
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = true;
            }
            foreach (GameObject nonRenderer in nonRenderers)
            {
                nonRenderer.SetActive(true);
            }
        }

        /// <summary>Make the FogOfWarObject invisible</summary>
        private void SetInvisible()
        {
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = false;
            }
            foreach (GameObject nonRenderer in nonRenderers)
            {
                nonRenderer.SetActive(false);
            }
        }

        /// <summary>Handle this object entering the given mask's radius</summary>
        private void MaskEnter(FogOfWarMask mask)
        {
            mask.OnMaskDisable.AddListener(MaskExit);
            if (!masksOverlapping.Contains(mask))
            {
                masksOverlapping.Add(mask);
            }
            if (masksOverlapping.Count == 1)
            {
                IsVisible = true;
            }
        }

        /// <summary>Handle this object exiting the given mask's radius</summary>
        private void MaskExit(FogOfWarMask mask)
        {
            mask.OnMaskDisable.RemoveListener(MaskExit);
            if (masksOverlapping.Contains(mask))
            {
                masksOverlapping.Remove(mask);
            }
            if (masksOverlapping.Count == 0)
            {
                IsVisible = false;
            }
        }
    }
}