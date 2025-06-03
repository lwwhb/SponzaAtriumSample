namespace UnityEngine.AzureSky
{
    /// <summary>
    /// This class is an example of how to manually link the custom weather properties to its targets without using the override system and reflection.
    /// </summary>
    // But in case you:
    //    - Changed the order of the weather groups in the list.
    //    - Changed the order of the weather properties in the list.
    //    - Added a new weather group or weather property to the list.
    //    - Deleted a weather group or weather property from the list.
    // You must fix the idexation of the functions GetFloatOutput() and GetColorOutput().
    // You can find the weather group index and the weather property index in the Inspector of each weather preset next to its name.
    public sealed class AzureWeatherPropertyLinker : MonoBehaviour
    {
        /// <summary>The refenrece to the AzureSkyRenderer component.</summary>
        public AzureSkyRenderer azureSkyRenderer { get => m_azureSkyRenderer; set => m_azureSkyRenderer = value; }
        [SerializeField] private AzureSkyRenderer m_azureSkyRenderer = null;

        /// <summary>The refenrece to the Light component from the prefab's directional light.</summary>
        public Light directionalLight { get => m_directionalLight; set => m_directionalLight = value; }
        [SerializeField] private Light m_directionalLight = null;

        /// <summary>The refenrece to the AzureParticleFX component attached to the rain particle system.</summary>
        public AzureParticleFX rainParticleFX { get => m_rainParticleFX; set => m_rainParticleFX = value; }
        [SerializeField] private AzureParticleFX m_rainParticleFX = null;

        /// <summary>The refenrece to the AzureParticleFX component attached to the storm particle system.</summary>
        public AzureParticleFX stormParticleFX { get => m_stormParticleFX; set => m_stormParticleFX = value; }
        [SerializeField] private AzureParticleFX m_stormParticleFX = null;

        /// <summary>The refenrece to the AzureParticleFX component attached to the snow particle system.</summary>
        public AzureParticleFX snowParticleFX { get => m_snowParticleFX; set => m_snowParticleFX = value; }
        [SerializeField] private AzureParticleFX m_snowParticleFX = null;

        /// <summary>The refenrece to the AzureSoundFX component attached to the calm rain game object.</summary>
        public AzureSoundFX calmRainSoundFX { get => m_calmRainSoundFX; set => m_calmRainSoundFX = value; }
        [SerializeField] private AzureSoundFX m_calmRainSoundFX = null;

        /// <summary>The refenrece to the AzureSoundFX component attached to the medium rain game object.</summary>
        public AzureSoundFX mediumRainSoundFX { get => m_mediumRainSoundFX; set => m_mediumRainSoundFX = value; }
        [SerializeField] private AzureSoundFX m_mediumRainSoundFX = null;

        /// <summary>The refenrece to the AzureSoundFX component attached to the storm rain game object.</summary>
        public AzureSoundFX stormRainSoundFX { get => m_stormRainSoundFX; set => m_stormRainSoundFX = value; }
        [SerializeField] private AzureSoundFX m_stormRainSoundFX = null;

        /// <summary>The refenrece to the AzureSoundFX component attached to the calm wind game object.</summary>
        public AzureSoundFX calmWindSoundFX { get => m_calmWindSoundFX; set => m_calmWindSoundFX = value; }
        [SerializeField] private AzureSoundFX m_calmWindSoundFX = null;

        /// <summary>The refenrece to the AzureSoundFX component attached to the medium wind game object.</summary>
        public AzureSoundFX mediumWindSoundFX { get => m_mediumWindSoundFX; set => m_mediumWindSoundFX = value; }
        [SerializeField] private AzureSoundFX m_mediumWindSoundFX = null;

        /// <summary>The refenrece to the AzureSoundFX component attached to the storm wind game object.</summary>
        public AzureSoundFX stormWindSoundFX { get => m_stormWindSoundFX; set => m_stormWindSoundFX = value; }
        [SerializeField] private AzureSoundFX m_stormWindSoundFX = null;

        /// <summary>The refenrece to the material used to render the rain particle system.</summary>
        public Material rainMaterial { get => m_rainMaterial; set => m_rainMaterial = value; }
        [SerializeField] private Material m_rainMaterial = null;

        /// <summary>The refenrece to the material used to render the storm particle system.</summary>
        public Material stormMaterial { get => m_stormMaterial; set => m_stormMaterial = value; }
        [SerializeField] private Material m_stormMaterial = null;

        /// <summary>The refenrece to the material used to render the snow particle system.</summary>
        public Material snowMaterial { get => m_snowMaterial; set => m_snowMaterial = value; }
        [SerializeField] private Material m_snowMaterial = null;

        /// <summary>The refenrece to the WindZone component attached to the wind zone game object.</summary>
        public WindZone windZone { get => m_windZone; set => m_windZone = value; }
        [SerializeField] private WindZone m_windZone = null;

        // Registering to the Azure[Sky] event triggered after the weather system is updated.
        // This make sure the weather preset blends and transitions have been completed.
        // Note that AzureNotificationCenter.OnAfterWeatherSystemUpdate sends the instance of the weather system that
        // triggered the event, so there is no need to create a reference of the AzureCoreSystem to access it.
        private void OnEnable()
        {
            AzureNotificationCenter.OnAfterWeatherSystemUpdate += OnAfterWeatherSystemUpdate;
        }

        // Unregistering from the Azure[Sky] event.
        private void OnDisable()
        {
            AzureNotificationCenter.OnAfterWeatherSystemUpdate -= OnAfterWeatherSystemUpdate;
        }

        // It will be executed every time the Weather System is updated by the AzureCoreSystem component.
        // It is good for performance because it is not executed every frame.
        // It is executed in time intervals synchronized with the AzureCoreSystem component.
        private void OnAfterWeatherSystemUpdate(AzureWeatherSystem azureWeatherSystem)
        {
            // Scattering weather group
            m_azureSkyRenderer.molecularDensity = azureWeatherSystem.GetFloatOutput(0, 0);
            m_azureSkyRenderer.rayleigh = azureWeatherSystem.GetFloatOutput(0, 1);
            m_azureSkyRenderer.mie = azureWeatherSystem.GetFloatOutput(0, 2);
            m_azureSkyRenderer.rayleighColor = azureWeatherSystem.GetColorOutput(0, 3);
            m_azureSkyRenderer.mieColor = azureWeatherSystem.GetColorOutput(0, 4);

            // Outer Space weather group
            m_azureSkyRenderer.starsIntensity = azureWeatherSystem.GetFloatOutput(1, 0);
            m_azureSkyRenderer.milkyWayIntensity = azureWeatherSystem.GetFloatOutput(1, 1);
            m_azureSkyRenderer.constellationIntensity = azureWeatherSystem.GetFloatOutput(1, 2);
            m_azureSkyRenderer.sunOpacity = azureWeatherSystem.GetFloatOutput(1, 3);
            m_azureSkyRenderer.moonOpacity = azureWeatherSystem.GetFloatOutput(1, 4);

            // Fog Scattering weather group
            m_azureSkyRenderer.globalFogDistance = azureWeatherSystem.GetFloatOutput(2, 0);
            m_azureSkyRenderer.globalFogSmoothStep = azureWeatherSystem.GetFloatOutput(2, 1);
            m_azureSkyRenderer.globalFogDensity = azureWeatherSystem.GetFloatOutput(2, 2);
            m_azureSkyRenderer.heightFogDistance = azureWeatherSystem.GetFloatOutput(2, 3);
            m_azureSkyRenderer.heightFogSmoothStep = azureWeatherSystem.GetFloatOutput(2, 4);
            m_azureSkyRenderer.heightFogDensity = azureWeatherSystem.GetFloatOutput(2, 5);
            m_azureSkyRenderer.heightFogScatteringMultiplier = azureWeatherSystem.GetFloatOutput(2, 6);

            // Directional Light weather group
            m_directionalLight.intensity = azureWeatherSystem.GetFloatOutput(3, 0);
            m_directionalLight.color = azureWeatherSystem.GetColorOutput(3, 1);
            m_directionalLight.shadowStrength = azureWeatherSystem.GetFloatOutput(3, 2);

            // Environment weather group
            RenderSettings.ambientIntensity = azureWeatherSystem.GetFloatOutput(4, 0);
            RenderSettings.ambientSkyColor = azureWeatherSystem.GetColorOutput(4, 1);
            RenderSettings.ambientEquatorColor = azureWeatherSystem.GetColorOutput(4, 2);
            RenderSettings.ambientGroundColor = azureWeatherSystem.GetColorOutput(4, 3);

            // Particles weather group
            m_rainParticleFX.intensity = azureWeatherSystem.GetFloatOutput(5, 0);
            m_stormParticleFX.intensity = azureWeatherSystem.GetFloatOutput(5, 1);
            m_snowParticleFX.intensity = azureWeatherSystem.GetFloatOutput(5, 2);
            m_rainMaterial.SetColor("_TintColor", azureWeatherSystem.GetColorOutput(5, 3));
            m_stormMaterial.SetColor("_TintColor", azureWeatherSystem.GetColorOutput(5, 4));
            m_snowMaterial.SetColor("_TintColor", azureWeatherSystem.GetColorOutput(5, 5));

            // Sound FX weather group
            m_calmRainSoundFX.volume = azureWeatherSystem.GetFloatOutput(6, 0);
            m_mediumRainSoundFX.volume = azureWeatherSystem.GetFloatOutput(6, 1);
            m_stormRainSoundFX.volume = azureWeatherSystem.GetFloatOutput(6, 2);
            m_calmWindSoundFX.volume = azureWeatherSystem.GetFloatOutput(6, 3);
            m_mediumWindSoundFX.volume = azureWeatherSystem.GetFloatOutput(6, 4);
            m_stormWindSoundFX.volume = azureWeatherSystem.GetFloatOutput(6, 5);

            // Dynamic Clouds weather group
            m_azureSkyRenderer.dynamicCloudDensity = azureWeatherSystem.GetFloatOutput(7, 0);
            m_azureSkyRenderer.dynamicCloudAltitude = azureWeatherSystem.GetFloatOutput(7, 1);
            m_azureSkyRenderer.dynamicCloudSpeed = azureWeatherSystem.GetFloatOutput(7, 2);
            m_azureSkyRenderer.dynamicCloudDirection = azureWeatherSystem.GetFloatOutput(7, 3);
            m_azureSkyRenderer.dynamicCloudColor1 = azureWeatherSystem.GetColorOutput(7, 4);
            m_azureSkyRenderer.dynamicCloudColor2 = azureWeatherSystem.GetColorOutput(7, 5);

            // Wind weather group
            m_windZone.windMain = azureWeatherSystem.GetFloatOutput(8, 0);
            m_windZone.transform.eulerAngles = azureWeatherSystem.GetVector3Output(8, 1);
        }

    }
}