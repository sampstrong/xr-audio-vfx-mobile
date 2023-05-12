Shader "Raymarch/DynamicGlowingOrbsMultiColorOptimized"
{
    Properties
    {
    	[Header(Main Colors)]
    	[Space]
        // _MainTex ("Texture", 2D) = "white" {}
    	_BaseColor ("Base Color", Color) = (0.25, 0.44, 0.6, 1.0)
    	[HDR] _GlowColor ("Glow Color", Color) = (1.5,0.0,0.0,1)
    	// _FresnelIntensity ("Fresnel Intensity", Range(0, 10)) = 5.46
        // _FresnelRamp ("Fresnel Ramp", Range(0, 10)) = 0.65
    	
    	[Header(Raymarching)]
    	[Space]
    	// _NoiseScale ("Noise Scale", Float) = 106.3
    	// _SmoothAmount ("Smooth Amount", Range(0, 0.2)) = 0.1563
        // _GyroidScale ("Gyroid Scale", Float) = 15.0
    	// _GyroidExtrusion ("Gyroid Extrusion", Range(0.0, 5)) = 0.05
    	_GyroidThickness ("Gyroid Thickness", Range(0.0, 0.1)) = 0.1
    	_GyroidSmoothAmount ("Gyroid Smooth Amount", Float) = 0.04
    	
    	[Header(Lighting)]
    	[Space]
    	// _Gloss ("Gloss", Range(1, 100)) = 1.5
    	// _AmbientLight ("AmbientLight", Float) = 1
    	_MainLightColor ("Main Light Color", Color) = (1,1,1,1)
    	_MainLightPos ("Main Light Position", Vector) = (0,0,0,0)
    	
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            // #include "Lighting.cginc"
            #include "AutoLight.cginc"
            // #include "Assets/DynamicOrbs/cginc/noise.cginc"
            #include "Assets/DynamicOrbs/cginc/raymarching.cginc"
            
			// original values
			// #define MAX_STEPS 100
			// #define MAX_DIST 100
			// #define SURF_DIST 1e-3 // 0.001

            // new optimized values
            #define MAX_STEPS 30
			#define MAX_DIST 15
			#define SURF_DIST 1e-2 // 0.01

            // constant values for optimization
            #define GLOSS 2
            #define GYROID_SCALE 3.15
            #define SMOOTH 0.1563
            #define F_INTENSITY 0.85
            #define F_RAMP 0.77

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                //float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            	float3 ro : TEXCOORD1;
            	float3 hitPos : TEXCOORD2;
            };

            // sampler2D _MainTex;
            // float4 _MainTex_ST;
            // float _SmoothAmount;
            // float _FresnelRamp, _FresnelIntensity;
            fixed4 _BaseColor, _GlowColor;
            // float _NoiseScale;
            // float _Gloss;

            // float _GyroidScale;
            

            fixed4 _MainLightColor;
            float4 _MainLightPos;
            float _AmbientLight = 0.21;

            float _GyroidThickness;
            float _GyroidSmoothAmount;
            float _GyroidExtrusion = 1;
            

            uniform int _NumberOfObjects;
            uniform float4 _Positions[8];
            uniform float _Sizes[8];
            // uniform float4x4 _Rotations[8];
            uniform fixed4 _Colors[8];

            // uniform float4 _LightPos;
            // uniform fixed4 _LightCol;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                // o.uv = TRANSFORM_TEX(v.uv, _MainTex);
            	o.ro = _WorldSpaceCameraPos; // world space
            	o.hitPos = mul(unity_ObjectToWorld, v.vertex); // world space
                return o;
            }


            // ------ lighting ------
            
            float3 diff(float3 n)
            {
            	float3 ld = _MainLightPos;
            	float3 lc = _MainLightColor;
                float lf = max(0, dot(ld, n));
                float3 d = lc * lf;

                return d;
            }

            float3 amb(float3 n)
            {
            	float3 a = _AmbientLight;
                return a;
            }

            float3 spec(float3 n, float3 wp)
            {
                float3 c = _WorldSpaceCameraPos;
                float3 f = c - wp;
                float3 vd = normalize(f);
                float3 vr = reflect(-vd, n);
            	
            	float s = max(0, dot(vr, _MainLightPos));
                s = pow(s, GLOSS);

                return s;
            }

            float3 light(float3 n, float3 wp)
            {
	            float3 d = diff(n);
                float3 a = amb(n);
                float3 s = spec(n, wp);
            	float3 ds = s * _MainLightColor;
                float3 dl = a + d;
                float3 l = dl * _BaseColor.rgb + ds;

            	return l;
            }


            
            // ------ distance functions ------

            
            float sphere(float3 p, float r, float3 wp)
            {
				p -= wp;
            	
	            float d = length(p) - r;
            	return d;
            }

            float gyroid(float3 p, float3 wp)
            {
				p -= wp;
            	
            	float r = GYROID_SCALE;
	            p *= r;
            	// float thickness = (sin(_Time.y * 0.5) * 0.5 + 0.5) * _GyroidThickness;
            	float thickness = _GyroidThickness;

				p.y += _Time.y; // animate
            	
            	float gyroid = abs(0.7 * dot(sin(p), cos(p.yzx)) / r) - thickness;
            	
            	return gyroid;
            }
            

            float sGyroid(float3 p, float r, float3 wp)
            {
	            // sphere shell gyroid
            	float s = sphere(p, r, wp);
            	s = abs(s) - _GyroidThickness;
				float g = gyroid(p, float3(0,0,0));
            	float k = _GyroidSmoothAmount;
            	s = smin(s, g, -k);

            	return s;
            }
            

			float orb(float3 p, float r, float3 wp)
            {
	            // main sphere
            	float d = sphere(p, r, wp);

            	// gyroid ridges
            	float s = sGyroid(p, r, wp);

            	// combined
            	float k = _GyroidSmoothAmount;
				d = smin(s, d, k);

            	return d;
            }

            
                        
			// ------ raymarching ------

			float4 dist(float3 p)
            {
            	float4 d = 0.0;
            	float4 ld = 0.0;
	   
            	if (_NumberOfObjects <= 0) return 1.0;
            	
            	for (int i = 0; i < _NumberOfObjects; i++)
            	{
            		float4 s = float4(_Colors[i].rgb, orb(p, _Sizes[i], _Positions[i].xyz));
					if (i == 0)
					{
						ld = s;
						continue;
					}
	   
            		d = sminColor(ld, s, SMOOTH);
            		ld = d;
            	}
            	
				return d;
			}
            

            float4 march(float3 ro, float3 rd) {
				float4 dO = 0;
				float4 dS;
				for (int i = 0; i < MAX_STEPS; i++) {
					float3 p = ro + rd * dO.w;
					dS = dist(p);
					dO += dS;
					if (dS.w<SURF_DIST || dO.w>MAX_DIST) break;
				}
				return dO;
			}
            

            float fres(float3 n, float3 rd)
            {
				float3 vd = normalize(rd);
            	
	            float f = 1 - max(0.0, dot(n, vd));
                f = pow(f, F_RAMP) * F_INTENSITY;

            	return f;
            }

			float3 norm(float3 p) {
				float2 e = float2(1e-2, 0);

				float3 n = dist(p).w - float3(
					dist(p-e.xyy).w,
					dist(p-e.yxy).w,
					dist(p-e.yyx).w
				);

				return normalize(n);
			}

			
            float sss(float3 p, float3 n, float3 ro)
            {
            	float f = fres(n, ro);
            	float s = smoothstep(0.7, 0.0, f);

                float b = gyroid(p, float3(0,0,0));
                s *= smoothstep(0.0, 0.2, b);
            	float w = abs(sin(p.z * 50 + _Time.y * 2.0));
            	s *= smoothstep(-0.5, 1, w);
            	
            	return s;
            }



            fixed4 frag (v2f i) : SV_Target
            {
                float3 ro = i.ro; 
                float3 rd = normalize(i.hitPos - ro);
            	
                fixed4 col = 0;

            	float4 d = march(ro, rd);

                if (d.w < MAX_DIST)
                {
                    float3 p = ro + rd * d.w;
                    float3 n = norm(p);
					float3 l = light(n, p);
                	// float3 l = getDiffuseLight(n); // option for better performance
                	
					// sub surface scattering
					float s = sss(p, n, ro - p);
                	
                    col.rgb = l * _BaseColor;
                	col.rgb += s * d.rgb;
                }
                else
                {
	                 discard;
                }
                
                return col;
            }
            ENDCG
        }
    }
}
