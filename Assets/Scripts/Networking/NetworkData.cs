using UnityEngine;

namespace GameStudio.HunterGatherer.Networking
{
	[CreateAssetMenu(fileName = "NetworkData", menuName = "NetworkData", order = 0)]
	public class NetworkData : ScriptableObject
	{
		public int maxConnections;
		public int minimumPlayerToStart;
	}
}