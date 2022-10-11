using GameStudio.HunterGatherer.Divisions;
using System;
using System.Collections.Generic;

namespace GameStudio.HunterGatherer.Networking.Events
{
    /// <summary>Decribes a change of the god favour amount for a client</summary>
    public struct GodFavorResourceAddedInfo
    {
        public int CollectorViewID;
        public int GodFavorPickupID;
        public float Amount;
        
        /// <summary>Used for serializing this object to be send over the network</summary>
        public static byte[] Serialize(object obj)
        {
            GodFavorResourceAddedInfo info = (GodFavorResourceAddedInfo)obj;
            List<byte> bytes = new List<byte>();
            foreach (byte b in BitConverter.GetBytes(info.CollectorViewID)) { bytes.Add(b); }
            foreach (byte b in BitConverter.GetBytes(info.GodFavorPickupID)) { bytes.Add(b); }
            foreach (byte b in BitConverter.GetBytes(info.Amount)) { bytes.Add(b); }
            return bytes.ToArray();
        }

        /// <summary>Used for deserializing this object after it has been received from the server</summary>
        public static object Deserialize(byte[] data)
        {
            GodFavorResourceAddedInfo info;
            info.CollectorViewID = BitConverter.ToInt32(data, 0);
            info.GodFavorPickupID = BitConverter.ToInt32(data, 4);
            info.Amount = BitConverter.ToSingle(data, 8);
            return info;
        }
    }
}
