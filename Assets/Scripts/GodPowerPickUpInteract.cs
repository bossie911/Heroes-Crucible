using GameStudio.HunterGatherer.GodFavor;
using GameStudio.HunterGatherer.GodFavor.UI;
using UnityEngine;

public class GodPowerPickUpInteract : MonoBehaviour
{
    public void SetRandomGodPower()
    {
        GodFavorUI.Instance.SetGod((GodPowerManager.Gods)Random.Range(0, System.Enum.GetValues(typeof(GodPowerManager.Gods)).Length - 1));
    }

    public void SetGodPower(GodPowerManager.Gods god)
    {
        GodFavorUI.Instance.SetGod(god);
    }
}
