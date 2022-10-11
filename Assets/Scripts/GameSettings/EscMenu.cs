using GameStudio.HunterGatherer.GameState;
using GameStudio.HunterGatherer.Networking;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Mirror;
using NetworkRoomManager = GameStudio.HunterGatherer.Networking.NetworkRoomManager;

namespace GameStudio.HunterGatherer.GameSettings
{
    /// <summary>Manages the escape menu and it showing up or hiding based on input</summary>
    public class EscMenu : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private Image imgOverlayBack = null;

        [SerializeField]
        private Button btnDisconnect = null;

        private bool showingOverlay;

        public UnityEvent OnToggleMenu { get; } = new UnityEvent(); //Event to be triggered when the menu is toggled, used by the input system for example

        private void Awake()
        {
            btnDisconnect.onClick.AddListener(Disconnect);
            ApplyOverlayStatus();
        }

        private void Update()
        {
            PlayerState playerState = GameStateManager.Instance is null ? PlayerState.Loading : GameStateManager.Instance.CurrentState;
            // Toggle overlay on ESC if we are playing
            if (Input.GetKeyDown(KeyCode.Escape) && (playerState == PlayerState.Playing || playerState == PlayerState.Spectating))
            {
                ToggleEscapeOverlay();
            }

            // Turn overlay off if showing and game ended
            if (showingOverlay && playerState != PlayerState.Playing && playerState != PlayerState.Spectating)
            {
                ToggleEscapeOverlay();
            }
        }

        /// <summary>Toggle the overlay status</summary>
        private void ToggleEscapeOverlay()
        {
            showingOverlay = !showingOverlay;
            OnToggleMenu.Invoke();
            ApplyOverlayStatus();
        }

        /// <summary>Apply the current overlay status to the overlay</summary>
        private void ApplyOverlayStatus()
        {
            imgOverlayBack.enabled = showingOverlay;
            if (showingOverlay)
            {
                btnDisconnect.gameObject.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Confined;
                btnDisconnect.gameObject.SetActive(false);
            }
        }

        /// <summary>Disconnect from current session</summary>
        private void Disconnect()
        {
            GameStateManager.Instance.SendHeroDeathOnLeave(NetworkRoomManager.LocalPlayerID);
        }
    }
}