namespace UnityEngine.AzureSky
{
    /// <summary>This class handles the attributes used to render the sky and fog scattering effect.</summary>
    [ExecuteInEditMode]
    [AddComponentMenu("Azure[Sky] Dynamic Skybox/Azure Sky Renderer")]
    public sealed class AzureSkyRenderer : MonoBehaviour
    {
        #if UNITY_EDITOR
        [SerializeField] private bool m_showReferencesTab;
        [SerializeField] private bool m_showScatteringTab;
        [SerializeField] private bool m_showOutterSpaceTab;
        [SerializeField] private bool m_showFogScatteringTab;
        [SerializeField] private bool m_showDynamicCloudTab;
        [SerializeField] private bool m_showOptionsTab;
        #endif

        ////////////////////
        // References Tab //
        ////////////////////

        /// <summary>The transform used to represent the position of the sun in the sky.</summary>
        public Transform sunTransform { get => m_sunTransform; set => m_sunTransform = value; }
        [SerializeField] private Transform m_sunTransform = null;

        /// <summary>The transform used to represent the position of the moon in the sky.</summary>
        public Transform moonTransform { get => m_moonTransform; set => m_moonTransform = value; }
        [SerializeField] private Transform m_moonTransform = null;

        /// <summary>The transform used to represent the position of the starfield in the sky.</summary>
        public Transform starfieldTransform { get => m_starfieldTransform; set => m_starfieldTransform = value; }
        [SerializeField] private Transform m_starfieldTransform = null;

        /// <summary>The material that renders the sky.</summary>
        public Material skyMaterial { get => m_skyMaterial; set => m_skyMaterial = value; }
        [SerializeField] private Material m_skyMaterial = null;

        /// <summary>The material that renders the fog scattering effect.</summary>
        public Material fogMaterial { get => m_fogMaterial; set => m_fogMaterial = value; }
        [SerializeField] private Material m_fogMaterial = null;

        /// <summary>The cubemap texture used to render the sun sphere.</summary>
        public Cubemap sunTexture { get => m_sunTexture; set => m_sunTexture = value; }
        [SerializeField] private Cubemap m_sunTexture = null;

        /// <summary>The cubemap texture used to render the moon sphere.</summary>
        public Cubemap moonTexture { get => m_moonTexture; set => m_moonTexture = value; }
        [SerializeField] private Cubemap m_moonTexture = null;

        /// <summary>The cubemap texture used to render the regular stars and the Milky Way.</summary>
        public Cubemap starfieldTexture { get => m_starfieldTexture; set => m_starfieldTexture = value; }
        [SerializeField] private Cubemap m_starfieldTexture = null;

        /// <summary>The cubemap texture used to render the sky constellation.</summary>
        public Cubemap constellationTexture { get => m_constellationTexture; set => m_constellationTexture = value; }
        [SerializeField] private Cubemap m_constellationTexture = null;

        /// <summary>The 2D texture used to render the dynamic clouds.</summary>
        public Texture2D dynamicCloudTexture { get => m_dynamicCloudTexture; set => m_dynamicCloudTexture = value; }
        [SerializeField] private Texture2D m_dynamicCloudTexture = null;

        /// <summary>The shader used to render only the sky without clouds.</summary>
        public Shader emptySkyShader { get => m_emptySkyShader; set => m_emptySkyShader = value; }
        [SerializeField] private Shader m_emptySkyShader = null;

        /// <summary>The shader used to render sky with dynamic clouds.</summary>
        public Shader dynamicCloudShader { get => m_dynamicCloudShader; set => m_dynamicCloudShader = value; }
        [SerializeField] private Shader m_dynamicCloudShader = null;

        ////////////////////
        // Scattering Tab //
        ////////////////////

        /// <summary>The Vector3 that represents the wavelength of the visible light.</summary>
        public Vector3 wavelength { get => m_wavelength; set => m_wavelength = value; }
        [SerializeField] private Vector3 m_wavelength = new Vector3(680.0f, 550.0f, 450.0f);

        /// <summary>The molecular density of the air.</summary>
        public float molecularDensity { get => m_molecularDensity; set => m_molecularDensity = value; }
        [SerializeField] private float m_molecularDensity = 2.545f;

        /// <summary>The rayleigh altitude in meters.</summary>
        public float kr { get => m_kr; set => m_kr = value; }
        [SerializeField] private float m_kr = 8400.0f;

        /// <summary>The mie altitude in meters.</summary>
        public float km { get => m_km; set => m_km = value; }
        [SerializeField] private float m_km = 1200.0f;

        /// <summary>The rayleigh scattering multiplier.</summary>
        public float rayleigh { get => m_rayleigh; set => m_rayleigh = value; }
        [SerializeField] private float m_rayleigh = 1.5f;

        /// <summary>The mie scattering multiplier.</summary>
        public float mie { get => m_mie; set => m_mie = value; }
        [SerializeField] private float m_mie = 1.0f;

        /// <summary>The mie directionality factor.</summary>
        public float mieDirectionalityFactor { get => m_mieDirectionalityFactor; set => m_mieDirectionalityFactor = value; }
        [SerializeField] private float m_mieDirectionalityFactor = 0.75f;

        /// <summary>The scattering intensity multiplier.</summary>
        public float scattering { get => m_scattering; set => m_scattering = value; }
        [SerializeField] private float m_scattering = 15.0f;

        /// <summary>The luminance of the sky when there is no sun or moon in the sky.</summary>
        public float skyLuminance { get => m_skyLuminance; set => m_skyLuminance = value; }
        [SerializeField] private float m_skyLuminance = 0.1f;

        /// <summary>The exposure of the internal sky shader tonemapping.</summary>
        public float exposure { get => m_exposure; set => m_exposure = value; }
        [SerializeField] private float m_exposure = 2.0f;

        /// <summary>The rayleigh color multiplier.</summary>
        public Color rayleighColor { get => m_rayleighColor; set => m_rayleighColor = value; }
        [SerializeField] private Color m_rayleighColor = Color.white;

        /// <summary>The mie color multiplier.</summary>
        public Color mieColor { get => m_mieColor; set => m_mieColor = value; }
        [SerializeField] private Color m_mieColor = Color.white;

        //////////////////////
        // Outter Space Tab //
        //////////////////////

        /// <summary>The size of the Sun sphere in the sky.</summary>
        public float sunSize { get => m_sunSize; set => m_sunSize = value; }
        [SerializeField] private float m_sunSize = 1.0f;

        /// <summary>The opacity of the Sun sphere.</summary>
        public float sunOpacity { get => m_sunOpacity; set => m_sunOpacity = value; }
        [SerializeField] private float m_sunOpacity = 1.0f;

        /// <summary>The color multiplier of the Sun sphere.</summary>
        public Color sunColor { get => m_sunColor; set => m_sunColor = value; }
        [SerializeField] private Color m_sunColor = Color.white;

        /// <summary>The size of the Moon sphere in the sky.</summary>
        public float moonSize { get => m_moonSize; set => m_moonSize = value; }
        [SerializeField] private float m_moonSize = 1.0f;

        /// <summary>The opacity of the Moon sphere.</summary>
        public float moonOpacity { get => m_moonOpacity; set => m_moonOpacity = value; }
        [SerializeField] private float m_moonOpacity = 1.0f;

        /// <summary>The color multiplier of the Moon sphere.</summary>
        public Color moonColor { get => m_moonColor; set => m_moonColor = value; }
        [SerializeField] private Color m_moonColor = Color.white;

        /// <summary>The rotation offset to adjust the moon cubemap texture in its sphere.</summary>
        public Vector3 moonRotationOffset { get => m_moonRotationOffset; set => m_moonRotationOffset = value; }
        [SerializeField] private Vector3 m_moonRotationOffset = Vector3.zero;

        /// <summary>The intensity of the regular stars.</summary>
        public float starsIntensity { get => m_starsIntensity; set => m_starsIntensity = value; }
        [SerializeField] private float m_starsIntensity = 0.5f;

        /// <summary>The intensity of the Milky Way.</summary>
        public float milkyWayIntensity { get => m_milkyWayIntensity; set => m_milkyWayIntensity = value; }
        [SerializeField] private float m_milkyWayIntensity = 0.0f;

        /// <summary>The color multiplier of the entire starfield.</summary>
        public Color starfieldColor { get => m_starfieldColor; set => m_starfieldColor = value; }
        [SerializeField] private Color m_starfieldColor = Color.white;

        /// <summary>The extinction of the light coming from the outer space, caused by the atmosphere density.</summary>
        public float skyExtinction { get => m_skyExtinction; set => m_skyExtinction = value; }
        [SerializeField] private float m_skyExtinction = 0.0f;

        /// <summary>The intensity of the stars constellation.</summary>
        public float constellationIntensity { get => m_constellationIntensity; set => m_constellationIntensity = value; }
        [SerializeField] private float m_constellationIntensity = 0.0f;

        /// <summary>The color of the sky constellation.</summary>
        public Color constellationColor { get => m_constellationColor; set => m_constellationColor = value; }
        [SerializeField] private Color m_constellationColor = Color.white;

        /// <summary>The rotation offset to adjust the starfield cubemap texture in the sky sphere.</summary>
        public Vector3 starfieldRotationOffset { get => m_starfieldRotationOffset; set => m_starfieldRotationOffset = value; }
        [SerializeField] private Vector3 m_starfieldRotationOffset = Vector3.zero;

        /// <summary>The moon rotation offset.</summary>
        private Quaternion m_moonRotation;
        /// <summary>The moon rotation offset matrix.</summary>
        private Matrix4x4 m_moonRotationMatrix;

        /// <summary>The starfield rotation offset.</summary>
        private Quaternion m_starfieldRotation;
        /// <summary>The starfield rotation offset matrix.</summary>
        private Matrix4x4 m_starfieldRotationMatrix;

        ////////////////////////
        // Fog Scattering Tab //
        ////////////////////////

        /// <summary>The distance of the mie bright influence in the fog scattering effect.</summary>
        public float mieDistance { get => m_mieDistance; set => m_mieDistance = value; }
        [SerializeField] private float m_mieDistance = 1.0f;

        /// <summary>The distance of the global fog scattering effect.</summary>
        public float globalFogDistance { get => m_globalFogDistance; set => m_globalFogDistance = value; }
        [SerializeField] private float m_globalFogDistance = 1000.0f;

        /// <summary>The smooth step transition from where there is no global fog in the scene to where is completely foggy.</summary>
        public float globalFogSmoothStep { get => m_globalFogSmoothStep; set => m_globalFogSmoothStep = value; }
        [SerializeField] private float m_globalFogSmoothStep = 0.25f;

        /// <summary>The density of the global fog scattering effect.</summary>
        public float globalFogDensity { get => m_globalFogDensity; set => m_globalFogDensity = value; }
        [SerializeField] private float m_globalFogDensity = 1.0f;

        /// <summary>The distance of the height fog scattering effect.</summary>
        public float heightFogDistance { get => m_heightFogDistance; set => m_heightFogDistance = value; }
        [SerializeField] private float m_heightFogDistance = 100.0f;

        /// <summary>The smooth step transition from where there is no height fog in the scene to where is completely foggy.</summary>
        public float heightFogSmoothStep { get => m_heightFogSmoothStep; set => m_heightFogSmoothStep = value; }
        [SerializeField] private float m_heightFogSmoothStep = 1.0f;

        /// <summary>The density of the height fog scattering effect.</summary>
        public float heightFogDensity { get => m_heightFogDensity; set => m_heightFogDensity = value; }
        [SerializeField] private float m_heightFogDensity = 0.0f;

        /// <summary>The height altitude where the height fog scattering effect should start.</summary>
        public float heightFogStartAltitude { get => m_heightFogStartAltitude; set => m_heightFogStartAltitude = value; }
        [SerializeField] private float m_heightFogStartAltitude = 0.0f;

        /// <summary>The height altitude where the height fog scattering effect should end.</summary>
        public float heightFogEndAltitude { get => m_heightFogEndAltitude; set => m_heightFogEndAltitude = value; }
        [SerializeField] private float m_heightFogEndAltitude = 100.0f;

        /// <summary>The distance of the bluish color effect of the fog at distance.</summary>
        public float fogBluishDistance { get => m_fogBluishDistance; set => m_fogBluishDistance = value; }
        [SerializeField] private float m_fogBluishDistance = 12288.0f;

        /// <summary>The intensity of the bluish color effect of the fog at distance.</summary>
        public float fogBluishIntensity { get => m_fogBluishIntensity; set => m_fogBluishIntensity = value; }
        [SerializeField] private float m_fogBluishIntensity = 0.15f;

        /// <summary>The scattering multiplier based on the height fog.</summary>
        public float heightFogScatteringMultiplier { get => m_heightFogScatteringMultiplier; set => m_heightFogScatteringMultiplier = value; }
        [SerializeField] private float m_heightFogScatteringMultiplier = 0.5f;

        ////////////////////
        // Dynamic Clouds //
        ////////////////////
        
        /// <summary>The altitude of the dynamic clouds in the sky.</summary>
        public float dynamicCloudAltitude { get => m_dynamicCloudAltitude; set => m_dynamicCloudAltitude = value; }
        [SerializeField] private float m_dynamicCloudAltitude = 7.5f;

        /// <summary>The movement direction of the dynamic clouds.</summary>
        public float dynamicCloudDirection { get => m_dynamicCloudDirection; set => m_dynamicCloudDirection = value; }
        [SerializeField] private float m_dynamicCloudDirection = 0.0f;

        /// <summary>The movement speed of the dynamic clouds.</summary>
        public float dynamicCloudSpeed { get => m_dynamicCloudSpeed; set => m_dynamicCloudSpeed = value; }
        [SerializeField] private float m_dynamicCloudSpeed = 0.1f;

        /// <summary>The coverage of the dynamic clouds.</summary>
        public float dynamicCloudDensity { get => m_dynamicCloudDensity; set => m_dynamicCloudDensity = value; }
        [SerializeField] private float m_dynamicCloudDensity = 0.75f;

        /// <summary>The first color of the dynamic clouds.</summary>
        public Color dynamicCloudColor1 { get => m_dynamicCloudColor1; set => m_dynamicCloudColor1 = value; }
        [SerializeField] private Color m_dynamicCloudColor1 = Color.white;

        /// <summary>The second color of the dynamic clouds.</summary>
        public Color dynamicCloudColor2 { get => m_dynamicCloudColor2; set => m_dynamicCloudColor2 = value; }
        [SerializeField] private Color m_dynamicCloudColor2 = Color.white;

        /// <summary>The dynamic cloud uv.</summary>
        public Vector2 dynamicCloudUV => m_dynamicCloudUV;
        private Vector2 m_dynamicCloudUV = Vector2.zero;

        /////////////////
        // Options Tab //
        /////////////////

        /// <summary>How the shader uniforms of the sky and fog material should be updated? Locally every frame or from an external call?</summary>
        public AzureUpdateMode updateMode { get => m_updateMode; set => m_updateMode = value; }
        [SerializeField] private AzureUpdateMode m_updateMode = AzureUpdateMode.LocallyEveryFrame;

        /// <summary>How the clouds should be rendered.</summary>
        public AzureCloudMode cloudMode { get => m_cloudMode; set { m_cloudMode = value; UpdateCloudMode(); } }
        [SerializeField] private AzureCloudMode m_cloudMode = AzureCloudMode.Dynamic;


        void Awake()
        {
            UpdateCloudMode();

            InitializeSkySystem();

            // First update of the shader uniforms
            if (Application.isPlaying)
            {
                if (m_updateMode == AzureUpdateMode.LocallyEveryFrame)
                {
                    UpdateSkySystem();
                }
            }
        }

        void Update()
        {
            // Only in gameplay
            if (Application.isPlaying)
            {
                // Clouds movement
                if (m_cloudMode == AzureCloudMode.Dynamic)
                {
                    m_dynamicCloudUV = CalculeDynamicCloudUV();
                    Shader.SetGlobalVector(AzureShaderUniforms.DynamicCloudDirection, m_dynamicCloudUV);
                }

                // Update the shader uniforms every frame
                if (m_updateMode == AzureUpdateMode.LocallyEveryFrame)
                {
                    UpdateSkySystem();
                }
            }

            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                InitializeSkySystem();
                UpdateSkySystem();
            }
            #endif
        }

        private void OnEnable()
        {
            if (m_skyMaterial)
                RenderSettings.skybox = m_skyMaterial;
        }

        /// <summary>Sets the shader uniforms that only requires one update at start.</summary>
        public void InitializeSkySystem()
        {
            Shader.SetGlobalTexture(AzureShaderUniforms.SunTexture, m_sunTexture);
            Shader.SetGlobalTexture(AzureShaderUniforms.MoonTexture, m_moonTexture);
            Shader.SetGlobalTexture(AzureShaderUniforms.StarfieldTexture, m_starfieldTexture);
            Shader.SetGlobalTexture(AzureShaderUniforms.ConstellationTexture, m_constellationTexture);
            Shader.SetGlobalTexture(AzureShaderUniforms.DynamicCloudTexture, m_dynamicCloudTexture);
        }

        /// <summary>Sets the shader uniforms that requires constantly update.</summary>
        public void UpdateSkySystem()
        {
            m_moonRotation = Quaternion.Euler(m_moonRotationOffset);
            m_moonRotationMatrix = Matrix4x4.TRS(Vector3.zero, m_moonRotation, Vector3.one);

            m_starfieldRotation = Quaternion.Euler(m_starfieldRotationOffset);
            m_starfieldRotationMatrix = Matrix4x4.TRS(Vector3.zero, m_starfieldRotation, Vector3.one);

            Shader.SetGlobalVector(AzureShaderUniforms.SunDirection, -m_sunTransform.forward);
            Shader.SetGlobalVector(AzureShaderUniforms.MoonDirection, -m_moonTransform.forward);
            Shader.SetGlobalMatrix(AzureShaderUniforms.SunMatrix, m_sunTransform.worldToLocalMatrix);
            Shader.SetGlobalMatrix(AzureShaderUniforms.MoonMatrix, m_moonRotationMatrix * m_moonTransform.worldToLocalMatrix);
            Shader.SetGlobalMatrix(AzureShaderUniforms.UpDirectionMatrix, transform.worldToLocalMatrix);
            Shader.SetGlobalMatrix(AzureShaderUniforms.StarfieldMatrix, m_starfieldRotationMatrix * m_starfieldTransform.worldToLocalMatrix);
            Shader.SetGlobalFloat(AzureShaderUniforms.Kr, m_kr);
            Shader.SetGlobalFloat(AzureShaderUniforms.Km, m_km);
            Shader.SetGlobalVector(AzureShaderUniforms.Rayleigh, ComputeRayleigh() * m_rayleigh);
            Shader.SetGlobalVector(AzureShaderUniforms.Mie, ComputeMie() * m_mie);
            Shader.SetGlobalVector(AzureShaderUniforms.MieG, ComputeMieG());
            Shader.SetGlobalFloat(AzureShaderUniforms.MieDistance, m_mieDistance);
            Shader.SetGlobalFloat(AzureShaderUniforms.Scattering, m_scattering);
            Shader.SetGlobalFloat(AzureShaderUniforms.SkyLuminance, m_skyLuminance);
            Shader.SetGlobalFloat(AzureShaderUniforms.Exposure, m_exposure);
            Shader.SetGlobalColor(AzureShaderUniforms.RayleighColor, m_rayleighColor);
            Shader.SetGlobalColor(AzureShaderUniforms.MieColor, m_mieColor);
            Shader.SetGlobalFloat(AzureShaderUniforms.SunRadius, m_sunSize * 696340.0f * 10.0f);
            Shader.SetGlobalVector(AzureShaderUniforms.SunPosition, -m_sunTransform.forward * 149600000.0f);
            Shader.SetGlobalFloat(AzureShaderUniforms.SunOpacity, m_sunOpacity);
            Shader.SetGlobalColor(AzureShaderUniforms.SunColor, m_sunColor);
            Shader.SetGlobalFloat(AzureShaderUniforms.MoonRadius, m_moonSize * 1737.4f * 10.0f);
            Shader.SetGlobalVector(AzureShaderUniforms.MoonPosition, -m_moonTransform.forward * 384400.0f);
            Shader.SetGlobalFloat(AzureShaderUniforms.MoonOpacity, m_moonOpacity);
            Shader.SetGlobalColor(AzureShaderUniforms.MoonColor, m_moonColor);
            Shader.SetGlobalFloat(AzureShaderUniforms.StarsIntensity, m_starsIntensity);
            Shader.SetGlobalFloat(AzureShaderUniforms.MilkyWayIntensity, m_milkyWayIntensity);
            Shader.SetGlobalColor(AzureShaderUniforms.StarFieldColor, m_starfieldColor);
            Shader.SetGlobalFloat(AzureShaderUniforms.SkyExtinction, m_skyExtinction);
            Shader.SetGlobalFloat(AzureShaderUniforms.ConstellationIntensity, m_constellationIntensity);
            Shader.SetGlobalColor(AzureShaderUniforms.ConstellationColor, m_constellationColor);
            Shader.SetGlobalFloat(AzureShaderUniforms.GlobalFogDistance, m_globalFogDistance);
            Shader.SetGlobalFloat(AzureShaderUniforms.GlobalFogSmoothStep, m_globalFogSmoothStep);
            Shader.SetGlobalFloat(AzureShaderUniforms.GlobalFogDensity, m_globalFogDensity);
            Shader.SetGlobalFloat(AzureShaderUniforms.HeightFogDistance, m_heightFogDistance);
            Shader.SetGlobalFloat(AzureShaderUniforms.HeightFogSmoothStep, m_heightFogSmoothStep);
            Shader.SetGlobalFloat(AzureShaderUniforms.HeightFogDensity, m_heightFogDensity);
            Shader.SetGlobalFloat(AzureShaderUniforms.HeightFogStartAltitude, m_heightFogStartAltitude);
            Shader.SetGlobalFloat(AzureShaderUniforms.HeightFogEndAltitude, m_heightFogEndAltitude);
            Shader.SetGlobalFloat(AzureShaderUniforms.FogBluishDistance, m_fogBluishDistance);
            Shader.SetGlobalFloat(AzureShaderUniforms.FogBluishIntensity, m_fogBluishIntensity);
            Shader.SetGlobalFloat(AzureShaderUniforms.HeightFogScatteringMultiplier, m_heightFogScatteringMultiplier);

            // Clouds
            Shader.SetGlobalFloat(AzureShaderUniforms.DynamicCloudAltitude, m_dynamicCloudAltitude);
            Shader.SetGlobalFloat(AzureShaderUniforms.DynamicCloudDensity, Mathf.Lerp(25.0f, 0.0f, m_dynamicCloudDensity));
            Shader.SetGlobalVector(AzureShaderUniforms.DynamicCloudColor1, m_dynamicCloudColor1);
            Shader.SetGlobalVector(AzureShaderUniforms.DynamicCloudColor2, m_dynamicCloudColor2);
        }

        /// <summary>Total rayleigh computation.</summary>
        private Vector3 ComputeRayleigh()
        {
            Vector3 rayleigh = Vector3.one;
            Vector3 lambda = m_wavelength * 1e-9f;
            float n = 1.0003f; // Refractive index of air
            float pn = 0.035f; // Depolarization factor for standard air.
            float n2 = n * n;
            //float N = 2.545E25f;
            float N = m_molecularDensity * 1E25f;
            float temp = (8.0f * Mathf.PI * Mathf.PI * Mathf.PI * ((n2 - 1.0f) * (n2 - 1.0f))) / (3.0f * N) * ((6.0f + 3.0f * pn) / (6.0f - 7.0f * pn));

            rayleigh.x = temp / Mathf.Pow(lambda.x, 4.0f);
            rayleigh.y = temp / Mathf.Pow(lambda.y, 4.0f);
            rayleigh.z = temp / Mathf.Pow(lambda.z, 4.0f);

            return rayleigh;
        }

        /// <summary>Total mie computation.</summary>
        private Vector3 ComputeMie()
        {
            Vector3 mie;
            Vector3 k = new Vector3(686.0f, 678.0f, 682.0f);

            //float c = (0.6544f * Turbidity - 0.6510f) * 1e-16f;
            float c = (0.6544f * 5.0f - 0.6510f) * 10f * 1e-9f;
            mie.x = (434.0f * c * Mathf.PI * Mathf.Pow((4.0f * Mathf.PI) / m_wavelength.x, 2.0f) * k.x);
            mie.y = (434.0f * c * Mathf.PI * Mathf.Pow((4.0f * Mathf.PI) / m_wavelength.y, 2.0f) * k.y);
            mie.z = (434.0f * c * Mathf.PI * Mathf.Pow((4.0f * Mathf.PI) / m_wavelength.z, 2.0f) * k.z);

            //float c = (6544f * 5.0f - 6510f) * 10.0f * 1.0e-9f;
            //mie.x = (0.434f * c * Mathf.PI * Mathf.Pow((2.0f * Mathf.PI) / m_wavelength.x, 2.0f) * k.x) / 3.0f;
            //mie.y = (0.434f * c * Mathf.PI * Mathf.Pow((2.0f * Mathf.PI) / m_wavelength.y, 2.0f) * k.y) / 3.0f;
            //mie.z = (0.434f * c * Mathf.PI * Mathf.Pow((2.0f * Mathf.PI) / m_wavelength.z, 2.0f) * k.z) / 3.0f;

            return mie;
        }

        /// <summary>Computed the mie directionality factor.</summary>
        private Vector3 ComputeMieG()
        {
            return new Vector3(1.0f - m_mieDirectionalityFactor * m_mieDirectionalityFactor, 1.0f + m_mieDirectionalityFactor * m_mieDirectionalityFactor, 2.0f * m_mieDirectionalityFactor);
        }

        /// <summary>Compute the dynamic cloud uv position based on the direction and speed and returns it as a Vector2.</summary>
        private Vector2 CalculeDynamicCloudUV()
        {
            float x = m_dynamicCloudUV.x;
            float z = m_dynamicCloudUV.y;
            float windSpeed = m_dynamicCloudSpeed * 0.05f * Time.deltaTime;

            x += windSpeed * Mathf.Sin(0.01745329f * m_dynamicCloudDirection);
            z += windSpeed * Mathf.Cos(0.01745329f * m_dynamicCloudDirection);

            if (x >= 1.0f) x -= 1.0f;
            if (z >= 1.0f) z -= 1.0f;

            return new Vector2(x, z);
        }

        /// <summary>Updates the sky material shader according to the cloud mode selected.</summary>
        public void UpdateCloudMode()
        {
            switch (m_cloudMode)
            {
                case AzureCloudMode.Off:
                    if (m_emptySkyShader != null) m_skyMaterial.shader = m_emptySkyShader;
                    break;

                case AzureCloudMode.Dynamic:
                    if (m_dynamicCloudShader != null) m_skyMaterial.shader = m_dynamicCloudShader;
                    break;
            }
        }
    }
}