using GameStudio.HunterGatherer.Divisions;
using UnityEngine;
using UnityEngine.Events;
using GameStudio.HunterGatherer.Networking;
using Mirror;

namespace GameStudio.HunterGatherer.Resources
{
    [RequireComponent(typeof(Collider))]
    public class PickUpInteract : NetworkBehaviour
    {
        [Tooltip("Make sure to only select one layer!")]
        [SerializeField] LayerMask unitLayer;
        float layerIndex => Mathf.RoundToInt(Mathf.Log(unitLayer.value, 2));

        bool used = false;

        public UnityEvent OnInteract;

        new Collider collider;
        [SerializeField] GameObject model;

        void Awake()
        {
            collider = GetComponent<Collider>();
        }

        void OnEnable()
        {
            collider.enabled = true;
            if (model)
                model.SetActive(true);
        }

        void OnTriggerEnter(Collider other)
        {
            if (used)
                return;

            if (other.gameObject.layer == layerIndex)
            {
                collider.enabled = false;

                if (other.transform.root.GetComponent<Unit>().IsMine)
                {
                    OnInteract?.Invoke();
                }
                
                used = true;
                DestroyObject();
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            if (used)
                return;

            if (collision.collider.gameObject.layer == layerIndex)
            {
                collider.enabled = false;

                if (collision.collider.transform.root.GetComponent<Unit>().IsMine)
                {
                    OnInteract?.Invoke();
                }

                used = true;
                DestroyObject();
            }
        }
        
    
        [Command(requiresAuthority = false)]
        public void DestroyObject()
        {
            NetworkServer.Destroy(this.gameObject);
        }
    }
}