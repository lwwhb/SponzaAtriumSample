Shader "Azure[Sky] Dynamic Skybox/Volumetric Light"
{
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "IgnoreProjector" = "True"}

        Pass
        {
            Name "OutsideLight"
            Cull Back
            ZWrite Off
            ZTest LEqual
            Blend OneMinusDstColor One // Soft additive
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            uniform float  _Azure_VolumetricLightIntensity;
            uniform float4 _Azure_VolumetricLightColor;
            uniform float3 _Azure_VolumetricLightMieG;
            uniform float  _Azure_VolumetricLightExtinctionDistance;
            uniform float  _Azure_VolumetricLightExtinctionSmoothStep;

            struct appdata
            {
                float4 vertex : POSITION;
            };
            
            struct v2f
            {
                float4 vertex    : SV_POSITION;
                float3 ro        : TEXCOORD0;
                float3 hitPos    : TEXCOORD1;
                float4 projPos   : TEXCOORD2;
            };

            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.ro = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1));
                o.hitPos = v.vertex;
                o.projPos = ComputeScreenPos(o.vertex);
                COMPUTE_EYEDEPTH(o.projPos.z);

                return o;
            }

            // From: https://www.shadertoy.com/view/XljGDy
            // ray origin, ray direction, sphere center, sphere radius, depth buffer
            float sphDensity(float3  ro, float3  rd, float3  sc, float sr, float dbuffer)
            {
                // normalize the problem to the canonical sphere
                float ndbuffer = dbuffer / sr;
                float3  rc = (ro - sc) / sr;

                // find intersection with sphere
                float b = dot(rd, rc);
                float c = dot(rc, rc) - 1.0;
                float h = b * b - c;

                // not intersecting
                if (h < 0.0) return 0.0;

                h = sqrt(h);

                //return h*h*h;

                float t1 = -b - h;
                float t2 = -b + h;

                // not visible (behind camera or behind ndbuffer)
                if (t2<0.0 || t1>ndbuffer) return 0.0;

                // clip integration segment from camera to ndbuffer
                t1 = max(t1, 0.0);
                t2 = min(t2, ndbuffer);

                // analytical integration of an inverse squared density
                float i1 = -(c * t1 + b * t1 * t1 + t1 * t1 * t1 / 3.0);
                float i2 = -(c * t2 + b * t2 * t2 + t2 * t2 * t2 / 3.0);
                return (i2 - i1) * (3.0 / 4.0);
            }

            uniform sampler2D _CameraDepthTexture;

            float4 frag(v2f i) : SV_Target
            {
                // Starting the output color completely black!
                float4 col = float4(0.0f, 0.0f, 0.0f, 0.0f);

                float3 ro = i.ro; // Ray Origin! It is the camera position converted to the object space!
                float3 rd = normalize(i.hitPos - ro); // Ray Direction!
                float density = sphDensity(ro, rd, float3(0.0f, 0.0f, 0.0f), 1.0f, 500.0f);

                if (density <= 0.0f)
                {
                    discard;
                }
                else
                {
                    float  miePhase = _Azure_VolumetricLightMieG.x / pow(_Azure_VolumetricLightMieG.y - _Azure_VolumetricLightMieG.z * density, 0.5f);
                    col.rgb = _Azure_VolumetricLightColor * miePhase * density;
                    col.rgb = col.rgb * _Azure_VolumetricLightIntensity;
                    col.a = density;

                    float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
                    float partZ = i.projPos.z;
                    float fade = saturate(0.05f * (sceneZ - partZ));
                    col *= fade;

                    // Light extinction along distance
                    float dist = distance(_WorldSpaceCameraPos, unity_ObjectToWorld._m03_m13_m23);
                    dist = smoothstep(-_Azure_VolumetricLightExtinctionSmoothStep, 1.25f, dist / _Azure_VolumetricLightExtinctionDistance);
                    float extinction = lerp(density, 0.0f, dist);

                    return saturate(col * extinction);
                }

                return col;
            }
            ENDHLSL
        }

        Pass
        {
            Name "InsideLight"
            Cull Front
            ZWrite Off
            ZTest Off
            Blend OneMinusDstColor One // Soft additive

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            uniform float  _Azure_VolumetricLightIntensity;
            uniform float4 _Azure_VolumetricLightColor;
            uniform float3 _Azure_VolumetricLightMieG;
            uniform float  _Azure_VolumetricLightExtinctionDistance;
            uniform float  _Azure_VolumetricLightExtinctionSmoothStep;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex    : SV_POSITION;
                float3 ro        : TEXCOORD0;
                float3 hitPos    : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.ro = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1));
                o.hitPos = v.vertex;

                return o;
            }

            // From: https://www.shadertoy.com/view/XljGDy
            // ray origin, ray direction, sphere center, sphere radius, depth buffer
            float sphDensity(float3  ro, float3  rd, float3  sc, float sr, float dbuffer)
            {
                // normalize the problem to the canonical sphere
                float ndbuffer = dbuffer / sr;
                float3  rc = (ro - sc) / sr;

                // find intersection with sphere
                float b = dot(rd, rc);
                float c = dot(rc, rc) - 1.0;
                float h = b * b - c;

                // not intersecting
                if (h < 0.0) return 0.0;

                h = sqrt(h);

                //return h*h*h;

                float t1 = -b - h;
                float t2 = -b + h;

                // not visible (behind camera or behind ndbuffer)
                if (t2<0.0 || t1>ndbuffer) return 0.0;

                // clip integration segment from camera to ndbuffer
                t1 = max(t1, 0.0);
                t2 = min(t2, ndbuffer);

                // analytical integration of an inverse squared density
                float i1 = -(c * t1 + b * t1 * t1 + t1 * t1 * t1 / 3.0);
                float i2 = -(c * t2 + b * t2 * t2 + t2 * t2 * t2 / 3.0);
                return (i2 - i1) * (3.0 / 4.0);
            }

            float4 frag(v2f i) : SV_Target
            {
                // Starting the output color completely black!
                float4 col = float4(0.0f, 0.0f, 0.0f, 0.0f);

                float3 ro = i.ro; // Ray Origin! It is the camera position converted to the object space!
                float3 rd = normalize(i.hitPos - ro); // Ray Direction!
                float density = sphDensity(ro, rd, float3(0.0f, 0.0f, 0.0f), 1.0f, 500.0f);

                if (density <= 0.0f)
                {
                    discard;
                }
                else
                {
                    float  miePhase = _Azure_VolumetricLightMieG.x / pow(_Azure_VolumetricLightMieG.y - _Azure_VolumetricLightMieG.z * density, 0.5f);
                    col.rgb = _Azure_VolumetricLightColor * miePhase * density;
                    col.rgb = saturate(col.rgb * _Azure_VolumetricLightIntensity);
                    col.a = density;

                    // Light extinction along distance
                    float dist = distance(_WorldSpaceCameraPos, unity_ObjectToWorld._m03_m13_m23);
                    dist = smoothstep(-_Azure_VolumetricLightExtinctionSmoothStep, 1.25f, dist / _Azure_VolumetricLightExtinctionDistance);
                    float extinction = lerp(density, 0.0f, dist);

                    return col * extinction;
                }

                return col;
            }
            ENDHLSL
        }
    }
}