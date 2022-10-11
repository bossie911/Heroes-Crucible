using System.Collections.Generic;
using UnityEngine;
using GameStudio.HunterGatherer.Networking;
using GameStudio.HunterGatherer.Resources;
using UnityEngine.Serialization;

public class ResourceManager : MonoBehaviour
{
    [SerializeField] private ResourceDistributorSemiRandom resourcePlacerSemiRandom;

    [FormerlySerializedAs("ActivePickUps")]
    public List<GameObject> activePickUps;

    private void Start()
    {
        NetworkEvents.OnAllPlayersEnteredScene += OnAllPlayersLoaded; //OnAllPlayersEnnteredScene should only be ran on the server
    }

    private void OnDestroy()
    {
        NetworkEvents.OnAllPlayersEnteredScene -= OnAllPlayersLoaded;
    }

    /// <summary>Called when all players have loaded the game scene, it spawns all resources</summary> 
    private void OnAllPlayersLoaded() 
    {
        //Debug.LogError("sdfjklsdfjk");
        
        if (resourcePlacerSemiRandom != null)
        {
            Debug.LogError("jeqeqwejdqdjq");
            //resourcePlacerSemiRandom.SpawnResources(); //SHOULD ONLY BE RAN AS SERVER
        }
    }
}