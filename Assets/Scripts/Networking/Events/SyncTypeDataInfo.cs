using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using GameStudio.HunterGatherer.Divisions;
using System.Runtime.Serialization.Formatters.Binary;

namespace GameStudio.HunterGatherer.Networking.Events
{
    /// <summary>Value type detailing Information about a scene object to be spawned by the host</summary>
    public struct SyncDivisionDataInfo
    {
        public int divisionID;
        public SingleDivisionTypeData divisionTypeData;
        static BinaryFormatter bf = new BinaryFormatter();

        /// <param name="divisionID">The Photonview ID of the division you want to sync</param>
        /// <param name="divisionTypeData">The new data that should be synced</param>
        public SyncDivisionDataInfo(int divisionID, SingleDivisionTypeData divisionTypeData)
        {
            this.divisionID = divisionID;
            this.divisionTypeData = divisionTypeData;
        }

        /// <summary>Used for serializing this object to be send over the network</summary>
        public static byte[] Serialize(object obj)
        {
            List<byte> bytes = new List<byte>();
            SyncDivisionDataInfo toSerialize = (SyncDivisionDataInfo) obj;

            var serializableFields = toSerialize.divisionTypeData.GetType().GetFields()
                .Where(field => field.FieldType.IsSerializable).ToArray();
            List<object> data = new List<object>();

            // Add the divisionID and all Serializable SingleDivisionTypeData values to one list
            data.Add(toSerialize.divisionID);
            foreach (var field in serializableFields)
            {
                data.Add(field.GetValue(toSerialize.divisionTypeData));
            }

            // Convert the list to bytes
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, data);
                bytes.AddRange(ms.ToArray());
                ms.Dispose();
            }

            return bytes.ToArray();
        }

        /// <summary>Used for Deserializing this object to be received from the network</summary>
        public static object Deserialize(byte[] data)
        {
            SyncDivisionDataInfo newData = new SyncDivisionDataInfo();
            newData.divisionTypeData =
                new SingleDivisionTypeData(
                    UnityEngine.ScriptableObject.CreateInstance(typeof(DivisionTypeData)) as DivisionTypeData);

            // Get the the serializableFields
            var serializableFields = newData.divisionTypeData.GetType().GetFields()
                .Where(field => field.FieldType.IsSerializable).ToArray();

            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(data, 0, data.Length);
                ms.Seek(0, SeekOrigin.Begin);

                // Convert the received bytes to an object array 
                // The first element will be our viewID and the other the values from SingleDivisionTypeData
                List<object> receivedData = bf.Deserialize(ms) as List<Object>;

                // All serializableFields will use i - 1 since they don't have the divisionID offset
                for (int i = 0; i < receivedData.Count; i++)
                {
                    if (i == 0)
                        newData.divisionID = (int) receivedData[i];
                    else
                    {
                        Type type = serializableFields[i - 1].FieldType;
                        try
                        {
                            // Most of the time setting DivisionType throws an error
                            serializableFields[i - 1].SetValue(newData.divisionTypeData,
                                Convert.ChangeType(receivedData[i], type));
                        }
                        catch (Exception ex) when (ex is System.InvalidCastException || ex is System.ArgumentException)
                        {
                            // This should fix that error
                            if (type == typeof(string))
                            {
                                serializableFields[i - 1].SetValue(newData.divisionTypeData,
                                    Enum.Parse(typeof(DivisionType), receivedData[i] as string));
                            }
                        }
                    }
                }

                ms.Dispose();
            }

            return newData;
        }
    }
}