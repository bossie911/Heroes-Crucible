namespace GameStudio.HunterGatherer.Networking
{
    /// <summary>The Wrapper class used for representing a room on the server</summary>
    public class Room
    {
        public readonly string Name;
        public readonly byte MaxPlayers;
        public readonly byte PlayerCount;

        /// <summary>Room constructor</summary>
        public Room(string name, byte maxPlayers, byte playerCount)
        {
            Name = name;
            MaxPlayers = maxPlayers;
            PlayerCount = playerCount;
        }
    }
}