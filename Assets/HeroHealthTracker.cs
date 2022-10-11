using GameStudio.HunterGatherer.Divisions;
using GameStudio.HunterGatherer.Divisions.UnitBehaviours;
using GameStudio.HunterGatherer.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroHealthTracker : MonoBehaviour
{
    [SerializeField]
    private Slider healthbar;
    private Division HeroDivision;

    public void Perp(Division hero)
    {
        HeroDivision = hero.gameObject.GetComponent<Division>();

        SetMaxHealth();
        HeroDivision.onHealthChanged.AddListener(UpdateHealth);
    }
    private void UpdateHealth(float healthValue)
    {
        healthbar.value = healthValue;
    }
    private void SetMaxHealth()
    {
        healthbar.maxValue = HeroDivision.GetDivisionHealth(true);
        healthbar.value = healthbar.maxValue;
    }
}
