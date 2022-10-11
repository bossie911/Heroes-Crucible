using GameStudio.HunterGatherer.Divisions;
using System;
using System.Collections.Generic;

namespace GameStudio.HunterGatherer.Networking.Events
{
    /// <summary>Decribes a change of division for a unit</summary>
    public struct UnitHitInfo
    {
        public int UnitViewID;
        public int AttackerViewID;
        public HitType HitType;
        public float Damage;
        public float KnockbackDistance;
        public float KnockbackDuration;

        /// <summary>Used for serializing this object to be send over the network</summary>
        public static byte[] Serialize(object obj)
        {
            UnitHitInfo info = (UnitHitInfo)obj;
            List<byte> bytes = new List<byte>();
            foreach (byte b in BitConverter.GetBytes(info.UnitViewID)) { bytes.Add(b); }
            foreach (byte b in BitConverter.GetBytes(info.AttackerViewID)) { bytes.Add(b); }
            foreach (byte b in BitConverter.GetBytes((int)info.HitType)) { bytes.Add(b); }
            foreach (byte b in BitConverter.GetBytes(info.Damage)) { bytes.Add(b); }
            foreach (byte b in BitConverter.GetBytes(info.KnockbackDistance)) { bytes.Add(b); }
            foreach (byte b in BitConverter.GetBytes(info.KnockbackDuration)) { bytes.Add(b); }
            return bytes.ToArray();
        }

        /// <summary>Used for deserializing this object after it has been received from the server</summary>
        public static object Deserialize(byte[] data)
        {
            UnitHitInfo info;
            info.UnitViewID = BitConverter.ToInt32(data, 0);
            info.AttackerViewID = BitConverter.ToInt32(data, 4);
            info.HitType = (HitType)BitConverter.ToInt32(data, 8);
            info.Damage = BitConverter.ToSingle(data, 12);
            info.KnockbackDistance = BitConverter.ToSingle(data, 16);
            info.KnockbackDuration = BitConverter.ToSingle(data, 20);
            return info;
        }
    }
}