using System;
using System.Collections.Generic;

namespace GameStudio.HunterGatherer.Networking.Events
{
    /// <summary>Decribes a change of division for a unit</summary>
    public struct UnitSetDivisionInfo
    {
        public int UnitViewID;
        public int DivisionViewID;

        /// <summary>Used for serializing this object to be send over the network</summary>
        public static byte[] Serialize(object obj)
        {
            UnitSetDivisionInfo info = (UnitSetDivisionInfo)obj;
            List<byte> bytes = new List<byte>();
            foreach (byte b in BitConverter.GetBytes(info.UnitViewID)) { bytes.Add(b); }
            foreach (byte b in BitConverter.GetBytes(info.DivisionViewID)) { bytes.Add(b); }
            return bytes.ToArray();
        }

        /// <summary>Used for deserializing this object after it has been received from the server</summary>
        public static object Deserialize(byte[] data)
        {
            UnitSetDivisionInfo info;
            info.UnitViewID = BitConverter.ToInt32(data, 0);
            info.DivisionViewID = BitConverter.ToInt32(data, 4);
            return info;
        }
    }
}