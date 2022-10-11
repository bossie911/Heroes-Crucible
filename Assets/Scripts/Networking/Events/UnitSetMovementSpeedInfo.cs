using GameStudio.HunterGatherer.Divisions;
using System;
using System.Collections.Generic;

namespace GameStudio.HunterGatherer.Networking.Events
{
    /// <summary>Decribes a change of movement speed for a unit</summary>
    public struct UnitSetMovementSpeedInfo
    {
        public int UnitViewID;
        public float Speed;

        /// <summary>Used for serializing this object to be send over the network</summary>
        public static byte[] Serialize(object obj)
        {
            UnitSetMovementSpeedInfo info = (UnitSetMovementSpeedInfo)obj;
            List<byte> bytes = new List<byte>();
            foreach (byte b in BitConverter.GetBytes(info.UnitViewID)) { bytes.Add(b); }
            foreach (byte b in BitConverter.GetBytes((double)info.Speed)) { bytes.Add(b); }
            return bytes.ToArray();
        }

        /// <summary>Used for deserializing this object after it has been received from the server</summary>
        public static object Deserialize(byte[] data)
        {
            UnitSetMovementSpeedInfo info;
            info.UnitViewID = BitConverter.ToInt32(data, 0);
            info.Speed = (float) BitConverter.ToDouble(data, 4);
            return info;
        }
    }
}