using GameStudio.HunterGatherer.Divisions;
using GameStudio.HunterGatherer.Networking;
using UnityEngine;

public class GodPowerInteract : InteractBehaviour
{
    [SerializeField]
    private string layerName = "GodPower";

    private int damageAmount;
    private GameObject godPower;
    private bool isDamaging;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(layerName))
        {
            godPower = collision.gameObject;

            if (godPower.GetComponent<LightningBolt>())
            {
                damageAmount = godPower.GetComponent<LightningBolt>().damageAmount;
                isDamaging = true;
            }
            OnInteract.Invoke();
        }
    }

    protected override void Interact()
    {
        if (isDamaging) DamageUnit();
    }

    private void DamageUnit()
    {
        GetComponentInParent<Unit>().TakeDamage(damageAmount, godPower.gameObject);
        isDamaging = false;
    }
}