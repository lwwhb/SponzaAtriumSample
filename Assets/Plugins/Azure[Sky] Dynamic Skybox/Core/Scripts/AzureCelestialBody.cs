using System;

namespace UnityEngine.AzureSky
{
    /// <summary>A celestial body instance.</summary>
    [Serializable]
    public sealed class AzureCelestialBody
    {
        /// <summary>The transform that will receive the celestial body coordinate.</summary>
        public Transform transform { get => m_transform; set => m_transform = value; }
        [SerializeField] private Transform m_transform;

        /// <summary>The celestial body that this instance will simulate.</summary>
        public AzureCelestialBodyType type { get => m_type; set => m_type = value; }
        [SerializeField] private AzureCelestialBodyType m_type;
    }
}