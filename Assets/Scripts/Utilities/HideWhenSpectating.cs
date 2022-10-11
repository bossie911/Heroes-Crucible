using GameStudio.HunterGatherer.Networking;
using UnityEngine;

namespace GameStudio.HunterGatherer.Utilities
{
    /// <summary>Hides the gameobject this component is attached to when the player enters spectator mode</summary>
    public class HideWhenSpectating : MonoBehaviour
    {
        //TODO:Refector
        private void Start()
        {

            //NetworkingPlayerManager.Instance.OnStartSpectating.AddListener(HideWindow);
        }

        /// <summary>Hide the divisionOverview window</summary>
        private void HideWindow()
        {
            gameObject.SetActive(false);
        }
    }
}