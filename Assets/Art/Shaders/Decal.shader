Shader "Projector/Decal"
{
    Properties
    {
        _Color("Decal Color", Color) = (1,1,1,1)
        _Texture("Decal texture", 2D) = "" {}
    }
    
    Subshader
    {
        Tags { "RenderType"="Transparent"  "Queue"="Transparent+100"}
        Pass
        {
            ZWrite Off
            Offset -1, -1
            Fog { Mode Off }
            ColorMask RGB
            Blend SrcAlpha OneMinusSrcAlpha
            Stencil
            {
                Ref 2
                Comp NotEqual
                Pass Replace
            }
 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct v2f
            {
                float4 pos      : SV_POSITION;
                float4 uv       : TEXCOORD0;
            };
            
            sampler2D _Texture;
            float4x4 unity_Projector;
            float4 _Color;
            
            v2f vert(appdata_tan v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos (v.vertex);
                o.uv = mul (unity_Projector, v.vertex);
                return o;
            }
            
            half4 frag (v2f i) : COLOR
            {
                half4 tex = tex2Dproj(_Texture, UNITY_PROJ_COORD(i.uv));

                clip(tex.a - 0.5);
                tex *= _Color;
                tex.a = _Color.a;
                return tex;
            }
            ENDCG    
        }
    }
}