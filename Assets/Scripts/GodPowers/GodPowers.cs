using System.Collections;
using GameStudio.HunterGatherer.GodFavor;
using Mirror;
using UnityEngine;

public class GodPowers : NetworkBehaviour
{
    [Header("Screen shake")]
    [SerializeField]
    private bool applyScreenShake;
    [SerializeField]
    private float duration = 1f;
    [SerializeField]
    private float magnitude = 1f;

    
    protected virtual void OnEnable()
    {
        if (applyScreenShake)
        {
            DoScreenShake();
        }
    }

    protected IEnumerator Die(float duration)
    {
        yield return new WaitForSeconds(duration);
        DestroyObject(this.gameObject);
        if (GodPowerManager.activeManager != null)
        {
            GodPowerManager.activeManager.IsPlacingGodPower = false;
        }
    }

    protected void DoScreenShake()
    {
        StartCoroutine(ScreenShake.Instance.Shake(duration, magnitude,
            new Vector2(transform.position.x, transform.position.z)));
    }
    
    [Command(requiresAuthority = false)]
    public void DestroyObject(GameObject go)
    {
        if (go != null && go.activeInHierarchy)
        {
            NetworkServer.Destroy(go);
        }
    }
}
