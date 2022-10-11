using System.Collections.Generic;
using GameStudio.HunterGatherer.Networking;
using GameStudio.HunterGatherer.Selection;
using GameStudio.HunterGatherer.Utilities;
using UnityEngine;

namespace GameStudio.HunterGatherer.Divisions.Decals
{
    /// <summary> DivisionUnitDecals manages all the highlight decals for each unit when selected. </summary>
    [RequireComponent(typeof(Division))]
    public class DivisionUnitDecals : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private GameObject prefab = null;
        [SerializeField]
        private Material sourceMaterial = null;

        public Division Division { get; private set; }
        public Material DivisionMaterial { get; private set; }
        public float OrthographicSize { get; private set; } = 1.2f;
        private List<Projector> DecalProjectors { get; set; } = new List<Projector>();
        private List<Unit> Units => Division.VisibleUnits;
        private SelectableObject SelectableObject => Division.SelectableObject;

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
            if (!Division.IsMine) return;
            if (!Parent)
            {
                Parent = GameObjectExtensions.FindOrCreateGameObject("DecalPool");
                ObjectPool = new ObjectPool<Projector>(Parent.transform, prefab, 40);
            }

            OrthographicSize = prefab.GetComponent<Projector>().orthographicSize;
            SelectableObject.OnSelect.AddListener(Show);
            SelectableObject.OnDeselect.AddListener(Hide);
            DivisionMaterial = new Material(sourceMaterial)
            {
                color = Division.DivisionColor
            };

            this.enabled = true;
            NetworkEvents.OnStartMatch -= StartObject;
        }
        

        private void OnDisable()
        {
            if (!Division.IsMine) return;
            SelectableObject.OnSelect.RemoveListener(Show);
            SelectableObject.OnDeselect.RemoveListener(Hide);
            DivisionMaterial = null;
        }

        private void OnDestroy()
        {
            ObjectPool = null;
            NetworkEvents.OnStartMatch -= StartObject;
        }

        private void Update()
        {
            if (!Division.IsMine) return;

            if (Division.IsSelected)
            {
                if (DecalProjectors.Count < Units.Count)
                {
                    AddProjectors(Units.Count - DecalProjectors.Count);
                }
                else if (DecalProjectors.Count > Units.Count)
                {
                    RemoveProjectors(DecalProjectors.Count - Units.Count);
                }

                PositionAndRotateProjectors();
            }
        }

        /// <summary> Called by OnSelect. Retrieves projectors for each unit </summary>
        public void Show()
        {
            AddProjectors(Units.Count);
        }

        /// <summary> Called by OnDeselect. Returns all projectors </summary>
        public void Hide()
        {
            RemoveProjectors(DecalProjectors.Count);
        }

        /// <summary> Adds the given amount of projectors. </summary>
        private void AddProjectors(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                Projector projector = ObjectPool.DequeueT();
                projector.material = DivisionMaterial;
                DecalProjectors.Add(projector);
            }
        }

        /// <summary> Removes the given amount of projectors. </summary>
        private void RemoveProjectors(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                Projector projector = DecalProjectors[0];
                ObjectPool.EnqueueT(projector);
                DecalProjectors.RemoveAt(0);
            }
        }

        /// <summary> Positions and rotates all projectors. </summary>
        private void PositionAndRotateProjectors()
        {
            for (int i = 0; i < DecalProjectors.Count; i++)
            {
                DecalProjectors[i].transform.position = Units[i].transform.position + Up;
                DecalProjectors[i].transform.rotation = Quaternion.LookRotation(Division.MoveTarget.Direction) * Quaternion.Euler(90, 0, 0);
            }
        }
    }
}