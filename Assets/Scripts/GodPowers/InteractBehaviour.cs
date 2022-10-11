using GameStudio.HunterGatherer.Networking;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class InteractBehaviour : MonoBehaviour
{
    public UnityEvent OnInteract { get; } = new UnityEvent();
    public UnityEvent OnEnter { get; } = new UnityEvent();
    public UnityEvent OnExit { get; } = new UnityEvent();

    protected virtual void Awake()
    {
        OnInteract.AddListener(Interact);
        OnEnter.AddListener(Enter);
        OnExit.AddListener(Exit);
    }

    protected virtual void Interact() {}

    protected virtual void Enter() {}
    
    protected virtual void Exit() {}
}