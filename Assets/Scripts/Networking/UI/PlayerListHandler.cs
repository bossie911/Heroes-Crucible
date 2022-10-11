using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace GameStudio.HunterGatherer.Networking.UI
{
    /// <summary>
    /// Handles the players just joining the room by providing functionality for players joining, leaving and starting the game
    /// </summary>
    public class PlayerListHandler : MonoBehaviour
    {
        [Serializable]
        private struct PlayerListItem
        {
            public GameObject Root;
            public TMP_Text TxtItemText;
            public GameObject IconPerson;
            public GameObject GodIcon;
        }

        public static PlayerListHandler s_Instance;

        #region Serialized Fields
        [Header("References")]
        [SerializeField] private PlayerListItem[] items = null;

        [SerializeField] private GameObject gameTitle = null;

        [SerializeField] private Button btnPlay = null;

        [SerializeField] private Button btnLeave = null;

        [SerializeField] private Image imgPlayFilled = null;

        [SerializeField] private GameObject multiplayerWindow = null;

        [SerializeField] private LoginFormHandler loginFormHandler = null;

        [SerializeField] private GameObject menuButtons = null;

        [SerializeField] private GameObject selectionScreen = null;

        [SerializeField] private GameObject tutorialScreen = null;
        #endregion

        #region Private Fields
        private int m_MaxPlayers = -1;
        private Player[] players;
        #endregion

        /// <summary>
        /// Initailizes singleton and sets gameobject state to false
        /// </summary>
        private void Awake()
        {
	        if(s_Instance is null)
	        {
                s_Instance = this;
                if(gameObject.activeSelf)
                {
                    gameObject.SetActive(false);
                }
	        }
	        else
	        {               
                Debug.LogError($"Not allowed to have 2 instances of a PlayerListHandler. This instance is being destroyed.");
                Destroy(this.gameObject);
                return;
	        }
        }


        /// <summary>
        /// Binds events and sets initial state of the object.
        /// </summary>
        private void Start()
        {
            btnPlay.onClick.AddListener(OnPlayButtonClick);
            btnLeave.onClick.AddListener(OnLeaveButtonClick);
            NetworkEvents.OnClientDisconnect += OnDisconnected;
            NetworkEvents.OnRoomClientEnter += UpdatePlayers;

            gameObject.SetActive(false);
            tutorialScreen.SetActive(false);
        }

        /// <summary>
        /// Removes listeners form events.
        /// </summary>
        private void OnDestroy()
        {
            btnPlay.onClick.RemoveListener(OnPlayButtonClick);
            btnLeave.onClick.RemoveListener(OnLeaveButtonClick);
            NetworkEvents.OnClientDisconnect -= OnDisconnected;
            NetworkEvents.OnRoomClientEnter -= UpdatePlayers;
        }

        /// <summary>
        /// Enables the right object when the playerlist needs to open and updates the players in the list.
        /// Should be called when a roomplayer is fully initialized.
        /// </summary>
        public void Open()
        {
            selectionScreen.SetActive(true);
            gameObject.SetActive(true);
            multiplayerWindow.SetActive(false);
            menuButtons.SetActive(false);
            gameTitle.SetActive(false);

            //get max player count and player list
            m_MaxPlayers = NetworkRoomManager.Instance.MaxConnections;
            UpdatePlayers();
        }        


        /// <summary>
        /// Updates the playerlist to represent all players with the right nicknames.
        /// </summary>
        private void UpdatePlayers()
        {
            var networkedPlayers = NetworkRoomManager.Instance.connectedPlayers;
            var biggerValue = m_MaxPlayers > networkedPlayers.Length ? m_MaxPlayers : networkedPlayers.Length;

            players = new Player[biggerValue];

            for (var i = 0; i < networkedPlayers.Length; i++)
            {
                players[i] = networkedPlayers[i];
            }

            //update player items based on players in players array
            for (var i = 0; i < biggerValue; i++)
            {
                items[i].Root.SetActive(true);

                if (players[i] != null)
                {
                    items[i].TxtItemText.text = players[i].NickName;
                    items[i].IconPerson.SetActive(true);
                }
                else
                {
                    items[i].TxtItemText.text = "";
                    items[i].IconPerson.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Handles leaving the lobby when the return button is pressed.
        /// </summary>
        public void OnLeaveButtonClick()
        {
            btnLeave.interactable = false;
            NetworkRoomManager.Instance.StopClient();
        }

        /// <summary>
        /// Handles starting the lobby when the play button is pressed.
        /// </summary>
        private void OnPlayButtonClick()
        {
            btnPlay.interactable = false;
            NetworkClient.localPlayer.GetComponent<RoomPlayer>().StartGame();
        }     

        /// <summary>
        /// Handle for when a player leaves the room.
        /// </summary>
        private void OnDisconnected()
        {
            players = null;
            btnLeave.interactable = true;
            btnPlay.interactable = true;
            imgPlayFilled.fillAmount = 0f;

            gameTitle.SetActive(true);
            selectionScreen.SetActive(false);
            gameObject.SetActive(false);
            tutorialScreen.SetActive(false);

            multiplayerWindow.SetActive(true);
            menuButtons.SetActive(true);
            loginFormHandler.Open();
        }

        /// <summary>
        /// Handles the enabling of the playbutton for the first player in the room.
        /// And updates the playes if the username was not yet recieved by the client.
        /// </summary>
        private void FixedUpdate()
        {
            var firstPlayer = NetworkRoomManager.Instance.FirstPlayer;
            btnPlay.gameObject.SetActive(   NetworkRoomManager.LocalPlayerID == 0 && 
                                            NetworkClient.localPlayer.GetComponent<NetworkRoomPlayer>() == firstPlayer &&
                                            NetworkRoomManager.Instance.PlayerCount >= NetworkRoomManager.Instance.MinimumPlayerToStart);
            if (items.Any(x => x.TxtItemText.text == ""))
            {
                UpdatePlayers();
            }
        }
    }
}