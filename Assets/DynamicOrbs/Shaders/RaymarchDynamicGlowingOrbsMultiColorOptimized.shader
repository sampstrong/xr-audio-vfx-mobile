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
            #define MAX_STEPS 35
			#define MAX_DIST 20
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
            uniform float4x4 _Rotations[8];
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
            
            float3 getDiffuseLight(float3 normal)
            {
            	float3 lightDir = _MainLightPos;
            	float3 lightColor = _MainLightColor;
                float lightFalloff = max(0, dot(lightDir, normal));
                float3 directDiffuseLight = lightColor * lightFalloff;

                return directDiffuseLight;
            }

            float3 getAmbientLight(float3 normal)
            {
            	float3 a = _AmbientLight;
                return a;
            }

            float3 getSpecularLight(float3 normal, float3 worldPos)
            {
                float3 camPos = _WorldSpaceCameraPos;
                float3 fragToCam = camPos - worldPos;
                float3 viewDir = normalize(fragToCam);
                float3 viewReflect = reflect(-viewDir, normal);
            	
            	float specularFalloff = max(0, dot(viewReflect, _MainLightPos));
                specularFalloff = pow(specularFalloff, GLOSS);

                return specularFalloff;
            }

            float3 applyLighting(float3 normal, float3 worldPos)
            {
	            float3 diffuse = getDiffuseLight(normal);
                float3 ambient = getAmbientLight(normal);
                float3 specular = getSpecularLight(normal, worldPos);
            	float3 directSpecular = specular * _MainLightColor;
                float3 diffuseLight = ambient + diffuse;
                float3 finalSurfaceColor = diffuseLight * _BaseColor.rgb + directSpecular;

            	return finalSurfaceColor;
            }


            
            // ------ distance functions ------

            
            float sphere(float3 p, float r, float3 worldPos, float4x4 rotMatrix)
            {
				p -= worldPos;
            	p = mul(p, rotMatrix);
            	
	            float d = length(p) - r;
            	return d;
            }

            float gyroid(float3 p, float3 worldPos, float4x4 rotMatrix)
            {
				p -= worldPos;
            	p = mul(p, rotMatrix);
            	
            	float rescaleFactor = GYROID_SCALE;
	            p *= rescaleFactor;
            	// float thickness = (sin(_Time.y * 0.5) * 0.5 + 0.5) * _GyroidThickness;
            	float thickness = _GyroidThickness;

				p.y += _Time.y; // animate
            	
            	float gyroid = abs(0.7 * dot(sin(p), cos(p.yzx)) / rescaleFactor) - thickness;
            	
            	return gyroid;
            }
            

            float ballGyroidHollowUniversal(float3 p, float r, float3 worldPos, float4x4 rotMatrix)
            {
	            // sphere shell gyroid
            	float s = sphere(p, r, worldPos, rotMatrix);
            	s = abs(s) - _GyroidThickness;
				float g = gyroid(p, float3(0,0,0), _Rotations[0]);
            	float k = _GyroidSmoothAmount;
            	s = smin(s, g, -k);

            	return s;
            }
            

			float orb(float3 p, float r, float3 worldPos, float4x4 rotMatrix)
            {
	            // main sphere
            	float d = sphere(p, r, worldPos, rotMatrix);

            	// gyroid ridges
            	float s = ballGyroidHollowUniversal(p, r, worldPos, _Rotations[0]);

            	// combined
            	float k = _GyroidSmoothAmount;
				d = smin(s, d, k);

            	return d;
            }

            
                        
			// ------ raymarching ------

			float4 getDistColor(float3 p)
            {
            	float4 d = 0.0;
            	float4 lastDist = 0.0;
	   
            	if (_NumberOfObjects <= 0) return 1.0;
            	
            	for (int i = 0; i < _NumberOfObjects; i++)
            	{
            		float4 s = float4(_Colors[i].rgb, orb(p, _Sizes[i], _Positions[i].xyz, _Rotations[i]));
					if (i == 0)
					{
						lastDist = s;
						continue;
					}
	   
            		d = sminColor(lastDist, s, SMOOTH);
            		lastDist = d;
            	}
            	
				return d;
			}
            

            float4 rayMarchColor(float3 ro, float3 rd) {
				float4 dO = 0;
				float4 dS;
				for (int i = 0; i < MAX_STEPS; i++) {
					float3 p = ro + rd * dO.w;
					dS = getDistColor(p);
					dO += dS;
					if (dS.w<SURF_DIST || dO.w>MAX_DIST) break;
				}
				return dO;
			}
            

            float getFresnel(float3 normal, float3 rd)
            {
				float3 viewDir = normalize(rd);
            	
	            float fresnelAmount = 1 - max(0.0, dot(normal, viewDir));
                fresnelAmount = pow(fresnelAmount, F_RAMP) * F_INTENSITY;

            	return fresnelAmount;
            }

			float3 getNormal(float3 p) {
				float2 e = float2(1e-2, 0);

				float3 n = getDistColor(p).w - float3(
					getDistColor(p-e.xyy).w,
					getDistColor(p-e.yxy).w,
					getDistColor(p-e.yyx).w
				);

				return normalize(n);
			}

			
            float subSurfaceScattering(float3 p, float3 n, float3 ro)
            {
            	float f = getFresnel(n, ro);
            	float sss = smoothstep(0.7, 0.0, f);

                float b = gyroid(p, float3(0,0,0), _Rotations[0]);
                sss *= smoothstep(0.0, 0.2, b);
            	float s = abs(sin(p.z * 50 + _Time.y * 2.0));
            	sss *= smoothstep(-0.5, 1, s);
            	
            	return sss;
            }



            fixed4 frag (v2f i) : SV_Target
            {
                float3 ro = i.ro; 
                float3 rd = normalize(i.hitPos - ro);
            	
                fixed4 col = 0;

            	float4 d = rayMarchColor(ro, rd);

                if (d.w < MAX_DIST)
                {
                    float3 p = ro + rd * d.w;
                    float3 n = getNormal(p);
					float3 l = applyLighting(n, p);
                	
                	// update transforms
                	// p -= _Positions[0];
                	float3 pRot = mul(p, _Rotations[0]);
                	
					// sub surface scattering
					float sss = subSurfaceScattering(p, n, ro - p);
                	
                	
                    col.rgb = l * _BaseColor;
                	// col.rgb += sss * (_GlowColor * _Intensity);
                	col.rgb += sss * d.rgb;

                	// surface dots
                	// float noise = 1.0 - clamp(snoise(pRot * _NoiseScale), 0.6, 0.8);
                	// noise = lerp(noise, 0.5, clamp(1.0 - _GlowColor.r * sss, 0.0, 1.0));
                	// col.rgb *= noise;

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
