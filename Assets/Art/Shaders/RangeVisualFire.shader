Shader "Custom/Range Visual Fire"
{
    Properties {
        [HDR]_Color ("Main Color", Color) = (1,1,1,1)
        [HDR]_SecondaryColor ("Secondary Color", Color) = (1,1,1,1)
        _NoiseScale ("Noise Scale", float) = 1
        _NoiseSpeed ("Noise Speed", vector) = (1,-1,0,0)
        _Offset ("Depth offset", float) = 1
//        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
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
        fixed _NoiseScale;
        fixed2 _NoiseSpeed;

        struct Input {
            float2 uv_MainTex;
            float4 screenPos;
        };

        inline float unity_noise_randomValue (float2 uv)
        {
            return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453);
        }

        inline float unity_noise_interpolate (float a, float b, float t)
        {
            return (1.0-t)*a + (t*b);
        }

        inline float unity_valueNoise (float2 uv)
        {
            float2 i = floor(uv);
            float2 f = frac(uv);
            f = f * f * (3.0 - 2.0 * f);

            uv = abs(frac(uv) - 0.5);
            float2 c0 = i + float2(0.0, 0.0);
            float2 c1 = i + float2(1.0, 0.0);
            float2 c2 = i + float2(0.0, 1.0);
            float2 c3 = i + float2(1.0, 1.0);
            float r0 = unity_noise_randomValue(c0);
            float r1 = unity_noise_randomValue(c1);
            float r2 = unity_noise_randomValue(c2);
            float r3 = unity_noise_randomValue(c3);

            float bottomOfGrid = unity_noise_interpolate(r0, r1, f.x);
            float topOfGrid = unity_noise_interpolate(r2, r3, f.x);
            float t = unity_noise_interpolate(bottomOfGrid, topOfGrid, f.y);
            return t;
        }

        float Unity_SimpleNoise_float(float2 UV, float Scale)
        {
            float t = 0.0;

            float freq = pow(2.0, float(0));
            float amp = pow(0.5, float(3-0));
            t += unity_valueNoise(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

            freq = pow(2.0, float(1));
            amp = pow(0.5, float(3-1));
            t += unity_valueNoise(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

            freq = pow(2.0, float(2));
            amp = pow(0.5, float(3-2));
            t += unity_valueNoise(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

            return t;
        }
        
        void surf (Input IN, inout SurfaceOutput o) {
            float offset = _Offset * Unity_SimpleNoise_float(IN.uv_MainTex + _Time.x * _NoiseSpeed, _NoiseScale);
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
