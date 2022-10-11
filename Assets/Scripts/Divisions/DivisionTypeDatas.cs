using System.Collections.Generic;
using UnityEngine;

namespace GameStudio.HunterGatherer.Divisions
{
    /// <summary>ScriptableObject holding a list of all the divisionTypeDatas, used to find the data when you have the type</summary>
    [CreateAssetMenu(fileName = "DivisionTypeDatas", menuName = "ScriptableObjects/DivisionTypeDatas")]
    public class DivisionTypeDatas : ScriptableObject
    {
        [SerializeField]
        private List<DivisionTypeData> divisionTypeDatas = new List<DivisionTypeData>();
        public int DivisionCount => divisionTypeDatas.Count;

        /// <summary>Return divisionTypeData matching the given divisionType</summary>
        public DivisionTypeData GetDivisionTypeData(DivisionType divisionType)
        {
            foreach (DivisionTypeData divisionTypeData in divisionTypeDatas)
            {
                if (divisionTypeData.Type == divisionType)
                {
                    return divisionTypeData;
                }
            }
            Debug.LogWarning("Couldn't find a divisionTypeData of the given divisionType!");
            return null;
        }

        /// <summary>Return divisionTypeData matching the given divisionType</summary>
        public DivisionTypeData GetDivisionTypeData(int index) => divisionTypeDatas[index];
    }
}