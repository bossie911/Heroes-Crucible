using System.Collections;
using FMODUnity;
using GameStudio.HunterGatherer.Divisions;
using UnityEngine;

public class UnitCombatAudio : MonoBehaviour
{
    [SerializeField]
    private Unit unit = null;

    [SerializeField]
    private float attackSoundDuration = 0.5f;

    // Depends on what unit but a bow fire, sword slash or pike strike
    [SerializeField]
    private EventReference attackReference;

    // a scream of sorts
    [SerializeField]
    private EventReference hitReference;

    // a object hitting a hard surface
    [SerializeField]
    private EventReference blockReference;


    void Start()
    {
        unit.onHit.AddListener(Hit);
        unit.OnBlock.AddListener(Block);
    }

    public void Attack()
    {
        //Debug.Log($"Is mine: {unit.Division.IsMine}; State: Attacking");
        RuntimeManager.PlayOneShot(attackReference, transform.position);
    }

    private void Block()
    {
        //Debug.Log($"Is mine: {unit.Division.IsMine}; State: Block");
        StartCoroutine(PlayOneShotAfter(blockReference));
    }

    private void Hit()
    {
        //Debug.Log($"Is mine: {unit.Division.IsMine}; State: Hit");
        StartCoroutine(PlayOneShotAfter(hitReference));
    }

    IEnumerator PlayOneShotAfter(EventReference audioEvent)
    {
        yield return new WaitForSecondsRealtime(attackSoundDuration);
        RuntimeManager.PlayOneShot(audioEvent, transform.position);
    }
}
