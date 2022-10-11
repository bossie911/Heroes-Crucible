using UnityEngine;

namespace GameStudio.HunterGatherer.Utilities
{
    /// <summary>Toggles the activeInHierarchy of a given gameobject</summary>
    public class ToggleActive : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private GameObject objectToToggle = null;

        /// <summary>Toggle activeInHierarchy for objectToToggle</summary>
        public void Toggle()
        {
            objectToToggle.SetActive(!objectToToggle.activeInHierarchy);
        }

        /// <summary>Deactivate objectToToggle</summary>
        public void Deactivate()
        {
            objectToToggle.SetActive(false);
        }
    }
}
