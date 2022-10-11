using GameStudio.HunterGatherer.CustomEvents;
using GameStudio.HunterGatherer.FogOfWar;
using GameStudio.HunterGatherer.Networking;
using System.Collections.Generic;
using GameStudio.HunterGatherer.Divisions;
using UnityEngine;
using UnityEngine.Events;

namespace GameStudio.HunterGatherer.Selection
{
    /// <summary>Can be attached to any object to make it selectable by the selection manager</summary>
    public class SelectableObject : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField, Tooltip("Text used in tooltip, unless overridden in GetTooltipText")]
        private string objectName = string.Empty;

        [SerializeField]
        private bool giveDecal = true;

        [SerializeField]
        private float hoverDecalSize = 1.1f;

        [SerializeField]
        private float selectDecalSize = 0.9f;

        [SerializeField]
        private Color gizmoColor = Color.white;

        [SerializeField]
        private float gizmoRadius = 4f;

        private bool isSelectable;

        public bool IsHovered { get; private set; }
        public bool IsSelected { get; private set; }
        public bool GiveDecal => giveDecal;
        public string ObjectName
        {
            get
            {
                return objectName;
            }
            set
            {
                objectName = value;
            }
        }
        public float HoverDecalSize { get { return hoverDecalSize; } }
        public float SelectDecalSize { get { return selectDecalSize; } }
        public SelectableObjectGroup Group { get; private set; }
        //public NetworkedMovingObject NetworkedMovable { get; private set; }
        public FogOfWarObject FogOfWarObject { get; private set; }
        public UnityEvent OnIsNotSelectable { get; } = new UnityEvent();

        // Used to keep unit in SelectionManager.Instance.SelectableObjects, but not be able to select it
        public bool IsSelectable
        {
            get
            {
                return isSelectable;
            }
            set
            {
                isSelectable = value;

                if (!value)
                {
                    OnIsNotSelectable.Invoke();
                    if (IsSelected)
                    {
                        SelectionManager.Instance.DeselectObjects(new List<SelectableObject> { this });
                    }
                }
            }
        }

        public UnityEvent OnSelect;
        public UnityEvent OnDeselect;
        public UnityEvent OnHoverStart;
        public UnityEvent OnHoverEnd;
        public UnityEventDivisionList OnInteract = new UnityEventDivisionList();

        private void Awake()
        {
            //NetworkedMovable = GetComponent<NetworkedMovingObject>();
            FogOfWarObject = GetComponentInChildren<FogOfWarObject>();
        }

        private void Start()
        {
            OnSelect.AddListener(Select);
            OnDeselect.AddListener(Deselect);
            OnHoverStart.AddListener(HoverStart);
            OnHoverEnd.AddListener(HoverEnd);
        }

        private void OnEnable()
        {
            IsSelectable = true;
            SelectionManager.Instance.AddSelectableObject(this);
        }

        private void OnDisable()
        {
            SelectionManager.Instance.RemoveSelectableObject(this);
            Group = SelectableObjectGroup.Object;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawSphere(transform.position, gizmoRadius);
        }

        /// <summary>Set the selectable object to selected</summary>
        private void Select()
        {
            IsSelected = true;
        }

        /// <summary>Set the selectable object to deselected</summary>
        private void Deselect()
        {
            IsSelected = false;
        }

        /// <summary>Set the selectable object to hovered</summary>
        private void HoverStart()
        {
            IsHovered = true;
        }

        /// <summary>Set the selectable object to not hovered</summary>
        private void HoverEnd()
        {
            IsHovered = false;
        }

        /// <summary>Set the SelectableObjectGroup of the SelectableObject</summary>
        public void SetGroup(SelectableObjectGroup selectableObjectGroup)
        {
            Group = selectableObjectGroup;
        }

        /// <summary>Return tooltip text for selectable object</summary>
        public string GetTooltipText()
        {
            string text = objectName;
            if (TryGetComponent<Division>(out var division))
            {
                string playerName = division.PlayerName;
                text += " (" + (playerName != "" ? playerName : "player") + ")";
            }

            return text;
        }
    }
}