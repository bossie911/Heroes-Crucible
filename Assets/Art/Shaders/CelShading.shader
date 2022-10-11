Shader "Custom/CelShading"
{
    Properties
    {
        [Header(Main Maps)]
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        
        [Header(Diffuse Shadow)]
        _DiffuseShadowSmoothness ("Diffuse Smoothness", Range(0,0.5)) = 0.1
        _GIShadowSmoothness ("GI Smoothness", Range(0,0.5)) = 0.0
        
        [Header(Specular Highlight)]
        [PowerSlider(3.0)] _Specular ("Specular", Range(0,1)) = 0.015
        _SpecularFade ("Specular Fade", Range(0,1)) = 0.5
        _SpecularSmoothness ("Specular Smoothness", Range(0,0.5)) = 0.0
        
        [Header(Rim Highlight)]
        _RimAmount ("Rim Amount", Range(0,1)) = 0.5
        _RimThreshold("Rim Threshold", Range(0, 1)) = 0.1
        _RimFade ("Rim Fade", Range(0,1)) = 0.5
        _RimSmoothness ("Rim Smoothness", Range(0,0.5)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
 
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Cel fullforwardshadows

		half _DiffuseShadowSmoothness;
		half _GIShadowSmoothness;
        
        half _Specular;
		half _SpecularSmoothness;
        half _SpecularFade;
        
        half _RimAmount;
        half _RimThreshold;
        half _RimFade;

        void LightingCel_GI (SurfaceOutput s, UnityGIInput data, inout UnityGI gi)
        {
            UnityGI o_gi;
            ResetUnityGI(o_gi);

            // Base pass with Lightmap support is responsible for handling ShadowMask / blending here for performance reason
            #if defined(HANDLE_SHADOWS_BLENDING_IN_GI)
                half bakedAtten = UnitySampleBakedOcclusion(data.lightmapUV.xy, data.worldPos);
                float zDist = dot(_WorldSpaceCameraPos - data.worldPos, UNITY_MATRIX_V[2].xyz);
                float fadeDist = UnityComputeShadowFadeDistance(data.worldPos, zDist);
                data.atten = UnityMixRealtimeAndBakedShadows(data.atten, bakedAtten, UnityComputeShadowFade(fadeDist));
            #endif

			// Store shadow attenuation in the indirect specular variable
            o_gi.light = data.light;
            o_gi.indirect.specular = smoothstep(0.5-_GIShadowSmoothness, 0.5+_GIShadowSmoothness, data.atten);
            o_gi.light.color *= o_gi.indirect.specular.x;
            half3 normalWorld = s.Normal;

            #if UNITY_SHOULD_SAMPLE_SH
                o_gi.indirect.diffuse = ShadeSHPerPixel(s.Normal, data.ambient, data.worldPos);
            #endif

            #if defined(LIGHTMAP_ON)
                // Baked lightmaps
                half4 bakedColorTex = UNITY_SAMPLE_TEX2D(unity_Lightmap, data.lightmapUV.xy);
                half3 bakedColor = DecodeLightmap(bakedColorTex);

                #ifdef DIRLIGHTMAP_COMBINED
                    fixed4 bakedDirTex = UNITY_SAMPLE_TEX2D_SAMPLER (unity_LightmapInd, unity_Lightmap, data.lightmapUV.xy);
                    o_gi.indirect.diffuse += DecodeDirectionalLightmap (bakedColor, bakedDirTex, normalWorld);

                    #if defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN)
                        ResetUnityLight(o_gi.light);
                        o_gi.indirect.diffuse = SubtractMainLightWithRealtimeAttenuationFromLightmap (o_gi.indirect.diffuse, data.atten, bakedColorTex, normalWorld);
                    #endif

                #else // not directional lightmap
                    o_gi.indirect.diffuse += bakedColor;

                    #if defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN)
                        ResetUnityLight(o_gi.light);
                        o_gi.indirect.diffuse = SubtractMainLightWithRealtimeAttenuationFromLightmap(o_gi.indirect.diffuse, data.atten, bakedColorTex, normalWorld);
                    #endif

                #endif
            #endif

            #ifdef DYNAMICLIGHTMAP_ON
                // Dynamic lightmaps 
                fixed4 realtimeColorTex = UNITY_SAMPLE_TEX2D(unity_DynamicLightmap, data.lightmapUV.zw);
                half3 realtimeColor = DecodeRealtimeLightmap (realtimeColorTex);

                #ifdef DIRLIGHTMAP_COMBINED
                    half4 realtimeDirTex = UNITY_SAMPLE_TEX2D_SAMPLER(unity_DynamicDirectionality, unity_DynamicLightmap, data.lightmapUV.zw);
                    o_gi.indirect.diffuse += DecodeDirectionalLightmap (realtimeColor, realtimeDirTex, normalWorld);
                #else
                    o_gi.indirect.diffuse += realtimeColor;
                #endif
            #endif

            gi = o_gi;
        }
        
        half4 LightingCel (SurfaceOutput s, half3 viewDir, UnityGI gi)
        {
			// Normalize normal vector and store shadow attenuation
            s.Normal = normalize(s.Normal);
            half atten = gi.indirect.specular.x;

			// Diffuse lighting
            half NdotL = dot (s.Normal, gi.light.dir);
			float lightIntensity = smoothstep(-_DiffuseShadowSmoothness, _DiffuseShadowSmoothness, NdotL);

			// Specular Highlight
			float NdotH = dot(s.Normal, normalize(gi.light.dir + viewDir)) * atten * lightIntensity;
            float specular = 1 - s.Specular;
			float specularIntensity = smoothstep(specular - _SpecularSmoothness, specular + _SpecularSmoothness, NdotH) * _SpecularFade;

            // Rim lighting
            float4 rimDot = 1 - dot(viewDir, s.Normal);
            float rimIntensity = rimDot * pow(NdotL, _RimThreshold);
            float rim = 1-_RimAmount;
            rimIntensity = smoothstep(rim - 0.01, rim + 0.01, rimIntensity) * _RimFade;

            half4 c;
            c.rgb = s.Albedo * gi.light.color * (lightIntensity + specularIntensity + rimIntensity);
            c.a = s.Alpha;

            #ifdef UNITY_LIGHT_FUNCTION_APPLY_INDIRECT
                float lightBrightness = max(gi.light.color.r, max(gi.light.color.g, gi.light.color.b));
                c.rgb += s.Albedo * gi.indirect.diffuse * (1-(min(atten, lightIntensity) * lightBrightness));
            #endif

            return c;
        }
        
        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutput o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Specular = _Specular;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
