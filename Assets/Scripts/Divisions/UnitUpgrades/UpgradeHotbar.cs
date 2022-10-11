using System;
using System.Collections;
using System.Collections.Generic;
using GameStudio.HunterGatherer;
using GameStudio.HunterGatherer.Divisions.UI;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeHotbar : MonoBehaviour
{
    [SerializeField] private List<GameObject> upgradePlaces;
    [SerializeField] private string upgradeDivision = "OnUpgraded";
    [SerializeField] private DivisionOverviewItem DivisionOverviewItem;
    [SerializeField] private List<Image> borders;
    [SerializeField] private Color altColor;
    [SerializeField] private float altColorBorder = 0.7f;

    private void Start()
    {
        //adds listener to upgrading a division and calls set image.
        EventManager.Instance.AddListener(upgradeDivision, upgradeEvent);

        //change color of sprite and borders
        if (DivisionOverviewItem.division.DivisionColor.grayscale > altColorBorder)
        {
            for (int i = 0; i < borders.Count; i++)
            {
                borders[i].color = altColor;
            }


            for (int i = 0; i < upgradePlaces.Count; i++)
            {
                upgradePlaces[i].GetComponent<Image>().color = altColor;
            }
        }
    }

    private void upgradeEvent(EventArgs args)
    {
        {//TODO
            /*UpgradeEventArgs upgradeEventArgs = (UpgradeEventArgs) args;
            if (DivisionOverviewItem.division.NetworkedMovingObject == upgradeEventArgs.NetworkedMovingObject)
            {
                SetImageUpgrade(upgradeEventArgs.Upgrade.UpgradeImage);
            }*/
        }
    }

    /// <summary>
    /// Adds the upgrade sprite to an empty image. and makes the image active.
    /// </summary>
    /// <param name="sprite">the upgrade sprite.</param>
    public void SetImageUpgrade(Sprite sprite)
    {
        foreach (GameObject go in upgradePlaces)
        {
            if (go.GetComponent<Image>().sprite == null)
            {
                Image image = go.GetComponent<Image>();
                image.sprite = sprite;
                go.SetActive(true);
                return;
            }
        }
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(upgradeDivision,upgradeEvent);
    }
}