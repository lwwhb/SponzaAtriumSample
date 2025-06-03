using System;

namespace UnityEngine.AzureSky
{
    /// <summary>Class that represents a global weather preset.</summary>
    [Serializable]
    public sealed class AzureGlobalWeather
    {
        /// <summary>The weather preset for this global weather.</summary>
        public AzureWeatherPreset weatherPreset { get => m_weatherPreset; set => m_weatherPreset = value; }
        [SerializeField] private AzureWeatherPreset m_weatherPreset;

        /// <summary>The time in seconds of the weather transition to this preset.</summary>
        public float transitionTime { get => m_transitionTime; set => m_transitionTime = value; }
        [SerializeField] private float m_transitionTime = 10f;

        public AzureGlobalWeather(float transitionTime)
        {
            m_transitionTime = transitionTime;
        }
    }
}