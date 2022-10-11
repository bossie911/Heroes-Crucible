using GameStudio.HunterGatherer.Divisions.Upgrades;
using System.Collections.Generic;
using UnityEngine;

public class Station : MonoBehaviour
{
    public UpgradeTypes stationType;
    public List<UpgradeBase> upgrades;
    public int listNmbr;
    [SerializeField]
    private UpgradeStation upgradeStation;
    [SerializeField]
    private GameObject FlagUsed;
    [SerializeField]
    public GameObject StationIcon;
    [SerializeField]
    private GameObject Indicator;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Hero")
        {
            upgradeStation.Trigger(other, listNmbr);
        }
    }
    public void DeactivateDecal()
    {
        FlagUsed.SetActive(false);
        StationIcon.SetActive(false);
        Indicator.SetActive(false);
    }
}
