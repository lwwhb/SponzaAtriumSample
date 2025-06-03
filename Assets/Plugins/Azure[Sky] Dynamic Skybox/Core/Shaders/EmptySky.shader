Shader "Azure[Sky] Dynamic Skybox/Empty Sky"
{
    SubShader
    {
        Tags { "Queue" = "Background" "RenderType" = "Background" "PreviewType" = "Skybox" "IgnoreProjector" = "True" }
        Cull Back     // Render side
        Fog{Mode Off} // Don't use fog
        ZWrite Off    // Don't draw to depth buffer

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vertex_program
            #pragma fragment fragment_program
            #pragma target 3.0
            #include "UnityCG.cginc"

            // Constants
            #define PI 3.1415926535
            #define Pi316 0.0596831
            #define Pi14 0.07957747
            
            // Textures
            uniform samplerCUBE _Azure_SunTexture;
            uniform samplerCUBE _Azure_MoonTexture;
            uniform samplerCUBE _Azure_StarfieldTexture;
            uniform samplerCUBE _Azure_ConstellationTexture;

            // Directions
            uniform float3   _Azure_SunDirection;
            uniform float3   _Azure_MoonDirection;
            uniform float4x4 _Azure_SunMatrix;
            uniform float4x4 _Azure_MoonMatrix;
            uniform float4x4 _Azure_UpDirectionMatrix;
            uniform float4x4 _Azure_StarfieldMatrix;

            // Scattering
            uniform float  _Azure_Kr;
            uniform float  _Azure_Km;
            uniform float3 _Azure_Rayleigh;
            uniform float3 _Azure_Mie;
            uniform float3 _Azure_MieG;
            uniform float  _Azure_Scattering;
            uniform float  _Azure_SkyLuminance;
            uniform float  _Azure_Exposure;
            uniform float4 _Azure_RayleighColor;
            uniform float4 _Azure_MieColor;

            // Outer Space
            uniform float3 _Azure_SunPosition;
            uniform float  _Azure_SunRadius;
            uniform float  _Azure_SunOpacity;
            uniform float4 _Azure_SunColor;
            uniform float3 _Azure_MoonPosition;
            uniform float  _Azure_MoonRadius;
            uniform float  _Azure_MoonOpacity;
            uniform float4 _Azure_MoonColor;
            uniform float  _Azure_StarsIntensity;
            uniform float  _Azure_MilkyWayIntensity;
            uniform float4 _Azure_StarFieldColor;
            uniform float  _Azure_ConstellationIntensity;
            uniform float4 _Azure_ConstellationColor;
            uniform float  _Azure_SkyExtinction;

            // Sphere Raytracing
            float iSphere(in float3 origin, in float3 direction, in float3 position, in float radius, out float3 normalDirection)
            {
                float3 rc = origin - position;
                float c = dot(rc, rc) - (radius * radius);
                float b = dot(direction, rc);
                float d = b * b - c;
                float t = -b - sqrt(abs(d));
                float st = step(0.0f, min(t, d));

                normalDirection = normalize(-position + (origin + direction * t));

                if (st > 0.0f) { return 1.0f; }
                return 0.0f;
            }

            // Attributes transfered from the mesh data to the vertex program
            struct Attributes
            {
                float4 vertex : POSITION;
            };

            // Attributes transfered from the vertex program to the fragment program
            struct Varyings
            {
                float4 Position : SV_POSITION;
                float3 WorldPos : TEXCOORD0;
                float3 StarPos  : TEXCOORD1;
            };

            // Vertex shader program
            Varyings vertex_program(Attributes v)
            {
                Varyings Output = (Varyings)0;

                Output.Position = UnityObjectToClipPos(v.vertex);
                Output.WorldPos = mul((float3x3)unity_WorldToObject, v.vertex.xyz);
                Output.WorldPos = mul((float3x3)_Azure_UpDirectionMatrix, Output.WorldPos);
                Output.StarPos = mul((float3x3)_Azure_StarfieldMatrix, Output.WorldPos);

                return Output;
            }

            // Fragment shader program
            float4 fragment_program(Varyings Input) : SV_Target
            {
                // Directions
                float3 viewDir = normalize(Input.WorldPos);
                float sunCosTheta = dot(viewDir, _Azure_SunDirection);
                float moonCosTheta = dot(viewDir, _Azure_MoonDirection);
                float skyCosTheta = dot(viewDir, float3(0.0f, -1.0f, 0.0f));
                float r = length(float3(0.0f, 50.0f, 0.0f));
                float sunRise = saturate(dot(float3(0.0f, 500.0f, 0.0f), _Azure_SunDirection) / r);
                float moonRise = saturate(dot(float3(0.0f, 500.0f, 0.0f), _Azure_MoonDirection) / r);
                float sunset = dot(float3(0.0f, 1.0f, 0.0f), _Azure_SunDirection);

                // Optical depth
                float zenith = acos(saturate(dot(float3(0.0f, 1.0f, 0.0f), viewDir)));
                float z = cos(zenith) + 0.15f * pow(93.885f - ((zenith * 180.0f) / PI), -1.253f);
                float SR = _Azure_Kr / z;
                float SM = _Azure_Km / z;

                // Extinction
                float3 fex = exp(-(_Azure_Rayleigh * SR + _Azure_Mie * SM));
                float horizonMask = saturate(viewDir.y * 10.0f);
                float3 extinction = saturate(fex * (1.0f - _Azure_SkyExtinction)) * horizonMask;

                // Default sky - When there is no sun or moon in the sky!
                float3 Esun = 1.0f - fex;
                float  rayPhase = 2.0f + 0.5f * pow(skyCosTheta, 2.0f);
                float3 BrTheta = Pi316 * _Azure_Rayleigh * rayPhase * _Azure_RayleighColor.rgb;
                float3 BrmTheta = BrTheta / (_Azure_Rayleigh + _Azure_Mie);
                float3 defaultDayLight = BrmTheta * Esun * _Azure_Scattering * _Azure_SkyLuminance * (1.0f - fex);
                defaultDayLight *= 1.0 - sunRise;
                defaultDayLight *= 1.0 - moonRise;

                // Sun inScattering
                Esun = lerp(fex, (1.0f - fex), sunset);
                rayPhase = 2.0f + 0.5f * pow(sunCosTheta, 2.0f);
                float  miePhase = _Azure_MieG.x / pow(_Azure_MieG.y - _Azure_MieG.z * sunCosTheta, 1.5f);
                BrTheta = Pi316 * _Azure_Rayleigh * rayPhase * _Azure_RayleighColor.rgb;
                float3 BmTheta = Pi14 * _Azure_Mie * miePhase * _Azure_MieColor.rgb;
                BrmTheta = (BrTheta + BmTheta) / (_Azure_Rayleigh + _Azure_Mie);
                float3 sunInScatter = BrmTheta * Esun * _Azure_Scattering * (1.0f - fex);
                sunInScatter *= sunRise;

                // Moon inScattering
                Esun = 1.0f - fex;
                rayPhase = 2.0 + 0.5 * pow(moonCosTheta, 2.0f);
                miePhase = _Azure_MieG.x / pow(_Azure_MieG.y - _Azure_MieG.z * moonCosTheta, 1.5f);
                BrTheta = Pi316 * _Azure_Rayleigh * rayPhase * _Azure_RayleighColor.rgb;
                BmTheta = Pi14 * _Azure_Mie * miePhase * _Azure_MieColor.rgb;
                BrmTheta = (BrTheta + BmTheta) / (_Azure_Rayleigh + _Azure_Mie);
                float3 moonInScatter = BrmTheta * Esun * _Azure_Scattering * 0.1f * (1.0f - fex);
                moonInScatter *= moonRise;
                moonInScatter *= 1.0f - sunRise;

                // Sun sphere
                float3 sunNormal = float3(0.0f, 0.0f, 0.0f);
                float sunSphere = iSphere(float3(0.0f, 0.0f, 0.0f), viewDir, _Azure_SunPosition, _Azure_SunRadius, sunNormal);
                float3 sunUVW = mul((float3x3)_Azure_SunMatrix, sunNormal);
                float3 sunColor = texCUBE(_Azure_SunTexture, sunUVW).rgb * _Azure_SunColor.rgb * _Azure_SunOpacity * sunSphere;
                float sunMask = 1.0f - sunSphere * _Azure_SunOpacity;
                sunColor = pow(sunColor, 2.0f);

                // Moon sphere
                float3 moonNormal = float3(0.0f, 0.0f, 0.0f);
                float moonSphere = iSphere(float3(0.0f, 0.0f, 0.0f), viewDir, _Azure_MoonPosition, _Azure_MoonRadius, moonNormal);
                float3 moonUVW = mul((float3x3)_Azure_MoonMatrix, moonNormal);
                moonUVW.x *= -1.0f;
                float4 moonTexture = saturate(texCUBE(_Azure_MoonTexture, moonUVW) * moonCosTheta);
                float moonMask = 1.0f - moonSphere * _Azure_MoonOpacity;
                float moonLighting = max(dot(moonNormal, _Azure_SunDirection), 0.0f) * moonSphere * 2.0f;
                float3 moonColor = moonTexture.rgb * moonLighting * _Azure_MoonColor.rgb * _Azure_MoonOpacity;

                // Starfield
                float4 starTex = texCUBE(_Azure_StarfieldTexture, Input.StarPos);
                float  constellationTex = texCUBE(_Azure_ConstellationTexture, Input.StarPos).r;
                float3 stars = starTex.rgb * pow(starTex.a, 2.0f) * _Azure_StarFieldColor.rgb * _Azure_StarsIntensity;
                float3 milkyWay = pow(starTex.rgb, 1.5f) * _Azure_StarFieldColor.rgb *_Azure_MilkyWayIntensity;
                float3 constellation = _Azure_ConstellationColor.rgb * constellationTex * _Azure_ConstellationIntensity;
                float3 starfield = (stars + milkyWay + constellation) * moonMask * sunMask;

                // Output
                float3 OutputColor = (sunColor + moonColor + starfield) * extinction + defaultDayLight + sunInScatter + moonInScatter;

                // Tonemapping
                OutputColor = saturate(1.0f - exp(-_Azure_Exposure * OutputColor));

                // Color correction
                #ifndef UNITY_COLORSPACE_GAMMA
                OutputColor = pow(OutputColor, 2.2f);
                #endif

                return float4(OutputColor, 1.0f);
            }

            ENDHLSL
        }
    }
}