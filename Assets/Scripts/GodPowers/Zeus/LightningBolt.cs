using UnityEngine;

public class LightningBolt : GodPowers
{
    [SerializeField]
    public int damageAmount = 2;
    [SerializeField]
    private float lightningBoltDuration = 0.5f;

    protected override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(Die(lightningBoltDuration));
    }
}
