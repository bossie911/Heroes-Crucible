Shader "Hidden/Custom/LineRendererReplacement"
{
    Properties
    {
      _Color ("Color", Color) = (1,1,1,1)
      _MainTex ("Comic Texture", 2D) = "white" {}
      _EmissionColor ("Emission Color", Color) = (1,1,1,1)
      _EmissionMap ("Texture", 2D) = "white" {}
    }
    CGINCLUDE
        #pragma vertex vert
        #include "UnityCG.cginc"
    
        struct appdata
        {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
            float2 controlUV : TEXCOORD1;
        }; 

        struct v2f
        {
            float4 vertex : SV_POSITION;
            float2 uv : TEXCOORD0;
            float2 controlUV : TEXCOORD1;
        };

        half4 _Color;
        sampler2D _MainTex;
        float4 _MainTex_ST;
        half4 _EmissionColor;
        sampler2D _EmissionMap;
        sampler2D _Control;
        float4 _Control_ST;
         
        v2f vert (appdata v)
        {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.uv = TRANSFORM_TEX(v.uv, _MainTex);
            o.controlUV = TRANSFORM_TEX(v.controlUV, _Control);
            return o;
        }

        fixed4 FlatColor(v2f i)
        {
            half4 mainTex = tex2D(_MainTex, i.uv);
            return fixed4((_Color * mainTex.rgb) * min(mainTex.a, _Color.a), 0);
        }
    ENDCG
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma fragment frag
          
            fixed4 frag (v2f i) : SV_Target
            {
                return FlatColor(i);
            }
            ENDCG
        }
    }
    SubShader
    {
        Tags { "RenderType"="Terrain" }
        Pass
        {
            CGPROGRAM
            #pragma fragment frag
          
            fixed4 frag (v2f i) : SV_Target
            {
                return FlatColor(i);
            }
            ENDCG
        }
    }
    SubShader
    {
        Tags { "RenderType"="TransparantCutout" }
        Pass
        {
            CGPROGRAM
            #pragma fragment frag
          
            fixed4 frag (v2f i) : SV_Target
            {
                return FlatColor(i);
            }
            ENDCG
        }
    }
    SubShader
    {
        Tags { "RenderType"="Background" }
        Pass
        {
            CGPROGRAM
            #pragma fragment frag
          
            fixed4 frag (v2f i) : SV_Target
            {
                return FlatColor(i);
            }
            ENDCG
        }
    }
    SubShader
    {
        Tags { "RenderType"="Overlay" }
        Pass
        {
            CGPROGRAM
            #pragma fragment frag
          
            fixed4 frag (v2f i) : SV_Target
            {
                return FlatColor(i);
            }
            ENDCG
        }
    }
    SubShader
    {
        Tags { "RenderType"="TreeOpaque" }
        Pass
        {
            CGPROGRAM
            #pragma fragment frag
          
            fixed4 frag (v2f i) : SV_Target
            {
                return FlatColor(i);
            }
            ENDCG
        }
    }
    SubShader
    {
        Tags { "RenderType"="TreeTransparentCutout" }
        Pass
        {
            CGPROGRAM
            #pragma fragment frag
          
            fixed4 frag (v2f i) : SV_Target
            {
                return FlatColor(i);
            }
            ENDCG
        }
    }
    SubShader
    {
        Tags { "RenderType"="TreeBillboard" }
        Pass
        {
            CGPROGRAM
            #pragma fragment frag
          
            fixed4 frag (v2f i) : SV_Target
            {
                return FlatColor(i);
            }
            ENDCG
        }
    }
    SubShader
    {
        Tags { "RenderType"="Grass" }
        Pass
        {
            CGPROGRAM
            #pragma fragment frag
          
            fixed4 frag (v2f i) : SV_Target
            {
                return FlatColor(i);
            }
            ENDCG
        }
    }
    SubShader
    {
        Tags { "RenderType"="GrassBillboard" }
        Pass
        {
            CGPROGRAM
            #pragma fragment frag
          
            fixed4 frag (v2f i) : SV_Target
            {
                return FlatColor(i);
            }
            ENDCG
        }
    }
}
