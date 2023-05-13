Shader "Raymarch/OrbsSRPBatcher"
{
    Properties
    {
    	[Header(Main Colors)]
    	[Space]
    	_BaseColor ("Base Color", Color) = (0.25, 0.44, 0.6, 1.0)
    	[HDR] _GlowColor ("Glow Color", Color) = (1.5,0.0,0.0,1)
    	
    	[Header(Raymarching)]
    	[Space]
    	_GyroidThickness ("Gyroid Thickness", Range(0.0, 0.1)) = 0.1
    	_GyroidSmoothAmount ("Gyroid Smooth Amount", Float) = 0.04
    	
    	[Header(Lighting)]
    	[Space]
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
            #include "AutoLight.cginc"
            #include "Assets/DynamicOrbs/cginc/raymarching.cginc"

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
                float4 vertex : SV_POSITION;
            	float3 ro : TEXCOORD1;
            	float3 hitPos : TEXCOORD2;
            };

            CBUFFER_START(UnityPerMaterial)
            
            fixed4 _BaseColor, _GlowColor;

            fixed4 _MainLightColor;
            fixed4 _MainLightPos;
            fixed _AmbientLight = 0.21;

            fixed _GyroidThickness;
            fixed _GyroidSmoothAmount;
            fixed _GyroidExtrusion = 1;

            uniform int _NumberOfObjects;
            uniform half4 _Positions[8];
            uniform half _Sizes[8];
            uniform fixed4 _Colors[8];
            
			CBUFFER_END

            
			CBUFFER_START(UnityPerDraw)

            //float4x4 unity_ObjectToWorld;
            
			CBUFFER_END


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
            	o.ro = _WorldSpaceCameraPos; // world space
            	o.hitPos = mul(unity_ObjectToWorld, v.vertex); // world space
                return o;
            }


            // ------ lighting ------
            
            fixed3 diff(float3 n)
            {
            	float3 ld = _MainLightPos;
            	float3 lc = _MainLightColor;
                fixed lf = max(0, dot(ld, n));
                float3 d = lc * lf;

                return d;
            }

            fixed3 amb(float3 n)
            {
            	float3 a = _AmbientLight;
                return a;
            }

            fixed3 spec(float3 n, float3 wp)
            {
                fixed3 c = _WorldSpaceCameraPos;
                fixed3 f = c - wp;
                fixed3 vd = normalize(f);
                fixed3 vr = reflect(-vd, n);
            	
            	fixed s = max(0, dot(vr, _MainLightPos));
                s = pow(s, GLOSS);

                return s;
            }

            fixed3 light(float3 n, float3 wp)
            {
	            fixed3 d = diff(n);
                fixed3 a = amb(n);
                fixed3 s = spec(n, wp);
            	fixed3 ds = s * _MainLightColor;
                fixed3 dl = a + d;
                fixed3 l = dl * _BaseColor.rgb + ds;

            	return l;
            }


            
            // ------ distance functions ------

            
            half sphere(float3 p, float r, float3 wp)
            {
				p -= wp;
            	
	            float d = length(p) - r;
            	return d;
            }

            half gyroid(float3 p, float3 wp)
            {
				p -= wp;
            	
            	half r = GYROID_SCALE;
	            p *= r;
				
            	half thickness = _GyroidThickness;

				p.y += _Time.y; // animate
            	
            	half gyroid = abs(0.7 * dot(sin(p), cos(p.yzx)) / r) - thickness;
            	
            	return gyroid;
            }
            

            half sGyroid(float3 p, float r, float3 wp)
            {
	            // sphere shell gyroid
            	half s = sphere(p, r, wp);
            	s = abs(s) - _GyroidThickness;
				half g = gyroid(p, float3(0,0,0));
            	half k = _GyroidSmoothAmount;
            	s = smin(s, g, -k);

            	return s;
            }
            

			half orb(float3 p, float r, float3 wp)
            {
	            // main sphere
            	half d = sphere(p, r, wp);

            	// gyroid ridges
            	half s = sGyroid(p, r, wp);

            	// combined
            	half k = _GyroidSmoothAmount;
				d = smin(s, d, k);

            	return d;
            }

            
                        
			// ------ raymarching ------

			half4 dist(half3 p)
            {
            	half4 d = 0.0;
            	half4 ld = 0.0;
	   
            	if (_NumberOfObjects <= 0) return 1.0;
            	
            	for (int i = 0; i < _NumberOfObjects; i++)
            	{
            		half4 s = half4(_Colors[i].rgb, orb(p, _Sizes[i], _Positions[i].xyz));
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
            

            half4 march(half3 ro, half3 rd) {
				half4 dO = 0;
				half4 dS;
				for (int i = 0; i < MAX_STEPS; i++) {
					half3 p = ro + rd * dO.w;
					dS = dist(p);
					dO += dS;
					if (dS.w<SURF_DIST || dO.w>MAX_DIST) break;
				}
				return dO;
			}
            

            half fres(half3 n, half3 rd)
            {
				half3 vd = normalize(rd);
            	
	            half f = 1 - max(0.0, dot(n, vd));
                f = pow(f, F_RAMP) * F_INTENSITY;

            	return f;
            }

			half3 norm(half3 p) {
				half2 e = half2(1e-2, 0);

				half3 n = dist(p).w - half3(
					dist(p-e.xyy).w,
					dist(p-e.yxy).w,
					dist(p-e.yyx).w
				);

				return normalize(n);
			}

			
            fixed sss(half3 p, half3 n, half3 ro)
            {
            	fixed f = fres(n, ro);
            	fixed s = smoothstep(0.7, 0.0, f);

                fixed b = gyroid(p, half3(0,0,0));
                s *= smoothstep(0.0, 0.2, b);
            	fixed w = abs(sin(p.z * 50 + _Time.y * 2.0));
            	s *= smoothstep(-0.5, 1, w);
            	
            	return s;
            }



            fixed4 frag (v2f i) : SV_Target
            {
                half3 ro = i.ro; 
                half3 rd = normalize(i.hitPos - ro);
            	
                fixed4 col = 0;

            	half4 d = march(ro, rd);

                if (d.w < MAX_DIST)
                {
                    half3 p = ro + rd * d.w;
                    half3 n = norm(p);
					half3 l = light(n, p);
                	// float3 l = getDiffuseLight(n); // option for better performance
                	
					// sub surface scattering
					fixed s = sss(p, n, ro - p);
                	
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
