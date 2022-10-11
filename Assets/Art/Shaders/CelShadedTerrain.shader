Shader "Custom/CelShadedTerrain"
{
    Properties
    {
        _CliffTexture("Cliff texture", 2D) = "white" {}
        [Normal]_CliffNormal("Cliff normal", 2D) = "bump" {} 
        _CliffNormalStrength("Cliff normal strength", float) = 1
        _CliffSmoothness("Cliff smoothness", Range(0,1)) = 0
        _CliffMetallic("Cliff metallic", Range(0,1)) = 0
        _TextureBlend("Texture Blend", Range(0,0.5)) = 0
        _NormalThreshold("Normal Threshold", Range(0,1)) = 0.5
        
        
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
        Tags { "RenderType"="Terrain" }
        LOD 200
 
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Cel fullforwardshadows
 
        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

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

            //return s.Gloss;
            return c;
        }
        
 
        sampler2D _CliffTexture;
        float4 _CliffTexture_ST;
        sampler2D _CliffNormal;
        float _NormalThreshold;

        float _CliffNormalStrength;
        float _CliffMetallic;
        float _CliffSmoothness;
        float _TextureBlend;
 
        sampler2D _Control;
 
        // Textures
        sampler2D _Splat0, _Splat1, _Splat2, _Splat3;
        float4 _Splat0_ST, _Splat1_ST, _Splat2_ST, _Splat3_ST;
 
        //Normal Textures
        sampler2D _Normal0, _Normal1, _Normal2, _Normal3;
 
        //Normal scales
        float _NormalScale0, _NormalScale1, _NormalScale2, _NormalScale3;
 
        //Smoothness
        half _Smoothness0;
        half _Smoothness1;
        half _Smoothness2;
        half _Smoothness3;
 
        //Metallic
        half _Metallic0;
        half _Metallic1;
        half _Metallic2;
        half _Metallic3;
 
 
        struct Input
        {
            float2 uv_Control;
            float3 worldPos;
            float3 worldNormal;
            INTERNAL_DATA
        };
 
 
        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)
 
        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 splatControl = tex2D(_Control, IN.uv_Control);
            half3 normal = o.Normal;

            // Determine color
            fixed4 col = smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splatControl.r) * tex2D (_Splat0, IN.uv_Control * _Splat0_ST.xy);
            col += smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splatControl.g) * tex2D(_Splat1, IN.uv_Control * _Splat1_ST.xy);
            col += smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splatControl.b) * tex2D (_Splat2, IN.uv_Control * _Splat2_ST.xy);
            col += smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splatControl.a) * tex2D (_Splat3, IN.uv_Control * _Splat3_ST.xy);

            // Determine normal vector
            o.Normal = smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splatControl.r) * UnpackNormalWithScale(tex2D(_Normal0, IN.uv_Control * _Splat0_ST.xy), _NormalScale0);
            o.Normal += smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splatControl.g) * UnpackNormalWithScale(tex2D(_Normal1, IN.uv_Control * _Splat1_ST.xy), _NormalScale1);
            o.Normal += smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splatControl.b) * UnpackNormalWithScale(tex2D(_Normal2, IN.uv_Control * _Splat2_ST.xy), _NormalScale2);
            o.Normal += smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splatControl.a) * UnpackNormalWithScale(tex2D(_Normal3, IN.uv_Control * _Splat3_ST.xy), _NormalScale3);

            // Determine specular
            o.Specular = smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splatControl.r) * _Metallic0;
            o.Specular += smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splatControl.g) * _Metallic1;
            o.Specular += smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splatControl.b) * _Metallic2;
            o.Specular += smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splatControl.a) * _Metallic3;

            // Determine gloss
            o.Gloss = smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splatControl.r) * _Smoothness0;
            o.Gloss += smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splatControl.g) * _Smoothness1;
            o.Gloss += smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splatControl.b) * _Smoothness2;
            o.Gloss += smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splatControl.a) * _Smoothness3;

            // Determine cliff texture
            float3 vec = abs(WorldNormalVector (IN, normal));
            float threshold =  smoothstep(_NormalThreshold - _TextureBlend, _NormalThreshold + _TextureBlend, abs(dot(vec, float3(0, 1, 0))));
            fixed4 cliffColorXY = tex2D(_CliffTexture, IN.worldPos.xy * _CliffTexture_ST.xy);
            fixed4 cliffColorYZ = tex2D(_CliffTexture, IN.worldPos.yz * _CliffTexture_ST.xy);
            fixed4 cliffColor = vec.x * cliffColorYZ + vec.z * cliffColorXY;
 
            float3 cliffNormalXY = UnpackNormalWithScale(tex2D(_CliffNormal, IN.worldPos.xy * _CliffTexture_ST.xy), _CliffNormalStrength);
            float3 cliffNormalYZ = UnpackNormalWithScale(tex2D(_CliffNormal, IN.worldPos.yz * _CliffTexture_ST.xy), _CliffNormalStrength);
            float3 cliffNormal = vec.x * cliffNormalYZ + vec.z * cliffNormalXY;
 
            col = lerp(cliffColor, col, threshold);
            o.Normal = lerp(cliffNormal, o.Normal, threshold);
            o.Specular = lerp(_Specular, o.Specular, threshold);
            o.Gloss = lerp(_CliffMetallic, o.Gloss, threshold);
 
            o.Albedo = col.rgb;
            o.Alpha = col.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}