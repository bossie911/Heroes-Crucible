using System.Collections.Generic;
using UnityEngine;

namespace GameStudio.HunterGatherer.Divisions
{
    /// <summary>ScriptableObject that holds color values for divisions and units<summary>
    [CreateAssetMenu(fileName = "DivisionColorPalette", menuName = "ScriptableObjects/DivisionColorPalette")]
    public class DivisionColorPalette : ScriptableObject
    {
        [SerializeField]
        private List<DivisionTextureColorPair> textureColorPairs = new List<DivisionTextureColorPair>();

        public Color GetDivisionColor(int playerID)
        {
            return textureColorPairs[playerID].divisionColor;
        }

        public Texture GetDivisionTexture(int playerID)
        {
            return textureColorPairs[playerID].divisionTexture;
        }

        public Texture GetFlagTexture(DivisionType divisionType)
        {
            if (divisionType == DivisionType.Archers)
            {
                return textureColorPairs[0].archerFlagTexture;
            }
            else if (divisionType == DivisionType.Pikemen)
            {
                return textureColorPairs[0].pikemanFlagTexture;
            }
            else if (divisionType == DivisionType.Swordsmen)
            {
                return textureColorPairs[0].swordsmanFlagTexture;
            }
            else
            {
                return textureColorPairs[0].heroFlagTexture;
            }
        }
    }
}