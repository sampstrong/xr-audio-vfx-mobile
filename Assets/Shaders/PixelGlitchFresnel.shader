Shader "SamStrong/PixelGlitchFresnel"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _WireframeTex ("Wireframe Texture", 2D) = "white" {}
        [HDR] _WireframeColor ("Wireframe Color", Color) = (1,1,1,1)
        [HDR] _FresnelColor ("Fresnel Color", Color) = (1,1,1,1)
        _FresnelIntensity ("Fresnel Intensity", Range(0, 10)) = 0.0
        _FresnelRamp ("Fresnel Ramp", Range(0, 10)) = 0.0
        _BackFaceMin ("Back Face Min", Range(0, 1)) = 0.05
        _OpacityMultiplier ("Opacity Multiplier", Range(0, 1)) = 1.0
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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv1 : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                float3 normal : TEXCOORD2;
                float3 worldPos : TEXCOORD3;
                UNITY_FOG_COORDS(4)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _WireframeTex;
            float4 _WireframeTex_ST;
            float4 _WireframeColor;
            float4 _FresnelColor;
            float _FresnelIntensity, _FresnelRamp, _BackFaceMin;
            float _OpacityMultiplier;

            v2f vert (appdata v)
            {
                v2f o;
                v.uv2 = v.uv1;
                o.normal = v.normal;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.uv1 = TRANSFORM_TEX(v.uv1, _MainTex);
                o.uv2 = TRANSFORM_TEX(v.uv2, _WireframeTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                // Get normal and view vectors
                float3 normal = normalize(i.normal);
                float3 camPos = _WorldSpaceCameraPos;
                float3 fragToCam = camPos - i.worldPos;
                float3 viewDir = normalize(fragToCam);

                // Fresnel
                float fresnelAmount = 1 - max(0.0, dot(viewDir, normal));
                fresnelAmount = pow(fresnelAmount, _FresnelRamp) * _FresnelIntensity;
                float4 fresColor = float4(fresnelAmount * _FresnelColor);

                float reverseFresnelAmount = 1 - max(0.0, dot(-viewDir, normal));
                reverseFresnelAmount = pow(reverseFresnelAmount, _FresnelRamp) * _FresnelIntensity;

                fresColor = clamp(fresColor * reverseFresnelAmount + _BackFaceMin, 0.0, 1.0);
                
                
                // Texture mapping
                fixed4 main = tex2D(_MainTex, i.uv1);
                fixed4 wireframe = tex2D(_WireframeTex, i.uv2);

                if (wireframe.a > 0.0)
                {
                    wireframe *= _WireframeColor;
                }
                
                float4 finalColor;

                if (wireframe.a > 0.0)
                {
                    finalColor = wireframe;
                }
                else
                {
                    finalColor = clamp(fresColor + main, 0.0, 1.0);
                }

                finalColor *= _OpacityMultiplier;
                
                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, finalColor);
                return finalColor;
            }
            ENDCG
        }
    }
}
