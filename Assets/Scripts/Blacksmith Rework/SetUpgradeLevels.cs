using GameStudio.HunterGatherer.Divisions;
using GameStudio.HunterGatherer.Divisions.Upgrades;
using GameStudio.HunterGatherer.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SetUpgradeLevels : MonoBehaviour
{
    private UpgradeFunctionality upgradefunctionality;
    
    [SerializeField, Range(0,3)]
    private int archerLevel, pikemenLevel, swordsmenLevel;

    [SerializeField]
    private List<UpgradeBase> archerUpgrades;

    [SerializeField]
    private List<UpgradeBase> pikemenUpgrades;

    [SerializeField]
    private List<UpgradeBase> swordsmenUpgrades;

    private List<GameObject> divisionlist;
    private bool upgraded = false;

    private void Start()
    {
        upgradefunctionality = GameObject.Find("Upgrades").GetComponent<UpgradeFunctionality>();
        if (GameObject.FindGameObjectsWithTag("Division").Where(x => x.GetComponent<Division>().IsMine) != null)
        {
            divisionlist = GameObject.FindGameObjectsWithTag("Division").Where(x => x.GetComponent<Division>().IsMine).ToList();
            UnitUpgrader();
        }

    }
    
    private void UnitUpgrader()
    {
        GiveArchersUpgrades();
        GivePikemenUpgrades();
        GiveSwordsmenUpgrades();
        upgraded = true;
    }

    private void GiveArchersUpgrades()
    {
        List<GameObject> archers = divisionlist.Where(x => x.GetComponent<Division>().Type.ToString() == "Archers").ToList();
        foreach (var division in archers)
        {
            for (int i = 0; i < archerLevel; i++)
            {
                upgradefunctionality.Upgrade(division.GetComponent<Division>(), archerUpgrades[upgradefunctionality.ArcherLevel - 1]);
                upgradefunctionality.ArcherLevel += 1;
            }
        }
    }
    private void GivePikemenUpgrades()
    {
        List<GameObject> pikemen = divisionlist.Where(x => x.GetComponent<Division>().Type.ToString() == "Pikemen").ToList();
        foreach (var division in pikemen)
        {
            for (int i = 0; i < pikemenLevel; i++)
            {
                upgradefunctionality.Upgrade(division.GetComponent<Division>(), pikemenUpgrades[upgradefunctionality.PikemenLevel - 1]);
                upgradefunctionality.PikemenLevel += 1;
            }
        }
    }
    private void GiveSwordsmenUpgrades()
    {
        List<GameObject> swordsmen = divisionlist.Where(x => x.GetComponent<Division>().Type.ToString() == "Pikemen").ToList();
        foreach (var division in swordsmen)
        {
            for (int i = 0; i < swordsmenLevel; i++)
            {
                upgradefunctionality.Upgrade(division.GetComponent<Division>(), swordsmenUpgrades[upgradefunctionality.SwordsmenLevel - 1]);
                upgradefunctionality.SwordsmenLevel += 1;
            }
        }
    }
}
