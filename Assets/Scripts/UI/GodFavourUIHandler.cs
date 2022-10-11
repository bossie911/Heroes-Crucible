using GameStudio.HunterGatherer.GodFavor.UI;
using UnityEngine;

public class GodFavourUIHandler : MonoBehaviour
{
    /// <summary>
    /// This script is used to call GodFavourUI functions from event triggers that are set in the inspector.
    /// E.g. from prefabs using OnInteract() from the PickUpInteract script.
    /// </summary>

    public void AddGodFavor(float Amount) {
        GodFavorUI.Instance.AddGodFavor(Amount);
    }
}
