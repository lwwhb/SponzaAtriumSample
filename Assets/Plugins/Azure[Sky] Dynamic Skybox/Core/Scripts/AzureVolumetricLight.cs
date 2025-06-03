namespace UnityEngine.AzureSky
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Light))]
    [AddComponentMenu("Azure[Sky] Dynamic Skybox/Azure Volumetric Light")]
    public sealed class AzureVolumetricLight : MonoBehaviour
    {
        #pragma warning disable 0108 // Remove warning caused by obsolete members that do not actually exist
        /// <summary>The reference to the target light component.</summary>
        public Light light { get => m_light; set => m_light = value; }
        [SerializeField] private Light m_light = null;
        #pragma warning restore 0108

        /// <summary>The mesh used to render the light in the scene.</summary>
        public Mesh lightMesh { get => m_lightMesh; set => m_lightMesh = value; }
        [SerializeField] private Mesh m_lightMesh = null;

        /// <summary>The material used to render the light in the scene.</summary>
        public Material lightMaterial { get => m_lightMaterial; set => m_lightMaterial = value; }
        [SerializeField] private Material m_lightMaterial = null;

        /// <summary>The mie directionality factor.</summary>
        public float mieDirectionalityFactor { get => m_mieDirectionalityFactor; set => m_mieDirectionalityFactor = value; }
        [Range(0.0f, 0.9f)] [SerializeField] private float m_mieDirectionalityFactor = 0.75f;

        /// <summary>The volumetric light intensity multiplier.</summary>
        public float intensityMultiplier { get => m_intensityMultiplier; set => m_intensityMultiplier = value; }
        [Range(0.0f, 1.0f)][SerializeField] private float m_intensityMultiplier = 0.7f;

        /// <summary>The volumetric light range multiplier.</summary>
        public float rangeMultiplier { get => m_rangeMultiplier; set => m_rangeMultiplier = value; }
        [Range(0.0f, 1.0f)][SerializeField] private float m_rangeMultiplier = 1.0f;

        /// <summary>The volumetric light angle multiplier (Spot Light Only!).</summary>
        public float angleMultiplier { get => m_angleMultiplier; set => m_angleMultiplier = value; }
        [Range(0.0f, 5.0f)][SerializeField] private float m_angleMultiplier = 1.0f;

        /// <summary>The transformation matrix used to render the light mesh in the scene.</summary>
        public Matrix4x4 trsMatrix { get => m_trsMatrix; set => m_trsMatrix = value; }
        private Matrix4x4 m_trsMatrix = Matrix4x4.identity;

        /// <summary>The distance this light is from the camera.</summary>
        public float distanceToCamera { get => m_distanceToCamera; set => m_distanceToCamera = value; }
        private float m_distanceToCamera = 0.0f;

        /// <summary>Is the camera inside this light sphere range? (1 = true) (0 = false). Point/Spot light only!</summary>
        public int isCameraInsideSphereRange { get => m_isCameraInsideSphereRange; set => m_isCameraInsideSphereRange = value; }
        private int m_isCameraInsideSphereRange = 0;

        /// <summary>Is the camera inside this light cone range? (1 = true) (0 = false). Spot light only!</summary>
        public int isCameraInsideConeRange { get => m_isCameraInsideConeRange; set => m_isCameraInsideConeRange = value; }
        private int m_isCameraInsideConeRange = 0;

        /// <summary>The property block used to render this volumetric light instance.</summary>
        public MaterialPropertyBlock propertyBlock { get => m_propertyBlock; set => m_propertyBlock = value; }
        [SerializeField] private MaterialPropertyBlock m_propertyBlock = null;

        private float m_spotLightAngle = 1.0f;
        private float m_spotLightAngleRad = 1.0f;
        private float m_spotLightAngleScale = 0.0f;
        private float m_lightRange = 1.0f;
        private float m_spotAngleToCamera = 0.0f;
        private Vector3 m_spotToCameraVector = Vector3.zero;

        private void Start()
        {
            m_light = GetComponent<Light>();
            m_propertyBlock = new MaterialPropertyBlock();
        }

        private void OnEnable()
        {
            AzureNotificationCenter.OnVolumetricLightPreRender += OnVolumetricLightPreRender;
        }

        private void OnDisable()
        {
            AzureNotificationCenter.OnVolumetricLightPreRender -= OnVolumetricLightPreRender;
        }

        /// <summary>Check if a sphere is inside the camera frustum planes.</summary>
        bool CheckSphereCulling(Plane[] planes, Vector3 center, float radius)
        {
            for (int i = 0; i < planes.Length; i++)
            {
                if (planes[i].normal.x * center.x + planes[i].normal.y * center.y +
                  planes[i].normal.z * center.z + planes[i].distance < -radius)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Event triggered from the OnPreRender event of the current camera rendering the volumetric lights using the "AzureVolumetricLightRenderer.cs" component.
        /// See the OnPreRender method of the AzureVolumetricLightRenderer.cs component attached to the camera.
        /// We need to register to the event this way, because the OnPreRender event is only called by the scripts attached to the camera.
        /// We can't write our code into the OnPreRender event right here in this script!
        /// We use the OnPreRender event to adjust the light mesh and material setup just before the camera rendering.
        /// </summary>
        private void OnVolumetricLightPreRender(AzureVolumetricLightRenderer renderer)
        {
            if (m_light == null) return;
            if (m_propertyBlock == null) { m_propertyBlock = new MaterialPropertyBlock(); }

            // Renders the volumetric light according to its type.
            switch (m_light.type)
            {
                case LightType.Spot:

                    m_spotLightAngle = m_light.spotAngle * 0.5f * m_angleMultiplier;
                    m_lightRange = m_light.range * m_rangeMultiplier;

                    // Cancel this volumetric light rendering if the light is outside frustum culling of the current camera rendering
                    if (!CheckSphereCulling(renderer.cameraFrustumPlanes, transform.position, m_lightRange)) return;

                    // Set the spotlight transformation matrix
                    m_spotLightAngleRad = m_spotLightAngle * Mathf.Deg2Rad;
                    m_spotLightAngleScale = Mathf.Tan(m_spotLightAngleRad) * m_lightRange * 2.0f;
                    m_trsMatrix = Matrix4x4.TRS(transform.position, transform.rotation, new Vector3(m_spotLightAngleScale, m_spotLightAngleScale, m_lightRange));

                    // Get the light distance from the camera
                    m_distanceToCamera = Vector3.Distance(renderer.camera.transform.position, transform.position);

                    // Check if the camera is inside the volumetric light sphere
                    m_isCameraInsideSphereRange = m_distanceToCamera < m_lightRange ? 1 : 0;
                    m_isCameraInsideConeRange = 0;

                    // Check if the camera is inside the volumetric light cone
                    if (m_isCameraInsideSphereRange == 1)
                    {
                        // Calculate the direction vector from the spotlight position to the camera position
                        m_spotToCameraVector = renderer.camera.transform.position - transform.position;
                        // Calculate the angle between the spotlight's forward direction and the direction to the camera
                        m_spotAngleToCamera = Vector3.Angle(transform.forward, m_spotToCameraVector);
                        // If the angle is less than half of the spotlight's range angle, the camera is inside the cone
                        m_isCameraInsideConeRange = m_spotAngleToCamera < m_spotLightAngle ? 1 : 0;
                    }

                    // Set the parameters of the material property block
                    m_propertyBlock.SetFloat(AzureShaderUniforms.VolumetricLightIntensity, m_light.intensity * m_intensityMultiplier);
                    m_propertyBlock.SetColor(AzureShaderUniforms.VolumetricLightColor, m_light.color);
                    m_propertyBlock.SetFloat(AzureShaderUniforms.VolumetricLightExtinctionDistance, renderer.lightExtinctionDistance);
                    m_propertyBlock.SetFloat(AzureShaderUniforms.VolumetricLightExtinctionSmoothStep, renderer.lightExtinctionSmoothStep);
                    m_propertyBlock.SetVector(AzureShaderUniforms.VolumetricLightMieG, ComputeMieG());
                    m_propertyBlock.SetVector(AzureShaderUniforms.VolumetricLightWorldPosition, transform.position);
                    renderer.commandBuffer.DrawMesh(m_lightMesh, m_trsMatrix, m_lightMaterial, 0, m_isCameraInsideConeRange, m_propertyBlock);
                    break;

                case LightType.Directional:
                    // Current not supported!
                    break;

                case LightType.Point:

                    // Get the light distance from the camera
                    m_distanceToCamera = Vector3.Distance(transform.position, renderer.camera.transform.position);

                    // Cancel this volumetric light rendering if the light is outside frustum culling of the current camera rendering
                    if (!CheckSphereCulling(renderer.cameraFrustumPlanes, transform.position, m_light.range)) return;

                    // Check if the camera is inside the volumetric light sphere
                    m_isCameraInsideSphereRange = m_distanceToCamera < m_light.range ? 1 : 0;

                    // Set the point light transformation matrix
                    m_lightRange = m_light.range * m_rangeMultiplier;
                    m_trsMatrix = Matrix4x4.TRS(transform.position, transform.rotation, new Vector3(m_lightRange, m_lightRange, m_lightRange));

                    // Set the parameters of the material property block
                    m_propertyBlock.SetFloat(AzureShaderUniforms.VolumetricLightIntensity, m_light.intensity * m_intensityMultiplier);
                    m_propertyBlock.SetColor(AzureShaderUniforms.VolumetricLightColor, m_light.color);
                    m_propertyBlock.SetFloat(AzureShaderUniforms.VolumetricLightExtinctionDistance, renderer.lightExtinctionDistance);
                    m_propertyBlock.SetFloat(AzureShaderUniforms.VolumetricLightExtinctionSmoothStep, renderer.lightExtinctionSmoothStep);
                    m_propertyBlock.SetVector(AzureShaderUniforms.VolumetricLightMieG, ComputeMieG());
                    renderer.commandBuffer.DrawMesh(m_lightMesh, m_trsMatrix, m_lightMaterial, 0, m_isCameraInsideSphereRange, m_propertyBlock);
                    break;
            }
        }

        /// <summary>Computed the mie directionality factor.</summary>
        private Vector3 ComputeMieG()
        {
            return new Vector3(1.0f - m_mieDirectionalityFactor * m_mieDirectionalityFactor, 1.0f + m_mieDirectionalityFactor * m_mieDirectionalityFactor, 2.0f * m_mieDirectionalityFactor);
        }
    }
}