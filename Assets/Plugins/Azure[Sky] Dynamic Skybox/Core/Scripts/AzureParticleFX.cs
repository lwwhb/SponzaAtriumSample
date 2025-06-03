namespace UnityEngine.AzureSky
{
    [RequireComponent(typeof(ParticleSystem))]
    [AddComponentMenu("Azure[Sky] Dynamic Skybox/Azure Particle FX")]
    public sealed class AzureParticleFX : MonoBehaviour
    {
        /// <summary>The particle system attached to this game object.</summary>
        private ParticleSystem m_particleSystem;

        /// <summary>The reference to the emission module of the attached particle system.</summary>
        private ParticleSystem.EmissionModule m_particleEmission;

        /// <summary>The emission intensity of the particle system attached to this game object.</summary>
        public float intensity
        {
            get => m_intensity;
            set
            {
                m_intensity = value;

                if (m_particleSystem)
                {
                    m_particleEmission.rateOverTimeMultiplier = value;
                    if (value > 0)
                    {
                        if (!m_particleSystem.isPlaying) m_particleSystem.Play();
                    }
                    else if (m_particleSystem.isPlaying) m_particleSystem.Stop();
                }
            }
        }
        [SerializeField] private float m_intensity;

        private void Awake()
        {
            m_particleSystem = GetComponent<ParticleSystem>();
            if (m_particleSystem) m_particleEmission = m_particleSystem.emission;
        }
    }
}