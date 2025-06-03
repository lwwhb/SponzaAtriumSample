namespace UnityEngine.AzureSky
{
    public sealed class AzureThunderPrefab : MonoBehaviour
    {
        /// <summary>The lightning frequency that affect the clouds and scene while this thunder prefab is playing.</summary>
        public AnimationCurve lightFrequency { get => m_lightFrequency; set => m_lightFrequency = value; }
        [SerializeField] private AnimationCurve m_lightFrequency = null;

        /// <summary>The delay to play the thunder audio clip after this prefab instantiation.</summary>
        public float audioDelay { get => m_audioDelay; set => m_audioDelay = value; }
        [SerializeField] private float m_audioDelay = 0.0f;

        /// <summary>The reference to the AudioSource component attached to this prefab.</summary>
        private AudioSource m_audioSource = null;

        /// <summary>The reference to the directional light used to lighting the scene while this thunder is playing.</summary>
        private Light m_light = null;


        // Internal use
        private float m_time = 0.0f;
        private bool m_canPlayAudioClip = true;


        private void Start()
        {
            m_audioSource = GetComponent<AudioSource>();
            m_light = GetComponent<Light>();
        }

        private void Update()
        {
            m_time += Time.deltaTime;

            m_light.intensity = m_lightFrequency.Evaluate(m_time / m_audioSource.clip.length);
            Shader.SetGlobalFloat (AzureShaderUniforms.ThunderLightningEffect, m_light.intensity);

            if (m_time >= m_audioDelay && m_canPlayAudioClip)
            {
                m_audioSource.Play();
                m_canPlayAudioClip = false;
            }

            if(m_time >= m_audioDelay + m_audioSource.clip.length)
                Destroy(gameObject);
        }
    }
}