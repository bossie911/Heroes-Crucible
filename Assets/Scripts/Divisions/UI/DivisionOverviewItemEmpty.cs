using GameStudio.HunterGatherer.Networking;
using GameStudio.HunterGatherer.Structures;
using UnityEngine;
using UnityEngine.UI;

namespace GameStudio.HunterGatherer.Divisions.UI
{
    /// <summary>Handles an empty slot in the DivisionOverview, displaying a plus when the hero is in the base to add a new division</summary>
    public class DivisionOverviewItemEmpty : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private Button btnAddDivision = null;

        [SerializeField]
        private float divisionSpawnMaxOffset = 5f;

        private Vector3 spawnPosition;
        private Image[] btnImages;

        private void Awake()
        {
            btnImages = btnAddDivision.GetComponentsInChildren<Image>();
            btnAddDivision.onClick.AddListener(SpawnDivision);
        }

        /// <summary>Spawn division at the set spawn position</summary>
        private void SpawnDivision()
        {
            Vector3 spawnPosition = this.spawnPosition + new Vector3(Random.Range(-divisionSpawnMaxOffset, divisionSpawnMaxOffset), 0, Random.Range(-divisionSpawnMaxOffset, divisionSpawnMaxOffset));
            //todo NetworkingPlayerManager.Instance.SpawnDivision(spawnPosition, Quaternion.identity);
        }

        /// <summary> Set the interactable state of the add division buttons </summary>
        public void SetButtonState(bool state)
        {
            Color editBtnColor = (state) ? Color.white : Color.red;


            foreach (Image i in btnImages)
            {
                i.color = editBtnColor;
            }
        }
    }
}