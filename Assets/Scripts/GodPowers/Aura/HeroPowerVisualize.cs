using UnityEngine;
using GameStudio.HunterGatherer.GodFavor.UI;
using GameStudio.HunterGatherer.Networking;
using Mirror;
using static GameStudio.HunterGatherer.GodFavor.GodPowerManager;
using System.Collections.Generic;

public class HeroPowerVisualize : MonoBehaviour
{
    //todo
    /*[SerializeField]
    private NetworkedMovingObject networkedMovingObject = null;
    public NetworkedMovingObject NetworkedMovingObject => networkedMovingObject;*/

    [SerializeField]
    private GameObject hero;

    [SerializeField]
    private GameObject auraInput;

    private GameObject aura;

    [SerializeField]
    private int powerCost = 20;

    [SerializeField]
    private float offset = 0.5f;

    [SerializeField]
    private List<Texture> GodAuras;

    private bool auraExists => aura != null;

    private Vector3 heroPosition;

    [SerializeField, Tooltip("Starting god power for a player")]
    private int startingGodPower = 20;

    private bool IsMine => GetComponent<NetworkIdentity>().isLocalPlayer;

    void Start()
    {
        hero = this.gameObject;
        heroPosition = new Vector3(hero.transform.position.x, hero.transform.position.y + offset, hero.transform.position.z);
        GodFavorUI.Instance?.OnGodFavorAmountChanged?.AddListener(GodFavorChange);
        if (IsMine)
        {
            GodFavorChange(startingGodPower);
        }
    }

    void Update()
    {
        //Update the aura position based on the hero
        if (auraExists)
        {
            aura.transform.position = new Vector3(hero.transform.position.x, hero.transform.position.y + offset, hero.transform.position.z);
        }
    }

    //Return whether the GodPower is usable
    private bool CanUsePower()
    {
        return GodFavorUI.Instance.Amount >= powerCost;
    }

    private void GodFavorChange(float amount)
    {
        //Instantiate the aura when you can use your god power, destroy it when you cannot
        //todo
        /*if (CanUsePower() && !auraExists && IsMine)
        {
            aura = NetworkingService.Instance.Instantiate(auraInput.name, heroPosition, Quaternion.identity);
            switch (GodFavorUI.CurrentGod)
            {
                case Gods.Zeus:
                    aura.GetComponent<ParticleSystemRenderer>().material.mainTexture = GodAuras[0];
                    break;
                case Gods.Ares:
                    aura.GetComponent<ParticleSystemRenderer>().material.mainTexture = GodAuras[1];
                    break;
                case Gods.Athena:
                    aura.GetComponent<ParticleSystemRenderer>().material.mainTexture = GodAuras[2];
                    break;
            }
        }
        else if (!CanUsePower() && auraExists && IsMine)
        {
            NetworkingService.Instance.Destroy(aura.GetComponent<NetworkedMovingObject>());
            aura = null;
        }*/
    }
}