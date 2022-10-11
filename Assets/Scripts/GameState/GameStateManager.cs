using GameStudio.HunterGatherer.Networking;
using GameStudio.HunterGatherer.Networking.Events;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using GameStudio.HunterGatherer.GameTime;
using UnityEngine;
using NetworkRoomManager = GameStudio.HunterGatherer.Networking.NetworkRoomManager;

namespace GameStudio.HunterGatherer.GameState
{
    /// <summary>Handles the state of the player (e.g. if it is player, lost or won)</summary>
    public class GameStateManager : NetworkBehaviour
    {
        [Header("Settings")]
        [SerializeField, Tooltip("The amount of miliseconds between players that die can be for it to be a draw")]
        private double drawMargin = 80;


        private bool soloPlay;

        public static GameStateManager Instance { get; private set; }

        public PlayerState CurrentState => PlayerStates.ContainsKey(NetworkRoomManager.LocalPlayerID) ? PlayerStates[NetworkRoomManager.LocalPlayerID] : PlayerState.Loading;

        public readonly SyncDictionary<int, PlayerState> PlayerStates = new SyncDictionary<int, PlayerState>();

        public double DrawMargin { get { return drawMargin; } }

        public static event Action<PlayerState> OnGameStateChanged;

        private void Awake()
        {
            Instance = this;
        }

        public void Start()
        {
            if (!isServer) return;
            NetworkEvents.OnStartMatch += PlayersLoaded;
            NetworkEvents.OnHeroDeath += CheckWinState;
        }

        public override void OnStartClient()
        {
            PlayerStates.Callback += PlayerStatesChange;
        }

        private void PlayerStatesChange(SyncIDictionary<int, PlayerState>.Operation op, int key, PlayerState state)
        {
            if(key == NetworkRoomManager.LocalPlayerID)OnGameStateChanged?.Invoke(state);
            if(PlayerStates.Values.Contains(PlayerState.Won)) GameTimeManager.Instance.StopGameTime();
        }

        private void OnDestroy()
        {
            if (!isServer) return;
            NetworkEvents.OnStartMatch -= PlayersLoaded;
            NetworkEvents.OnHeroDeath -= CheckWinState;
            Instance = null;
        }

        /// <summary>Sets the current state to PLAYING when all players are loaded</summary>
        private void PlayersLoaded()
        {
            foreach (var player in NetworkRoomManager.Instance.roomSlots)
            {
                PlayerStates.Add(player.index, PlayerState.Playing);
            }
        }

        /// <summary>Checks if someone is the last player alive</summary>
        public void CheckWinState(int playerID)
        {

            //get states of all players and filter them on players playing
            if (PlayerStates.ContainsKey(playerID))
            {
                PlayerStates[playerID] = PlayerState.Lost;
            }
            var playersPlaying = PlayerStates.Where(state => state.Value == PlayerState.Playing).ToList();
            if (playersPlaying.Count == 1)
            {
                Debug.Log($"setting player: {playersPlaying[0].Key} state to won");
                PlayerStates[playersPlaying[0].Key] = PlayerState.Won;
            }
        }
        
        public void UpdatePlayerStates()
        {
	        foreach (var playersToRemove in PlayerStates.Keys.Where(x => NetworkRoomManager.Instance.roomSlots.Select(y => y.index).ToList().Contains(x)))
	        {
                PlayerStates.Remove(playersToRemove);
	        }
            var playersPlaying = PlayerStates.Where(state => state.Value == PlayerState.Playing).ToList();
            if (playersPlaying.Count == 1)
            {
                Debug.Log($"setting player: {playersPlaying[0].Key} state to won");
                PlayerStates[playersPlaying[0].Key] = PlayerState.Won;
            }
        }


        [Command(requiresAuthority = false)]
        public void SendHeroDeathOnLeave(int value, NetworkConnectionToClient conn = null)
        {
            NetworkEvents.HeroDeath(value);
            DisconnectAfterSettingLoseState(conn);
        }

        [TargetRpc]
        private void DisconnectAfterSettingLoseState(NetworkConnection target)
        {
            NetworkRoomManager.Instance.StopClient();
        }
    }
}