using UnityEngine;

namespace GameStudio.HunterGatherer.CameraUtility
{
    /// <summary>Render everything the camera sees with a shader. Replace any shaders if necessary</summary>
    [RequireComponent(typeof(Camera))]
    public class ReplacementShaderEffect : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private Shader replacementShader = null;

        private Camera shaderCamera;

        private void Awake()
        {
            shaderCamera = GetComponent<Camera>();
        }

        private void OnEnable()
        {
            if (replacementShader != null)
            {
                shaderCamera.SetReplacementShader(replacementShader, "RenderType");
            }
        }

        private void OnDisable()
        {
            shaderCamera.ResetReplacementShader();
        }
    }
}
