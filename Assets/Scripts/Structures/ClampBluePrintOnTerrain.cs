using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameStudio.HunterGatherer.Networking;
using GameStudio.HunterGatherer.Divisions;
using UnityEngine.AI;

namespace GameStudio.HunterGatherer.Structures
{
    /// <summary> Checks if the base is placable by shooting raycasts on the terrain </summary>
    public class ClampBluePrintOnTerrain : MonoBehaviour
    {
        [SerializeField]
        private static int layerMask = 0;

        [SerializeField]
        private Color baseColor = Color.clear;

        [SerializeField]
        private Color highLightColor = Color.clear;

        [SerializeField]
        private Color isNotPlacableColour = Color.clear;

        [SerializeField]
        private float visionRange = 0;

        [SerializeField]
        private float maxHeightDifference = 0;

        [SerializeField]
        private float maxRayDistance = 0;

        private RaycastHit correctPosHit;
        private bool baseIsBeingPlaced = true;
        private MeshRenderer[] baseMeshRenderers;
        private bool isValidBasePosition;
        private GameObject hero;
        private bool isBaseColliding;
        private float baseWidth;

        public bool IsValidPosition => isValidBasePosition;

        private void Awake()
        {
            baseMeshRenderers = GetComponentsInChildren<MeshRenderer>();
            layerMask = LayerMask.GetMask("Terrain");

            hero = NetworkRoomManager.Instance.CurrentPlayer.GetPlayerDivisionOfType(DivisionType.Hero).gameObject;
            
            baseWidth = 0.5f * GetComponent<Collider>().bounds.size.x;
        }

        private void Start()
        {
            StartCoroutine(IsBasePlacable());
        }

        /// <summary>Corrects the position of each baseobject on the Terrain </summary>
        private void CorrectBaseHeight(MeshRenderer renderer)
        {
            float heightOffset = renderer.bounds.size.y / 2;

            if (Physics.Raycast(renderer.transform.position + Vector3.up * maxRayDistance, Vector3.down, out correctPosHit, maxRayDistance * 2, layerMask))
            {        
                renderer.transform.position = correctPosHit.point + new Vector3(0, heightOffset, 0);
            }
        }

        /// <summary>Checks if given object is on a navmesh </summary>
        public bool IsObjectOnNavMesh(GameObject rendererObject)
        {
            Vector3 rendererPosition = rendererObject.transform.position;

            // Check for nearest point on navmesh to renderer, within raydistance
            if (NavMesh.SamplePosition(rendererPosition, out NavMeshHit hit, maxRayDistance, NavMesh.AllAreas))
            {
                // Check if the positions are vertically aligned
                if (Mathf.Approximately(rendererPosition.x, hit.position.x)
                    && Mathf.Approximately(rendererPosition.z, hit.position.z))
                {
                    // Lastly, check if object is below navmesh
                    return rendererPosition.y >= hit.position.y;
                }
            }
            return false;
        }

        /// <summary>Checks if the base position is viable </summary>
        private IEnumerator IsBasePlacable()
        {
            while (baseIsBeingPlaced)
            {
                float distanceToCliffEdge = float.MaxValue;

                if (NavMesh.FindClosestEdge(transform.position, out NavMeshHit hit, NavMesh.AllAreas))
                {
                    distanceToCliffEdge = hit.distance;
                }

                isValidBasePosition = Vector3.Distance(transform.position, hero.transform.position) <= visionRange;
                baseColor = isValidBasePosition ? highLightColor : isNotPlacableColour;

                foreach (MeshRenderer meshRenderer in baseMeshRenderers)
                {
                    meshRenderer.material.color = baseColor;
                    CorrectBaseHeight(meshRenderer);

                    if (!IsObjectOnNavMesh(meshRenderer.gameObject))
                    {
                        isValidBasePosition = false;
                        baseColor = isNotPlacableColour;
                    }

                    foreach (MeshRenderer renderer in baseMeshRenderers)
                    {
                        if (Mathf.Abs(renderer.transform.position.y - meshRenderer.transform.position.y) >= maxHeightDifference || distanceToCliffEdge < baseWidth || isBaseColliding)
                        {
                            isValidBasePosition = false;
                            baseColor = isNotPlacableColour;
                        }
                    }
                }
                yield return null;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            //When the base collides with other, it will set the bool to true;
            if (other.CompareTag("Base"))
            {
                isBaseColliding = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            //When the base stops colliding with another base , it will set the bool to false;
            if (other.CompareTag("Base"))
            {
                isBaseColliding = false;
            }
        }
    }
}


