using GameStudio.HunterGatherer.Divisions;
using GameStudio.HunterGatherer.Minimap;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GameStudio.HunterGatherer.Selection
{
    /// <summary>Handle selection of selectable objects</summary>
    public class SelectionManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private Camera cam = null;

        [SerializeField]
        private Color innerColor = Color.clear; // Selection box inner color

        [SerializeField]
        private Color borderColor = Color.green; // Selection box border color

        [SerializeField]
        private float borderThickness = 2f; // Selection box border color

        [SerializeField]
        private float minDragDistance = 0f;

        [Header("References")]
        [SerializeField]
        private GameObject prefabMoveTargetDecal = null;

        [SerializeField]
        private GameObject divisionItemHolder = null;

        [SerializeField]
        private FormationDragDecal prefabFormationDragDecal = null;

        [SerializeField]
        private MinimapIcons minimapIcons = null;

        private List<ISelectionCallbacks> callbackTargets = new List<ISelectionCallbacks>();
        private bool isSelecting;
        private bool isInteracting;
        private bool isDragging;
        private Vector3 mousePosStart;
        private Texture2D whiteTexture; // Texture used for drawing rectangles
        private Vector3 mouseDragStart;
        private Vector3 mouseDragEnd;
        private MoveTargetDecal activeMoveTargetDecal;
        private FormationDragDecal formationDragDecal;
        private SelectableObject hoveredObjectAtStartInteract;
        public UnityEvent onDivisionSelected;

        private static SelectionManager m_Instance = null;

        public static SelectionManager Instance
        { 
            get 
            {
                if (m_Instance is null)
                    m_Instance = GameObject.FindGameObjectWithTag("SelectionManager").GetComponent<SelectionManager>();
                return m_Instance;
            }
        }
        public List<SelectableObject> SelectedObjects { get; private set; } = new List<SelectableObject>();
        public List<SelectableObject> SelectableObjects { get; } = new List<SelectableObject>();
        public SelectableObject HoveredObject { get; private set; }
        public SelectionState State { get; set; } = SelectionState.SelectAndInteract;

        public List<Division> SelectedOwnedDivisions
        {
            get
            {
                List<Division> divisions = new List<Division>();
                foreach (SelectableObject selectableObject in SelectedObjects)
                {
                    Division division = selectableObject.GetComponent<Division>();
                    if (division != null && division.IsMine)
                    {
                        divisions.Add(division);
                    }
                }
                return divisions;
            }
        }

        public List<Division> SelectableDivisions
        {
            get
            {
                List<Division> divisions = new List<Division>();
                foreach (SelectableObject selectableObject in SelectableObjects)
                {
                    Division division = selectableObject.GetComponent<Division>();
                    if(division != null)
                    {
                        divisions.Add(division);
                    }
                }
                return divisions;
            }
        }

        public List<Division> SelectableOwnedDivisions
        {
            get
            {
                List<Division> divisions = new List<Division>();
                foreach (SelectableObject selectableObject in SelectableObjects)
                {
                    if (selectableObject.TryGetComponent(out Division division) && division.IsMine)
                    {
                        divisions.Add(division);
                    }
                }
                return divisions;
            }
        }

        public bool IsDragBigEnough => Vector3.Distance(mouseDragStart, mouseDragEnd) > minDragDistance;

        private void Awake()
        {
            m_Instance = this;
        }

        private void OnDestroy()
        {
            SelectableObjects.Clear();
            SelectedObjects.Clear();
            callbackTargets.Clear();
            HoveredObject = null;
        }

        private void Start()
        {
            whiteTexture = new Texture2D(1, 1);
            whiteTexture.SetPixel(0, 0, Color.white);
            whiteTexture.Apply();
            if (onDivisionSelected == null)
                onDivisionSelected = new UnityEvent();
        }

        private void Update()
        {
            HandleHover();

            // Check if the mousecursor is pressed, and the currently hover object is null.
            // If the formationDragDecal is already active, it's a drag, and not an attack.
            if (isInteracting &&
                (HoveredObject == null || (formationDragDecal != null && formationDragDecal.Active)) &&
                State == SelectionState.SelectAndInteract &&
                SelectedOwnedDivisions.Count > 0)
            {
                CheckDrag();
            }

            // Guard clause
            if (State != SelectionState.SelectAndInteract)
            {
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                StartSelection();
            }

            if (Input.GetMouseButtonUp(0) && isSelecting)
            {
                FinishSelection();
            }

            if (Input.GetMouseButtonDown(1))
            {
                StartInteract();
            }

            if (Input.GetMouseButtonUp(1) && isInteracting)
            {
                FinishInteract();
            }
        }

        private void OnGUI()
        {
            if (isSelecting)
            {
                DrawSelectionBox(mousePosStart, Input.mousePosition);
            }
        }

        /// <summary>Update the hovered selectable object</summary>
        private void HandleHover()
        {
            // Exit if over UI, reset hovered unit
            if (EventSystem.current.IsPointerOverGameObject())
            {
                HandleHoverEnd();
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000f, LayerMask.GetMask("Entities")))
            {
                SelectableObject selectableObject = hit.collider.GetComponentInParent<SelectableObject>();

                // In case you are hovering over a unit, try to get the selectable object of it's division
                if (selectableObject == null)
                {
                    Unit unit = hit.collider.GetComponentInParent<Unit>();

                    if (unit != null && unit.Division != null)
                    {
                        selectableObject = unit.Division.SelectableObject;
                    }
                }

                // Set selectableObject to null if it's not in SelectableObjects or it's not selectable
                if (selectableObject != null && (!SelectableObjects.Contains(selectableObject) || !selectableObject.IsSelectable))
                {
                    selectableObject = null;
                }

                // Call hover callbacks if hoveredObject changes
                if (selectableObject == null || selectableObject != HoveredObject)
                {
                    HandleHoverEnd();

                    HoveredObject = selectableObject;

                    if (HoveredObject != null)
                    {
                        HoveredObject.OnHoverStart.Invoke();

                        // OnHoverStart callback
                        callbackTargets.ForEach(x => x.OnTargetHoverStart(HoveredObject));
                    }
                }
            }
            else
            {
                HandleHoverEnd();
            }
        }

        /// <summary>Invoke OnhoverEnd for hovered entity</summary>
        private void HandleHoverEnd()
        {
            if (HoveredObject != null)
            {
                HoveredObject.OnHoverEnd.Invoke();

                // OnHoverEnd callback
                callbackTargets.ForEach(x => x.OnTargetHoverEnd(HoveredObject));

                HoveredObject = null;
            }
        }

        /// <summary>Start the selection box</summary>
        private void StartSelection()
        {
            // Exit if over UI
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            // Start selection box
            mousePosStart = Input.mousePosition;
            isSelecting = true;
        }

        /// <summary>Finish the selection box and select found selectable objects</summary>
        private void FinishSelection()
        {
            List<SelectableObject> foundSelectableObjects = new List<SelectableObject>();

            // Find selectable objects in selection box
            SelectableObjects.ForEach(x =>
            {
                if ((IsInSelectionBox(x.gameObject) || x.IsHovered) && x.IsSelectable)
                {
                    foundSelectableObjects.Add(x);
                }
            });

            // Select found objects
            SelectObjects(foundSelectableObjects);

            // End selection box
            isSelecting = false;

            if (foundSelectableObjects.Count > 0)
            {
                onDivisionSelected.Invoke();
            }
        }

        /// <summary>Starts interact with prerequisites for finish interact</summary>
        private void StartInteract()
        {
            // Exit if over UI
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            mouseDragStart = RaycastTerrain();
            isInteracting = true;
            hoveredObjectAtStartInteract = HoveredObject;
        }

        /// <summary>If the player is currently dragging, show the formationDragDecal</summary>
        private void CheckDrag()
        {
            List<Division> selectedDivisions = SelectableOwnedDivisions.Where(x => x.IsSelected).ToList();
            //quick and dirty fix to ensure ratio is correct for the selected unit(s)
            Division divisionRatio = selectedDivisions.First();
            int ratio = 1;
            if (selectedDivisions.Count == 1 && divisionRatio.TypeData.Type == DivisionType.Hero)
            {
                ratio = 1;
            }
            else if (selectedDivisions.Count > 1 && divisionRatio.TypeData.Type == DivisionType.Hero)
            {
                divisionRatio = selectedDivisions.First(x => x.TypeData.Type != DivisionType.Hero);
                ratio = divisionRatio.TypeData.MaxUnitCount;
            }
            else
            {
                ratio = divisionRatio.TypeData.MaxUnitCount;
            }

            // Create decal if it is null
            if (formationDragDecal == null)
            {
                formationDragDecal = Instantiate(prefabFormationDragDecal);
            }
            if (!formationDragDecal.Active)
            {
                // Fill it
                formationDragDecal.CreateProjectors(selectedDivisions);

                Vector3 averagePosition = Vector3.zero;
                selectedDivisions.ForEach(o => averagePosition += o.transform.position);
                averagePosition /= selectedDivisions.Count;

                Vector3 averageDirection = mouseDragStart * 2 - averagePosition;
                if (FormationLock.Instance.formationLocked)
                    averageDirection = Quaternion.Euler(0, FormationLock.RotationOffset, 0) * averageDirection;
                //formationDragDecal.UpdateDragDecal(mouseDragStart, averageDirection);
            }

            if (IsDragBigEnough || isDragging)
            {
                //set the formation ratio to match the drag distance and adjust the decal to fit (somewhat)
                mouseDragEnd = RaycastTerrain();

                float dragDistance = Mathf.Floor(Vector3.Distance(mouseDragStart, mouseDragEnd));
                dragDistance = Mathf.Clamp(dragDistance, 1, ratio);//Should not be greater than the formation ratio

                if (dragDistance > 1)
                {
                    foreach (var division in selectedDivisions)
                    {
                        division.formationRatio = dragDistance;
                        division.formationAdjusted = true;
                    }
                }

                isDragging = true;
                formationDragDecal.UpdateDragDecal(mouseDragStart, mouseDragEnd);
                formationDragDecal.PositionProjector();
            }
        }

        /// <summary>Interact with the hovered selectable object</summary>
        private void FinishInteract()
        {
            isInteracting = false;
            List<Division> divisions = SelectedOwnedDivisions;

            isDragging = false;
            // Exit if no friendly divisions
            if (divisions.Count == 0)
            {
                return;
            }

            if (HoveredObject != null && HoveredObject == hoveredObjectAtStartInteract)
            {
                HoveredObject.OnInteract.Invoke(divisions);
            }
            else
            {
                GiveMoveOrder(divisions);
            }
            formationDragDecal?.RemoveAllProjectors();
        }

        /// <summary>Give the given state controllers a move order</summary>
        private void GiveMoveOrder(List<Division> divisions)
        {
            // Exit if over UI
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            if (formationDragDecal && formationDragDecal.Active)
            {
                formationDragDecal.ApplyMoveOrders(mouseDragStart.y);
            }
        }

        /// <summary>Places move target decal on given position</summary>
        private void PlaceMoveTargetDecal(Vector3 position)
        {
            MoveTargetDecal decal = activeMoveTargetDecal ?? (activeMoveTargetDecal = Instantiate(prefabMoveTargetDecal).GetComponent<MoveTargetDecal>());
            decal.gameObject.SetActive(true);
            decal.PlaceOnPosition(position);
        }

        /// <summary>Helper method for raycasting on terrain</summary>
        private Vector3 RaycastTerrain()
        {
            Vector3 mousePosition = Vector3.zero;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000f, LayerMask.GetMask("Terrain")))
            {
                mousePosition = hit.point;
            }
            return mousePosition;
        }

        /// <summary>Select given selectable objects</summary>
        public void SelectObjects(List<SelectableObject> newSelectedObjects)
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                Select(newSelectedObjects);
            }

            if (!isSelecting && SelectedObjects.Count >= 1 && !SelectedObjects.Contains(newSelectedObjects[0]))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    PressShiftSelect(newSelectedObjects);
                }
            }

            EndSelection(newSelectedObjects);
        }

        private void Select(List<SelectableObject> newSelectedObjects)
        {
            // Check what part of the current selection to keep

            // Find out if all passed selectable objects are already selected
            bool allAlreadySelected = true;
            newSelectedObjects.ForEach(x =>
            {
                if (!SelectedObjects.Contains(x))
                {
                    allAlreadySelected = false;
                }
            });

            // If all passed objects are already selected, exclude them from the new selection, otherwise add all already selected objects to the new selection
            if (allAlreadySelected)
            {
                List<SelectableObject> newSelectedObjectsOverride = new List<SelectableObject>();
                SelectedObjects.ForEach(x =>
                {
                    if (!newSelectedObjects.Contains(x))
                    {
                        newSelectedObjectsOverride.Add(x);
                    };
                });
                newSelectedObjects = newSelectedObjectsOverride;
            }
            else
            {
                // Add already selected objects
                SelectedObjects.ForEach(x =>
                {
                    if (!newSelectedObjects.Contains(x))
                    {
                        newSelectedObjects.Add(x);
                    }
                });
            }
        }

        private void PressShiftSelect(List<SelectableObject> newSelectedObjects)
        {
            Select(newSelectedObjects);

            //Selecting items inbetween
            int startID = newSelectedObjects[0].gameObject.GetComponent<Division>().overviewItem.id;
            int endID = newSelectedObjects[1].gameObject.GetComponent<Division>().overviewItem.id;

            if (startID > endID)
            {
                int amountToLoop = startID - endID - 1;

                for (int i = 0; i < amountToLoop; i++)
                {
                    startID--;
                    newSelectedObjects.Add(CheckChildrenForID(startID));
                }
            }
            else
            {
                int amountToLoop = endID - startID - 1;

                for (int i = 0; i < amountToLoop; i++)
                {
                    startID++;
                    newSelectedObjects.Add(CheckChildrenForID(startID));
                }
            }
        }

        private void EndSelection(List<SelectableObject> newSelectedObjects)
        {
            // Deselect all currently selected objects
            foreach (SelectableObject selectableObject in SelectedObjects)
            {
                selectableObject.OnDeselect.Invoke();
            }

            // Deselect callback
            callbackTargets.ForEach(x => x.OnTargetsDeselected(SelectedObjects));

            // Remove newSelectedObjects that are not selectable
            for (int i = newSelectedObjects.Count - 1; i >= 0; i--)
            {
                SelectableObject selectableObject = newSelectedObjects[i];
                if (!selectableObject.IsSelectable)
                {
                    newSelectedObjects.Remove(selectableObject);
                }
            }

            // Filter newSelectedObjects if friendlies (or otherwise enemies) are selected
            List<SelectableObject> friendlies = newSelectedObjects.Where(x => x.Group == SelectableObjectGroup.Friendly).ToList();
            if (friendlies.Count > 0)
            {
                newSelectedObjects = friendlies;
            }
            else
            {
                List<SelectableObject> enemies = newSelectedObjects.Where(x => x.Group == SelectableObjectGroup.Enemy).ToList();
                if (enemies.Count > 0)
                {
                    newSelectedObjects = enemies;
                }
            }

            // Set selectedObjects to new selectedObjects
            SelectedObjects = newSelectedObjects;

            foreach (SelectableObject selectableObject in SelectedObjects)
            {
                selectableObject.OnSelect.Invoke();
            }

            // TargetsSelected callback
            callbackTargets.ForEach(x => x.OnTargetsSelected(SelectedObjects));
        }

        /// <summary>Deselect given selectable objects</summary>
        public void DeselectObjects(List<SelectableObject> selectableObjectsToDeselect)
        {
            // Go through given list and deselect units that are currently selected
            for (int i = selectableObjectsToDeselect.Count - 1; i >= 0; i--)
            {
                SelectableObject selectableObject = selectableObjectsToDeselect[i];
                if (selectableObject.IsSelected)
                {
                    selectableObject.OnDeselect.Invoke();
                    SelectedObjects.Remove(selectableObject);
                }
                else
                {
                    selectableObjectsToDeselect.Remove(selectableObject);
                }
            }

            // TargetsSelected callback
            callbackTargets.ForEach(x => x.OnTargetsDeselected(selectableObjectsToDeselect));
        }

        /// <summary>Return if gameObject is within selection box</summary>
        private bool IsInSelectionBox(GameObject gameObject)
        {
            Bounds viewportBounds = GetViewportBounds(mousePosStart, Input.mousePosition);

            Vector3 position = cam.WorldToViewportPoint(gameObject.transform.position);
            bool contains = viewportBounds.Contains(position);
            return contains;
        }

        /// <summary>Return viewport bounds from two given positions</summary>
        private Bounds GetViewportBounds(Vector3 screenPosition1, Vector3 screenPosition2)
        {
            Vector3 v1 = cam.ScreenToViewportPoint(screenPosition1);
            Vector3 v2 = cam.ScreenToViewportPoint(screenPosition2);
            Vector3 min = Vector3.Min(v1, v2);
            Vector3 max = Vector3.Max(v1, v2);
            min.z = cam.nearClipPlane;
            max.z = cam.farClipPlane;

            Bounds bounds = new Bounds();
            bounds.SetMinMax(min, max);
            return bounds;
        }

        /// <summary>Draw selection box using two points as corners</summary>
        private void DrawSelectionBox(Vector3 pos1, Vector3 pos2)
        {
            // Move origin from bottom left to top left
            pos1.y = Screen.height - pos1.y;
            pos2.y = Screen.height - pos2.y;

            // Calculate corners
            Vector3 topLeft = Vector3.Min(pos1, pos2);
            Vector3 bottomRight = Vector3.Max(pos1, pos2);

            // Create rect
            Rect rect = Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);

            // Draw
            GUI.color = innerColor;
            GUI.DrawTexture(rect, whiteTexture); // Center
            GUI.color = borderColor;
            GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, rect.width, borderThickness), whiteTexture); // Top
            GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, borderThickness, rect.height), whiteTexture); // Left
            GUI.DrawTexture(new Rect(rect.xMax - borderThickness, rect.yMin, borderThickness, rect.height), whiteTexture); // Right
            GUI.DrawTexture(new Rect(rect.xMin, rect.yMax - borderThickness, rect.width, borderThickness), whiteTexture); // Bottom
            GUI.color = Color.white;
        }

        /// <summary>Add selectable object to list of selectable objects</summary>
        public void AddSelectableObject(SelectableObject selectableObject)
        {
            if (!SelectableObjects.Contains(selectableObject))
            {
                SelectableObjects.Add(selectableObject);

                if (selectableObject.TryGetComponent(out Division division))
                {
                    minimapIcons.Divisions.Add(division);
                }
            }
        }

        /// <summary>Remove selectable object from list of selectable objects</summary>
        public void RemoveSelectableObject(SelectableObject selectableObject)
        {
            // Exit if selectableObject isn't in SelectableObjects
            if (!SelectableObjects.Contains(selectableObject))
            {
                return;
            }

            SelectableObjects.Remove(selectableObject);

            // If selectableObject is a division owned by the player
            if (selectableObject.TryGetComponent(out Division division))
            {
                minimapIcons.Divisions.Remove(division);
            }

            if (selectableObject.IsSelected)
            {
                SelectedObjects.Remove(selectableObject);
                selectableObject.OnDeselect.Invoke();

                // Deselect callback
                callbackTargets.ForEach(x => x.OnTargetsDeselected(new List<SelectableObject> { selectableObject }));
            }

            if (selectableObject.IsHovered)
            {
                selectableObject.OnHoverEnd.Invoke();

                // OnHoverEnd callback
                callbackTargets.ForEach(x => x.OnTargetHoverEnd(HoveredObject));
                HoveredObject = null;
            }
        }

        /// <summary>Add callback target to list of callback targets</summary>
        public void AddCallbackTarget(ISelectionCallbacks target)
        {
            callbackTargets.Add(target);
        }

        /// <summary>Remove callback target from list of callback targets</summary>
        public void RemoveCallbackTarget(ISelectionCallbacks target)
        {
            callbackTargets.Remove(target);
        }

        /// <summary>Checks if a child has the given id, if so. Return the selectableObject</summary>
        private SelectableObject CheckChildrenForID(int checkId)
        {
            //Add the item with the same id as current id to the selectedobjects
            foreach (Transform child in divisionItemHolder.transform)
            {
                if (child.gameObject.tag == "DivisionCard")
                {
                    if (child.gameObject.GetComponent<Divisions.UI.DivisionOverviewItem>().id == checkId)
                    {
                        return child.gameObject.GetComponent<Divisions.UI.DivisionOverviewItem>().division.gameObject.GetComponent<SelectableObject>();
                    }
                }
            }
            return null;
        }
    }
}