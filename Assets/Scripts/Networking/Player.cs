using System;

namespace GameStudio.HunterGatherer.Networking
{
    /// <summary>The Wrapper class used for representing a networking client</summary>
    public class Player : IEquatable<Player>
    {
        public readonly int PlayerID;
        public readonly string NickName;
        public readonly bool IsLocal;
        public readonly bool IsMasterClient;

        public Player(int playerID, string nickName, bool isLocal, bool isMasterClient)
        {
            this.PlayerID = playerID;
            this.NickName = nickName;
            this.IsLocal = isLocal;
            this.IsMasterClient = false;
        }

        public static bool operator ==(Player lhs, Player rhs)
        {
            // Check for null on left side.
            if (Object.ReferenceEquals(lhs, null))
            {
                if (Object.ReferenceEquals(rhs, null))
                {
                    // null == null = true.
                    return true;
                }

                // Only the left side is null.
                return false;
            }
            // Equals handles case of null on right side.
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Player lhs, Player rhs)
        {
            return !(lhs == rhs);
        }

        public bool Equals(Player p)
        {
            // If parameter is null, return false.
            if (Object.ReferenceEquals(p, null))
            {
                return false;
            }

            // Optimization for a common success case.
            if (Object.ReferenceEquals(this, p))
            {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (this.GetType() != p.GetType())
            {
                return false;
            }

            return PlayerID == p.PlayerID;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Player);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return PlayerID.GetHashCode();
            }
        }
    }
}