using GameStudio.HunterGatherer.Divisions;
using Mirror;
using UnityEngine;

namespace GameStudio.HunterGatherer.GodFavor
{
    public class GodShield : GodPowers
    {
        [SerializeField] private float godShieldDuration = 7.5f;

        private void OnEnable()
        {
            StartCoroutine(Die(godShieldDuration));
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<Projectile>())
            {
                if (other.gameObject.GetComponent<Projectile>().targetUnit.IsMine)
                {
                    DestroyObject(other.gameObject);
                }
            }
        }
    }
}
