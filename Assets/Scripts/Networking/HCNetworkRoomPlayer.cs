using System.Collections;
using System.IO;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameStudio.HunterGatherer.Networking
{
	public class HCNetworkRoomPlayer : NetworkRoomPlayer
	{
		public override void IndexChanged(int oldIndex, int newIndex)
		{
			if(!isLocalPlayer) return;
			NetworkRoomManager.LocalPlayerID = index;
		}
	}
}