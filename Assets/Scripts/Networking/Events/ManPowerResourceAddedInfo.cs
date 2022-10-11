using GameStudio.HunterGatherer.Divisions;
using System;
using System.Collections.Generic;

namespace GameStudio.HunterGatherer.Networking.Events
{
    /// <summary>Decribes a change of the manpower count for a client</summary>
    public struct ManPowerResourceAddedInfo
    {
        public int AttackerViewID;
        public DivisionType DivisionType;

        /// <summary>Used for serializing this object to be send over the network</summary>
        public static byte[] Serialize(object obj)
        {
            ManPowerResourceAddedInfo info = (ManPowerResourceAddedInfo)obj;
            List<byte> bytes = new List<byte>();
            foreach (byte b in BitConverter.GetBytes(info.AttackerViewID)) { bytes.Add(b); }
            foreach (byte b in BitConverter.GetBytes((int)info.DivisionType)) { bytes.Add(b); }
            return bytes.ToArray();
        }

        /// <summary>Used for deserializing this object after it has been received from the server</summary>
        public static object Deserialize(byte[] data)
        {
            ManPowerResourceAddedInfo info;
            info.AttackerViewID = BitConverter.ToInt32(data, 0);
            info.DivisionType = (DivisionType)BitConverter.ToInt32(data, 4);
            return info;
        }
    }
}
