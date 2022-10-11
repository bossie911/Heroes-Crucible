using System;
using GameStudio.HunterGatherer.Divisions;
using GameStudio.HunterGatherer.Networking;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace GameStudio.HunterGatherer.Divisions.UI
{
    /// <summary> Handles the healthbar in the division selection UI </summary>
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Image bar = null;
        [SerializeField] private Color color;
        [SerializeField] private GameObject normalBorder = null;
        [SerializeField] private GameObject heroBorder = null;
        private Division division;
        private RectTransform healthRectTransform;
        private float maxHealthPoints = 10;
        private float maxWidth;
        private float previousHealth;

        public UnityEvent onHeal;

        /// <summary> Sets the health values of the healthbar to the health values of the hero </summary>
        public void SetHealth(float health)
        {
            if (health < 0)
                return;

            previousHealth = healthRectTransform.sizeDelta.x * maxHealthPoints;
            float healthPercent = health / maxHealthPoints;

            healthRectTransform.sizeDelta = new Vector2(maxWidth * healthPercent, healthRectTransform.rect.height);
        }

        private void OnHeal()
        {
            onHeal?.Invoke();
        }


        /// <summary>
        /// updates healthbar to max health.
        /// </summary>
        public void UpdateMaxHealth()
        {
            maxHealthPoints = division.GetDivisionHealth(true);
            SetHealth(maxHealthPoints);
        }

        /// <summary> Initializes the start values of the healthbar </summary>
        public void Initialize(Division division)
        {
            this.division = division;
            division.OnDivisionTypeChanged.AddListener(UpdateMaxHealth);
            division.onHealthChanged.AddListener(SetHealth);
            division.onHeal.AddListener(OnHeal);


            healthRectTransform = bar.GetComponent<RectTransform>();
            maxWidth = healthRectTransform.rect.width;
            UpdateUnitCount(division.Units.Count);
            SetHealthBarColor(color);

            if (division.Type == DivisionType.Hero)
            {
                EnableHeroHealthBar();
            }

            
        }

        /// <summary> Updated the healthbar correctly when new units are added. </summary>
        public void UpdateUnitCount(int unitCount)
        {
            if (unitCount <= 0)
                return;
            maxHealthPoints = division.GetDivisionHealth(true);
        }

        public void SetHealthBarColor(Color col)
        {
            bar.color = col;
        }

        private void EnableHeroHealthBar()
        {
            if (normalBorder)
            {
                normalBorder.SetActive(false);
            }

            if (heroBorder)
            {
                heroBorder.SetActive(true);
            }
        }

        private void OnEnable()
        {
            if (division)
            {               
                division.OnDivisionTypeChanged.AddListener(UpdateMaxHealth);
            }
        }

        private void OnDisable()
        {
            division.OnDivisionTypeChanged.RemoveListener(UpdateMaxHealth);
        }
    }
}