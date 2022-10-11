Shader "Hidden/Custom/LineRenderer"
{
    HLSLINCLUDE
        // StdLib.hlsl holds pre-configured vertex shaders (VertDefault), varying structs (VaryingsDefault), and most of the data you need to write common effects.
        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        TEXTURE2D_SAMPLER2D(_FlatColorTex, sampler_FlatColorTex);
        TEXTURE2D_SAMPLER2D(_MaskTex, sampler_MaskTex);
        float4 _FogTintColor;
        float4 _OutlineColor;

    
        sampler2D  _CameraDepthTexture;
        sampler2D  _CameraDepthNormalsTexture;
    
        float _Width;
        float _Height;
    
        half _DetailOutlineThickness;
        half _DepthSensitivity;
        half _NormalSensitivityInfluence;
        half _DepthSensitivityInfluence;
        half _NormalsSensitivity;
        half _ColorSensitivity;

        float3 _CameraDirection;

    
        half Outline(float2 UV, float OutlineThickness, float DepthSensitivity, float NormalsSensitivity, float ColorSensitivity)
        {
            float halfScaleFloor = floor(OutlineThickness * 0.5);
            float halfScaleCeil = ceil(OutlineThickness * 0.5);
            float2 Texel = (1.0) / float2(_Width, _Height);

            float2 uvSamples[4];
            float depthSamples[4];
            float3 normalSamples[4], colorSamples[4];

            uvSamples[0] = UV - float2(Texel.x, Texel.y) * halfScaleFloor;
            uvSamples[1] = UV + float2(Texel.x, Texel.y) * halfScaleCeil;
            uvSamples[2] = UV + float2(Texel.x * halfScaleCeil, -Texel.y * halfScaleFloor);
            uvSamples[3] = UV + float2(-Texel.x * halfScaleFloor, Texel.y * halfScaleCeil);

            for(int i = 0; i < 4 ; i++)
            {
                depthSamples[i] = tex2D(_CameraDepthTexture, uvSamples[i]).r;
                float depth = LinearEyeDepth(depthSamples[i]);
                normalSamples[i] = DecodeViewNormalStereo(tex2D(_CameraDepthNormalsTexture, uvSamples[i]));
                float normal = dot(_CameraDirection, normalSamples[i]);
                colorSamples[i] = SAMPLE_TEXTURE2D(_FlatColorTex, sampler_FlatColorTex, uvSamples[i]);
                
                depthSamples[i] = lerp(depthSamples[i], depthSamples[i] / normal, _NormalSensitivityInfluence);
                normalSamples[i] = lerp(normalSamples[i], normalSamples[i] / depth, _DepthSensitivityInfluence);
                colorSamples[i] = lerp(colorSamples[i], colorSamples[i] / depth, _DepthSensitivityInfluence);
            }
            
            // Depth
            float depthFiniteDifference0 = depthSamples[1] - depthSamples[0];
            float depthFiniteDifference1 = depthSamples[3] - depthSamples[2];
            half edgeDepth = sqrt(pow(depthFiniteDifference0, 2) + pow(depthFiniteDifference1, 2)) * 100;
            half depthThreshold = (1/DepthSensitivity) * depthSamples[0]; // / (dot(normalSamples[0], _CameraDirection));
            edgeDepth = edgeDepth > depthThreshold ? 1 : 0;

            // Normals
            half3 normalFiniteDifference0 = normalSamples[1] - normalSamples[0];
            half3 normalFiniteDifference1 = normalSamples[3] - normalSamples[2];
            half edgeNormal = sqrt(dot(normalFiniteDifference0, normalFiniteDifference0) + dot(normalFiniteDifference1, normalFiniteDifference1));
            edgeNormal = edgeNormal > (1/NormalsSensitivity) ? 1 : 0;

            // Color
            float3 colorFiniteDifference0 = colorSamples[1] - colorSamples[0];
            float3 colorFiniteDifference1 = colorSamples[3] - colorSamples[2];
            float edgeColor = sqrt(dot(colorFiniteDifference0, colorFiniteDifference0) + dot(colorFiniteDifference1, colorFiniteDifference1));
	        edgeColor = edgeColor > (1/ColorSensitivity) ? 1 : 0;
            
            return max(edgeDepth, max(edgeNormal, edgeColor));
        }

        
        float4 Frag(VaryingsDefault i) : SV_Target
        {
            float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);

            // Calculate outlines
            half outline = Outline(i.texcoord, _DetailOutlineThickness, _DepthSensitivity, _NormalsSensitivity, _ColorSensitivity);
            half4 maskColor = lerp(_FogTintColor, _OutlineColor, outline * _OutlineColor.a);
            color = lerp(color, _OutlineColor, outline * _OutlineColor.a);

            return color;
        }
    ENDHLSL
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            HLSLPROGRAM
                #pragma vertex VertDefault
                #pragma fragment Frag
            ENDHLSL
        }
    }
}
