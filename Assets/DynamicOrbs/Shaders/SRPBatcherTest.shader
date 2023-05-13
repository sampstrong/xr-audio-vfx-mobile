Shader "Unlit/SRPBatcherTest"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            
            
            CGPROGRAM

            
            //CBUFFER_START(UnityPerDraw)
            //    float4x4 unity_ObjectToWorld;
            //    float4x4 unity_WorldToObject;
            //    float4 unity_LODFade;
            //    real4 unity_WorldTransformParams;
            //CBUFFER_END
            

            
            ENDCG
        }
    }
}
