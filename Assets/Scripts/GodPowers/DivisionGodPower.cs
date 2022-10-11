using System;
using GameStudio.HunterGatherer.Divisions;
using GameStudio.HunterGatherer.GodFavor.UI;
using GameStudio.HunterGatherer.Networking;
using Mirror;
using UnityEngine;

namespace GameStudio.HunterGatherer.GodFavor
{
    [RequireComponent(typeof(Division))]
    public class DivisionGodPower : NetworkBehaviour
    {
        private Division _division;
        private GodPowerManager.Gods currentlyActiveGod;

        public Division Division
        {
            get
            {
                if (_division == null)
                    _division = GetComponent<Division>();
                return _division;
            }
        }
        
        [SerializeField] private GameObject aresDivisionVisualPrefab;
        [SyncVar] private GameObject aresDivisionVisual;

        private bool isPowerSet = false;
        private float previousHitChance;

        void OnEnable()
        {
            NetworkEvents.OnStartMatch += Setup;
        }

        private void OnDestroy()
        {
            NetworkEvents.OnStartMatch -= Setup;
        }

        private void Setup()
        {
            if (!Division.IsMine)
                return;

            if (GodFavorUI.Instance == null)
            {
                if (!NetworkServer.active)
                {
                    Debug.LogWarning("GodFavorUI script is missing in scene, disabling god favor functionality.");
                }
                enabled = false;
                return;
            }
            Division.OnDestroyDivision.AddListener(OnDivisionDisable);
            GodFavorUI.Instance.OnGodPowerTrigger.AddListener(OnGodPowerTrigger);
            GodFavorUI.Instance.OnGodPowerFinished.AddListener(OnGodPowerFinished);

            NetworkEvents.OnStartMatch -= Setup;
        }

        void OnGodPowerTrigger(GodPowerManager.Gods god)
        {
            SetPower(god);
        }

        void OnGodPowerFinished(GodPowerManager.Gods god)
        {
            ResetPower(god);
        }

        void OnDisable()
        {
            if (!Division.IsMine)
                return;
            

            if (GodFavorUI.Instance != null)
            {
                GodFavorUI.Instance.OnGodPowerTrigger.RemoveListener(OnGodPowerTrigger);
                GodFavorUI.Instance.OnGodPowerFinished.RemoveListener(OnGodPowerFinished);
            }

            if (Division != null)
                Division.OnDestroyDivision.RemoveListener(OnDivisionDisable);
        }

        void OnDivisionDisable(Division division)
        {
            ResetPower(GodFavorUI.CurrentGod, true);
        }

        /// <summary>
        /// Sets god power effect and stores base stats
        /// </summary>
        /// <param name="god"></param>
        private void SetPower(GodPowerManager.Gods god)
        {
            if (isPowerSet)
                return;
            currentlyActiveGod = god;

            switch (god)
            {
                case GodPowerManager.Gods.Zeus:
                    break;
                case GodPowerManager.Gods.Ares:
                    previousHitChance = Division.TypeData.HitChance;
                    Division.TypeData.HitChance = GodPowerManager.activeManager.AresHitChance;
                    
                    SpawnObject();
                    
                    isPowerSet = true;
                    break;
                case GodPowerManager.Gods.Athena:
                    break;
                case GodPowerManager.Gods.Null:
                    break;
            }
        }

        /// <summary>
        /// Resets god power effect to base stats
        /// </summary>
        /// <param name="god"></param>
        private void ResetPower(GodPowerManager.Gods god, bool nullify = true)
        {
            if (god != currentlyActiveGod)
                ResetPower(currentlyActiveGod, nullify);
            if (!isPowerSet)
                return;
            
            switch (god)
            {
                case GodPowerManager.Gods.Zeus:
                    break;
                case GodPowerManager.Gods.Ares:
                    Division.TypeData.HitChance = previousHitChance;
                    DestroyObject(aresDivisionVisual);
                    isPowerSet = false;
                    if (nullify)
                    {
                       // aresDivisionVisual = null;
                    }
                    break;
                
                case GodPowerManager.Gods.Athena:
                    break;
                
                case GodPowerManager.Gods.Null:
                    break;
            }
        }

        private void LateUpdate()
        {
            if (!Division.IsMine)
                return;

            if (aresDivisionVisual != null)
            {
                UpdateAresPosition();
            }

            if (GodPowerManager.activeManager == null)
                return;

            if (!GodPowerManager.activeManager.IsGodPowerActive || (GodPowerManager.activeManager.IsGodPowerActive && GodPowerManager.activeManager.IsPlacingGodPower))
                return;

            if (CheckRange())
            {
                SetPower(currentlyActiveGod);
            }
            else
            {
                ResetPower(currentlyActiveGod, false);
            }
        }

        [Command(requiresAuthority = false)]
        public void UpdateAresPosition()
        {
            aresDivisionVisual.transform.position = transform.position + Vector3.up;
        }

        /// <summary>
        /// Checks range of currently active god power and disables it's effect when out of range
        /// </summary>
        /// <returns></returns>
        private bool CheckRange()
        {
            float distance = Vector3.Distance(transform.position, GodPowerManager.activeManager.transform.position);
            switch (currentlyActiveGod)
            {
                case GodPowerManager.Gods.Zeus:
                    break;
                case GodPowerManager.Gods.Ares:
                    if (distance < GodPowerManager.activeManager.AresPowerRange)
                        return true;
                    else
                        return false;
                case GodPowerManager.Gods.Athena:
                    break;
                case GodPowerManager.Gods.Null:
                    break;
            }

            return false;
        }

        private void OnDrawGizmos()
        {
            if (GodPowerManager.activeManager == null)
                return;

            if (GodPowerManager.activeManager.IsGodPowerActive && isPowerSet)
            {
                switch (GodFavorUI.CurrentGod)
                {
                    case GodPowerManager.Gods.Zeus:
                        break;
                    case GodPowerManager.Gods.Ares:
                        Gizmos.color = Color.red;
                        Gizmos.DrawWireSphere(transform.position, 2);
                        break;
                    case GodPowerManager.Gods.Athena:
                        break;
                }
            }
        }

        [Command(requiresAuthority = false)]
        public void SpawnObject()
        {
            var go = Instantiate(aresDivisionVisualPrefab, Vector3.zero, Quaternion.identity);
            go.SetActive(true);
            NetworkServer.Spawn(go);
            aresDivisionVisual = go;
        }
        
        [Command(requiresAuthority = false)]
        public void DestroyObject(GameObject go)
        {
            if (go != null && go.activeInHierarchy)
            {
                NetworkServer.Destroy(go);
            }
        }
    }
}