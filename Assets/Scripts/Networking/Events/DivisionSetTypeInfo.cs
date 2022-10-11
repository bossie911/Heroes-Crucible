using GameStudio.HunterGatherer.Divisions;
using System;
using System.Collections.Generic;

namespace GameStudio.HunterGatherer.Networking.Events
{
    /// <summary>Decribes a change of DivisionType for a division</summary>
    public struct DivisionSetTypeInfo
    {
        public int DivisionViewID;
        public DivisionType DivisionType;

        /// <summary>Used for serializing this object to be send over the network</summary>
        public static byte[] Serialize(object obj)
        {
            DivisionSetTypeInfo info = (DivisionSetTypeInfo)obj;
            List<byte> bytes = new List<byte>();
            foreach (byte b in BitConverter.GetBytes(info.DivisionViewID)) { bytes.Add(b); }
            foreach (byte b in BitConverter.GetBytes((int)info.DivisionType)) { bytes.Add(b); }
            return bytes.ToArray();
        }

        /// <summary>Used for deserializing this object after it has been received from the server</summary>
        public static object Deserialize(byte[] data)
        {
            DivisionSetTypeInfo info;
            info.DivisionViewID = BitConverter.ToInt32(data, 0);
            info.DivisionType = (DivisionType)BitConverter.ToInt32(data, 4);
            return info;
        }
    }
}