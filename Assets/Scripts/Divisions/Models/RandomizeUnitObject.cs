using System.Collections.Generic;
using UnityEngine;

namespace GameStudio.HunterGatherer.Divisions
{
    public class RandomizeUnitObject : MonoBehaviour
    {
        [SerializeField] private List<GameObject> objects = null;

        private int selectedObject = 0;

        void Awake()
        {
            SelectRandomObject();
        }

        /// <summary>
        /// Selects a random object in the list to enable and disables the others
        /// </summary>
        private void SelectRandomObject()
        {
            selectedObject = Random.Range(0, objects.Count);

            for (int i = 0; i < objects.Count; i++)
            {
                if (i == selectedObject)
                {
                    objects[i]?.SetActive(true);
                }
                else
                {
                    objects[i]?.SetActive(false);
                }
            }
        }
    }
}
