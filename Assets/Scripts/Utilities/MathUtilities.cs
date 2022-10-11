using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameStudio.HunterGatherer.Utilities
{
    /// <summary> Helper methods for math related issues </summary>
    public static class MathUtilities
    {
        /// <summary> Checks if the given float is an integer (whole number) </summary>
        public static bool IsInteger(this float value)
        {
            return Mathf.Abs(value % 1) <= float.Epsilon * 100;
        }
    }
}