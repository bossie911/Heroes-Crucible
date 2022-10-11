using System.Collections.Generic;
using GameStudio.HunterGatherer.Networking;
using Mirror;
using UnityEngine;

namespace GameStudio.HunterGatherer.Resources
{
    public class ResourceDistributorSemiRandom : NetworkBehaviour
    {
        [SerializeField]
        private ResourceSetting[] resourceSettings;
        [SerializeField]
        private ResourceManager resourceManager;

        public override void OnStartServer()
        {
            SpawnResources();
        }
        
        [Server]
        public void SpawnResources()
        {
            int iRandom = 0;
            List<GameObject> spawnedResources;

            Dictionary<GameObject, Transform[]> resourceSpawnPoints = new Dictionary<GameObject, Transform[]>();
            for (int i = 0; i < resourceSettings.Length; i++)
            {
                Transform[] spawnPoints = resourceSettings[i].spawnPoints.GetComponentsInChildren<Transform>();
                resourceSpawnPoints.Add(resourceSettings[i].resource, spawnPoints);
            }

            foreach (ResourceSetting setting in resourceSettings)
            {
                spawnedResources = new List<GameObject>();

                for (int i = 0; i < resourceSpawnPoints[setting.resource].Length; i++)
                {
                    //todo remove GameObject go = NetworkingService.Instance.Instantiate(setting.resource.name, resourceSpawnPoints[setting.resource][i].position, resourceSpawnPoints[setting.resource][i].rotation, true);
                    GameObject go = Instantiate(setting.resource, resourceSpawnPoints[setting.resource][i].position, resourceSpawnPoints[setting.resource][i].rotation);
                    if(!go.activeSelf) go.SetActive(true);//thisone
                    NetworkServer.Spawn(go);
                    
                    resourceManager.activePickUps.Add(go);
                    spawnedResources.Add(go);
                }
            }
        }

        private void OnValidate()
        {
            for (int i = 0; i < resourceSettings.Length; i++)
            {
                resourceSettings[i].OnValidate();
            }
        }
    }

    [System.Serializable]
    public class ResourceSetting
    {
        public string name;
        public GameObject resource;
        public GameObject spawnPoints;

        [Tooltip("Min active resources for one player")]
        public int minActiveResources;

        [Tooltip("Max active resource for one player")]
        public int maxActiveResources;
        public int playerMultiplier;

        public void OnValidate()
        {
            if (maxActiveResources < minActiveResources)
                minActiveResources = maxActiveResources;

            if (minActiveResources > maxActiveResources)
                maxActiveResources = minActiveResources;
        }
    }
}