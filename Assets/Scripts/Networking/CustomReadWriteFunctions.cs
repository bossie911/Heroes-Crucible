using Mirror;
using UnityEngine;

namespace GameStudio.HunterGatherer.Networking
{
	public static class CustomReadWriteFunctions
	{

        public static void WriteMyType(this NetworkWriter writer, Color32[] value)
        {
	        writer.WriteInt(value.Length);
	        foreach (var c in value)
	        {
		        writer.WriteColor32(c);
	        }
        }

        public static Color32[] ReadMyType(this NetworkReader reader)
        {
            var length = reader.ReadInt();
            var colors = new Color32[length];
            for (var i = 0; i < length; i++)
            {
                colors[i] = reader.ReadColor32();
            }
            return colors;
        }
    }
}