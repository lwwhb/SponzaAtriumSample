namespace UnityEngine.AzureSky
{
    [RequireComponent(typeof(AudioSource))]
    [AddComponentMenu("Azure[Sky] Dynamic Skybox/Azure Sound FX")]
    public sealed class AzureSoundFX : MonoBehaviour
    {
        /// <summary>The audio source attached to this game object.</summary>
        private AudioSource m_audioSource;

        /// <summary>The volume of the audio source attached to this game object.</summary>
        public float volume
        {
            get => m_volume;
            set
            {
                m_volume = value;

                if (m_audioSource)
                {
                    m_audioSource.volume = value;
                    if (value > 0)
                    {
                        if (!m_audioSource.isPlaying) m_audioSource.Play();
                    }
                    else if (m_audioSource.isPlaying) m_audioSource.Stop();
                }
            }
        }
        [SerializeField] private float m_volume;

        private void Awake()
        {
            m_audioSource = GetComponent<AudioSource>();
        }
    }
}