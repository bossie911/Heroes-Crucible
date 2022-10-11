using GameStudio.HunterGatherer.Divisions;

using System;
using System.Collections.Generic;

using UnityEngine;

namespace GameStudio.HunterGatherer.Networking.Events
{
    /// <summary>Decribes a change of DivisionType for a division</summary>
    public struct DivisionSetMoveTargetInfo
    {
        public int divisionId;
        public Vector3 position;
        public Vector3 direction;

        /// <summary>Used for serializing this object to be send over the network</summary>
        public static byte[] Serialize(object obj)
        {
            DivisionSetMoveTargetInfo info = (DivisionSetMoveTargetInfo)obj;
            List<byte> bytes = new List<byte>();
            foreach (byte b in BitConverter.GetBytes(info.divisionId)) { bytes.Add(b); }
            
            //convert position
            foreach (byte b in BitConverter.GetBytes(info.position.x)) bytes.Add(b);
            foreach (byte b in BitConverter.GetBytes(info.position.y)) bytes.Add(b);
            foreach (byte b in BitConverter.GetBytes(info.position.z)) bytes.Add(b);

            //Convert direction
            foreach (byte b in BitConverter.GetBytes(info.direction.x)) bytes.Add(b);
            foreach (byte b in BitConverter.GetBytes(info.direction.y)) bytes.Add(b);
            foreach (byte b in BitConverter.GetBytes(info.direction.z)) bytes.Add(b);

            return bytes.ToArray();
        }

        /// <summary>Used for deserializing this object after it has been received from the server</summary>
        public static object Deserialize(byte[] data)
        {
            return new DivisionSetMoveTargetInfo()
            {
                divisionId = BitConverter.ToInt32(data, 0),
                position = new Vector3
                {
                    x = BitConverter.ToSingle(data, 4),
                    y = BitConverter.ToSingle(data, 8),
                    z = BitConverter.ToSingle(data, 12),
                },
                direction = new Vector3
                {
                    x = BitConverter.ToSingle(data, 16),
                    y = BitConverter.ToSingle(data, 20),
                    z = BitConverter.ToSingle(data, 24),
                },
            };
        }
    }
}