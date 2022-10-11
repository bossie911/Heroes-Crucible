using System;
using GameStudio.HunterGatherer.CustomEvents;
using GameStudio.HunterGatherer.Divisions;
using GameStudio.HunterGatherer.Networking;
using GameStudio.HunterGatherer.Networking.Events;
using GameStudio.HunterGatherer.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameStudio.HunterGatherer.GodFavor.UI
{
    [System.Serializable]
    public class GodPowerEvent : UnityEvent<GodPowerManager.Gods> { }

    [System.Serializable]
    public class TooltipData
    {
        public string title;
        public string description;

        public TooltipData(string title, string description)
        {
            this.title = title;
            this.description = description;
        }
    }

    public class GodFavorUI : MonoBehaviour
    {
        [Header("Settings")]
        private static GodPowerManager.Gods _currentGod = GodPowerManager.Gods.Ares;
        [SerializeField] private Color aresColor;
        [SerializeField] private Color zeusColor;
        [SerializeField] private Color athenaColor;
        [SerializeField] private int startAmount = 0;
        [SerializeField] private int maxAmount;
        
        [Header("Icons")]
        [SerializeField]
        private Sprite zeusIcon;
        [SerializeField]
        private TooltipData zeusTooltip;
        [SerializeField]
        private Sprite aresIcon;
        [SerializeField]
        private TooltipData aresTooltip;
        [SerializeField]
        private Sprite athenaIcon;
        [SerializeField]
        private TooltipData athenaTooltip;

        private float _amount;
        
        [SerializeField]
        private Image cooldownFiller;

        [SerializeField] private Slider favorChargeSlider;

        public GodPowerEvent OnGodPowerTrigger = new GodPowerEvent();
        public GodPowerEvent OnGodPowerFinished = new GodPowerEvent();
        
        public UnityEventFloat OnGodFavorAmountChanged = new UnityEventFloat();
        
        public float Amount
        {
            get { return _amount; }
            set
            {
                if (Math.Abs(_amount - value) < float.Epsilon)
                    return;

                _amount = value;
                OnGodFavorAmountChanged.Invoke(value);
            }
        }

        public static GodPowerManager.Gods CurrentGod
        {
            get
            {
                return _currentGod;
            }
            set
            {
                //UpdateGodIcon(value);
                _currentGod = value;
            }
        }

        private Image _godIcon;
        public Image GodIcon
        {
            get
            {
                if (_godIcon == null)
                    _godIcon = transform.GetChild(0).GetComponent<Image>();
                return _godIcon;
            }
        }

        private ShowTooltip _godTooltip;
        public ShowTooltip GodToolTip
        {
            get
            {
                if (_godTooltip == null)
                    _godTooltip = GetComponent<ShowTooltip>();
                return _godTooltip;
            }
        }

        public static GodFavorUI Instance { get; private set; }

        private void OnValidate()
        {
            startAmount = startAmount < 0 ? 0 : startAmount > maxAmount ? maxAmount : startAmount;
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogWarning("Singleton GodFavorUI has already been created, deleting object " + gameObject.name);
                Destroy(gameObject);
            }

            CurrentGod = GodPowerManager.Gods.Null;
            UpdateGodIcon(CurrentGod);
            
            OnGodFavorAmountChanged.AddListener(UpdateUI);
            Amount = startAmount;
        }

        /// <summary>
        /// Set's the cooldown filler within the range of 0 to 1
        /// </summary>
        /// <param name="progress"></param>
        public void SetCooldownFiller(float progress)
        {
            cooldownFiller.fillAmount = progress;
        }

        /// <summary>Add given amount of manpower</summary>
        public void AddGodFavor(float amount)
        {
            Amount += amount;
            Amount = Amount < 0 ? 0 : Amount > maxAmount ? maxAmount : Amount;
        }

        /// <summary>Remove given amount of manpower</summary>
        public void RemoveGodFavor(float amount)
        {
            Amount -= amount;
            Amount = Amount < 0 ? 0 : Amount > maxAmount ? maxAmount : Amount;
        }

        /// <summary></summary>
        private void UpdateUI(float chargeAmount)
        {
            favorChargeSlider.value = chargeAmount;
        }

        public void TriggerGodPower()
        {
            GodPowerManager.activeManager?.GodPower(CurrentGod);
        }
        
        /// <summary>
        /// Sets god favor UI icon
        /// </summary>
        /// <param name="god"></param>
        private void UpdateGodIcon(GodPowerManager.Gods god)
        {
            switch (god)
            {
                case GodPowerManager.Gods.Zeus:
                    GodIcon.sprite = zeusIcon;
                    GodToolTip.Title = zeusTooltip.title;
                    GodToolTip.Description = zeusTooltip.description;
                    SetButtonColor(zeusColor);
                    break;
                case GodPowerManager.Gods.Ares:
                    GodIcon.sprite = aresIcon;
                    GodToolTip.Title = aresTooltip.title;
                    GodToolTip.Description = aresTooltip.description;
                    SetButtonColor(aresColor);
                    break;
                case GodPowerManager.Gods.Athena:
                    GodIcon.sprite = athenaIcon;
                    GodToolTip.Title = athenaTooltip.title;
                    GodToolTip.Description = athenaTooltip.description;
                    SetButtonColor(athenaColor);
                    break;
            }
        }

        private void SetButtonColor(Color color)
        {
            GetComponent<Image>().color = color;
        }

        public void SetGod(GodPowerManager.Gods god)
        {
            CurrentGod = god;
            UpdateGodIcon(god);
        }
    }
}