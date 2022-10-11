using GameStudio.HunterGatherer.Divisions.Upgrades;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using GameStudio.HunterGatherer.Divisions;
using GameStudio.HunterGatherer;
using GameStudio.HunterGatherer.Structures;
using GameStudio.HunterGatherer.Divisions.UI;
using GameStudio.HunterGatherer.FogOfWar;
using Mirror;


//Upgrade types that can be provided by stations in Alphabetical order
public enum UpgradeTypes
{
    Archers,
    Pikemen,
    Swordsmen
}

public class UpgradeStation : StructureInteract
{
    [SerializeField]
    List<Station> stations;
    [SerializeField]
    List<UpgradeBase> Upgrades;
    [SerializeField]
    private Sprite EmptySprite;

    [SerializeField]
    GameObject Fire;
    public int upgradeTypeCount;
    
    [SerializeField] private FogOfWarObject fogOfWarObject;

    private UpgradeFunctionality upgradeFunctionality;

    private Station lastCol;

    private List<UpgradeTypes> types = Enum.GetValues(typeof(UpgradeTypes)).Cast<UpgradeTypes>().ToList();
    private void Start()
    {
        upgradeFunctionality = GameObject.Find("Upgrades").GetComponent<UpgradeFunctionality>();
        AssignUpgradeTypes();
    }
    void OnEnable()
    {
        foreach (var station in stations)
        {
            station.GetComponent<Collider>().enabled = true;
        }
        EventManager.Instance.AddListener(onStructureEnter, UseUpgrade);
    }

    //void  OnDisable()
    //{
    //    EventManager.Instance.RemoveListener(onStructureEnter, UseUpgrade);
    //}

    public void Trigger(Collider other, int listNmbr)
    {
        if (_hasMaxUses && _uses >= _maxUses)
        {
            return;
        }

        if (other.gameObject.tag != "Hero")
            return;

        if (previousCollider != null)
            return;

        Division division = other.transform.root.GetComponent<Unit>().Division;

        if (!division)
            return;

        if (!division.IsMine)
            return;
        
        lastCol = stations[listNmbr];
        EventManager.Instance.Invoke(onStructureEnter);
        previousCollider = division.gameObject;
        DeactiveStationCommand();

        previousCollider = null;
        lastCol = null;
    }

    [Command(requiresAuthority = false)]
    public void DeactiveStationCommand()
    {
        DeactivateStation();
    }

    //get blank stations, assign them a type, if a station detects collision perform an upgrade if possible
    private void AssignUpgradeTypes()
    {
        //Assign a random upgrade to each of the stations in the upgrade station
        for (int i = 0; i < stations.Count; i++)
        {
            stations[i].stationType = RandomizeUpgrades();
            
            var a = Upgrades.Where(x => x.name.Split('_')[0] == stations[i].stationType.ToString()).ToList();
            for (int j = 0; j < upgradeTypeCount; j++)
            {
                stations[i].upgrades.Add(a[j]);
            }
            stations[i].listNmbr = i;
            stations[i].GetComponent<UpgradeStationIcon>().AssignCorrectFlag();
        }

        fogOfWarObject.enabled = true;
    }

    //randomize the upgradeType assigned to each station, removing any that have been picked already to ensure both stations have a different upgrade type
    private UpgradeTypes RandomizeUpgrades()
    {
        UpgradeTypes randomType = types[Random.Range(0, 3)];
        return randomType;
    }

    public void UseUpgrade()
    {
        List<GameObject> list;

        list = GameObject.FindGameObjectsWithTag("Division").Where(x => x.GetComponent<Division>().IsMine && x.GetComponent<Division>().Type.ToString() == lastCol.stationType.ToString()).ToList();
        foreach (var division in list)
        {
            switch (lastCol.stationType)
            {
                case UpgradeTypes.Archers:
                    if (upgradeFunctionality.ArcherLevel < lastCol.upgrades.Count + 1)
                    {
                        upgradeFunctionality.Upgrade(division.GetComponent<Division>(), lastCol.upgrades[upgradeFunctionality.ArcherLevel - 1]);//Each station has its own list of upgrades to ensure each unit gets the right upgrade
                    }
                    break;
                
                case UpgradeTypes.Pikemen:
                    if (upgradeFunctionality.PikemenLevel < lastCol.upgrades.Count + 1)
                    {
                        upgradeFunctionality.Upgrade(division.GetComponent<Division>(), lastCol.upgrades[upgradeFunctionality.PikemenLevel - 1]);
                    }
                    break;
                
                case UpgradeTypes.Swordsmen:
                    if (upgradeFunctionality.SwordsmenLevel < lastCol.upgrades.Count + 1)
                    {
                        upgradeFunctionality.Upgrade(division.GetComponent<Division>(), lastCol.upgrades[upgradeFunctionality.SwordsmenLevel - 1]);
                    }
                    break;
            }
        }
        
        Getitem();
        if (lastCol.stationType == UpgradeTypes.Archers) upgradeFunctionality.ArcherLevel += 1;
        else if (lastCol.stationType == UpgradeTypes.Swordsmen) upgradeFunctionality.SwordsmenLevel += 1;
        else upgradeFunctionality.PikemenLevel += 1;
        
        lastCol = null;
    }

    public void Getitem()
    {
        var DivisionOverviewItemList = DivisionOverview.Instance.transform.Find("ItemList");
        var DivisionOverviewItems = DivisionOverviewItemList.GetComponentsInChildren<DivisionOverviewItem>();

        foreach (var item in DivisionOverviewItems)
        {
            if (item.division.Type.ToString() == lastCol.stationType.ToString())
            {
                var DivisionPanel = item.transform.Find("DivisionPanel");
                var UpgradesByUnit = DivisionPanel.transform.Find("UpgradesByUnit");
                UpgradesByUnit.GetComponent<UpgradeHotbar>().SetImageUpgrade(EmptySprite); ;
            }
        }
    }
    
    [ClientRpc (includeOwner = true)]
    public void DeactivateStation()
    {
        Fire.GetComponent<MeshRenderer>().material.color = Color.gray;
        
        foreach (var station in stations)
        {
            station.DeactivateDecal();
        }
        _hasMaxUses = true;
        _uses++;
        GetComponent<UpgradeStation>().enabled = false;
    }
}
