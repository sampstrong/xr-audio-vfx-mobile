Shader "SamStrong/BasicFresnelStriped"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [HDR] _Color ("Color", Color) = (1,1,1,1)
        _FresnelIntensity ("Fresnel Intensity", Range(0, 10)) = 0.0
        _FresnelRamp ("Fresnel Ramp", Range(0, 10)) = 0.0
        _StripeDensity ("Stripe Density", Range(10, 100)) = 50.0
        _StripeSpeed ("Stripe Speed", Range(0, 20)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "IgnoreProjector"="True" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Front
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
                float3 worldPos : TEXCOORD3;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _FresnelIntensity, _FresnelRamp;
            float _StripeDensity, _StripeSpeed;
            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = normalize(WorldSpaceViewDir(v.vertex));
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float fresnelAmount = 1 - max(0.0, dot(i.normal, i.viewDir));
                fresnelAmount = pow(fresnelAmount, _FresnelRamp) * _FresnelIntensity;

                float stripes = sin((i.uv.y * _StripeDensity) + (_Time.y * _StripeSpeed)) * 0.5 + 0.5;

                fresnelAmount *= stripes;

                fresnelAmount = clamp(fresnelAmount, 0.0, 1.0);

                fresnelAmount *= _Color;

                // float4 worldPosColor = float4(i.worldPos.x, i.worldPos.y, i.worldPos.z, 1.0);
                // fresnelAmount *= worldPosColor;
                
                return fresnelAmount;
            }
            ENDCG
        }
    }
}
