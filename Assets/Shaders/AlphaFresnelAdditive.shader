Shader "SamStrong/AlphaFresnelAdditive"
{
    Properties
    {
        _FresnelColor ("Fresnel Color", Color) = (1,1,1,1)
        _Power ("Power", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        
        Blend One One

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            struct VertexInput
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv0 : TEXCOORD0;
            };

            struct VertexOutput
            {
                float4 clipSpacePos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            float4 _FresnelColor;
            float _Power;

            VertexOutput vert (VertexInput v)
            {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normal = v.normal;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.clipSpacePos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float4 frag (VertexOutput o) : SV_Target
            {
                float2 uv = o.uv0;

                // Normalize normal vector
                float3 normal = normalize(o.normal);

                // Get View Direction Vector and Normalize
                float3 camPos = _WorldSpaceCameraPos;
                float3 fragToCam = camPos - o.worldPos;
                float3 viewDir = normalize(fragToCam);
                
                // Create Fresnel
                float3 fresnel = clamp(max(0.0, dot(viewDir, normal) * _Power), 0.0, 1.0);
                float3 invertedFresnel = 1 - float4(fresnel.rgb, 1.0);

                return float4(invertedFresnel * _FresnelColor, 1.0);
            }
            ENDCG
        }
    }
}
