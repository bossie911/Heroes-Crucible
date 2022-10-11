using UnityEngine;

namespace GameStudio.HunterGatherer.Divisions.Upgrades
{
    /// <summary>
    /// ScriptableObject Holding all modifiers an Upgrade could have.
    /// TODO: When implementing more buffs: Make a ScriptableObject that inherits from this class, but with duration and cost variables.
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerStatsUpgrade", menuName = "ScriptableObjects/PlayerStatsUpgrade", order = 8)]
    public class PlayerStatsUpgrade : ScriptableObject
    {
        [Range(1,5)]
        public int SwordsmenLevel = 1;

        [Range(1, 5)]
        public int ArcherLevel = 1;

        [Range(1, 5)]
        public int PikemenLevel = 1;





    }
}