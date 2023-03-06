Shader "Unlit/PixelGlitch"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _WireframeTex ("Wireframe Texture", 2D) = "white" {}
        [HDR] _WireframeColor ("Wireframe Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        Blend One One
        LOD 200
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv1 : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
            };

            struct v2f
            {
                float2 uv1 : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                UNITY_FOG_COORDS(2)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _WireframeTex;
            float4 _WireframeTex_ST;
            float4 _WireframeColor;

            v2f vert (appdata v)
            {
                v2f o;
                v.uv2 = v.uv1;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv1 = TRANSFORM_TEX(v.uv1, _MainTex);
                o.uv2 = TRANSFORM_TEX(v.uv2, _WireframeTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //i.uv2 *= 2;
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv1);
                fixed4 col2 = tex2D(_WireframeTex, i.uv2);

                if (col2.a > 0.0)
                {
                    col2 *= _WireframeColor;
                }
                
                float4 final;

                if (col2.a > 0.0)
                {
                    final = col2;
                }
                else
                {
                    final = col;
                }
                
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return final;
            }
            ENDCG
        }
    }
}
