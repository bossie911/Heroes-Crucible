Shader "Custom/Transparent Fresnel"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _FresnelColor ("Fresnel Color", Color) = (1,1,1,1)
        _FresnelPower ("Fresnel Power", float) = 1
        _FresnelStep ("Fresnel Step", float) = 1
        _FresnelStepSmoothness ("Fresnel Step Smoothness", Range (0,0.5)) = 1
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf NoLighting noforwardadd alpha:fade

        fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
         {
             fixed4 c;
             c.rgb = s.Albedo; 
             c.a = s.Alpha;
             return c;
         }

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        float FresnelEffect(float3 Normal, float3 ViewDir, float Power)
        {
            return pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
        }

        struct Input
        {
            float3 viewDir;
        };
        fixed4 _Color;
        fixed4 _FresnelColor;
        fixed _FresnelPower;
        fixed _FresnelStep;
        fixed _FresnelStepSmoothness;

        void surf (Input IN, inout SurfaceOutput o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = smoothstep(_FresnelStep - _FresnelStepSmoothness, _FresnelStep + _FresnelStepSmoothness, lerp(_Color, _FresnelColor, FresnelEffect(o.Normal, IN.viewDir, _FresnelPower)));
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
