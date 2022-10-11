using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameStudio.HunterGatherer.Divisions;
using GameStudio.HunterGatherer.Networking.UI;
using GameStudio.HunterGatherer.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using GameStudio.HunterGatherer.Divisions.UI;

namespace GameStudio.HunterGatherer.Networking
{
	public class RoomPlayer : NetworkBehaviour
	{
        #region Synced Fields
        [SyncVar] public string playerName = "";
		public readonly SyncList<Division> PlayerDivisions = new SyncList<Division>();

        [SyncVar] private bool AllPlayersEnteredAndReady;
        #endregion

        #region Seriaize Fields
        [SerializeField] private Division prefab;
        [SerializeField] private List<DivisionType> m_SpawningDivision = new List<DivisionType>();
        #endregion

        private void OnEnable()
		{
			SceneManager.sceneLoaded += OnSceneLoaded;
		}

		private void OnDisable()
		{
			SceneManager.sceneLoaded -= OnSceneLoaded;
		}
		
		private void Update()
		{
			if (PlayerListHandler.s_Instance != null && isLocalPlayer && !string.IsNullOrEmpty(playerName) && !PlayerListHandler.s_Instance.gameObject.activeSelf)
			{
				PlayerListHandler.s_Instance.Open();
			}
		}

		public override void OnStartLocalPlayer()
        {
			StartCoroutine(WaitTillReady());			
		}

        private IEnumerator WaitTillReady()
		{
			while (!NetworkClient.ready || !NetworkClient.active)
			{
				yield return null;
			}
			AfterReady();
		}

		private void AfterReady()
		{
			var username = LoginFormHandler.s_Username;
			playerName = username;
			ChangePlayerName(username);
			var networkRoomPlayer = GetComponent<NetworkRoomPlayer>();
			NetworkRoomManager.LocalPlayerID = networkRoomPlayer.index;
		}

		[Command]
		private void ChangePlayerName(string newPlayerName)
        {
			playerName = newPlayerName;
		}

		public void StartGame()
        {
			StartGameCMD();
        }

		[Command]
        private void StartGameCMD()
        {
			NetworkRoomManager.Instance.allPlayersReady = true;
		}

        //Game related functions
        
		/// <summary>
		/// Handle what needs to be done when gameplayscene loads for current player
		/// </summary>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
			if (scene.name == Path.GetFileNameWithoutExtension(NetworkRoomManager.Instance.GameplayScene) && GetComponent<NetworkRoomPlayer>().index == NetworkRoomManager.LocalPlayerID)
			{
				StartCoroutine(AfterGameSceneLoad());
			}
        }

		/// <summary>
		/// Handles what needs to be done after gameplayscene loads for current player
		/// </summary>
		private IEnumerator AfterGameSceneLoad()
        {
	        //Check if the player has entered the scene
	        while (!NetworkClient.ready || !NetworkClient.active || NetworkClient.isLoadingScene ||!SceneManager.GetActiveScene().isLoaded)
	        {
		        yield return null;
	        }
			
	        //When player has completely loaded scene, set up player in the scene.
	        PlayerEnteredGameScene();
	        
	        while (!AllPlayersEnteredAndReady)
	        {
		        yield return null;
	        }

			SpawnDivisions();
			
			//wait till all divisions are added to PlayerDivsions
			while (PlayerDivisions.Count < m_SpawningDivision.Count + 1)//+1 for hero
			{
				yield return null;
			}

			//setup divions and spawn unit
			foreach (var division in PlayerDivisions)
			{
				if (!DivisionOverview.Instance.divisionItems.ContainsKey(division))
				{
					NetworkEvents.DivisionCreated(division);
				}
				StartCoroutine(division.Setup());
				for (var j = 0; j < division.TypeData.MaxUnitCount; j++)
				{
					division.SpawnUnit();
				}
			}

			//wait till all units are added to division.Units
			foreach (var division in PlayerDivisions)
			{
				while (division.Units.Count < division.TypeData.MaxUnitCount)
				{
					yield return null;
				}
			}

			//setup units
			foreach (var division in PlayerDivisions)
	        {
		        foreach (var unit in division.Units)
		        {
			        unit.IsMine = division.IsMine;
		        }
				division.hadUnits = true;
		        division.MoveOrder(division.transform.position, Vector3.forward);
			}	        

	        CheckConnectedPlayers();
        }

        private void SpawnDivisions()
		{			
			var unitSpawnPoint = GameObject.Find("UnitSpawnPoints").transform.GetChild(NetworkRoomManager.LocalPlayerID).position;
			References.Instance.cameraController.SetPosition(unitSpawnPoint);

			SpawnHero(unitSpawnPoint, Quaternion.identity, NetworkRoomManager.LocalPlayerID);
			for (var i = 0; i < m_SpawningDivision.Count; i++)
			{
				var spawnpoint = new Vector3(unitSpawnPoint.x, 0, unitSpawnPoint.z);
				SpawnDivision(spawnpoint, Quaternion.identity, m_SpawningDivision[i], NetworkRoomManager.LocalPlayerID);
			}
		}

		/// <summary>
		/// /// <strong>This method is a [Command] so this code is ran on the server and can and should only be called from clients!</strong><br/>
		/// Creates a new division of type: <strong><paramref name="type"/></strong> at given position with given rotation.<br/>
		/// Adds this new division to PlayerDivisions
		/// </summary>
		/// <param name="conn">Should not be passed in.</param>
		[Command]
		private void SpawnDivision(Vector3 position, Quaternion rotation, DivisionType type, int ownerID, NetworkConnectionToClient conn = null)
		{
			var division = Instantiate(prefab, position, rotation);
			division.OwnerID = ownerID;
			division.gameObject.SetActive(true);
			division.SetType(type);

			NetworkServer.Spawn(division.gameObject, conn);
			
			PlayerDivisions.Add(division);
		}


		/// <summary>
		/// <strong>This method is a [Command] so this code is ran on the server and can and should only be called from clients!</strong><br/>
		/// Creates a new division of type: <strong>Hero</strong> at given position with given rotation.<br/>
		/// Adds this new division to PlayerDivisions
		/// </summary>
		/// <param name="conn">Should not be passed in.</param>
		[Command]
		private void SpawnHero(Vector3 position, Quaternion rotation, int ownerID, NetworkConnectionToClient conn = null)
		{
			var division = Instantiate(prefab, position, rotation);
			division.OwnerID = ownerID;
			division.gameObject.SetActive(true);
			division.SetType(DivisionType.Hero);

			NetworkServer.Spawn(division.gameObject, conn);
			PlayerDivisions.Add(division);
		}

		[Command]
		public void CheckConnectedPlayers()
		{
			NetworkRoomManager.Instance.SuccesfullLoadedPlayers++;
			if (NetworkRoomManager.Instance.SuccesfullLoadedPlayers >= NetworkRoomManager.Instance.connectedPlayers.Length)
			{	
				RPCStartGamePlayer();
				NetworkEvents.StartMatch();
			}
		}
		
		[ClientRpc(includeOwner = true)]
		public void RPCStartGamePlayer()
		{
			NetworkEvents.StartMatch();
			References.Instance.SelectionManager.enabled = true;
			References.Instance.CameraInput.enabled = true;
			References.Instance.GameTimeManager.isEnabled = true;
		}
		
		/// <summary>
		/// Check if all players have loaded into the scene (divisions not important) and when all players have loaded the scene, call the event that spawns resources.
		/// </summary>
		[Command]
		public void PlayerEnteredGameScene()
		{
			NetworkRoomManager.Instance.PlayersEnteredGameScene++;
			if (NetworkRoomManager.Instance.PlayersEnteredGameScene >= NetworkRoomManager.Instance.connectedPlayers.Length)
			{
				foreach (var roomPlayer in NetworkRoomManager.Instance.roomSlots.Select((x) => x.GetComponent<RoomPlayer>()))
				{
					roomPlayer.AllPlayersEnteredAndReady = true;
				}

				NetworkEvents.AllPlayersEnteredScene();
			}
		}
		
		public Division GetPlayerDivisionOfType(DivisionType type)
		{
			return PlayerDivisions.FirstOrDefault(x => x.Type == type);
		}
	}
}