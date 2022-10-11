using GameStudio.HunterGatherer.Networking;
using UnityEngine;
using UnityEngine.Events;
using GameStudio.HunterGatherer.Divisions;
using GameStudio.HunterGatherer.Divisions.Upgrades;
using System.Text;
using System.Collections.Generic;
using Mirror;

namespace GameStudio.HunterGatherer.Structures
{
    /// <summary> Check for possible interactions and if possible fire an event </summary>
    public class StructureInteract : NetworkBehaviour
    {
        [Header("Max Uses")]
        [SerializeField] protected bool _hasMaxUses = true;
        [SerializeField, Range(1, 10)] protected int _maxUses = 1;
        protected int _uses = 0;
        public string onMaxUsesReached = "onMaxUsesReached";

        [Header("Interaction Checks")]
        [SerializeField] bool _checkHero = true;
        [SerializeField] bool _checkUnit = false;
        [SerializeField] bool _checkDivision = true;

        [Header("Events")]
        public string onStructureEnter = "onStructureEnter";
        public string onStructureExit = "onStructureExit";

        [Header("Fog of War")]
        [SerializeField] bool _visibleInFogOfWar = true;
        [SerializeField] List<GameObject> structureModels;

        protected GameObject previousCollider = null;

        protected virtual void Awake()
        {
            onStructureEnter += gameObject.GetInstanceID();
            onStructureExit += gameObject.GetInstanceID();
            onMaxUsesReached += gameObject.GetInstanceID();

            EventManager.Instance.AddEvent(onStructureEnter, true);
            EventManager.Instance.AddEvent(onStructureExit, true);
            EventManager.Instance.AddEvent(onMaxUsesReached);

            FogOfWarCheck();
        }

        //Mostly for developer purposes; Structure testing becomes a lot harder if you don't see them
        protected void FogOfWarCheck()
        {
            if (_visibleInFogOfWar){
                foreach (var item in structureModels)
                {
                    item.SetActive(true);
                }
            }
        }

        protected bool IsValidInteraction(Collider collider)
        {
            bool valid = false;

            valid = !_checkHero && !_checkUnit && !_checkDivision;
            valid = _checkHero ? collider.GetComponent<Division>() && collider.CompareTag("Division") && collider.GetComponent<Division>().SelectableObject.ObjectName == "Hero" || valid : valid;
            valid = _checkUnit ? collider.GetComponentInParent<Unit>() && collider.CompareTag("Unit") || valid : valid;
            valid = _checkDivision ? collider.GetComponent<Division>() && collider.CompareTag("Division") && collider.GetComponent<Division>().SelectableObject.ObjectName != "Hero" || valid : valid;

            return valid;
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
        }

        protected virtual void OnTriggerExit(Collider other)
        {
        }

        public virtual void Use(object obj = null)
        {
            if (obj != null)
            {
                _uses++;
                CheckMaxUses();
            }
            else if (obj == null)
            {
                DestroyObject();
            }
        }

        public void CheckMaxUses()
        {
            if (_hasMaxUses && _uses >= _maxUses)
            {
                EventManager.Instance.Invoke(onMaxUsesReached);
                DestroyObject();
            }
        }

        [Command(requiresAuthority = false)]
        public void DestroyObject()
        {
            NetworkServer.Destroy(gameObject);
        }
    }
}