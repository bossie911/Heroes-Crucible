using System;
using GameStudio.HunterGatherer.Networking;
using GameStudio.HunterGatherer.Utilities;

using UnityEngine;

namespace GameStudio.HunterGatherer.Divisions.Decals
{
    /// <summary> This class creates a waypoint decal once the division receives a movement order. </summary>
    [RequireComponent(typeof(DivisionUnitDecals))]
    public class DivisionWaypointDecal : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private GameObject prefab = null;

        [SerializeField]
        private Material sourceDirectionMaterial = null;

        /// <summary> Small amount of spacing required due to the decals sprites having 2 pixels of transparent borders each </summary>
        public const float DirectionSpacing = 0.96f;

        private GameObject Parent { get; set; }
        private ObjectPool<Projector> ObjectPool { get; set; }

        private bool isMine;
        
        private float UnitDecalSize => DivisionUnitDecals.OrthographicSize;
        private Material Material => DivisionUnitDecals.DivisionMaterial;
        private Material DirectionMaterial;
        public Division Division { get; private set; }
        public DivisionUnitDecals DivisionUnitDecals { get; private set; }
        public Projector BoxProjector { get; private set; }
        public Projector DirectionProjector { get; private set; }
        private static Vector3 Up { get; } = new Vector3(0, 100, 0);

        private void Awake()
        {
            DivisionUnitDecals = GetComponent<DivisionUnitDecals>();
            Division = DivisionUnitDecals.Division;

            NetworkEvents.OnStartMatch += StartObject;
            this.enabled = false;
        }

        private void StartObject()
        {
            isMine = Division.IsMine;
            if (!isMine)
            {
                return;
            }

            if (!Parent)
            {
                Parent = GameObjectExtensions.FindOrCreateGameObject("DecalPool");
                ObjectPool = new ObjectPool<Projector>(Parent.transform, prefab, 100);
            }
           
            Division.OnChangedGoal.AddListener(ValidateOrder);
            DirectionMaterial = new Material(sourceDirectionMaterial)
            {
                color = Division.DivisionColor,
            };

            this.enabled = true;
            NetworkEvents.OnStartMatch -= StartObject;
        }

        private void OnDisable()
        {
            if (!Division.IsMine)
            {
                return;
            }

            Division.OnChangedGoal.RemoveListener(ValidateOrder);
            DisableProjector();
        }

        private void OnDestroy()
        {
            NetworkEvents.OnStartMatch -= StartObject;
            ObjectPool = null;
        }

        /// <summary> Check which order to execute. </summary>
        private void ValidateOrder(DivisionGoal divisionGoal)
        {
            switch (divisionGoal)
            {
                case DivisionGoal.Idle: DisableProjector(); break;
                case DivisionGoal.Move: PlaceMovementDecal(); break;
                case DivisionGoal.PlaceBase: PlaceMovementDecal(); break;
                case DivisionGoal.Attack: DisableProjector(); break;
            }
        }

        /// <summary> Disables the projector and returns it. </summary>
        private void DisableProjector()
        {
            if (BoxProjector != null)
            {
                BoxProjector.material = null;
                DirectionProjector.material = null;

                ObjectPool.EnqueueT(BoxProjector);
                ObjectPool.EnqueueT(DirectionProjector);

                BoxProjector = null;
                DirectionProjector = null;
            }
        }

        /// <summary> Places the movement decal. </summary>
        private void PlaceMovementDecal()
        {
            if (BoxProjector == null)
            {
                BoxProjector = ObjectPool.DequeueT();
                BoxProjector.material = Material;
                DirectionProjector = ObjectPool.DequeueT();
                DirectionProjector.material = DirectionMaterial;
            }

            CalculateProjectorDimensions();
            PositionProjectors();
        }

        /// <summary> Calculate the aspect ratios of the projectors </summary>
        private void CalculateProjectorDimensions()
        {
            Rect rectangle = FormationLayout.GenerateRect(Division.Units.Count, Division.FormationRatio, Division.UnitSpacing);

            // 1 orthographic size covers 2m^2 in Unity. Hence why we divide the size.
            BoxProjector.orthographicSize = (rectangle.height + UnitDecalSize) / 2f;
            BoxProjector.aspectRatio = (rectangle.width + UnitDecalSize) / (rectangle.height + UnitDecalSize);

            // DirectionProjector doesn't scale in size
            DirectionProjector.orthographicSize = 1f;
            DirectionProjector.aspectRatio = 1f;
        }

        /// <summary> Position the projectors </summary>
        private void PositionProjectors()
        {
            BoxProjector.transform.position = Division.MoveTarget.Position + Up;
            BoxProjector.transform.rotation = Quaternion.LookRotation(Division.MoveTarget.Direction) * Quaternion.Euler(90, 0, 0);

            DirectionProjector.transform.position = BoxProjector.transform.position +
                Division.MoveTarget.Direction * (BoxProjector.orthographicSize + DirectionSpacing);
            DirectionProjector.transform.rotation = BoxProjector.transform.rotation;
        }
    }
}