using GameStudio.HunterGatherer.Networking;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GodPowerVisualiser : NetworkBehaviour
{
    public float scaleTime = 2.05f;

    private void OnEnable()
    {
        StartScaling();
    }

    public void StartScaling()
    {
        StartCoroutine(ScaleOverTime());
    }

    private IEnumerator ScaleOverTime()
    {
        transform.localScale = new Vector3(10, 0.1f, 10);
        Vector3 originalScale = transform.localScale;
        float currentTime = 0.0f;

        do
        {
            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, currentTime / scaleTime);
            currentTime += Time.deltaTime;

            yield return null;
        } while (currentTime < scaleTime);

        // sets the exact value, because Lerp never get's there
        if (currentTime >= scaleTime)
        {
            transform.localScale = Vector3.zero;

            DestroyObject(this.gameObject);
            
            yield return null;
        }
    }

    [Command(requiresAuthority = false)]
    public void DestroyObject(GameObject go)
    {
        if(go != null && go.activeInHierarchy)
        {
            NetworkServer.Destroy(go);
        }
    }
}
