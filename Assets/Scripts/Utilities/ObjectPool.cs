using System.Collections.Generic;
using UnityEngine;

namespace GameStudio.HunterGatherer.Utilities
{
    /// <summary>
    /// Creates a pool of type Behaviour for recycling gameobjects.
    /// </summary>
    /// <typeparam name="T">T of type Behaviour, must be on the prefab.</typeparam>
    public class ObjectPool<T> where T : Behaviour
    {
        private readonly Queue<T> pool = new Queue<T>();

        public int MaxPoolSize { get; set; } = 10;
        public GameObject Prefab { get; set; }
        public Transform Parent { get; set; }

        /// <summary>
        /// Initialize the ObjectPool for use.
        /// </summary>
        /// <param name="parent">Parent Transform, pooled objects are stored under this object. </param>
        /// <param name="prefab">The object that should be pooled. </param>
        /// <param name="maxPoolSize">The maximum size of the pool. </param>
        public ObjectPool(Transform parent, GameObject prefab, int maxPoolSize = 10)
        {
            Parent = parent;
            Prefab = prefab;
            MaxPoolSize = maxPoolSize;
        }

        /// <summary> Dispose all references</summary>
        ~ObjectPool()
        {
            foreach (T obj in pool)
            {
                if (obj != null)
                {
                    Object.Destroy(obj.gameObject);
                }
            }
        }

        /// <summary> Dequeues T from the pool. </summary>
        public T DequeueT()
        {
            if (pool.Count == 0)
            {
                GameObject go = Object.Instantiate(Prefab, Parent);
                return go.GetComponent<T>();
            }
            T o = pool.Dequeue();
            o.gameObject.SetActive(true);
            return o;
        }

        /// <summary> Enqueues T into the pool. </summary>
        public void EnqueueT(T obj)
        {
            if (obj == null)
            {
                return;
            }
            if (pool.Count >= MaxPoolSize)
            {
                Object.Destroy(obj.gameObject);
                return;
            }
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
        }
    }
}