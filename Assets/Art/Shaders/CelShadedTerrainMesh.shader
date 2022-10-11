Shader "Custom/CelShadedTerrainMesh"
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
		
		
				
		[NoScaleOffset]_V_T2M_Control ("Control Map (RGBA)", 2D) = "black" {}

		//TTM	
		[V_T2M_Layer] _V_T2M_Splat1 ("Layer 1 (R)", 2D) = "white" {}
		[HideInInspector] _V_T2M_Splat1_uvScale("", float) = 1	
		[HideInInspector] _V_T2M_Splat1_Glossiness("Smoothness", Range(0,1)) = 0.5
		[HideInInspector] _V_T2M_Splat1_Metallic("Metallic", Range(0,1)) = 0.0

		[V_T2M_Layer] _V_T2M_Splat2 ("Layer 2 (G)", 2D) = "white" {}
		[HideInInspector] _V_T2M_Splat2_uvScale("", float) = 1	
		[HideInInspector] _V_T2M_Splat2_Glossiness("Smoothness", Range(0,1)) = 0.5
		[HideInInspector] _V_T2M_Splat2_Metallic("Metallic", Range(0,1)) = 0.0

		[V_T2M_Layer] _V_T2M_Splat3 ("Layer 3 (B)", 2D) = "white" {}
		[HideInInspector] _V_T2M_Splat3_uvScale("", float) = 1	
		[HideInInspector] _V_T2M_Splat3_Glossiness("Smoothness", Range(0,1)) = 0.5
		[HideInInspector] _V_T2M_Splat3_Metallic("Metallic", Range(0,1)) = 0.0

		[V_T2M_Layer] _V_T2M_Splat4 ("Layer 4 (A)", 2D) = "white" {}
		[HideInInspector] _V_T2M_Splat4_uvScale("", float) = 1	
		[HideInInspector] _V_T2M_Splat4_Glossiness("Smoothness", Range(0,1)) = 0.5
		[HideInInspector] _V_T2M_Splat4_Metallic("Metallic", Range(0,1)) = 0.0



		//Fallback use only
		_Color("Color (Fallback use only!)", color) = (1, 1, 1, 1)
		[NoScaleOffset]_MainTex("BaseMap (Fallback use only!)", 2D) = "white" {}
	}

	SubShader 
	{ 
		Tags { "RenderType"="Terrain" }
		LOD 200
		
		CGPROGRAM
		// Physically based Cel lighting model, and enable shadows on all light types
		#pragma surface surf Cel fullforwardshadows vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0


		/// LIGHTING
		
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
        


		#define V_T2M_STANDARD
		#define V_T2M_3_TEX
		#define V_T2M_4_TEX
		
		#ifndef VACUUM_SHADERS_T2M_DEFERRED_CGINC
		#define VACUUM_SHADERS_T2M_DEFERRED_CGINC

		#include "Assets/VacuumShaders/Terrain To Mesh/Shaders/cginc/T2M_Variables.cginc"
		#include "Assets/VacuumShaders/Terrain To Mesh/Shaders/cginc/CurvedWorld.cginc"


		struct Input 
		{
			float2 uv_V_T2M_Control;

            float3 worldPos;
            float3 worldNormal;
            INTERNAL_DATA
			#ifdef V_T2M_2_CONTROL_MAPS
				float2 uv_V_T2M_Control2;
			#endif
		};


		void vert (inout appdata_full v, out Input o) 
		{
			UNITY_INITIALIZE_OUTPUT(Input,o); 

			//CurvedWorld vertex transform
			CURVED_WORLD_TRANSFORM_POINT_AND_NORMAL(v.vertex, v.normal, v.tangent);
		}

		
        sampler2D _CliffTexture;
        float4 _CliffTexture_ST;
        sampler2D _CliffNormal;
        float _NormalThreshold;

        float _CliffNormalStrength;
        float _CliffMetallic;
        float _CliffSmoothness;
        float _TextureBlend;


		#ifdef V_T2M_STANDARD
		void surf (Input IN, inout SurfaceOutput o)
		#else
		void surf (Input IN, inout SurfaceOutput o) 
		#endif
		{
			
            half3 normal = o.Normal;
			
			half4 splat_control = tex2D (_V_T2M_Control, IN.uv_V_T2M_Control);

			// fixed4 mainTex  = splat_control.r * tex2D (_V_T2M_Splat1, IN.uv_V_T2M_Control * _V_T2M_Splat1_uvScale);
			//        mainTex += splat_control.g * tex2D (_V_T2M_Splat2, IN.uv_V_T2M_Control * _V_T2M_Splat2_uvScale);
			fixed4 mainTex  = smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splat_control.r) * tex2D (_V_T2M_Splat1, IN.uv_V_T2M_Control * _V_T2M_Splat1_uvScale);
			       mainTex += smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splat_control.g) * tex2D (_V_T2M_Splat2, IN.uv_V_T2M_Control * _V_T2M_Splat2_uvScale);
			
			#ifdef V_T2M_3_TEX
				//mainTex += splat_control.b * tex2D (_V_T2M_Splat3, IN.uv_V_T2M_Control * _V_T2M_Splat3_uvScale);   // ASSET
				mainTex += smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splat_control.b) * tex2D (_V_T2M_Splat3, IN.uv_V_T2M_Control * _V_T2M_Splat3_uvScale);
				//mainTex += smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splat_control.b) * tex2D (_Splat0, IN.uv_Control * _Splat0_ST.xy); // OLD
			#endif
			#ifdef V_T2M_4_TEX
				//mainTex += splat_control.a * tex2D (_V_T2M_Splat4, IN.uv_V_T2M_Control * _V_T2M_Splat4_uvScale);
				mainTex += smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splat_control.a) * tex2D (_V_T2M_Splat4, IN.uv_V_T2M_Control * _V_T2M_Splat4_uvScale);
			#endif


			#ifdef V_T2M_2_CONTROL_MAPS
				 half4 splat_control2 = tex2D (_V_T2M_Control2, IN.uv_V_T2M_Control2);

				 //mainTex.rgb += tex2D (_V_T2M_Splat5, IN.uv_V_T2M_Control2 * _V_T2M_Splat5_uvScale) * splat_control2.r;
				 mainTex.rgb += tex2D (_V_T2M_Splat5, IN.uv_V_T2M_Control2 * _V_T2M_Splat5_uvScale) * smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splat_control2.r);

				 #ifdef V_T2M_6_TEX
					//mainTex.rgb += tex2D (_V_T2M_Splat6, IN.uv_V_T2M_Control2 * _V_T2M_Splat6_uvScale) * splat_control2.g;
					mainTex.rgb += tex2D (_V_T2M_Splat6, IN.uv_V_T2M_Control2 * _V_T2M_Splat6_uvScale) * smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splat_control2.g);
				 #endif

				 #ifdef V_T2M_7_TEX
					//mainTex.rgb += tex2D (_V_T2M_Splat7, IN.uv_V_T2M_Control2 * _V_T2M_Splat7_uvScale) * splat_control2.b;
					mainTex.rgb += tex2D (_V_T2M_Splat7, IN.uv_V_T2M_Control2 * _V_T2M_Splat7_uvScale) * smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splat_control2.b);
				 #endif

				 #ifdef V_T2M_8_TEX
					//mainTex.rgb += tex2D (_V_T2M_Splat8, IN.uv_V_T2M_Control2 * _V_T2M_Splat8_uvScale) * splat_control2.a;
					mainTex.rgb += tex2D (_V_T2M_Splat8, IN.uv_V_T2M_Control2 * _V_T2M_Splat8_uvScale) * smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splat_control2.a);
				 #endif
			#endif



			//mainTex.rgb *= _Color.rgb;

			 
			#ifdef V_T2M_BUMP
				fixed4 nrm = 0.0f;
				// nrm += splat_control.r * tex2D(_V_T2M_Splat1_bumpMap, IN.uv_V_T2M_Control * _V_T2M_Splat1_uvScale);
				// nrm += splat_control.g * tex2D(_V_T2M_Splat2_bumpMap, IN.uv_V_T2M_Control * _V_T2M_Splat2_uvScale);
				nrm += smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splat_control.r) * tex2D(_V_T2M_Splat1_bumpMap, IN.uv_V_T2M_Control * _V_T2M_Splat1_uvScale);
				nrm += smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splat_control.g) * tex2D(_V_T2M_Splat2_bumpMap, IN.uv_V_T2M_Control * _V_T2M_Splat2_uvScale);

				#ifdef V_T2M_3_TEX
					// nrm += splat_control.b * tex2D (_V_T2M_Splat3_bumpMap, IN.uv_V_T2M_Control * _V_T2M_Splat3_uvScale);
					nrm += smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splat_control.b) * tex2D (_V_T2M_Splat3_bumpMap, IN.uv_V_T2M_Control * _V_T2M_Splat3_uvScale);
				#endif

				#ifdef V_T2M_4_TEX
					// nrm += splat_control.a * tex2D (_V_T2M_Splat4_bumpMap, IN.uv_V_T2M_Control * _V_T2M_Splat4_uvScale);
					nrm += smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splat_control.a) * tex2D (_V_T2M_Splat4_bumpMap, IN.uv_V_T2M_Control * _V_T2M_Splat4_uvScale);
				#endif
				 
				 
				o.Normal = UnpackNormal(nrm);
			#endif


			

			#ifdef V_T2M_STANDARD
				half metallic = 0;
				// metallic += splat_control.r * _V_T2M_Splat1_Metallic;
				// metallic += splat_control.g * _V_T2M_Splat2_Metallic;
				metallic += smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splat_control.r) * _V_T2M_Splat1_Metallic;
				metallic += smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splat_control.g) * _V_T2M_Splat2_Metallic;
				#ifdef V_T2M_3_TEX
					// metallic += splat_control.b * _V_T2M_Splat3_Metallic;
					metallic += smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splat_control.b) * _V_T2M_Splat3_Metallic;
				#endif
				#ifdef V_T2M_4_TEX
					// metallic += splat_control.a * _V_T2M_Splat4_Metallic;
					metallic += smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splat_control.a) * _V_T2M_Splat4_Metallic;
				#endif
				#ifdef V_T2M_2_CONTROL_MAPS
					#ifdef V_T2M_5_TEX
						// metallic += splat_control2.r * _V_T2M_Splat5_Metallic;
						metallic += smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splat_control2.r) * _V_T2M_Splat5_Metallic;
					#endif
					#ifdef V_T2M_6_TEX
						// metallic += splat_control2.g * _V_T2M_Splat6_Metallic;
						metallic += smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splat_control2.g) * _V_T2M_Splat6_Metallic;
					#endif
					#ifdef V_T2M_7_TEX
						//metallic += splat_control2.b * _V_T2M_Splat7_Metallic;
						metallic += smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splat_control2.b) * _V_T2M_Splat7_Metallic;
					#endif
					#ifdef V_T2M_8_TEX
						//metallic += splat_control2.a * _V_T2M_Splat8_Metallic;
						metallic += smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splat_control2.a) * _V_T2M_Splat8_Metallic;
					#endif
				#endif


				half glossiness = 0;
				// glossiness += splat_control.r * _V_T2M_Splat1_Glossiness;
				// glossiness += splat_control.g * _V_T2M_Splat2_Glossiness;
				glossiness += smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splat_control.r) * _V_T2M_Splat1_Glossiness;
				glossiness += smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splat_control.g) * _V_T2M_Splat2_Glossiness;
				#ifdef V_T2M_3_TEX
					//glossiness += splat_control.b * _V_T2M_Splat3_Glossiness;
					glossiness += smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splat_control.b) * _V_T2M_Splat3_Glossiness;
				#endif
				#ifdef V_T2M_4_TEX
					//glossiness += splat_control.a * _V_T2M_Splat4_Glossiness;
					glossiness += smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splat_control.a) * _V_T2M_Splat4_Glossiness;
				#endif
				#ifdef V_T2M_2_CONTROL_MAPS
					#ifdef V_T2M_5_TEX
						//glossiness += splat_control2.r * _V_T2M_Splat5_Glossiness;
						glossiness += smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splat_control2.r) * _V_T2M_Splat5_Glossiness;
					#endif
					#ifdef V_T2M_6_TEX
						//glossiness += splat_control2.g * _V_T2M_Splat6_Glossiness;
						glossiness += smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splat_control2.g) * _V_T2M_Splat6_Glossiness;
					#endif
					#ifdef V_T2M_7_TEX
						//glossiness += splat_control2.b * _V_T2M_Splat7_Glossiness;
						glossiness += smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splat_control2.b) * _V_T2M_Splat7_Glossiness;
					#endif
					#ifdef V_T2M_8_TEX
						//glossiness += splat_control2.a * _V_T2M_Splat8_Glossiness;
						glossiness += smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splat_control2.a) * _V_T2M_Splat8_Glossiness;
					#endif
				#endif

				o.Specular = metallic;
				o.Gloss = glossiness;
			#else
				#ifdef V_T2M_SPECULAR
					o.Gloss = mainTex.a;

					half shininess = 0;
					// shininess += splat_control.r * _V_T2M_Splat1_Shininess;
					// shininess += splat_control.g * _V_T2M_Splat2_Shininess;
					shininess += smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splat_control.r) * _V_T2M_Splat1_Shininess;
					shininess += smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splat_control.g) * _V_T2M_Splat2_Shininess;
					#ifdef V_T2M_3_TEX
						//shininess += smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splat_control.b) * _V_T2M_Splat3_Shininess;
						shininess += smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splat_control.b) * _V_T2M_Splat3_Shininess;
					#endif
					#ifdef V_T2M_4_TEX
						//shininess += smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splat_control.a) * _V_T2M_Splat4_Shininess;
						shininess += smoothstep(0.5 - _TextureBlend, 0.5 + _TextureBlend, splat_control.a) * _V_T2M_Splat4_Shininess;
					#endif

					o.Specular = shininess;
				#endif
			#endif
			
			o.Albedo = mainTex.rgb;
			o.Alpha = 1.0;

		
	        // Determine cliff texture
	        float3 vec = abs(WorldNormalVector (IN, normal));
	        float threshold =  smoothstep(_NormalThreshold - _TextureBlend, _NormalThreshold + _TextureBlend, abs(dot(vec, float3(0, 1, 0))));
	        fixed4 cliffColorXY = tex2D(_CliffTexture, IN.worldPos.xy * _CliffTexture_ST.xy);
	        fixed4 cliffColorYZ = tex2D(_CliffTexture, IN.worldPos.yz * _CliffTexture_ST.xy);
	        fixed4 cliffColor = vec.x * cliffColorYZ + vec.z * cliffColorXY;

	        float3 cliffNormalXY = UnpackNormalWithScale(tex2D(_CliffNormal, IN.worldPos.xy * _CliffTexture_ST.xy), _CliffNormalStrength);
	        float3 cliffNormalYZ = UnpackNormalWithScale(tex2D(_CliffNormal, IN.worldPos.yz * _CliffTexture_ST.xy), _CliffNormalStrength);
	        float3 cliffNormal = vec.x * cliffNormalYZ + vec.z * cliffNormalXY;

	        mainTex = lerp(cliffColor, mainTex, threshold);
	        o.Normal = lerp(cliffNormal, o.Normal, threshold);
	        o.Specular = lerp(_Specular, o.Specular, threshold);
	        o.Gloss = lerp(_CliffMetallic, o.Gloss, threshold);

	        o.Albedo = mainTex.rgb;
	        o.Alpha = mainTex.a;
		}

		#endif	

		ENDCG
	} 

	FallBack "Hidden/VacuumShaders/Fallback/VertexLit"
}
