using System.Collections;
using GameStudio.HunterGatherer.Divisions;
using GameStudio.HunterGatherer.GodFavor;
using UnityEngine;
using GameStudio.HunterGatherer.Networking;
using GameStudio.HunterGatherer.Selection;
using GameStudio.HunterGatherer.GodFavor.UI;
using Mirror;

namespace GameStudio.HunterGatherer.BluePrints
{
    public class GodPowerBluePrintPlacer : BluePrintPlacer
    {
        [Header("General values")]
        private GameObject godPower;
        private Vector3 visualiserPosition;
        private GodPowerManager.Gods god;
        private LayerMask layerMaskGodShield;

        private GodPowerManager m_ActiveManager
        {
            get
            {
                if(!(GodPowerManager.activeManager is null))
                {
                    return GodPowerManager.activeManager;
                }
                if(TryGetComponent<GodPowerManager>(out var comp))
                {
                    return comp;
                }
                return null;
            }
        }

        private void Start()
        {
            layerMaskGodShield = LayerMask.GetMask("GodShield");
        }

        public override void ActivateBluePrintPositioning()
        {
            if (!m_ActiveManager.IsPlacingGodPower)
            {
                god = GodFavorUI.CurrentGod;
                switch (god)
                {
                    case GodPowerManager.Gods.Zeus:
                        highlightPrefab = m_ActiveManager.GodPowerZeusHighLight;
                        break;
                    case GodPowerManager.Gods.Ares:
                        break;
                    case GodPowerManager.Gods.Athena:
                        highlightPrefab = m_ActiveManager.GodPowerAthenaHighlight;
                        break;
                }

                base.ActivateBluePrintPositioning();
                m_ActiveManager.IsPlacingGodPower = true;
            }
        }

        private void Update()
        {
            CancelPlacement();

            if (god == GodPowerManager.Gods.Athena && godPower != null)
            {
                // This can be coded in a cleaner way (observer patern?)
                if(!godPower.activeSelf)
                {
                    godPower = null;
                    return;
                }

                godPower.transform.position = transform.position + Vector3.up * m_ActiveManager.ShieldDistance;
            }
        }

        /// <summary>Determine the position of the blueprint locally</summary>
        public override void HandleBluePrintPosition()
        {
            switch (god)
            {
                case GodPowerManager.Gods.Zeus:
                    base.HandleBluePrintPosition();
                    break;
                case GodPowerManager.Gods.Ares:
                    base.HandleBluePrintPosition();
                    break;
                case GodPowerManager.Gods.Athena:
                    highlight.transform.position = transform.position + Vector3.up * m_ActiveManager.ShieldDistance;
                    if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.G) && m_ActiveManager.IsShowingPowerRange)
                    {
                        ConfirmBluePrintPosition();
                        m_ActiveManager.IsShowingPowerRange = false;
                        m_ActiveManager.IsPlacingGodPower = true;
                    }
                    break;
            }
        }

        /// <summary> Cancel the blue print placement with right mouse click</summary>
        private void CancelPlacement()
        {
            if (Input.GetMouseButtonDown(1))
            {
                SelectionManager.Instance.State = SelectionState.SelectAndInteract;
                DestroyBlueBrint();
                m_ActiveManager.IsShowingPowerRange = false;
            }
        }

        /// <summary> Destroy method that gets added as listener when the base button should deactivate </summary>
        private void DestroyBlueBrint()
        {
            Destroy(highlight);
            m_ActiveManager.IsPlacingGodPower = false;
        }

        /// <summary>Place and instantiate the blueprint in multiplayer</summary>
        protected override void ConfirmBluePrintPosition()
        {
            visualiserPosition = hit.point;
            base.ConfirmBluePrintPosition();

            m_ActiveManager.IsPlacingGodPower = false;
            GodFavorUI.Instance.RemoveGodFavor(m_ActiveManager.GetGodFavorCost(god));
            
            // Trigger god favor event
            GodFavorUI.Instance.OnGodPowerTrigger?.Invoke(god);

            switch (god)
            {
                case GodPowerManager.Gods.Zeus:
                    InstantiateVisualiser(visualiserPosition);
                    break;
                case GodPowerManager.Gods.Ares:
                    break;
                case GodPowerManager.Gods.Athena:
                    StartCoroutine(Delay(m_ActiveManager.GodShieldDelayTime, m_ActiveManager.GodPowerAthena, null, GodPowerManager.Gods.Athena));
                    break;
            }
        }

        [Command(requiresAuthority = false)]
        public void InstantiateVisualiser(Vector3 position)
        {
            var godPowerVisualiser = Instantiate(m_ActiveManager.LightningWarning, position + Vector3.up * 0.5f, m_ActiveManager.LightningWarning.transform.rotation);
            godPowerVisualiser.SetActive(true);
            godPowerVisualiser.GetComponent<GodPowerVisualiser>().scaleTime = m_ActiveManager.LightningBoltDelayTime + 0.1f;
            visualiserPosition = godPowerVisualiser.transform.position;
            NetworkServer.Spawn(godPowerVisualiser);

            StartCoroutine(godPowerVisualiser, visualiserPosition);
        }

        [ClientRpc(includeOwner = true)]
        public void StartCoroutine(GameObject godPowerVisualiser, Vector3 visualiserPosition)
        {
            if (GetComponent<Unit>().IsMine)
            {
                this.visualiserPosition = visualiserPosition;
                StartCoroutine(Delay(m_ActiveManager.LightningBoltDelayTime, m_ActiveManager.GodPowerZeus, godPowerVisualiser, GodPowerManager.Gods.Zeus));
            }
        }
        
        // A short delay before the godpowers get placed
        IEnumerator Delay(float delayTime, GameObject godPowerPrefab, GameObject godPowerVisualiser, GodPowerManager.Gods god)
        {
            yield return new WaitForSeconds(delayTime);

            Vector3 position = Vector3.zero;
            if (godPowerVisualiser)
            {
                position = godPowerVisualiser.transform.position + Vector3.up * godPowerPrefab.transform.localScale.y;
                position = AdjustPosition(position, godPowerVisualiser);
            }
            else
            {
                position = transform.position + Vector3.up * m_ActiveManager.ShieldDistance;
            }
            SpawnObject(god, position);
        }

        private Vector3 AdjustPosition(Vector3 position, GameObject godPowerVisualiser)
        {
            // Adjust the y position of the lightning bolt if there is a godshield
            if (Physics.Raycast(godPowerVisualiser.transform.position, godPowerVisualiser.transform.TransformDirection(Vector3.up), out _, Mathf.Infinity, layerMaskGodShield))
            {
                return transform.position + Vector3.up * m_ActiveManager.ShieldDistance;
            }
            return position;
        }
        
        [Command(requiresAuthority = false)]
        public void SpawnObject(GodPowerManager.Gods god, Vector3 position)
        {
            GameObject prefab = null;
            
            switch (god)
            {
                case GodPowerManager.Gods.Ares:
                    break;
                
                case GodPowerManager.Gods.Athena:
                    prefab = m_ActiveManager.GodPowerAthena;
                    break;
                
                case GodPowerManager.Gods.Zeus:
                    prefab = m_ActiveManager.GodPowerZeus;
                    break;
            }

            if (prefab == null) return;


            GameObject go = Instantiate(prefab, position, Quaternion.identity);
            go.SetActive(true);
            NetworkServer.Spawn(go);
        }
    }
}