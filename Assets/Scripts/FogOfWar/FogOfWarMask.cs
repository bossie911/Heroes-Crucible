using GameStudio.HunterGatherer.CustomEvents;
using GameStudio.HunterGatherer.Networking;
using UnityEngine;

namespace GameStudio.HunterGatherer.FogOfWar
{
    /// <summary>Manages the fog of war mask that can be attached to selectable objects</summary>
    public class FogOfWarMask : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField, Tooltip("Height of the collider checking for fog of war objects")]
        private float height = 25f;

        [SerializeField, Tooltip("the radius the mask will expand to when when being created")]
        private float defaultColliderRadius = 1f;

        [Header("References")]
        [SerializeField, Tooltip("Reference to the green fog of war mask object underneath this. Goverens what gets enabled and disabled at proper time.")]
        private GameObject maskReference = null;

        //[SerializeField]
        //private NetworkedMovingObject networkedMovingObject = null;

        [SerializeField]
        private bool visibleForEveryone;

        private CapsuleCollider maskCollider;
        public UnityEventFogOfWarMask OnMaskDisable;

        public bool MaskReferenceEnabledInHierarchy { get { return maskReference.activeInHierarchy; } }
        public GameObject MaskReference { get { return maskReference; } }

        private void Awake()
        {
            if (!maskReference)
            {
                Debug.LogErrorFormat("{0} is an illegal value for this paramater (maskReference), it must be a child object with a capsule colider", maskReference);
                return;
            }
            /*if (!networkedMovingObject)
            {
                Debug.LogErrorFormat("{0} is an illegal value for this paramater (networkedMovingObject), it must be the networkedMovingObject that this mask utilizes", networkedMovingObject);
                return;
            }*/
            maskCollider = maskReference.GetComponent<CapsuleCollider>();
            if (!maskCollider)
            {
                Debug.LogErrorFormat("Could not find a CapsuleCollider on {0} be sure to add one!", maskReference.name);
                return;
            }
            maskCollider.height = height;
        }

        private void OnEnable()
        {
            //TODO SetMaskState(networkedMovingObject.IsMine || NetworkingPlayerManager.Instance.IsSpectating || visibleForEveryone);
            //NetworkingPlayerManager.Instance.OnStartSpectating.AddListener(StartSpectate);
        }

        private void OnDisable()
        {
            OnMaskDisable.Invoke(this);
            //NetworkingPlayerManager.Instance.OnStartSpectating.RemoveListener(StartSpectate);
        }

        /// <summary>Called by OnStartSpectating, enables this FogOfWarMasks as if it's your own</summary>
        private void StartSpectate()
        {
            SetMaskState(true);
        }

        /// <summary>Set the mask state based on the bool passed in. Also affects the Collider radius.</summary>
        public void SetMaskState(bool show)
        {
            maskReference.SetActive(show);
            if (show)
            {
                maskCollider.radius = defaultColliderRadius;
            }
            else
            {
                maskCollider.radius = Mathf.Epsilon;
            }
        }
    }
}