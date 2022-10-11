using GameStudio.HunterGatherer.Selection;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameStudio.HunterGatherer.UI
{
    /// <summary>Tooltip attached to the mouse used to display information about what the player is hovering over</summary>
    public class Tooltip : MonoBehaviour, ISelectionCallbacks
    {
        [Header("References")]
        [SerializeField]
        private RectTransform tooltipWindow = null;

        [SerializeField]
        private RectTransform contentDefault = null;

        [SerializeField]
        private TMP_Text contentDefaultText = null;

        [SerializeField]
        private TMP_Text contentDefaultSecondaryText = null;

        public static Tooltip Instance;

        private bool force;

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            SelectionManager.Instance.AddCallbackTarget(this);
            Disable();
        }

        private void OnDisable()
        {
            force = false;
            SelectionManager.Instance.RemoveCallbackTarget(this);
        }

        private void Update()
        {
            transform.position = Input.mousePosition;
        }

        /// <summary>Set the tooltip up with given text, optional secondary text, and optional forced (to not switch tooltips when hovering over something else)</summary>
        public void Setup(string text, string secondaryText = "", bool force = false)
        {
            // Exit if current tooltip is forced
            if (this.force) { return; }

            this.force = force;
            contentDefault.gameObject.SetActive(true);
            tooltipWindow.gameObject.SetActive(true);

            contentDefaultText.text = text;
            if (secondaryText != "")
            {
                contentDefaultSecondaryText.gameObject.SetActive(true);
                contentDefaultSecondaryText.text = secondaryText;
            }
            else
            {
                contentDefaultSecondaryText.gameObject.SetActive(false);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentDefault);
        }

        /// <summary>Disable the tooltip, with optional forceDisable to also disable forced tooltips</summary>
        public void Disable(bool forceDisable = false)
        {
            if (forceDisable || !force)
            {
                force = false;
                tooltipWindow.gameObject.SetActive(false);
            }
        }

        public void OnTargetsSelected(List<SelectableObject> selectableObjects)
        {
        }

        public void OnTargetsDeselected(List<SelectableObject> selectableObjects)
        {
        }

        /// <summary>Set the tooltip up with given selectable object</summary>
        public void OnTargetHoverStart(SelectableObject selectableObject)
        {
            Setup(selectableObject.GetTooltipText());
        }

        /// <summary>Disable tooltip when not hovering over an object anymore</summary>
        public void OnTargetHoverEnd(SelectableObject selectableObject)
        {
            Disable();
        }
    }
}