
using UnityEngine;

namespace GameStudio.HunterGatherer.Networking
{
	[CreateAssetMenu(fileName = "GameOptions", menuName = "GameOptions", order = 0)]
	public class GameOptions : ScriptableObject
	{
		public int MaximumAmountOfDivisions;
	}
}