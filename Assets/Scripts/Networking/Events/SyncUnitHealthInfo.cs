using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace GameStudio.HunterGatherer.Networking.Events
{
    [System.Serializable]
    public struct SyncUnitHealthInfo
    {
        public int unitID;
        public float hp;

        public SyncUnitHealthInfo(int unitID, float hp)
        {
            this.unitID = unitID;
            this.hp = hp;
        }

        /// <summary>Used for serializing this object to be send over the network</summary>
        public static byte[] Serialize(object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            SyncUnitHealthInfo toSerialize = (SyncUnitHealthInfo) obj;
            byte[] bytes;

            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, toSerialize);
                bytes = ms.ToArray();
                ms.Dispose();
            }

            return bytes;
        }

        /// <summary>Used for Deserializing this object to be received from the network</summary>
        public static object Deserialize(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                BinaryFormatter bf = new BinaryFormatter();
                ms.Seek(0, SeekOrigin.Begin);
                object o = bf.Deserialize(ms);
                return o;
            }
        }
    }
}