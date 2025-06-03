// Constants
#define PI 3.1415926535f
#define Pi316 0.0596831f
#define Pi14 0.07957747f

// Directions
uniform float3   _Azure_SunDirection;
uniform float3   _Azure_MoonDirection;
uniform float4x4 _Azure_SunMatrix;
uniform float4x4 _Azure_MoonMatrix;
uniform float4x4 _Azure_UpDirectionMatrix;
uniform float4x4 _Azure_FrustumCornersMatrix;

// Scattering
uniform float  _Azure_MieDistance;
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

// Fog paramters
uniform float _Azure_GlobalFogDistance;
uniform float _Azure_GlobalFogSmooth;
uniform float _Azure_GlobalFogDensity;
uniform float _Azure_HeightFogDistance;
uniform float _Azure_HeightFogSmooth;
uniform float _Azure_HeightFogDensity;
uniform float _Azure_HeightFogStartAltitude;
uniform float _Azure_HeightFogEndAltitude;
uniform float _Azure_FogBluishDistance;
uniform float _Azure_FogBluishIntensity;
uniform float _Azure_HeightFogScatterMultiplier;

float4 ComputeFogScatteringColor(float3 worldPos)
{
    // Reconstruct world space position and direction towards this screen pixel
    float dist = distance(_WorldSpaceCameraPos, worldPos);
    float depth = dist * _ProjectionParams.w;
    //float depth = 1.0f / (_ZBufferParams.z * dist + _ZBufferParams.w); // float LinearEyeDepth(float z)
    //float depth = 1.0f / (_ZBufferParams.x * dist + _ZBufferParams.y);   // float Linear01Depth(float z)
    float mieDepth = saturate(lerp(dist * (_ProjectionParams.z / 10000.0f), dist * (_ProjectionParams.z / 1000.0f), _Azure_MieDistance));

    // Global fog
    float globalFog = smoothstep(-_Azure_GlobalFogSmooth, 1.25, dist / _Azure_GlobalFogDistance) * _Azure_GlobalFogDensity;

    // Height fog
    float heightFogDistance = smoothstep(-_Azure_HeightFogSmooth, 1.25f, dist / _Azure_HeightFogDistance);
    float3 worldSpaceDirection = mul((float3x3)_Azure_UpDirectionMatrix, worldPos.xyz);
    float heightFog = saturate((worldSpaceDirection.y - _Azure_HeightFogStartAltitude) / (_Azure_HeightFogEndAltitude + _Azure_HeightFogStartAltitude));
    heightFog = 1.0f - heightFog;
    heightFog *= heightFog;
    heightFog *= heightFogDistance;
    heightFog *= _Azure_HeightFogDensity;

    // Total fog
    float totalFog = saturate(globalFog + heightFog);

    // Directions - New
    float3 viewDir = (_WorldSpaceCameraPos - worldPos) * -1.0f;
    viewDir = normalize(mul((float3x3)_Azure_UpDirectionMatrix, viewDir.xyz));
    float sunCosTheta = dot(viewDir, _Azure_SunDirection);
    float moonCosTheta = dot(viewDir, _Azure_MoonDirection);
    float skyCosTheta = dot(viewDir, float3(0.0f, -1.0f, 0.0f));
    float r = length(float3(0.0, 50.0, 0.0));
    float sunRise = saturate(dot(float3(0.0, 500.0, 0.0), _Azure_SunDirection) / r);
    float moonRise = saturate(dot(float3(0.0, 500.0, 0.0), _Azure_MoonDirection) / r);
    float sunDot = dot(float3(0.0f, 1.0f, 0.0f), _Azure_SunDirection);
    float moonDot = dot(float3(0.0f, 1.0f, 0.0f), _Azure_MoonDirection);

    // Optical Depth 1 - Better for sunset!
    float zenith = acos(saturate(dot(float3(-1.0f, 1.0f, -1.0f), depth)));
    float z1 = cos(zenith) + 0.15f * pow(93.885f - ((zenith * 180.0f) / PI), -1.253f);
    float SR1 = _Azure_Kr / z1;
    float SM1 = _Azure_Km / z1;

    // Optical Depth 2 - Better for noon time!
    float z2 = saturate((1.0f - depth * (_ProjectionParams.z / _Azure_FogBluishDistance)) * _Azure_FogBluishIntensity);
    float SR2 = _Azure_Kr / z2;
    float SM2 = _Azure_Km / z2;

    // Optical Depth 3 - Better for the background!
    zenith = acos(saturate(dot(float3(0.0f, 1.0f, 0.0f), viewDir)));
    float z3 = cos(zenith) + 0.15f * pow(93.885f - ((zenith * 180.0f) / PI), -1.253f);
    float SR3 = _Azure_Kr / z3;
    float SM3 = _Azure_Km / z3;

    // Extinction
    float3 fex1 = exp(-(_Azure_Rayleigh * SR1 + _Azure_Mie * SM1));
    float3 fex2 = exp(-(_Azure_Rayleigh * SR2 + _Azure_Mie * SM2));
    float3 fex3 = exp(-(_Azure_Rayleigh * SR3 + _Azure_Mie * SM3));
    float3 fex = lerp(fex2, fex3, depth);

    // Default sky - When there is no sun or moon in the sky!
    float3 Esun = 1.0f - fex;
    float  rayPhase = 2.0f + 0.5f * pow(skyCosTheta, 2.0f);
    float3 BrTheta = Pi316 * _Azure_Rayleigh * rayPhase * _Azure_RayleighColor.rgb;
    float3 BrmTheta = BrTheta / (_Azure_Rayleigh + _Azure_Mie);
    float3 defaultDayLight = BrmTheta * Esun * _Azure_Scattering * _Azure_SkyLuminance * (1.0f - fex);
    defaultDayLight *= 1.0f - sunRise;
    defaultDayLight *= 1.0f - moonRise;

    // Sun inScattering
    fex = lerp(fex1, fex2, sunDot - 0.1f);
    fex = lerp(fex, fex3, depth);
    Esun = lerp(fex, (1.0f - fex), sunDot);
    rayPhase = 2.0f + 0.5f * pow(sunCosTheta, 2.0f);
    float miePhase = _Azure_MieG.x / pow(_Azure_MieG.y - _Azure_MieG.z * sunCosTheta, 1.5f);
    BrTheta = Pi316 * _Azure_Rayleigh * rayPhase * _Azure_RayleighColor.rgb;
    float3 BmTheta = Pi14 * _Azure_Mie * miePhase * _Azure_MieColor.rgb * mieDepth;
    BrmTheta = (BrTheta + BmTheta) / (_Azure_Rayleigh + _Azure_Mie);
    float3 sunInScatter = BrmTheta * Esun * _Azure_Scattering * (1.0f - fex);
    sunInScatter *= sunRise;

    // Moon inScattering
    fex = lerp(fex1, fex2, moonDot - 0.1f);
    fex = lerp(fex, fex3, depth);
    Esun = 1.0f - fex;
    rayPhase = 2.0f + 0.5f * pow(moonCosTheta, 2.0f);
    miePhase = _Azure_MieG.x / pow(_Azure_MieG.y - _Azure_MieG.z * moonCosTheta, 1.5f);
    BrTheta = Pi316 * _Azure_Rayleigh * rayPhase * _Azure_RayleighColor.rgb;
    BmTheta = Pi14 * _Azure_Mie * miePhase * _Azure_MieColor.rgb * mieDepth;
    BrmTheta = (BrTheta + BmTheta) / (_Azure_Rayleigh + _Azure_Mie);
    float3 moonInScatter = BrmTheta * Esun * _Azure_Scattering * 0.1f * (1.0f - fex);
    moonInScatter *= moonRise;
    moonInScatter *= 1.0f - sunRise;

    // Output
    float3 OutputColor = defaultDayLight + sunInScatter + moonInScatter;
    OutputColor += heightFog * _Azure_HeightFogScatterMultiplier;

    // Tonemapping
    OutputColor = saturate(1.0f - exp(-_Azure_Exposure * OutputColor));

    // Color correction
    #ifndef UNITY_COLORSPACE_GAMMA
    OutputColor = pow(OutputColor, 2.2f);
    #endif

    return float4(OutputColor, totalFog);
}

// Apply fog scattering
float4 ApplyAzureFog(float4 fragOutput, float3 worldPos)
{
    // Fog color
    float4 fogData = float4(0.0, 0.0, 0.0, 0.0);
	#ifdef UNITY_PASS_FORWARDADD
    fogData = float4(0.0, 0.0, 0.0, 0.0);
	#else
    fogData = ComputeFogScatteringColor(worldPos);
	#endif

	// Apply fog
	#if defined(_ALPHAPREMULTIPLY_ON)
	fragOutput.a = lerp(fragOutput.a, 1.0f, fogData.a);
	#endif

	float3 OutputColor = lerp(fragOutput.rgb, fogData.rgb, fogData.a * lerp(fragOutput.a, 1.0f, 2.0f - fogData.a));
	return float4(OutputColor, fragOutput.a);
}

// DEPRECATED - backward compatibility (Actually, the projPos parameter is no longer used.)
float4 ApplyAzureFog(float4 fragOutput, float4 projPos, float3 worldPos)
{
    // Fog color
    float4 fogData = float4(0.0, 0.0, 0.0, 0.0);
    #ifdef UNITY_PASS_FORWARDADD
    fogData = float4(0.0, 0.0, 0.0, 0.0);
    #else
    fogData = ComputeFogScatteringColor(worldPos);
    #endif

    // Apply fog
    #if defined(_ALPHAPREMULTIPLY_ON)
    fragOutput.a = lerp(fragOutput.a, 1.0f, fogData.a);
    #endif

    float3 OutputColor = lerp(fragOutput.rgb, fogData.rgb, fogData.a * lerp(fragOutput.a, 1.0f, 2.0f - fogData.a));
    return float4(OutputColor, fragOutput.a);
}

// Apply fog scattering to additive/multiply blend mode.
float4 ApplyAzureFog(float4 fragOutput, float3 worldPos, float4 fogColor)
{
    // Fog color
    float3 fogScatteringColor = float3(0.0, 0.0, 0.0);
	#ifdef UNITY_PASS_FORWARDADD
	fogScatteringColor = float3(0.0, 0.0, 0.0);
	#else
	fogScatteringColor = fogColor;
	#endif

    // Reconstruct world space position and direction towards this screen pixel
    float dist = distance(_WorldSpaceCameraPos, worldPos);

    // Global fog
    float globalFog = smoothstep(-_Azure_GlobalFogSmooth, 1.25, dist / _Azure_GlobalFogDistance) * _Azure_GlobalFogDensity;

    // Height fog
    float heightFogDistance = smoothstep(-_Azure_HeightFogSmooth, 1.25f, dist / _Azure_HeightFogDistance);
    float3 worldSpaceDirection = mul((float3x3)_Azure_UpDirectionMatrix, worldPos.xyz);
    float heightFog = saturate((worldSpaceDirection.y - _Azure_HeightFogStartAltitude) / (_Azure_HeightFogEndAltitude + _Azure_HeightFogStartAltitude));
    heightFog = 1.0f - heightFog;
    heightFog *= heightFog;
    heightFog *= heightFogDistance;
    heightFog *= _Azure_HeightFogDensity;

    // Total fog
    float totalFog = saturate(globalFog + heightFog);

	// Apply fog
	#if defined(_ALPHAPREMULTIPLY_ON)
	fragOutput.a = lerp(fragOutput.a, 1.0, totalFog);
	#endif

	fogScatteringColor = lerp(fragOutput.rgb, fogScatteringColor, totalFog * lerp(fragOutput.a, 1.0, 2.0 - totalFog));
	return float4(fogScatteringColor, fragOutput.a);
}