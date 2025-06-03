namespace UnityEngine.AzureSky
{
    internal static class AzureShaderUniforms
    {
        // Textures
        internal static readonly int SunTexture = Shader.PropertyToID("_Azure_SunTexture");
        internal static readonly int MoonTexture = Shader.PropertyToID("_Azure_MoonTexture");
        internal static readonly int StarfieldTexture = Shader.PropertyToID("_Azure_StarfieldTexture");
        internal static readonly int ConstellationTexture = Shader.PropertyToID("_Azure_ConstellationTexture");
        internal static readonly int DynamicCloudTexture = Shader.PropertyToID("_Azure_DynamicCloudTexture");
        internal static readonly int FogScatteringDataTexture = Shader.PropertyToID("_Azure_FogScatteringDataTexture");

        // Directions
        internal static readonly int SunDirection = Shader.PropertyToID("_Azure_SunDirection");
        internal static readonly int MoonDirection = Shader.PropertyToID("_Azure_MoonDirection");
        internal static readonly int SunMatrix = Shader.PropertyToID("_Azure_SunMatrix");
        internal static readonly int MoonMatrix = Shader.PropertyToID("_Azure_MoonMatrix");
        internal static readonly int UpDirectionMatrix = Shader.PropertyToID("_Azure_UpDirectionMatrix");
        internal static readonly int StarfieldMatrix = Shader.PropertyToID("_Azure_StarfieldMatrix");
        internal static readonly int FrustumCornersMatrix = Shader.PropertyToID("_Azure_FrustumCornersMatrix");

        // Scattering
        internal static readonly int Kr = Shader.PropertyToID("_Azure_Kr");
        internal static readonly int Km = Shader.PropertyToID("_Azure_Km");
        internal static readonly int Rayleigh = Shader.PropertyToID("_Azure_Rayleigh");
        internal static readonly int Mie = Shader.PropertyToID("_Azure_Mie");
        internal static readonly int MieG = Shader.PropertyToID("_Azure_MieG");
        internal static readonly int Scattering = Shader.PropertyToID("_Azure_Scattering");
        internal static readonly int SkyLuminance = Shader.PropertyToID("_Azure_SkyLuminance");
        internal static readonly int Exposure = Shader.PropertyToID("_Azure_Exposure");
        internal static readonly int RayleighColor = Shader.PropertyToID("_Azure_RayleighColor");
        internal static readonly int MieColor = Shader.PropertyToID("_Azure_MieColor");

        // Outer space
        internal static readonly int SunRadius = Shader.PropertyToID("_Azure_SunRadius");
        internal static readonly int SunPosition = Shader.PropertyToID("_Azure_SunPosition");
        internal static readonly int SunOpacity = Shader.PropertyToID("_Azure_SunOpacity");
        internal static readonly int SunColor = Shader.PropertyToID("_Azure_SunColor");
        internal static readonly int MoonRadius = Shader.PropertyToID("_Azure_MoonRadius");
        internal static readonly int MoonPosition = Shader.PropertyToID("_Azure_MoonPosition");
        internal static readonly int MoonOpacity = Shader.PropertyToID("_Azure_MoonOpacity");
        internal static readonly int MoonColor = Shader.PropertyToID("_Azure_MoonColor");
        internal static readonly int StarsIntensity = Shader.PropertyToID("_Azure_StarsIntensity");
        internal static readonly int MilkyWayIntensity = Shader.PropertyToID("_Azure_MilkyWayIntensity");
        internal static readonly int StarFieldColor = Shader.PropertyToID("_Azure_StarFieldColor");
        internal static readonly int ConstellationIntensity = Shader.PropertyToID("_Azure_ConstellationIntensity");
        internal static readonly int ConstellationColor = Shader.PropertyToID("_Azure_ConstellationColor");
        internal static readonly int SkyExtinction = Shader.PropertyToID("_Azure_SkyExtinction");

        // Fog scattering
        internal static readonly int MieDistance = Shader.PropertyToID("_Azure_MieDistance");
        internal static readonly int GlobalFogDistance = Shader.PropertyToID("_Azure_GlobalFogDistance");
        internal static readonly int GlobalFogSmoothStep = Shader.PropertyToID("_Azure_GlobalFogSmooth");
        internal static readonly int GlobalFogDensity = Shader.PropertyToID("_Azure_GlobalFogDensity");
        internal static readonly int HeightFogDistance = Shader.PropertyToID("_Azure_HeightFogDistance");
        internal static readonly int HeightFogSmoothStep = Shader.PropertyToID("_Azure_HeightFogSmooth");
        internal static readonly int HeightFogDensity = Shader.PropertyToID("_Azure_HeightFogDensity");
        internal static readonly int HeightFogStartAltitude = Shader.PropertyToID("_Azure_HeightFogStartAltitude");
        internal static readonly int HeightFogEndAltitude = Shader.PropertyToID("_Azure_HeightFogEndAltitude");
        internal static readonly int FogBluishDistance = Shader.PropertyToID("_Azure_FogBluishDistance");
        internal static readonly int FogBluishIntensity = Shader.PropertyToID("_Azure_FogBluishIntensity");
        internal static readonly int HeightFogScatteringMultiplier = Shader.PropertyToID("_Azure_HeightFogScatterMultiplier");

        // Dynamic clouds
        internal static readonly int DynamicCloudAltitude = Shader.PropertyToID("_Azure_DynamicCloudAltitude");
        internal static readonly int DynamicCloudDirection = Shader.PropertyToID("_Azure_DynamicCloudDirection");
        internal static readonly int DynamicCloudDensity = Shader.PropertyToID("_Azure_DynamicCloudDensity");
        internal static readonly int DynamicCloudColor1 = Shader.PropertyToID("_Azure_DynamicCloudColor1");
        internal static readonly int DynamicCloudColor2 = Shader.PropertyToID("_Azure_DynamicCloudColor2");
        internal static readonly int ThunderLightningEffect = Shader.PropertyToID("_Azure_ThunderLightningEffect");

        // Volumetric lights
        internal static readonly int VolumetricLightIntensity = Shader.PropertyToID("_Azure_VolumetricLightIntensity");
        internal static readonly int VolumetricLightColor = Shader.PropertyToID("_Azure_VolumetricLightColor");
        internal static readonly int VolumetricLightExtinctionDistance = Shader.PropertyToID("_Azure_VolumetricLightExtinctionDistance");
        internal static readonly int VolumetricLightExtinctionSmoothStep = Shader.PropertyToID("_Azure_VolumetricLightExtinctionSmoothStep");
        internal static readonly int VolumetricLightMieG = Shader.PropertyToID("_Azure_VolumetricLightMieG");
        internal static readonly int VolumetricLightWorldPosition = Shader.PropertyToID("_Azure_VolumetricLightWorldPosition");
    }
}