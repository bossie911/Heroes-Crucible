using GameStudio.HunterGatherer.Divisions;
using GameStudio.HunterGatherer.Networking;
using Mirror;
using UnityEngine;
using NetworkRoomManager = GameStudio.HunterGatherer.Networking.NetworkRoomManager;

namespace GameStudio.HunterGatherer.Environment.Flood
{
    /// <summary>Handles the interaction with the flood</summary>
    public class FloodInteract : InteractBehaviour
    {
        [SerializeField] private string layerName = "Flood";

        [SerializeField] private bool destroyInstant = true;

        [SerializeField] private Unit unit;
        private Flood flood;
        private UnitRegen unitRegen;

        [SerializeField] public bool IsBuilding = false;
        private bool IsMine => unit == null ? IsBuilding : unit.IsMine;

        protected override void Awake()
        {
            base.Awake();
            unitRegen = GetComponentInParent<UnitRegen>();

        }

        private void OnTriggerEnter(Collider collision)
        {
            // Guard clause to return if object isn't owned
            if (!IsMine)
            {
                return;
            }

            //If it enters with the flood
            if (collision.gameObject.layer == LayerMask.NameToLayer(layerName))
            {
                flood = collision.gameObject.GetComponent<Flood>();
                OnInteract.Invoke();
                OnEnter.Invoke();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // Guard clause to return if object isn't owned
            if (!IsMine)
            {
                return;
            }

            //If it exits the flood
            if (other.gameObject.layer == LayerMask.NameToLayer(layerName))
            {
                OnExit.Invoke();
            }
        }

        private void OnTriggerStay(Collider collision)
        {
            // Guard clause to return if object isn't owned
            if (!IsMine)
            {
                return;
            }

            //If it collides with the flood
            if (collision.gameObject.layer == LayerMask.NameToLayer(layerName))
            {
                flood = collision.gameObject.GetComponent<Flood>();
                OnInteract.Invoke();
            }
        }

        /// <summary>Destroy networkedObject via networking service</summary>
        protected override void Interact()
        {
            if (destroyInstant || !unit)
            {
                Destroy(this.gameObject);
            }
            else
            {
                unit.TakeDamage(flood.Damage, flood.gameObject);
            }
        }

        protected override void Enter()
        {
            if (unit) unitRegen?.SetCanRegen(false);
        }

        protected override void Exit()
        {
            if (unit) unitRegen?.SetCanRegen(true);   
        }
    }
}