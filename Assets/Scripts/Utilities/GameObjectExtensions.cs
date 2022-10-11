using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameStudio.HunterGatherer.Utilities
{
    /// <summary> Extension class for gameobject. </summary>
    public static class GameObjectExtensions
    {
        /// <summary> Tries to find the gameobject, if it fails, it will create an empty gameobject instead. </summary>
        public static GameObject FindOrCreateGameObject(this string name)
        {
            GameObject go = GameObject.Find(name);
            if (!go)
            {
                go = new GameObject(name);
            }
            return go;
        }

        public static List<Object> ToListObject(this List<GameObject> list) => list.Select(x => (Object)x).ToList();
        public static List<Object> ToObjectList(this IEnumerable<Divisions.Division> list) => list.Select(x => (Object)x).ToList();
    }
}