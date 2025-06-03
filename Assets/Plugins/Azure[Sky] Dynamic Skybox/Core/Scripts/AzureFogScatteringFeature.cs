using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.AzureSky
{
    public class AzureFogScatteringFeature : ScriptableRendererFeature
    {
        public RenderPassEvent renderPassEvent { get => m_renderPassEvent; set => m_renderPassEvent = value; }
        [SerializeField] private RenderPassEvent m_renderPassEvent = RenderPassEvent.BeforeRenderingSkybox;

        /// <summary>The material that will render the fog scattering RenderTexture into the screen.</summary>
        public Material fogRendererMaterial { get => m_fogRendererMaterial; set => m_fogRendererMaterial = value; }
        [SerializeField] private Material m_fogRendererMaterial = null;

        /// <summary>The instance of the AzureFogScatteringPass class.</summary>
        private AzureFogScatteringPass m_azureFogScatteringPass;

        /// <summary>Called when the renderer feature is created or modified.</summary>
        public override void Create()
        {
            m_azureFogScatteringPass = new AzureFogScatteringPass();
            m_azureFogScatteringPass.fogRendererMaterial = m_fogRendererMaterial;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (m_fogRendererMaterial == null)
            {
                Debug.LogWarningFormat("Missing the Fog Renderer Material. {0} blit pass will not execute. Check for missing reference in the assigned renderer.", GetType().Name);
                return;
            }

            m_azureFogScatteringPass.renderPassEvent = m_renderPassEvent;
            renderer.EnqueuePass(m_azureFogScatteringPass);
        }

        private class AzureFogScatteringPass : ScriptableRenderPass
        {
            /// <summary>The material that will render the fog scattering RenderTexture into the screen.</summary>
            public Material fogRendererMaterial { get => m_fogRendererMaterial; set => m_fogRendererMaterial = value; }
            [SerializeField] private Material m_fogRendererMaterial = null;

            //private RenderTextureDescriptor m_textureDescriptor;
            private static Vector4 m_scaleBias = new Vector4(1f, 1f, 0f, 0f);
            private TextureHandle m_sourceTextureHandle;
            private TextureHandle m_destinationTextureHandle;

            // For backward compatibility use
            private RTHandle m_sourceRTHandle;
            private RTHandle m_destinationRTHandle;

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                if (m_fogRendererMaterial == null) return;

                UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

                // The following line ensures that the render pass doesn't blit from the back buffer.
                if (resourceData.isActiveTargetBackBuffer)
                    return;

                m_sourceTextureHandle = resourceData.activeColorTexture;

                // Define the texture descriptor for creating the destination render graph texture.
                var destinationDesc = renderGraph.GetTextureDesc(m_sourceTextureHandle);
                destinationDesc.name = $"CameraColor-FogScatteringPass";
                destinationDesc.clearBuffer = false;
                destinationDesc.depthBufferBits = 0;

                m_destinationTextureHandle = renderGraph.CreateTexture(destinationDesc);

                // This check is to avoid an error from the material preview in the scene
                if (!m_sourceTextureHandle.IsValid() || !m_destinationTextureHandle.IsValid())
                    return;

                // The AddBlitPass method adds the render graph pass that blits from the source to the destination texture.
                RenderGraphUtils.BlitMaterialParameters para = new(m_sourceTextureHandle, m_destinationTextureHandle, m_fogRendererMaterial, 0);
                renderGraph.AddBlitPass(para);

                // Use the destination texture as the camera texture to avoid the extra blit from the destination texture back to the camera texture.
                resourceData.cameraColor = m_destinationTextureHandle;
            }

            [System.Obsolete] // We still need this in case the Render Graphic Compatibility Mode is checked in the Graphics Settings
            public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
            {
                m_sourceRTHandle = renderingData.cameraData.renderer.cameraColorTargetHandle;
                var desc = renderingData.cameraData.cameraTargetDescriptor;
                desc.depthBufferBits = 0;
                RenderingUtils.ReAllocateHandleIfNeeded(ref m_destinationRTHandle, desc, FilterMode.Point, TextureWrapMode.Clamp, name: "_TemporaryDestinationHandle");
            }

            [System.Obsolete] // We still need this in case the Render Graphic Compatibility Mode is checked in the Graphics Settings
            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (m_fogRendererMaterial == null) return;

                CommandBuffer cmd = CommandBufferPool.Get();
                cmd.CopyTexture(m_sourceRTHandle, m_destinationRTHandle);
                Blitter.BlitTexture(cmd, m_destinationRTHandle, m_scaleBias, m_fogRendererMaterial, 0);
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                CommandBufferPool.Release(cmd);
            }
        }
    }
}