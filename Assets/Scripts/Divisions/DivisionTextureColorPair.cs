using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameStudio.HunterGatherer.Divisions {

    [Serializable]
    ///<summary> This class is used to make an object of an event and the time it will be invoked.</summary>
    public class DivisionTextureColorPair {
        [Tooltip("The color for own division")]
        public Color divisionColor = new Color();

        [Tooltip("The texture for own division")]
        public Texture divisionTexture = null;

        [Tooltip("The texture for own hero flag")]
        public Texture heroFlagTexture = null;

        [Tooltip("The texture for own swordsman flag")]
        public Texture swordsmanFlagTexture = null;

        [Tooltip("The texture for own pikeman flag")]
        public Texture pikemanFlagTexture = null;

        [Tooltip("The texture for own archer flag")]
        public Texture archerFlagTexture = null;
    }
}