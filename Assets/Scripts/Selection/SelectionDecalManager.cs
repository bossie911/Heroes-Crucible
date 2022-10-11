using System.Collections.Generic;
using UnityEngine;

namespace GameStudio.HunterGatherer.Selection
{
    /// <summary>Handle adding and removing selection decals for selectable objects when they are selected and/or hovered over</summary>
    public class SelectionDecalManager : MonoBehaviour, ISelectionCallbacks
    {
        [Header("Settings")]
        [SerializeField]
        private Color enemyColor = Color.red;

        [SerializeField]
        private Color objectColor = Color.white;

        [SerializeField]
        private Color friendlyColor = Color.blue;

        [Header("References")]
        [SerializeField]
        private GameObject prefabSelectionDecal = null;

        private const int decalStartCount = 5;
        private List<SelectionDecal> selectionDecalPool = new List<SelectionDecal>();
        private Dictionary<SelectableObject, SelectionDecal> selectionDecals = new Dictionary<SelectableObject, SelectionDecal>();
        private List<SelectableObject> trackedSelectableObjects = new List<SelectableObject>();
        private Color[] colors;

        private void Start()
        {
            SelectionManager.Instance.AddCallbackTarget(this);
            colors = new Color[]{ objectColor, enemyColor, friendlyColor };

            InitiatePool();
        }

        private void OnDestroy()
        {
            selectionDecalPool.Clear();
            trackedSelectableObjects.Clear();
            selectionDecals.Clear();
        }

        /// <summary>Initiates the pool by filling it with decals</summary>
        private void InitiatePool()
        {
            for (int i = 0; i <= decalStartCount; i++)
            {
                CreateNewDecal();
            }
        }

        /// <summary>Take a decal out of the pool and set it up with the given selectable object</summary>
        private void AddDecal(SelectableObject selectableObject)
        {
            if (selectionDecalPool.Count == 0)
            {
                CreateNewDecal();
            }

            SelectionDecal selectionDecal = selectionDecalPool[0];
            selectionDecalPool.Remove(selectionDecal);
            selectionDecal.Setup(selectableObject, colors[(int)selectableObject.Group]);
            selectionDecals.Add(selectableObject, selectionDecal);
            trackedSelectableObjects.Add(selectableObject);
        }

        /// <summary>Create a new selection decal and put it in the pool</summary>
        private void CreateNewDecal()
        {
            SelectionDecal newSelectionDecal = Instantiate(prefabSelectionDecal, transform).GetComponent<SelectionDecal>();
            selectionDecalPool.Add(newSelectionDecal);
        }

        /// <summary>Disable the decal of the given selectable object and put it back in the pool</summary>
        private void RemoveDecal(SelectableObject selectableObject)
        {
            SelectionDecal selectionDecal = selectionDecals[selectableObject];
            selectionDecal.Deactivate();
            selectionDecalPool.Add(selectionDecal);
            selectionDecals.Remove(selectableObject);
            trackedSelectableObjects.Remove(selectableObject);
        }

        /// <summary>Add a decal for every selected object that doesn't have a decal assigned yet</summary>
        public void OnTargetsSelected(List<SelectableObject> selectableObjects)
        {
            foreach (SelectableObject selectableObject in selectableObjects)
            {
                // Add decal if selectableObject isn't tracked yet
                if (!trackedSelectableObjects.Contains(selectableObject) && selectableObject.GiveDecal)
                {
                    AddDecal(selectableObject);
                }
            }
        }

        /// <summary>Remove the decal for every selectable object that has been deselected if it's not still hovered</summary>
        public void OnTargetsDeselected(List<SelectableObject> selectableObjects)
        {
            foreach (SelectableObject selectableObject in selectableObjects)
            {
                // Remove decal if selectableObject is tracked and is not being hovered
                if (trackedSelectableObjects.Contains(selectableObject) && !selectableObject.IsHovered)
                {
                    RemoveDecal(selectableObject);
                }
            }
        }

        /// <summary>Add a decal for the hovered object if it doesn't have a decal assigned yet</summary>
        public void OnTargetHoverStart(SelectableObject selectableObject)
        {
            // Add decal if selectableObject isn't tracked yet
            if (!trackedSelectableObjects.Contains(selectableObject) && selectableObject.GiveDecal)
            {
                AddDecal(selectableObject);
            }
        }

        /// <summary>Remove the decal for the hovered object if it's not still selected</summary>
        public void OnTargetHoverEnd(SelectableObject selectableObject)
        {
            // Remove decal if selectableObject is tracked and is not selected
            if (trackedSelectableObjects.Contains(selectableObject) && !selectableObject.IsSelected)
            {
                RemoveDecal(selectableObject);
            }
        }
    }
}