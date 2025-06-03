using UnityEngine.Rendering;

namespace UnityEngine.AzureSky
{
    [ExecuteInEditMode]
    [ImageEffectAllowedInSceneView]
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("Azure[Sky] Dynamic Skybox/Azure Volumetric Light Renderer")]
    public sealed class AzureVolumetricLightRenderer : MonoBehaviour
    {
        #pragma warning disable 0108 // Remove warning caused by obsolete members that do not actually exist
        /// <summary>The reference to the camera that the volumetric light should be rendered.</summary>
        public Camera camera { get => m_camera; set => m_camera = value; }
        private Camera m_camera = null;
        #pragma warning restore 0108

        /// <summary>Stores the calculated frustum planes from the camera this component is attached to.</summary>
        public Plane[] cameraFrustumPlanes { get => m_cameraFrustumPlanes; set => m_cameraFrustumPlanes = value; }
        private Plane[] m_cameraFrustumPlanes = null;

        /// <summary>The command buffer used to render the volumetric lights in the scene.</summary>
        public CommandBuffer commandBuffer { get => m_commandBuffer; set => m_commandBuffer = value; }
        [SerializeField] private CommandBuffer m_commandBuffer = null;

        /// <summary>The extinction of the light along the distance.</summary>
        public float lightExtinctionDistance { get => m_lightExtinctionDistance; set => m_lightExtinctionDistance = value; }
        [SerializeField] private float m_lightExtinctionDistance = 1000.0f;

        /// <summary>How smooth is the light extinction along the distance?</summary>
        public float lightExtinctionSmoothStep { get => m_lightExtinctionSmoothStep; set => m_lightExtinctionSmoothStep = value; }
        [SerializeField] private float m_lightExtinctionSmoothStep = 0.5f;

        private void Start()
        {
            m_camera = GetComponent<Camera>();
            m_commandBuffer = new CommandBuffer();
            m_commandBuffer.name = "Azure Volumetric Light Command Buffer";
            m_camera.AddCommandBuffer(CameraEvent.AfterSkybox, m_commandBuffer);
        }

        private void OnEnable()
        {
            RenderPipelineManager.beginCameraRendering += CustomOnPreRender;
            if (m_commandBuffer == null || m_camera == null) return;
            m_camera.AddCommandBuffer(CameraEvent.AfterSkybox, m_commandBuffer);
        }

        private void OnDisable()
        {
            RenderPipelineManager.beginCameraRendering -= CustomOnPreRender;
            if (m_commandBuffer == null || m_camera == null) return;
            m_camera.RemoveCommandBuffer(CameraEvent.AfterSkybox, m_commandBuffer);
        }

        /// <summary>We need to remove the command buffer from the scene view camera in case of loading another scene in editor.</summary>
        #if UNITY_EDITOR
        private void OnDestroy()
        {
            if (!Application.isPlaying)
            {
                UnityEditor.SceneView sceneView = UnityEditor.SceneView.lastActiveSceneView;
                if (sceneView != null)
                {
                    sceneView.camera.RemoveAllCommandBuffers();
                }
            }
        }
        #endif

        private void CustomOnPreRender(ScriptableRenderContext context, Camera camera)
        {
            if (m_commandBuffer == null || m_camera == null) return;

            // Clearing the command buffer to avoid garbage from the previous frame
            m_commandBuffer.Clear();

            // Calculate the camera frustum planes, so it can be used per each volumetric light component to compute a custom frustum culling
            m_cameraFrustumPlanes = GeometryUtility.CalculateFrustumPlanes(m_camera);

            // Trigger the OnVolumetricLightPreRender callback to notify all the volumetric lights that this camera is ready to render
            AzureNotificationCenter.Invoke.OnVolumetricLightPreRenderCallback(this);

            //context.ExecuteCommandBuffer(m_commandBuffer);
            //Debug.Log("Rendered!!!");
        }
    }
}