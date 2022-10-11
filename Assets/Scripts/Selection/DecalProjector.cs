using UnityEngine;

namespace GameStudio.HunterGatherer.Selection
{
    /// <summary>Creates instance of the given material with the given texture and applies it to the projector</summary>
    public class DecalProjector : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private Material decalMaterial = null;

        [SerializeField]
        private Texture2D decalTexture = null;

        private Material materialInstance;
        private Projector projector;

        private void Awake()
        {
            projector = GetComponent<Projector>();
            materialInstance = new Material(decalMaterial);
            materialInstance.SetTexture("_Texture", decalTexture);
            projector.material = materialInstance;
        }
    }
}