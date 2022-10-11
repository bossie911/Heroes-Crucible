using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayOneShot : MonoBehaviour
{
    [SerializeField]
    private EventReference attackEvent;

    public void Play()
    {
        //Debug.Log($"Is mine: {unit.Division.IsMine}; State: Attacking");
        RuntimeManager.PlayOneShot(attackEvent, transform.position);
    }
}
