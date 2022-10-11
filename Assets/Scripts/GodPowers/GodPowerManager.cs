using System;
using GameStudio.HunterGatherer.Divisions;
using System.Collections;
using UnityEngine;
using GameStudio.HunterGatherer.GodFavor.UI;
using GameStudio.HunterGatherer.Networking;
using GameStudio.HunterGatherer.BluePrints;
using GameStudio.HunterGatherer.UI;
using Mirror;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;

namespace GameStudio.HunterGatherer.GodFavor
{
    public class GodPowerManager : MonoBehaviour
    {
        public static GodPowerManager activeManager = null;

        private GodPowerBluePrintPlacer _godPowerBluePrintPlacer;
        
        [Header("Zeus")]
        public float ZeusFavorCost = 20f;
        [SerializeField]
        private float zeusCooldownDuration = 30f;
        public GameObject GodPowerZeus;
        public GameObject GodPowerZeusHighLight;
        public float LightningBoltDelayTime = 2f;
        public GameObject LightningWarning;

        [Header("Ares")]
        public float AresFavorCost = 20f;
        [SerializeField]
        private float aresCooldownDuration = 30f;
        public float AresPowerRange = 25f;
        public float AresHitChance = 1;
        public float AresPowerDuration = 10;
        [SerializeField] private GameObject AresRangeVisual;

        [Header("Athena")]
        public float AthenaFavorCost = 20f;
        [SerializeField]
        private float athenaCooldownDuration = 30f;
        public GameObject GodPowerAthena;
        public GameObject GodPowerAthenaHighlight;
        public float GodShieldDelayTime = 0.5f;
        public float ShieldDistance = 4f;

        private bool IsMine => GetComponent<Unit>().Division.IsMine;

        private GodPowerBluePrintPlacer GodPowerBluePrintPlacer
        {
            get
            {
                if (_godPowerBluePrintPlacer == null)
                    _godPowerBluePrintPlacer = GetComponent<GodPowerBluePrintPlacer>();
                return _godPowerBluePrintPlacer;
            }
        }

        // God variables
        /// <summary>
        /// God power cost of currently selected god
        /// </summary>
        private float godPowerCost
        {
            get
            {
                return GetGodFavorCost(GodFavorUI.CurrentGod);
            }
        }

        /// <summary>
        /// Gets the god favor cost of a god
        /// </summary>
        /// <param name="god"></param>
        /// <returns></returns>
        public float GetGodFavorCost(Gods god)
        {
            switch (GodFavorUI.CurrentGod)
            {
                case Gods.Zeus:
                    return ZeusFavorCost;
                case Gods.Ares:
                    return AresFavorCost;
                case Gods.Athena:
                    return AthenaFavorCost;
                case Gods.Null:
                    return Mathf.Infinity;
            }

            Debug.LogWarning("No selected god has been found, setting costs infinite");
            return Mathf.Infinity;
        }

        /// <summary>
        /// Cooldown duration of the currently selected god
        /// </summary>
        private float godPowerCooldownDuration
        {
            get
            {
                return GetGodFavorCooldown(GodFavorUI.CurrentGod);
            }
        }

        /// <summary>
        /// Gets the cooldown duration of a god
        /// </summary>
        /// <param name="god"></param>
        /// <returns></returns>
        public float GetGodFavorCooldown(Gods god)
        {
            switch (GodFavorUI.CurrentGod)
            {
                case Gods.Zeus:
                    return zeusCooldownDuration;
                case Gods.Ares:
                    return aresCooldownDuration;
                case Gods.Athena:
                    return athenaCooldownDuration;
            }

            Debug.LogWarning("No selected god has been found, preventing cooldown");
            return 0;
        }

        public bool IsShowingPowerRange = false;

        /// <summary>
        /// Is a god power currently showing its power range?
        /// </summary>
        public bool IsPlacingGodPower;

        /// <summary>
        /// Is a god power currently active (doesn't count showing power range)
        /// </summary>
        public bool IsGodPowerActive => (IsPlacingGodPower || StopGodPowerRoutine != null);

        [SerializeField]
        private string triggerTooltipTitle = "Press G to activate god power";
        [SerializeField]
        private string triggerTooltipDescription = "Press right mouse button to cancel";

        // Networking
        //TODO
        //public NetworkedMovingObject NetworkedMovingObject;

        public enum Gods
        {
            Ares = 0,
            Athena = 1,
            Zeus = 2,
            Null = 3
        }

        private void Start()
        {
            NetworkEvents.OnStartMatch += SetManager;
            
            GodFavorUI.Instance?.OnGodPowerTrigger?.AddListener(OnGodPowerTrigger);
            GodFavorUI.Instance?.OnGodPowerFinished?.AddListener(OnGodPowerFinished);
        }

        private void SetManager()
        {
            if (GetComponent<Unit>().IsMine)
            {
                activeManager = this;
            }
            
            NetworkEvents.OnStartMatch -= SetManager;
        }

        private void OnDestroy()
        {
            NetworkEvents.OnStartMatch -= SetManager;
        }

        void OnGodPowerTrigger(GodPowerManager.Gods god)
        {
            GodPowerCooldownRoutine = StartCoroutine(GodPowerCooldown(godPowerCooldownDuration));

            Tooltip.Instance.Disable(true);
        }


        public bool IsCoolingDown => GodPowerCooldownRoutine != null;
        private Coroutine GodPowerCooldownRoutine = null;

        private IEnumerator GodPowerCooldown(float duration)
        {
            float t = 0;
            do
            {
                t += Time.deltaTime;
                GodFavorUI.Instance.SetCooldownFiller(1 - (t / duration));
                yield return null;
            } while (t < duration);
            GodFavorUI.Instance.SetCooldownFiller(0);

            GodPowerCooldownRoutine = null;
        }

        void OnGodPowerFinished(Gods god) { }

        private void OnDisable()
        {
            if (activeManager == this)
                activeManager = null;
            GodFavorUI.Instance?.OnGodPowerTrigger?.RemoveListener(OnGodPowerTrigger);
            GodFavorUI.Instance?.OnGodPowerFinished?.RemoveListener(OnGodPowerFinished);
        }

        // Update is called once per frame
        void Update()
        {
            CheckInputs();
        }

        void CheckInputs()
        {
            if (Input.GetKeyDown(KeyCode.G) || (IsShowingPowerRange && Input.GetMouseButtonDown(0)))
            {
                if (CanUseGodPower())
                {
                    GodPower(GodFavorUI.CurrentGod);
                }
            }
            if (IsShowingPowerRange && Input.GetMouseButtonDown(1))
            {
                StopGodPower(GodFavorUI.CurrentGod);
            }
        }

        /// <summary>
        /// Can the player trigger the selected god power at this moment?
        /// </summary>
        /// <returns></returns>
        private bool CanUseGodPower()
        {
            return !IsGodPowerActive && IsMine && !IsCoolingDown && GodFavorUI.Instance.Amount >= godPowerCost;
        }

        /// <summary>
        /// Trigger god power if the player is able to
        /// </summary>
        /// <param name="currentGod"></param>
        public void GodPower(Gods currentGod)
        {
            if (StopGodPowerRoutine != null)
            {
                StopCoroutine(StopGodPowerRoutine);
                StopGodPowerRoutine = null;
            }

            Tooltip.Instance.Setup(triggerTooltipTitle, triggerTooltipDescription, true);

            // Trigger any special needs
            switch (currentGod)
            {
                case Gods.Zeus:
                    GodPowerBluePrintPlacer.ActivateBluePrintPositioning();
                    break;
                case Gods.Ares:
                    if (IsShowingPowerRange)
                    {
                        // Actually activate effect
                        GodFavorUI.Instance.OnGodPowerTrigger?.Invoke(currentGod);
                        StopGodPowerRoutine = StartCoroutine(StopGodPower(AresPowerDuration));
                        GodFavorUI.Instance.RemoveGodFavor(AresFavorCost);
                        IsShowingPowerRange = false;
                        AresRangeVisual.gameObject.SetActive(false);
                    }
                    else
                    {
                        // Show range of effect
                        IsShowingPowerRange = true;
                        AresRangeVisual.transform.localScale = new Vector3(AresPowerRange * 2,
                            AresPowerRange * 2, AresPowerRange * 2);
                        AresRangeVisual.gameObject.SetActive(true);
                    }

                    break;
                case Gods.Athena:
                    GodPowerBluePrintPlacer.ActivateBluePrintPositioning();
                    IsShowingPowerRange = true;
                    break;
            }
        }


        private Coroutine StopGodPowerRoutine = null;
        private IEnumerator StopGodPower(float time)
        {
            yield return new WaitForSeconds(time);
            StopGodPower(GodFavorUI.CurrentGod);
            StopGodPowerRoutine = null;
        }

        /// <summary>
        /// Cancel god power either when it is active or when it is showing range
        /// </summary>
        /// <param name="currentGod"></param>
        public void StopGodPower(Gods currentGod)
        {
            // Trigger god favor event
            if (IsGodPowerActive)
            {
                if (StopGodPowerRoutine != null)
                {
                    StopCoroutine(StopGodPowerRoutine);
                    StopGodPowerRoutine = null;
                }
                AresRangeVisual.gameObject.SetActive(false);
                GodFavorUI.Instance.OnGodPowerFinished?.Invoke(currentGod);
            }

            if (IsShowingPowerRange)
            {
                IsShowingPowerRange = false;
                AresRangeVisual.gameObject.SetActive(false);
                Tooltip.Instance.Disable(true);
            }
        }
    }
}