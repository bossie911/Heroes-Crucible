using GameStudio.HunterGatherer.Divisions;
using GameStudio.HunterGatherer.Divisions.Decals;
using GameStudio.HunterGatherer.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameStudio.HunterGatherer.Selection
{
    /// <summary>Formation decal shown when dragging the mouse to give a move order</summary>
    public class FormationDragDecal : MonoBehaviour
    {
        [SerializeField]
        private Material sourceMaterial = null;

        [SerializeField]
        private Material sourceDirectionMaterial = null;

        [SerializeField]
        private GameObject projectorPrefab = null;

        [SerializeField]
        private float divisionSpacing = 2f;

        [SerializeField]
        private float unitDecalSize = 1.2f;

        [SerializeField]
        private LayerMask terrainMask;

        private float rotationAngle;
        private float angle = 0;
        private float radius = 0;

        private ObjectPool<Projector> ObjectPool { get; set; }
        private List<DivisionProjectorObject> DivisionProjectors { get; set; } = new List<DivisionProjectorObject>();
        public bool Active { get => gameObject.activeInHierarchy; set => gameObject.SetActive(value); }

        private Vector3 direction;

        private void Awake()
        {
            Active = false;
            if (ObjectPool == null)
            {
                ObjectPool = new ObjectPool<Projector>(this.transform, projectorPrefab, 20);
            }
        }

        private void OnDestroy() => ObjectPool = null;

        /// <summary> Creates a direction and division formation projector for all the divisions. </summary>
        public void CreateProjectors(IEnumerable<Division> divisions)
        {
            foreach (Division division in divisions)
            {
                CreateProjector(division);
            }
            PositionProjector();
            Active = true;
        }

        /// <summary> Creates a direction and division formation projector. And stores these, together with the division in the list. </summary>
        private void CreateProjector(Division division)
        {
            // Exit if any projector already exists.
            if (DivisionProjectors.Any(x => x.Division == division))
            {
                return;
            }

            Projector divisionProjector = ObjectPool.DequeueT();
            Projector directionProjector = ObjectPool.DequeueT();

            Material divisionMaterial = new Material(sourceMaterial) { color = division.DivisionColor };
            Material directionMaterial = new Material(sourceDirectionMaterial) { color = division.DivisionColor };

            divisionProjector.material = divisionMaterial;
            directionProjector.material = directionMaterial;

            DivisionProjectors.Add(new DivisionProjectorObject
            {
                DivisionProjector = divisionProjector,
                DirectionProjector = directionProjector,
                Division = division,
            });
        }

        /// <summary> Removes all the DivisionProjectorObjects from the list </summary>
        public void RemoveAllProjectors()
        {
            DivisionProjectors.ForEach(dp =>
            {
                ObjectPool.EnqueueT(dp.DivisionProjector);
                ObjectPool.EnqueueT(dp.DirectionProjector);
            });
            DivisionProjectors.Clear();
            Active = false;
        }

        /// <summary> Applies the moveorder to all selected divisions. </summary>
        public void ApplyMoveOrders(float terrainHeight)
        {
            foreach (var dp in DivisionProjectors)
            {
                Vector3 worldPos = dp.DivisionProjector.transform.position;
                worldPos.y = terrainHeight;
                dp.Division.MoveOrder(worldPos, dp.DivisionProjector.transform.up);
            }
        }

        /// <summary> Position the projectors within the parent, and find shortest paths. <summary>
        public void DefaultPositionProjector()
        {
            DivisionProjectors = DivisionProjectors.Where(p => p.Division != null).ToList();
            // Calculate TotalWidth and Average Division Position
            float totalWidth = 0f;
            Vector3 averagePosition = Vector3.zero;
            DivisionProjectors.ForEach(p =>
            {
                totalWidth += FormationLayout.GenerateRect(p.Division.Units.Count, p.Division.FormationRatio, p.Division.UnitSpacing).width;
                averagePosition += p.Division.transform.position; 
            });
            totalWidth += divisionSpacing * (DivisionProjectors.Count - 1);
            averagePosition /= DivisionProjectors.Count;
            float halfWidth = totalWidth / 2f;

            // Seperate the placement line in even segments
            Vector3 leftCorner = transform.position - transform.right * halfWidth;
            Vector3 rightCorner = transform.position + transform.right * halfWidth;
            Vector3[] divisionApproximateLocations = new Vector3[DivisionProjectors.Count];
            for (int i = 0; i < divisionApproximateLocations.Length; i++)
            {
                float percentage = 1f / (divisionApproximateLocations.Length - 1) * i;
                divisionApproximateLocations[i] = Vector3.Lerp(leftCorner, rightCorner, percentage);
            }

            // Sort Divisions and DivisionPlacementPoints, and create a list to start making couples.
            DivisionProjectorObject[] closestToFurthestDivisions = DivisionProjectors.OrderBy(p => Vector3.Distance(p.Division.transform.position, averagePosition)).ToArray();
            Vector3[] closestToFurthestLocations = divisionApproximateLocations.OrderBy(x => Vector3.Distance(x, transform.position)).ToArray();
            (int Index, DivisionProjectorObject DivisionProjectObject)[] positionOrder = new (int, DivisionProjectorObject)[closestToFurthestDivisions.Length];

            // If it's uneven, assign the first variable to the center and increment starting index
            int startingIndex = 0;
            if (closestToFurthestDivisions.Length % 2 != 0)
            {
                startingIndex = 1;

                // Integer substracting is always rounded down. 9 / 2 = floor(4.5) = 4.
                // index 4 in array of size 9 is the center.
                // 0,1,2,3, [4] ,5,6,7,8
                positionOrder[0] = (
                    divisionApproximateLocations.Length / 2,
                    closestToFurthestDivisions[0]
                );
            }

            for (int i = startingIndex; i < closestToFurthestDivisions.Length; i += 2)
            {
                var divisionA = closestToFurthestDivisions[i];
                var divisionB = closestToFurthestDivisions[i + 1];

                Vector3 pointA = closestToFurthestLocations[i];
                Vector3 pointB = closestToFurthestLocations[i + 1];

                // Find shortest
                if (Vector3.Distance(divisionA.Division.transform.position, pointA) +
                    Vector3.Distance(divisionB.Division.transform.position, pointB) <
                    Vector3.Distance(divisionA.Division.transform.position, pointB) +
                    Vector3.Distance(divisionB.Division.transform.position, pointA))
                {
                    // A to A, B to B
                    positionOrder[i] = (Array.IndexOf(divisionApproximateLocations, pointA), divisionA);
                    positionOrder[i + 1] = (Array.IndexOf(divisionApproximateLocations, pointB), divisionB);
                }
                else
                {
                    // A to B, B to A
                    positionOrder[i] = (Array.IndexOf(divisionApproximateLocations, pointA), divisionB);
                    positionOrder[i + 1] = (Array.IndexOf(divisionApproximateLocations, pointB), divisionA);
                }
            }
            // Might be unnecessary.
            positionOrder = positionOrder.OrderBy(x => x.Index).ToArray();

            // Calculate true positions starting from left most side, working it's way to the right.
            float leftEdge = -halfWidth;
            foreach (var order in positionOrder)
            {
                Rect divisionRect = FormationLayout.GenerateRect(
                    order.DivisionProjectObject.Division.Units.Count, 
                    order.DivisionProjectObject.Division.FormationRatio, 
                    order.DivisionProjectObject.Division.UnitSpacing);
                float offset = leftEdge + (divisionRect.width / 2f);

                // Assign positions
                order.DivisionProjectObject.DivisionProjector.transform.localPosition = new Vector3(offset, 100, 0);
                order.DivisionProjectObject.DivisionProjector.transform.localEulerAngles = new Vector3(90, 0, 0);
                order.DivisionProjectObject.DirectionProjector.transform.localEulerAngles = new Vector3(90, 0, 0);
                // Update the projector width and height based on the amount of units in the division
                order.DivisionProjectObject.DivisionProjector.orthographicSize = (divisionRect.height + unitDecalSize) / 2f;
                order.DivisionProjectObject.DivisionProjector.aspectRatio = (divisionRect.width + unitDecalSize) / (divisionRect.height + unitDecalSize);

                // Update the directionProjector position
                order.DivisionProjectObject.DirectionProjector.transform.position = order.DivisionProjectObject.DivisionProjector.transform.position +
                    transform.forward * (order.DivisionProjectObject.DivisionProjector.orthographicSize + DivisionWaypointDecal.DirectionSpacing);

                // Update leftCorner for next iteration
                leftEdge += divisionRect.width + divisionSpacing;
            }
        }

        public void LockedPositionProjector()
        {
            DivisionProjectors = DivisionProjectors.Where(p => p.Division != null).ToList();
            Vector3 targetPosition = transform.position;

            foreach (DivisionProjectorObject divisionProjector in DivisionProjectors)
            {
                FormationLock.DivisionLockInfo lockInfo = FormationLock.Instance.divisionLockInfo[divisionProjector.Division];

                Vector3 offset = lockInfo.offset;
                Vector3 heightOffset = new Vector3(0, 100, 0);
                Vector3 finalPosition = offset + heightOffset;
                Rect divisionRect = FormationLayout.GenerateRect(
                    divisionProjector.Division.Units.Count,
                    divisionProjector.Division.FormationRatio,
                    divisionProjector.Division.UnitSpacing
                    );

                divisionProjector.DivisionProjector.transform.localPosition = finalPosition;
                divisionProjector.DivisionProjector.transform.localEulerAngles = new Vector3(90, 0, 0);
                divisionProjector.DirectionProjector.transform.localEulerAngles = new Vector3(90, 0, 0);
                // Update the projector width and height based on the amount of units in the division
                divisionProjector.DivisionProjector.orthographicSize = (divisionRect.height + unitDecalSize) / 2f;
                divisionProjector.DivisionProjector.aspectRatio = (divisionRect.width + unitDecalSize) / (divisionRect.height + unitDecalSize);

                // Update the directionProjector position
                divisionProjector.DirectionProjector.transform.position = divisionProjector.DivisionProjector.transform.position +
                divisionProjector.DivisionProjector.transform.up * (divisionProjector.DivisionProjector.orthographicSize + DivisionWaypointDecal.DirectionSpacing);
            }
        }

        public void PositionProjector()
        {
            if (FormationLock.Instance.formationLocked)
            {
                LockedPositionProjector();
            }
            else
            {
                DefaultPositionProjector();
            }
        }

        public void UpdatePosition(Vector3 position)
        {
            if (!Active)
            {
                return;
            }
            transform.position = position;
        }

        /// <summary>Place decal at given position, aiming at given endposition</summary>
        public void UpdateDragDecal(Vector3 position, Vector3 endPosition)
        {
            if (!Active)
            {
                return;
            }

            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(cameraRay, out hit, Mathf.Infinity, terrainMask))
            {
                direction = cameraRay.GetPoint(hit.distance);
            }

            transform.position = position;

            transform.LookAt(new Vector3(direction.x, transform.position.y, direction.z));
        }

        //This returns the angle in radians
        public float AngleInRad(Vector3 vec1, Vector3 vec2)
        {
            return Mathf.Atan2(vec2.y - vec1.y, vec2.x - vec1.x);
        }

        //This returns the angle in degress
        public float AngleInDeg(Vector3 vec1, Vector3 vec2)
        {
            return AngleInRad(vec1, vec2) * 180 / Mathf.PI;
        }
    }
}