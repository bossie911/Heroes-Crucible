using GameStudio.HunterGatherer.Networking;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using NetworkRoomManager = GameStudio.HunterGatherer.Networking.NetworkRoomManager;

namespace GameStudio.HunterGatherer.GameState.UI
{
    /// <summary>Handles the after game UI</summary>
    public class GameStateUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private GameObject txtWin = null;

        [SerializeField]
        private GameObject txtLoss = null;

        [SerializeField]
        private GameObject txtDraw = null;

        [SerializeField]
        private Button btnDisconnect = null;

        [SerializeField]
        private Button btnSpectate = null;

        [SerializeField]
        private TMP_Text txtSpectating = null;

        private void Awake()
        {
            GameStateManager.OnGameStateChanged += UpdateUI;
            btnDisconnect.onClick.AddListener(Disconnect);
            Hide();
        }

        private void OnDestroy()
        {
            GameStateManager.OnGameStateChanged -= UpdateUI;
        }

        /// <summary>Disconnects the client from its current room</summary>
        private void Disconnect()
        {
            Hide();
            GameStateManager.Instance.SendHeroDeathOnLeave(NetworkRoomManager.LocalPlayerID);           
        }
     
        /// <summary>Checks if it should draw the WON, LOST or DRAW UI</summary>
        private void UpdateUI(PlayerState newState)
        {
            switch (newState)
            {
                case PlayerState.Won:
                    txtWin.SetActive(true);
                    btnSpectate.gameObject.SetActive(false);
                    Show();
                    break;

                case PlayerState.Lost:
                    txtLoss.SetActive(true);
                    btnSpectate.gameObject.SetActive(false);
                    Show();
                    break;

                case PlayerState.Draw:
                    txtDraw.SetActive(true);
                    btnSpectate.gameObject.SetActive(false);
                    Show();
                    break;

                case PlayerState.Playing:
                    break;
            }
        }

        /// <summary>Shows the after game UI</summary>
        private void Show()
        {
            gameObject.SetActive(true);
        }

        /// <summary>Hides the after game UI</summary>
        private void Hide()
        {
            txtWin.SetActive(false);
            txtLoss.SetActive(false);
            txtDraw.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}