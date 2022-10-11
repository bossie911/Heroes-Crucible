Shader "Custom/Range Visual Shield"
{
    Properties {
        [HDR]_Color ("Main Color", Color) = (1,1,1,1)
        [HDR]_SecondaryColor ("Secondary Color", Color) = (1,1,1,1)
        _Offset ("Depth offset", float) = 1
    }

    SubShader {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        LOD 300
        
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        CGPROGRAM
        #pragma surface surf NoLighting noforwardadd alpha:fade

        fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
         {
             fixed4 c;
             c.rgb = s.Albedo; 
             c.a = s.Alpha;
             return c;
         }
        
        sampler2D _MainTex;
        sampler2D _CameraDepthTexture;
        half4 _Color;
        half4 _SecondaryColor;
        float _Offset;

        struct Input {
            float2 uv_MainTex;
            float4 screenPos;
        };
        
        void surf (Input IN, inout SurfaceOutput o) {
            float offset = _Offset;
            float alpha = saturate((IN.screenPos.w + offset) - LinearEyeDepth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(IN.screenPos))).r);

            half4 color = lerp(_Color, _SecondaryColor, step(1, alpha));
            o.Alpha = step(0.01, alpha) * color.a;
            o.Albedo = color;
            o.Emission = color;
        }

        
        ENDCG
    }

    FallBack "Standard"
}
