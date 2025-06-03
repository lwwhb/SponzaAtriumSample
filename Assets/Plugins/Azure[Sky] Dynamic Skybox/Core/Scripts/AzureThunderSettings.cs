using System;

namespace UnityEngine.AzureSky
{
    /// <summary>Thunder settings container.</summary>
    [Serializable] public sealed class AzureThunderSettings
    {
        /// <summary>The reference to the transform of the thunder prefab.</summary>
        public Transform transform { get => m_transform; set => m_transform = value; }
        [SerializeField] private Transform m_transform = null;

        /// <summary>The position to instantiate this thunder prefab.</summary>
        public Vector3 position { get => m_position; set => m_position = value; }
        [SerializeField] private Vector3 m_position = Vector3.zero;
    }
}