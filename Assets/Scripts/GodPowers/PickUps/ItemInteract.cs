using GameStudio.HunterGatherer.GodFavor.UI;
using GameStudio.HunterGatherer.Networking;
using System.Collections;
using UnityEngine;

public class ItemInteract : InteractBehaviour
{
    [SerializeField]
    private string layerName = "Items";
    [SerializeField]
    private float scaleSpeed = 0.25f;
    private Vector3 destinationScale = Vector3.zero;

    private GameObject item;

    private void OnTriggerEnter(Collider collision)
    {
        // Collide with favor pickup
        if (collision.gameObject.layer == LayerMask.NameToLayer(layerName))
        {
            item = collision.gameObject;
            //networkMovingObject = item.GetComponent<NetworkedMovingObject>();
            OnInteract.Invoke();
        }
    }

    /// <summary>
    /// Add god favor and remove pickup item with animation
    /// </summary>
    protected override void Interact()
    {
        base.Interact();
        // Scale the pickup until its size is 0
        StartCoroutine(ScaleOverTime(scaleSpeed));

        // if (GetComponentInParent<NetworkedMovingObject>().IsMine)
        // {
        //     GodFavorUI.Instance.AddGodFavor(10);
        // }
    }

    /// <summary>
    /// Scales the pickup item to 0 and removes it from the game
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    IEnumerator ScaleOverTime(float time)
    {
        Vector3 originalScale = item.transform.localScale;
        float currentTime = 0.0f;

        while (currentTime < time)
        {
            item.transform.localScale = Vector3.Lerp(originalScale, destinationScale, currentTime / time);
            currentTime += Time.deltaTime;

            yield return null;
        }

        // sets the exact value, because Lerp never get's there
        if (currentTime >= time)
        {
            item.transform.localScale = destinationScale;

            //if (networkMovingObject.IsMine) NetworkingService.Instance.Destroy(networkMovingObject);

            yield return null;
        }
    }
}
