using System.Collections;
using UnityEngine;
using GameStudio.HunterGatherer.Divisions;
using GameStudio.HunterGatherer.Networking;
using GameStudio.HunterGatherer.Selection;
using GameStudio.HunterGatherer.Structures;
using Mirror;

namespace GameStudio.HunterGatherer.BluePrints
{
    /// <summary> Script handling mechanic where the player chooses a position their object </summary>
    public class BluePrintPlacer : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField]
        protected GameObject highlightPrefab = null;

        [SerializeField]
        protected string nameOfHighLightedPrefab = "HighLightedBase";
        protected RaycastHit hit;
        protected Division heroDivision;

        [Header("Settings")]
        [SerializeField]
        private float maxRayDistance = 0;

        private bool isPlacingBluePrint;
        private int layerMask;
        protected GameObject highlight;

        private void Awake()
        {
            layerMask = LayerMask.GetMask("Terrain", "FogOfWar");
        }

        /// <summary> Method called to start choosing a position for the blueprint </summary>
        public virtual void ActivateBluePrintPositioning()
        {
            SelectionManager.Instance.State = SelectionState.PlacingStructure;
            //todo heroDivision = NetworkingPlayerManager.Instance.HeroDivision.GetComponent<Division>();
            highlight = Instantiate(highlightPrefab, Camera.main.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity);
            StartCoroutine(ChooseBluePrintPosition());
        }

        /// <summary> IEnumerator for placing the blueprint on the terrain </summary>
        private IEnumerator ChooseBluePrintPosition()
        {
            isPlacingBluePrint = true;

            while (isPlacingBluePrint && highlight != null)
            {
                HandleBluePrintPosition();
                yield return null;
            }
        }

        /// <summary>Determine the position of the blueprint locally</summary>
        public virtual void HandleBluePrintPosition()
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, maxRayDistance, layerMask))
            {
                highlight.transform.position = hit.point;

                if (highlight.GetComponent<ClampBluePrintOnTerrain>().IsValidPosition)
                {
                    if (Input.GetMouseButtonDown(0) || (this is GodPowerBluePrintPlacer && Input.GetKeyDown(KeyCode.G)))
                    {
                        ConfirmBluePrintPosition();
                    }
                }
            }
        }

        /// <summary>Place and instantiate the blueprint in multiplayer</summary>
        protected virtual void ConfirmBluePrintPosition()
        {
            isPlacingBluePrint = false;
            SelectionManager.Instance.State = SelectionState.SelectAndInteract;
            Destroy(highlight);
        }
    }
}
