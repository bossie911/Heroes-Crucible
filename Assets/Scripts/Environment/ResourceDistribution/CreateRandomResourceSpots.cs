using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using GameStudio.HunterGatherer.Environment;
using System.Linq;

namespace GameStudio.HunterGatherer.Resources
{
    public class CreateRandomResourceSpots : MonoBehaviour
    {
        [Header("Resource Settings")]
        [SerializeField, Tooltip("The resource, which the spawnpoints will be used for")]
        private GameObject resource;

        [SerializeField, Tooltip("The placeholder will be used when converting to spawnpoints")]
        private GameObject placeholderObject;

        [SerializeField, Tooltip("The gameobject that contains the spawnpoints")]
        private GameObject spawnsParent;
        public GameObject[] spawns
        {
            get
            {
                GameObject[] children = new GameObject[spawnsParent.transform.childCount];
                int index = 0;
                return children.Select(x => spawnsParent.transform.GetChild(index++).gameObject).ToArray();
            }
        }

        [Header("Rotation")]
        [SerializeField]
        private bool useCustomRotation = false;
        [SerializeField]
        private Quaternion spawnRotation = Quaternion.identity;
        [SerializeField]
        private bool useRandomRotation = false;
        [SerializeField]
        private Vector3 maxRotationOffset = Vector3.zero;

        [Header("Ground attachment settings")]
        [SerializeField]
        private bool useCustomDist = false;
        [SerializeField]
        private float distanceFromGround = 0;

        [SerializeField, Tooltip("minimum height on map where object can spawn")]
        public int minHeight = 10;

        [SerializeField]
        private int amountOfSpawns = 0;

        [FormerlySerializedAs("RangeSquare")]
        [Header("Range Settings")]
        public RangeSettings rangeSettings;

        [Header("PlacementSettings")]
        [SerializeField]
        private float navMeshSampleDistance = 25f;

        [SerializeField]
        private bool usePoissonDiscSampling = false;
        [SerializeField]
        public int range = 1;
        private int samplesBeforeRejection = 15;
        private List<Vector2> poissonPositions = new List<Vector2>();


        [Header("References")]
        [SerializeField]
        private float maxRayCastLength = 100f;

        /*
        [SerializeField]
        List<GameObject> floatingSpawns;
        */

        /// <summary>Attach the spawpoints to the ground</summary>
        /// <param name="spawnPoints">The spawnpoints to attach the ground</param>
        private void AttachToGround(List<GameObject> spawnPoints)
        {
            for (int i = 0; i < amountOfSpawns; i++)
            {
                Transform spawnPoint = spawnPoints[i].transform;
                if (spawnPoint == null)
                {
                    Debug.LogError("The spawnpoint is null");
                    break;
                }
                //Spawn the resource in the air.
                Vector3 oldPos = spawnPoint.position;
                oldPos.y = 100f;
                spawnPoint.position = oldPos;

                //Check if the resource has terrain below it.
                RaycastHit rayHit;
                if (Physics.Raycast(spawnPoint.position, Vector3.down, out rayHit, maxRayCastLength, LayerMask.GetMask("Terrain")))
                {
                    Vector3 spawnPos = rayHit.point;
                    Vector3 normal = rayHit.normal;

                    //Put the resource on top of the terrain.
                    if (!useCustomDist)
                    {
                        distanceFromGround = resource.transform.localScale.y / 2.0f;
                    }
                    spawnPos = Vector3.MoveTowards(spawnPos, spawnPos + (normal * distanceFromGround), maxRayCastLength);
                    spawnPoint.position = spawnPos;

                    //Rotate the resource towards the normal.
                    if (!useCustomRotation)
                    {
                        spawnRotation = Quaternion.LookRotation(normal);
                    }
                    spawnPoint.rotation = spawnRotation;

                    if (useRandomRotation)
                    {
                        RandomRotate(spawnPoint.gameObject);
                    }
                }
                else
                    Destroy(spawnPoint);
            }
        }

        /// <summary>Destroys spawns created in editor</summary>
        public void DestroyEditorSpawns()
        {
            if (spawns.ToList().Count != 0)
            {
                foreach (GameObject spawn in spawns.ToList())
                {
                    DestroyImmediate(spawn);
                }

                spawns.ToList().Clear();
            }
        }

        /// <summary>Spawns resources in editor like it would normaly when playing the game</summary>
        public void SpawnResourcesInEditor()
        {
            // Creating list of vertices to pick from in PointOnMap
            if (usePoissonDiscSampling == true)
            {
                poissonPositions = PoissonDiscSampling.GeneratePoints(range, new Vector2(rangeSettings.Height, rangeSettings.Width),
                    samplesBeforeRejection);
            }

            Vector3 terrainSize = new Vector3(0, 100, 0);
            for (int i = 0; i < amountOfSpawns; i++)
            {
                GameObject go = Instantiate(resource, GetPointOnMap(terrainSize), Quaternion.identity, spawnsParent.transform);
                go.SetActive(true);
                spawns.ToList().Add(go);
            }

            AttachToGround(spawns.ToList());
        }

        /// <summary>Rotate a gameobject using the random rotation offset</summary>
        /// <param name="go">The gameobject to rotate</param>
        void RandomRotate(GameObject go)
        {
            go.transform.rotation = Quaternion.Euler(
                go.transform.localRotation.eulerAngles.x + Random.Range(-maxRotationOffset.x, maxRotationOffset.x),
                go.transform.localRotation.eulerAngles.y + Random.Range(-maxRotationOffset.y, maxRotationOffset.y),
                go.transform.localRotation.eulerAngles.z + Random.Range(-maxRotationOffset.z, maxRotationOffset.z)
            );
        }

        /// <summary>Attach editor object to ground after user edit.</summary>
        public void RedoGroundAttachmentAfterEdit()
        {
            AttachToGround(spawns.ToList());
        }

        /// <summary>Converts the resource data to only include a spawnpoint and not the original spawn object.</summary>
        public void ConvertToSpawnPoints()
        {
            Vector3[] positions = new Vector3[spawns.ToList().Count];
            Quaternion[] rotations = new Quaternion[spawns.ToList().Count];
            for (int i = 0; i < spawns.ToList().Count; i++)
            {
                positions[i] = spawns.ToList()[i].transform.position;
                rotations[i] = spawns.ToList()[i].transform.rotation;
            }
            DestroyEditorSpawns();
            for (int i = 0; i < positions.Length; i++)
            {
                GameObject go = Instantiate(placeholderObject, positions[i], rotations[i], spawnsParent.transform);
                go.SetActive(true);
                spawns.ToList().Add(go);
            }
        }

        /// <summary>Converts the spawn input object with spawnpoint data to instances of the original spawn object so they can be edited.</summary>
        public void ConvertFromSpawnPoints()
        {
            Transform[] spawns = spawnsParent.transform.GetComponentsInChildren<Transform>();
            Vector3[] positions = new Vector3[spawns.Length];
            Quaternion[] rotations = new Quaternion[spawns.Length];
            for (int i = 0; i < spawns.Length; i++)
            {
                positions[i] = spawns[i].transform.position;
                rotations[i] = spawns[i].transform.rotation;
            }
            for (int i = 1; i < positions.Length; i++)
            {
                GameObject go = Instantiate(resource, positions[i], rotations[i], spawnsParent.transform);
                go.SetActive(true);
            }
        }

        /// <summary>Returns random point on given mapsize based on margin</summary>
        private Vector3 GetPointOnMap(Vector3 mapSize)
        {
            Vector3 position = Vector3.zero;

            if (usePoissonDiscSampling == true && poissonPositions.Count != 0)
            {
                int randomPoint = Random.Range(0, poissonPositions.Count);
                position = new Vector3(rangeSettings.BottomLeftCorner.x, 0, rangeSettings.BottomLeftCorner.y) +
                           new Vector3(poissonPositions[randomPoint].x, mapSize.y, poissonPositions[randomPoint].y) +
                           new Vector3(Random.Range(-4, 4), 0, Random.Range(-4, 4));
                poissonPositions.RemoveAt(randomPoint);
            }
            else
            {
                position = new Vector3(rangeSettings.BottomLeftCorner.x, 0, rangeSettings.BottomLeftCorner.y) +
                           new Vector3(Random.Range(0, rangeSettings.Height), mapSize.y, Random.Range(0, rangeSettings.Width));
            }

            // get full position
            RaycastHit rayHit;
            if (Physics.Raycast(position, Vector3.down, out rayHit, 100f, LayerMask.GetMask("Terrain")))
            {
                position = rayHit.point;
            }

            if (position.y < minHeight)
            {
                return GetPointOnMap(mapSize);
            }

            //clamp to navmesh
            NavMeshHit meshHit;
            if (NavMesh.SamplePosition(position, out meshHit, navMeshSampleDistance, NavMesh.AllAreas))
            {
                position = meshHit.position;
            }
            else
            {
                Debug.LogWarning("Can't clamp object at position: " + position +
                                 " to navmesh :: increase navmeshsampledistance");
            }

            return position;

        }

        /// <summary>Container for a spawnable resource containing the prefab reference and the count</summary>
        [System.Serializable]
        public struct SpawnableResource
        {
#pragma warning disable 0649
            public GameObject Go;
            public int Count;
#pragma warning restore 0649
        }

        /// <summary>Container for rangeSettings</summary>
        [System.Serializable]
        public struct RangeSettings
        {
#pragma warning disable 0649
            public Vector2 BottomLeftCorner;
            public int Width;
            public int Height;
#pragma warning restore 0649
        }
    }
}
