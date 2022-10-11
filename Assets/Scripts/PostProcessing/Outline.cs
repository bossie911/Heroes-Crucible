using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.PostProcessing
{
    [System.Serializable]
    [PostProcess(typeof(OutlineRenderer), PostProcessEvent.AfterStack, "GwCreates/Outlines", false)]
    public sealed class Outline : PostProcessEffectSettings
    {
        public ColorParameter OutlineColor = new ColorParameter { value = Color.black };
        
        [Header("Detail Outlines")]
        [FormerlySerializedAs("DetailOutlineThickness"), Min(0f), Tooltip("Thickness of the  outlines.")]
        public FloatParameter OutlineThickness = new FloatParameter() {value = 1f};
        
        [FormerlySerializedAs("DetailOutlineDepthSensitivity")] [Min(0f), Tooltip("Thickness of the detail outlines.")]
        public FloatParameter OutlineDepthSensitivity = new FloatParameter() {value = 0.1f};
        
        [FormerlySerializedAs("DetailOutlineDepthSensitivityInfluence"), Min(0f), Max(1f), Tooltip("Thickness of the detail outlines.")]
        public FloatParameter OutlineNormalSensitivityInfluence = new FloatParameter() {value = 0f};
        
        [FormerlySerializedAs("DetailOutlineDepthSensitivityInfluence"), Min(0f), Max(1f), Tooltip("Thickness of the detail outlines.")]
        public FloatParameter OutlineDepthSensitivityInfluence = new FloatParameter() {value = 0f};
        
        [FormerlySerializedAs("DetailOutlineNormalsSensitivity")] [Min(0f), Tooltip("Thickness of the detail outlines.")]
        public FloatParameter OutlineNormalsSensitivity = new FloatParameter() {value = 1f};
        
        [FormerlySerializedAs("DetailOutlineColorSensitivity")] [Min(0f), Tooltip("Thickness of the detail outlines.")]
        public FloatParameter OutlineColorSensitivity = new FloatParameter() {value = 1f};
        
        
        /// <summary>
        /// Returns <c>true</c> if the effect is currently enabled and supported.
        /// </summary>
        /// <param name="context">The current post-processing render context</param>
        /// <returns><c>true</c> if the effect is currently enabled and supported</returns>
        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            return enabled.value
#if UNITY_EDITOR
                   // Don't render motion blur preview when the editor is not playing as it can in some
                   // cases results in ugly artifacts (i.e. when resizing the game view).
                   && Application.isPlaying
#endif
                   && OutlineColor.value.a > 0;
        }
    }
    
    [UnityEngine.Scripting.Preserve]
    public sealed class OutlineRenderer : PostProcessEffectRenderer<Outline>
    {
        [SerializeField] private RenderTexture flatColorTexture = null;
        private Camera replacementCamera = null;

        public override DepthTextureMode GetCameraFlags()
        {
            return DepthTextureMode.DepthNormals | DepthTextureMode.Depth;
        }
        
        public override void Render(PostProcessRenderContext context)
        {
            // Create a new temporary render texture every frame
            if (replacementCamera != null)
                replacementCamera.targetTexture = null;
            RenderTexture.ReleaseTemporary(flatColorTexture);
            flatColorTexture = context.GetScreenSpaceTemporaryRT(); //RenderTexture.GetTemporary(context.width, context.height, 0, RenderTextureFormat.R16); //
            
            // Setup the camera
            SetupCamera(context);
            
            // Render replacement shader camera to render texure
            replacementCamera.Render();
            
            var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/LineRenderer"));
            sheet.properties.SetTexture("_FlatColorTex", flatColorTexture);
            sheet.properties.SetColor("_OutlineColor", settings.OutlineColor);
            sheet.properties.SetVector("_CameraDirection", Camera.main.transform.forward);
            
            sheet.properties.SetFloat("_DetailOutlineThickness", settings.OutlineThickness);
            sheet.properties.SetFloat("_DepthSensitivity", settings.OutlineDepthSensitivity);
            sheet.properties.SetFloat("_NormalSensitivityInfluence", settings.OutlineNormalSensitivityInfluence);
            sheet.properties.SetFloat("_DepthSensitivityInfluence", settings.OutlineDepthSensitivityInfluence);
            sheet.properties.SetFloat("_NormalsSensitivity", settings.OutlineNormalsSensitivity);
            sheet.properties.SetFloat("_ColorSensitivity", settings.OutlineColorSensitivity);
            
            sheet.properties.SetFloat("_Width", context.width);
            sheet.properties.SetFloat("_Height", context.height);
            
            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
        }
        
        /// <summary>
        /// Sets up a camera that renders using the Hand Drawn Replacement shader
        /// </summary>
        /// <param name="context"></param>
        private void SetupCamera(PostProcessRenderContext context)
        {
            // Create new camera when necessary
            if (replacementCamera == null)
            {
                GameObject obj = new GameObject("Flat Color Camera");
                obj.transform.parent = context.camera.transform;
                //obj.hideFlags = HideFlags.HideInHierarchy;
                replacementCamera = obj.AddComponent<Camera>();
                replacementCamera.enabled = false;
            }

            replacementCamera.CopyFrom(context.camera);
            replacementCamera.targetTexture = flatColorTexture;
            replacementCamera.forceIntoRenderTexture = true;
            replacementCamera.depthTextureMode = DepthTextureMode.None;
            replacementCamera.clearFlags = CameraClearFlags.SolidColor;
            replacementCamera.backgroundColor = Color.black;
            replacementCamera.SetReplacementShader(Shader.Find("Hidden/Custom/LineRendererReplacement"), "RenderType");
        }
    }
}
