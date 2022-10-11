using GameStudio.HunterGatherer.Divisions;
using System;
using System.Collections.Generic;

namespace GameStudio.HunterGatherer.Networking.Events
{
    /// <summary>Decribes a projectile being fired by a unit</summary>
    public struct UnitFireProjectileInfo
    {
        public int TargetViewID;
        public int AttackerViewID;
        public HitType HitType;
        public float Damage;

        /// <summary>Used for serializing this object to be send over the network</summary>
        public static byte[] Serialize(object obj)
        {
            UnitFireProjectileInfo info = (UnitFireProjectileInfo)obj;
            List<byte> bytes = new List<byte>();
            foreach (byte b in BitConverter.GetBytes(info.TargetViewID)) { bytes.Add(b); }
            foreach (byte b in BitConverter.GetBytes(info.AttackerViewID)) { bytes.Add(b); }
            foreach (byte b in BitConverter.GetBytes((int)info.HitType)) { bytes.Add(b); }
            foreach (byte b in BitConverter.GetBytes(info.Damage)) { bytes.Add(b); }
            return bytes.ToArray();
        }

        /// <summary>Used for deserializing this object after it has been received from the server</summary>
        public static object Deserialize(byte[] data)
        {
            UnitFireProjectileInfo info;
            info.TargetViewID = BitConverter.ToInt32(data, 0);
            info.AttackerViewID = BitConverter.ToInt32(data, 4);
            info.HitType = (HitType)BitConverter.ToInt32(data, 8);
            info.Damage = BitConverter.ToInt32(data, 12);
            return info;
        }
    }
}