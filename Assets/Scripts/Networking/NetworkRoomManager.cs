using System.Linq;
using GameStudio.HunterGatherer.GameState;
using kcp2k;
using Mirror;
using UnityEngine;

namespace GameStudio.HunterGatherer.Networking
{
    /// <summary>
    /// Handles all thing considering the networkroom and the connected players.
    /// </summary>
	public class NetworkRoomManager : Mirror.NetworkRoomManager
	{
        #region Static Properties
        public static NetworkRoomManager Instance => singleton as NetworkRoomManager;
        public static int LocalPlayerID { get; set; }
        #endregion

        #region Public Fields
        public int PlayersEnteredGameScene;
        public int SuccesfullLoadedPlayers; 

        public bool m_IsServer = false;
        #endregion

        #region Serialize Fields
        [SerializeField] private NetworkData NetworkData;
        [SerializeField] private GameOptions GameOptions;
        #endregion

        #region Public Properties
        public int MaxConnections => NetworkData.maxConnections;
        public int MinimumPlayerToStart => NetworkData.minimumPlayerToStart;
        public int PlayerCount => roomSlots.Count;
        public int MaximumAmountOfDivisions => GameOptions.MaximumAmountOfDivisions;
        public Player[] connectedPlayers => roomSlots.Select(x => new Player(x.index, x.GetComponent<RoomPlayer>().playerName, LocalPlayerID == x.index, false)).ToArray();
        public NetworkRoomPlayer FirstPlayer => roomSlots.OrderBy(x => x.index).FirstOrDefault();
        public RoomPlayer CurrentPlayer => roomSlots.FirstOrDefault(x => x.index == LocalPlayerID)?.GetComponent<RoomPlayer>();
        #endregion

        public override void Awake()
        {
            #if UNITY_SERVER
            m_IsServer = true;
            #endif
            var args = System.Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length - 1; i++)
            {
                var arg = args[i];
                if (arg == "-port")
                {
                    if (!ushort.TryParse(args[i + 1], out var port)) continue;
                    GetComponent<KcpTransport>().Port = port;
                }
                if (arg == "-server")
                {
                    m_IsServer = true;
                }
            }
            if (m_IsServer)
            {
                Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.ScriptOnly);
                Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.ScriptOnly);
                Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.ScriptOnly);
                Application.SetStackTraceLogType(LogType.Assert, StackTraceLogType.ScriptOnly);
                Application.SetStackTraceLogType(LogType.Exception, StackTraceLogType.ScriptOnly);
            }
            maxConnections = NetworkData.maxConnections;
            base.Awake();
        }

        public override void Start()
        {
            if (m_IsServer)
            {
                StartServer();
            }
            #if UNITY_SERVER
            if (autoStartServerBuild)
            {
                StartServer();
            }
            #endif
        }

        public override void OnRoomClientDisconnect()
        {
            Debug.Log($"Player left, PlayerCount:{PlayerCount}");
            NetworkEvents.ClientDisconnect();
        }

        public override void OnRoomClientEnter()
        {
            Debug.Log($"Player entered room, PlayerCount:{PlayerCount}");
            NetworkEvents.RoomClientEnter();
        }

        /// <summary>
        /// This is called on the server when all the players in the room are ready.<br/>
        /// Override checks for minimumplayes to start.
        /// </summary>
        public override void OnRoomServerPlayersReady()
        {
            Debug.Log($"All players ready starting game with {PlayerCount} players");
            if (PlayerCount < MinimumPlayerToStart) return;
            Debug.Log($"loading game scene");
            ServerChangeScene(GameplayScene);
        }

        /// <summary>
        /// <inheritdoc/>
        /// After disconnetcting all players it resets some values.
        /// </summary>
        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            if (conn.identity != null)
            {
                NetworkRoomPlayer roomPlayer = conn.identity.GetComponent<NetworkRoomPlayer>();

                if (roomPlayer != null)
                    roomSlots.Remove(roomPlayer);

                foreach (NetworkIdentity clientOwnedObject in conn.clientOwnedObjects)
                {
                    roomPlayer = clientOwnedObject.GetComponent<NetworkRoomPlayer>();
                    if (roomPlayer != null)
                        roomSlots.Remove(roomPlayer);
                }
                GameStateManager.Instance?.UpdatePlayerStates();
            }

            allPlayersReady = false;

            foreach (NetworkRoomPlayer player in roomSlots)
            {
                if (player != null)
                    player.GetComponent<NetworkRoomPlayer>().readyToBegin = false;
            }

            if (IsSceneActive(RoomScene))
                RecalculateRoomPlayerIndices();

            OnRoomServerDisconnect(conn);
            
            
            NetworkServer.DestroyPlayerForConnection(conn);

            if(roomSlots.Count <= 0)
            {
                LocalPlayerID = -1;
                PlayersEnteredGameScene = 0;
                SuccesfullLoadedPlayers = 0;
                StopServer();
                StartServer();
            }
        }

        /// <summary>
        /// Sets roomplayer object as player object.
        /// </summary>
        public override GameObject OnRoomServerCreateGamePlayer(NetworkConnectionToClient conn, GameObject roomPlayer)
        {
            return roomPlayer;
        }
	}
}