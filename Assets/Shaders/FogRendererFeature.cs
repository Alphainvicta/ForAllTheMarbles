using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FogRendererFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class FogSettings
    {
        public Material fogMaterial = null; 
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents; 
    }

    public FogSettings settings = new FogSettings(); 

    class FogPass : ScriptableRenderPass
    {
        private Material fogMaterial;
        private RTHandle source;

        public FogPass(Material fogMaterial)
        {
            this.fogMaterial = fogMaterial;
        }

        public void Setup(RTHandle source)
        {
            this.source = source;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            
            CommandBuffer cmd = CommandBufferPool.Get("FogPass");

            
            Blit(cmd, source, source, fogMaterial, 0);

            
            context.ExecuteCommandBuffer(cmd);

            
            CommandBufferPool.Release(cmd);
        }
    }

    private FogPass fogPass;

    public override void Create()
    {
        
        fogPass = new FogPass(settings.fogMaterial)
        {
            renderPassEvent = settings.renderPassEvent 
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        
        fogPass.Setup(renderer.cameraColorTargetHandle);

        
        renderer.EnqueuePass(fogPass);
    }
}