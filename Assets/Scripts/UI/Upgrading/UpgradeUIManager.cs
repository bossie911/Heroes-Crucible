using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameStudio.HunterGatherer.UI
{
    public class UpgradeUIManager : MonoBehaviour
    {
        [SerializeField] UIType[] typeUI;
        Dictionary<string, ButtonCarousel[]> pool = new Dictionary<string, ButtonCarousel[]>();
        Dictionary<Object, ButtonCarousel> activeUI = new Dictionary<Object, ButtonCarousel>();

        static UpgradeUIManager instance;

        public static UpgradeUIManager Instance => instance;

        void Awake()
        {
            if (instance != null)
                Destroy(gameObject);

            instance = this;
            //todo instance.GeneratePool();
        }

        /// <summary> Create an object pool of all diffirent UI types</summary>
        void GeneratePool()
        {
            for (int iType = 0, iUpgradeUI; iType < typeUI.Length; iType++)
            {
                // Create a new entry in the pool dictionairy using the Class it's full name (namespace + class name)
                pool.Add(typeUI[iType].script, new ButtonCarousel[typeUI[iType].poolSize]);
                for (iUpgradeUI = 0; iUpgradeUI < typeUI[iType].poolSize; iUpgradeUI++)
                {
                    // Instantiate a new instance of your prefab
                    pool[typeUI[iType].script][iUpgradeUI] = Instantiate(typeUI[iType].prefabUI, transform).GetComponent<ButtonCarousel>();
                    // Listen to for deactivation of an instance
                    pool[typeUI[iType].script][iUpgradeUI].OnDeActivate += OnDeactivateCarousel;
                }
            }
        }

        /// <summary>Show an UI instance based on your source</summary>
        /// <param name="source">The object which type will be checks</param>
        /// <param name="position">The position at which to show your UI instance</param>
        /// <param name="objects">The objects to use for your UI instance</param>
        public void Show(Object source, Vector3 position, List<Object> objects)
        {
            // Check if your source type exists as a key
            if (pool.ContainsKey(source.GetType().ToString()))
            {
                // Check if your source already has an active UI
                if (activeUI.ContainsKey(source))
                {
                    if (objects.Count <= 0)
                    {
                        activeUI[source].DeActivateAll();
                        activeUI.Remove(activeUI[source]);
                        return;
                    }

                    activeUI[source].Show(source, position, objects);
                    return;
                }

                // If there isn't an active instance for your source, find an in-active instance and show it
                for (int iUpgradeUI = 0; iUpgradeUI < pool[source.GetType().ToString()].Length; iUpgradeUI++)
                {
                    if (!pool[source.GetType().ToString()][iUpgradeUI].gameObject.activeInHierarchy)
                    {
                        pool[source.GetType().ToString()][iUpgradeUI].Show(source, position, objects);
                        activeUI.Add(source, pool[source.GetType().ToString()][iUpgradeUI]);
                        break;
                    }
                }
            }
            else
            {
                Debug.LogError($"<b>[{gameObject.name}]</b> Could not find an upgrade UI corresponding type {source.GetType().ToString()}");
            }
        }

        /// <summary>Show a UI instance based on your source</summary>
        /// <param name="type">Custom type to check instead of the source it's type</param>
        /// <param name="source">The object which be used as reference in the active list</param>
        /// <param name="position">The position at which to show your UI instance</param>
        /// <param name="objects">The objects to use for your UI instance</param>
        public void Show(System.Type type, Object source, Vector3 position, List<Object> objects)
        {
            if (pool.ContainsKey(type.ToString()))
            {
                if (activeUI.ContainsKey(source))
                {
                    if (objects.Count <= 0)
                    {
                        activeUI[source].DeActivateAll();
                        return;
                    }

                    activeUI[source].Show(source, position, objects);
                    return;
                }

                for (int iUpgradeUI = 0; iUpgradeUI < pool[type.ToString()].Length; iUpgradeUI++)
                {
                    if (!pool[type.ToString()][iUpgradeUI].gameObject.activeInHierarchy)
                    {
                        pool[type.ToString()][iUpgradeUI].Show(source, position, objects);
                        activeUI.Add(source, pool[type.ToString()][iUpgradeUI]);
                        break;
                    }
                }
            }
            else
            {
                Debug.LogError($"<b>[{gameObject.name}]</b> Could not find an upgrade UI corresponding type {type.ToString()}");
            }
        }

        /// <summary>Remove the deactivated carousel from the active list</summary>
        /// <param name="carousel">The UI instance to remove from active</summary>
        public void OnDeactivateCarousel(ButtonCarousel carousel)
        {
            foreach (Object key in activeUI.Keys)
            {
                if (activeUI[key] == carousel)
                {
                    activeUI.Remove(activeUI[key]);
                    break;
                }
            }
        }

        /// <summary>Deactivate all UI instance corresponding to source</summary>
        /// <param name="source">The source to check</param>
        public void DeActivateAll(Object source)
        {
            if (activeUI.ContainsKey(source))
            {
                activeUI[source].DeActivateAll();
            }
        }

        /// <summary>Deactivate all UI instance corresponding to source</summary>
        /// <param name="source">The source to check</param>
        public void DeActivate(Object source)
        {
            if (activeUI.ContainsKey(source))
            {
                activeUI[source].DeActivate();
            }
        }
    }
}