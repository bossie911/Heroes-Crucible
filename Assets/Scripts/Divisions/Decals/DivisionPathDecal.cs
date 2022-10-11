using GameStudio.HunterGatherer.Divisions;
using GameStudio.HunterGatherer.Networking;
using GameStudio.HunterGatherer.Utilities;
using Mirror;
using UnityEngine;
using NetworkRoomManager = Mirror.NetworkRoomManager;

namespace GameStudio.HunterGatherer.Divisions.Decals
{
    /// <summary>
    /// Displays a line to where the division is moving
    /// </summary>
    [RequireComponent(typeof(Division))]
    public class DivisionPathDecal : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private GameObject prefab = null;

        [SerializeField]
        private Material sourceMaterial = null;

        [Header("Values")]
        [SerializeField, Range(0.01f, 3f)]
        private float pathDecalWidth = 0.25f;

        [SerializeField]
        private Color attackColor = Color.red;

        [SerializeField]
        private Color moveColor = Color.black;

        private bool showDecal;

        private bool isMine;

        private Division Division { get; set; }
        private Material Material { get; set; }
        private Projector Projector { get; set; }

        private GameObject Parent { get; set; }
        private ObjectPool<Projector> ObjectPool { get; set; }
        private static Vector3 Up { get; } = new Vector3(0, 100, 0);

        private void Awake()
        {
            Division = GetComponent<Division>();

            NetworkEvents.OnStartMatch += StartObject;
            this.enabled = false;
        }

        private void StartObject()
        {
            isMine = Division.IsMine;
            if (!isMine) return;
            if (!Parent)
            {
                Parent = GameObjectExtensions.FindOrCreateGameObject("DecalPool");
                ObjectPool = new ObjectPool<Projector>(Parent.transform, prefab, 40);
            }

            Material = new Material(sourceMaterial)
            {
                renderQueue = sourceMaterial.renderQueue - 1,
            };

            Division.OnChangedGoal.AddListener(ValidateOrder);

            this.enabled = true;
            NetworkEvents.OnStartMatch -= StartObject;
        }

        private void OnDisable()
        {
            if (!isMine)
            {
                return;
            }

            DisableProjector();
            Material = null;
            Division.OnChangedGoal.RemoveListener(ValidateOrder);
        }

        private void OnDestroy()
        {
            NetworkEvents.OnStartMatch -= StartObject;
            ObjectPool = null;
        }

        private void Update()
        {
            if (!isMine)
            {
                return;
            }
            
            if (!showDecal)
            {
                return;
            }

            CalculateDimensions();
            PositionProjector();
        }

        /// <summary> Calculate projector dimensions in order to render the decal properly on the terrain </summary>
        private void CalculateDimensions()
        {
            float aspectRatio = (1f / pathDecalWidth) *
                Vector3.Distance(Division.transform.position, Division.MoveTarget.Position) /
                2f;
            Projector.aspectRatio = aspectRatio;
        }

        /// <summary> Position and rotates the projector between the division and movetarget </summary>
        private void PositionProjector()
        {
            Vector3 center = Vector3.Lerp(Division.transform.position, Division.MoveTarget.Position, 0.5f);
            Projector.transform.position = center + Up;

            // Calculate direction ourselves because the direction movetarget has, is for positioning.
            Vector3 dir = new Vector3(Division.transform.position.x - Division.MoveTarget.Position.x, 
                0,
                Division.transform.position.z - Division.MoveTarget.Position.z).normalized;
            Quaternion direction = (dir == Vector3.zero) ? Quaternion.identity : Quaternion.LookRotation(dir);
            Projector.transform.rotation = direction * Quaternion.Euler(90, 90, 0);
        }

        /// <summary> Check which order to execute. </summary>
        private void ValidateOrder(DivisionGoal divisionGoal)
        {
            switch (divisionGoal)
            {
                case DivisionGoal.Idle: DisableProjector(); break;
                case DivisionGoal.Move:
                    ActivateProjector();
                    Material.color = moveColor;
                    break;
                case DivisionGoal.PlaceBase:
                    ActivateProjector();
                    Material.color = moveColor;
                    break;
                case DivisionGoal.Attack:
                    ActivateProjector();
                    Material.color = attackColor;
                    break;
            }
        }

        /// <summary> Obtain projector and activate it</summary>
        private void ActivateProjector()
        {
            if (!Projector)
            {
                Projector = ObjectPool.DequeueT();
                Projector.material = Material;
                Projector.orthographicSize = pathDecalWidth;
                showDecal = true;
            }
        }

        /// <summary> Disables the projector and returns it. </summary>
        private void DisableProjector()
        {
            if (Projector)
            {
                Projector.material = null;
                ObjectPool.EnqueueT(Projector);
                Projector = null;
                showDecal = false;
            }
        }
    }
}