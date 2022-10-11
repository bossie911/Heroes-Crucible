using UnityEngine;

namespace GameStudio.HunterGatherer.Utilities
{
    /// <summary>Math parabola used for lerping using a parabola</summary>
    public static class Parabola
    {
        /// <summary>Lerp between start and end pos with given height and time from 0 to 1</summary>
        public static Vector3 Lerp(Vector3 startPos, Vector3 endPos, float height, float t)
        {
            float x = startPos.x + (endPos.x - startPos.x) * t;
            float y = startPos.y + ((endPos.y - startPos.y)) * t + height * (1 - (Mathf.Abs(0.5f - t) / 0.5f) * (Mathf.Abs(0.5f - t) / 0.5f));
            float z = startPos.z + (endPos.z - startPos.z) * t;
            return new Vector3(x, y, z);
        }
    }
}