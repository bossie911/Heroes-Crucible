using GameStudio.HunterGatherer.GodFavor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameStudio.HunterGatherer.Networking.Events
{
    /// <summary>Decribes a change of god for a specific player</summary>
    public struct GodSetInfo
    {
        public int actorNumber;
        public GodPowerManager.Gods GodType;

        /// <summary>Used for serializing this object to be sent over the network</summary>
        public static byte[] Serialize(object obj)
        {
            GodSetInfo info = (GodSetInfo)obj;
            List<byte> bytes = new List<byte>();
            foreach (byte b in BitConverter.GetBytes(info.actorNumber)) { bytes.Add(b); }
            foreach (byte b in BitConverter.GetBytes((int)info.GodType)) { bytes.Add(b); }
            return bytes.ToArray();
        }

        /// <summary>Used for deserializing this object after it has been received from the server</summary>
        public static object Deserialize(byte[] data)
        {
            GodSetInfo info;
            info.actorNumber = BitConverter.ToInt32(data, 0);
            info.GodType = (GodPowerManager.Gods)BitConverter.ToInt32(data, 4);
            return info;
        }
    }
}