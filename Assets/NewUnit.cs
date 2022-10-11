using System.Collections;
using System.Collections.Generic;
using GameStudio.HunterGatherer.Divisions;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class NewUnit : NetworkBehaviour
{
    public override void OnStartServer()
    {
        if (NavMesh.SamplePosition(transform.position, out var hit, Mathf.Infinity, NavMesh.AllAreas))
        {
            transform.position = hit.position;
        }
        GetComponent<NavMeshAgent>().enabled = true;

        
    }
    public override void OnStartClient()
    {
        GetComponent<NavMeshAgent>().enabled = false;
    }

    public void Update()
    {
        if(!isLocalPlayer && Input.GetKeyDown(KeyCode.A))
        {
            SetPos();
        }           
    }

    [Command]
    public void SetPos()
    {
        GetComponent<NavMeshAgent>().SetDestination(Vector3.forward * 10);
    }
}
