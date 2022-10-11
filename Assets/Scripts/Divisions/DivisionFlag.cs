using UnityEngine;

namespace GameStudio.HunterGatherer.Divisions
{
    /// <summary>Class handling the flag of a division</summary>
    public class DivisionFlag : MonoBehaviour
    {
        [SerializeField]
        private Renderer flag = null;
        
        private MaterialPropertyBlock _materialPropertyBlock;
        private MaterialPropertyBlock materialPropertyBlock
        {
            get
            {
                if (_materialPropertyBlock == null)
                    _materialPropertyBlock = new MaterialPropertyBlock();
                return _materialPropertyBlock;
            }
            set { _materialPropertyBlock = value; }
        }

        public void SetMaterial(Material material, Texture texture) {
            if (texture && material)
            {
                flag.sharedMaterial = material;
                flag.GetPropertyBlock(materialPropertyBlock);
                materialPropertyBlock.SetTexture("_MainTex", texture);
                flag.SetPropertyBlock(materialPropertyBlock);
            }
        }      
        
        private Camera camera;
        [SerializeField]
        private Vector2 scaleSizeRange = new Vector2(1f, 4f);
        [SerializeField]
        private Vector2 cameraScaleDistanceRange = new Vector2(10f, 50f);
    
        void Start()
        {
            camera = Camera.main;
        }

        void LateUpdate()
        {
            // Rotate flag toward camera
            transform.rotation = Quaternion.Euler(0, camera.transform.eulerAngles.y, 0);

            // Scale flag based on distance to camera
            float distance = (Vector3.Distance(transform.position, camera.transform.position) - cameraScaleDistanceRange.x) /
                             (cameraScaleDistanceRange.y - cameraScaleDistanceRange.x);
            transform.localScale = Vector3.one * Mathf.Lerp(scaleSizeRange.x, scaleSizeRange.y, Mathf.Clamp01(distance));
        }
    }
}