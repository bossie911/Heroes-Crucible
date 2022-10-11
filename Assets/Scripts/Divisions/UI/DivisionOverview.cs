using GameStudio.HunterGatherer.CustomEvents;
using GameStudio.HunterGatherer.Networking;
using GameStudio.HunterGatherer.Networking.Events;
using GameStudio.HunterGatherer.Structures;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameStudio.HunterGatherer.Divisions.UI
{
    /// <summary>Handles the UI element displaying the player's divisions</summary>
    public class DivisionOverview : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private DivisionOverviewItem prefabOverviewItem = null;

        [SerializeField]
        private DivisionOverviewItemEmpty prefabOverviewItemEmpty = null;

        [SerializeField]
        private RectTransform overviewItemParent = null;

        public Dictionary<Division,DivisionOverviewItem> divisionItems = new Dictionary<Division, DivisionOverviewItem>();
        List<DivisionOverviewItemEmpty> emptyDivisionSlots = new List<DivisionOverviewItemEmpty>();
        private bool _enemyInBase;
        private bool _heroInBase;

        private int indexID = 0;

        public static DivisionOverview Instance { get; private set; }

        public bool HeroIsInBase
        {
            get
            {
                return _heroInBase;
            }
            set
            {
                _heroInBase = value;
            }
        }
        public bool EnemyIsInBase
        {
            get
            {
                return _enemyInBase;
            }
            set
            {
                if (_enemyInBase != value)
                {
                    _enemyInBase = value;
                }
            }
        }

        private void Start()
        {
            if (Instance == null)
            {
                Instance = this;
                NetworkEvents.OnDivisionCreated += AddDivisionItem;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            NetworkEvents.OnDivisionCreated -= AddDivisionItem;
        }

        /// <summary>Create a new item for the given division and add it to the display</summary>
        private void AddDivisionItem(Division division)
        {
            // Guard clause to return if an item for the given division already exists
            if (divisionItems.ContainsKey(division) || division.OwnerID != NetworkRoomManager.LocalPlayerID) //TODO: manpower amount check doesn't belong here
            {
                return;
            }

            DivisionOverviewItem newItem = Instantiate(prefabOverviewItem, overviewItemParent).GetComponent<DivisionOverviewItem>();
            newItem.Setup(division);
            newItem.id = indexID++;
            divisionItems.Add(division, newItem);
            division.OnDestroyDivision.AddListener(RemoveDivisionItem);

            // Reenter empty items
            for (int i = emptyDivisionSlots.Count - 1; i >= 0; i--)
            {
                DivisionOverviewItemEmpty emptyItem = emptyDivisionSlots[i];
                emptyDivisionSlots.Remove(emptyItem);
                Destroy(emptyItem.gameObject);
            }
            int amountOfEmptyDivisions = NetworkRoomManager.Instance.MaximumAmountOfDivisions - NetworkRoomManager.Instance.CurrentPlayer.PlayerDivisions.Count;
            
            for (int i = 0; i < amountOfEmptyDivisions; i++)
            {
                DivisionOverviewItemEmpty emptyItem = Instantiate(prefabOverviewItemEmpty, overviewItemParent).GetComponent<DivisionOverviewItemEmpty>();
                emptyDivisionSlots.Add(emptyItem);
            }
            //int amountOfManpowerToRemove = 1;
            //todo NetworkEvents.AddDivisionItem(amountOfManpowerToRemove);

            //Binding the division overviewItem to the division
            division.overviewItem = newItem;
        }

        /// <summary>Remove the item belonging to the given division from the display and destroy it</summary>
        private void RemoveDivisionItem(Division division)
        {
            // Guard clause to return if no item exists for the given division
            if (!divisionItems.ContainsKey(division))
            {
                return;
            }

            division.OnDestroyDivision.RemoveListener(RemoveDivisionItem);

            DivisionOverviewItem item = divisionItems[division];
            divisionItems.Remove(division);
            Destroy(item.gameObject);
           
            // Add new empty item to replace division
            DivisionOverviewItemEmpty emptyItem = Instantiate(prefabOverviewItemEmpty, overviewItemParent).GetComponent<DivisionOverviewItemEmpty>();
            emptyDivisionSlots.Add(emptyItem);
        }
        
    }
}