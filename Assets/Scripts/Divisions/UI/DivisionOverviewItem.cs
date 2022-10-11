using GameStudio.HunterGatherer.Selection;
using GameStudio.HunterGatherer.Structures;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using GameStudio.HunterGatherer.Utilities;

namespace GameStudio.HunterGatherer.Divisions.UI
{
    /// <summary>Handles a singular divisionOverviewItem, displaying a division's information and selection status, and changing using events</summary>
    public class DivisionOverviewItem : MonoBehaviour, ISelectionCallbacks
    {
        [Header("Settings")] [SerializeField] private Color colorDefault = Color.clear;

        [SerializeField] private Color colorOnHover = Color.white;

        [SerializeField] private Color colorOnSelected = new Color(0.9f, 0.9f, 0.9f);

        [Header("References")] [SerializeField]
        public Image imgBackground = null;

        [SerializeField] private CameraUtility.CameraController cameraController;

        [SerializeField] private HealthBar healthBar = null;

        [SerializeField] private Image imgSelection = null;

        [SerializeField] public Image imgIcon = null;

        [SerializeField] private TMP_Text txtUnitCount = null;
        [SerializeField] public int maxUpgrades;
        [SerializeField] public int id;
        public Division division { get; private set; }
        private bool isHovered;
        private bool isRecentlyPressed;
        
        public int upgrades = 0;

        private void Awake()
        {
            SelectionManager.Instance.AddCallbackTarget(this);
            imgSelection.color = colorDefault;

            cameraController = References.Instance.cameraController;
        }

        private void OnDestroy()
        {
            SelectionManager.Instance.RemoveCallbackTarget(this);

            if (division != null)
            {
                division.OnUnitCountChanged.RemoveListener(SetUnitCount);
                division.OnDivisionTypeChanged.RemoveListener(UpdateType);
                if (division.TypeData != null)
                {
                    if (division.Type == DivisionType.Hero)
                    {
                        if (division.FirstUnit != null)
                        {
                            //division.FirstUnit.onHit.RemoveListener(healthBar.SetHealth);
                        }
                    }
                }
            }
        }
        
        /// <summary>Setup the overviewItem using the given division</summary>
        public void Setup(Division division)
        {
            this.division = division;
            division.OnUnitCountChanged.AddListener(SetUnitCount);
            division.OnDivisionTypeChanged.AddListener(UpdateType);
            healthBar.Initialize(division);
            imgBackground.color = division.DivisionColor;
            imgIcon.sprite = division.TypeData.Icon;
            txtUnitCount.enabled = division.Type != DivisionType.Hero;
            txtUnitCount.text = division.Units.Count.ToString();
        }

        /// <summary>Set the unit count to the given amount, called by an event</summary>
        private void SetUnitCount(int unitCount)
        {
            txtUnitCount.text = unitCount.ToString();
            healthBar.UpdateUnitCount(unitCount);
        }

        /// <summary>Update the icon showing the division type, called by an event
        /// Assuming a division can't switch from hero to another type or the other way around, OnEnterExitBase listeners are not updated here</summary>
        private void UpdateType()
        {
            imgIcon.sprite = division.TypeData.Icon;
        }


        public void OnTargetHoverEnd(SelectableObject selectableObject)
        {
        }

        public void OnTargetHoverStart(SelectableObject selectableObject)
        {
        }

        /// <summary>When the division is deselected, show deselected status</summary>
        public void OnTargetsDeselected(List<SelectableObject> selectableObjects)
        {
            // Guard clause to exit if division is not in given selectable objects
            if (!selectableObjects.Contains(division.SelectableObject))
            {
                return;
            }

            imgSelection.color = colorDefault;
        }

        /// <summary>When the division is selected, show selected status</summary>
        public void OnTargetsSelected(List<SelectableObject> selectableObjects)
        {
            // Guard clause to exit if division is not in given selectable objects
            if (!selectableObjects.Contains(division.SelectableObject))
            {
                return;
            }

            imgSelection.color = isHovered ? colorOnHover : colorOnSelected;
        }

        /// <summary>Consider item hovered when pointer enters</summary>
        public void PointerEnter()
        {
            isHovered = true;
            imgSelection.color = colorOnHover;
        }

        /// <summary>Consider item not hovered anymore when pointer exits</summary>
        public void PointerExit()
        {
            isHovered = false;
            imgSelection.color = division.IsSelected ? colorOnSelected : colorDefault;
        }

        /// <summary>Select the division belonging to the item when the item is clicked</summary>
        public void PointerClick()
        {
            SelectionManager.Instance.SelectObjects(new List<SelectableObject> {division.SelectableObject});
            DoubleClick();
        }

        private void DoubleClick()
        {
            //Timer for dubble clicking
            if (isRecentlyPressed)
            {
                Vector2 newPosition = new Vector2(division.gameObject.transform.position.x, division.gameObject.transform.position.z);
                cameraController.SetPosition(newPosition);
                isRecentlyPressed = false;
            }
            else
            {
                isRecentlyPressed = true;
                StartCoroutine(DubbleClickTimer());
            }
        }

        /// <summary>Reset the recentlypressed bool</summary>
        private IEnumerator DubbleClickTimer()
        {
            float duration = 1f;
            float time = 0;

            while (time < duration)
            {
                time += Time.deltaTime;
                yield return null;
            }
            isRecentlyPressed = false;
        }
    }
}