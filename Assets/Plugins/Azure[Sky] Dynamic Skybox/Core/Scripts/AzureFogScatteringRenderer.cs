//Based on Unity's GlobalFog.
namespace UnityEngine.AzureSky
{
    [ExecuteInEditMode]
    [ImageEffectAllowedInSceneView]
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("Azure[Sky] Dynamic Skybox/Azure Fog Scattering Renderer")]
    public sealed class AzureFogScatteringRenderer : MonoBehaviour
    {
        #pragma warning disable 0108 // Remove warning caused by obsolete members that do not actually exist
        /// <summary>The reference to the camera that the fog scattering should be rendered.</summary>
        public Camera camera { get => m_camera; set => m_camera = value; }
        private Camera m_camera = null;
        #pragma warning restore 0108

        /// <summary>The material that will render the fog scattering RenderTexture into the screen.</summary>
        public Material fogRendererMaterial { get => m_fogRendererMaterial; set => m_fogRendererMaterial = value; }
        [SerializeField] private Material m_fogRendererMaterial = null;

        /// <summary>The material that will compute the fog scattering effect and render it into a RenderTexture.</summary>
        public Material fogComputationMaterial { get => m_fogComputationMaterial; set => m_fogComputationMaterial = value; }
        [SerializeField] private Material m_fogComputationMaterial = null;

        /// <summary>The render texture that stores the fog scattering data. (RGB: Scattering Data), (Alpha: Fog Data).</summary>
        public RenderTexture fogScatteringRT { get => m_fogScatteringRT; set => m_fogScatteringRT = value; }
        [SerializeField] private RenderTexture m_fogScatteringRT = null;

        /// <summary>The start width resolution of the fog scattering render texture.</summary>
        public int fogRenderTextureWidth => m_fogRenderTextureWidth;
        [SerializeField] private int m_fogRenderTextureWidth = 1920;

        /// <summary>The start height resolution of the fog scattering render texture.</summary>
        public int fogRenderTextureHeight => m_fogRenderTextureHeight;
        [SerializeField] private int m_fogRenderTextureHeight = 1080;

        /// <summary>Stores the camera frunstum corners position.</summary>
        private Vector3[] m_frustumCorners = new Vector3[4];

        /// <summary>The view port rect.</summary>
        private Rect m_viewRect = new Rect(0, 0, 1, 1);

        /// <summary>The camera frustum corners matrix.</summary>
        private Matrix4x4 m_frustumCornersArray = Matrix4x4.identity;

        private void Start()
        {
            m_camera = GetComponent<Camera>();
            SetFogScatteringResolution(m_fogRenderTextureWidth, m_fogRenderTextureHeight);
        }

        private void OnEnable()
        {
            m_camera = GetComponent<Camera>();
        }

        [ImageEffectOpaque]
        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            m_camera.depthTextureMode |= DepthTextureMode.Depth;

            if (m_fogComputationMaterial == null || m_fogRendererMaterial == null)
            {
                Graphics.Blit(source, destination);
                return;
            }

            m_camera.CalculateFrustumCorners(m_viewRect, m_camera.farClipPlane, m_camera.stereoActiveEye, m_frustumCorners);
            m_frustumCornersArray = Matrix4x4.identity;
            m_frustumCornersArray.SetRow(0, m_camera.transform.TransformVector(m_frustumCorners[0]));  // bottom left
            m_frustumCornersArray.SetRow(2, m_camera.transform.TransformVector(m_frustumCorners[1]));  // top left
            m_frustumCornersArray.SetRow(3, m_camera.transform.TransformVector(m_frustumCorners[2]));  // top right
            m_frustumCornersArray.SetRow(1, m_camera.transform.TransformVector(m_frustumCorners[3]));  // bottom right
            m_fogComputationMaterial.SetMatrix(AzureShaderUniforms.FrustumCornersMatrix, m_frustumCornersArray);
            m_fogRendererMaterial.SetMatrix(AzureShaderUniforms.FrustumCornersMatrix, m_frustumCornersArray);

            // Compute the fog scattering effect and render it into a RenderTexture
            Graphics.Blit(null, m_fogScatteringRT, m_fogComputationMaterial, 0);

            // Render the fog scattering RenderTexture into the screen.
            m_fogRendererMaterial.SetTexture(AzureShaderUniforms.FogScatteringDataTexture, m_fogScatteringRT);
            Graphics.Blit(source, destination, m_fogRendererMaterial, 0);
        }

        /// <summary>Sets the resolution of the render texture that stores fog scattering data.</summary>
        public void SetFogScatteringResolution(int width, int height)
        {
            m_fogRenderTextureWidth = width;
            m_fogRenderTextureHeight = height;

            m_fogScatteringRT = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default)
            {
                name = "Fog Scattering RT",
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };

            m_fogScatteringRT.Create();
        }
    }
}