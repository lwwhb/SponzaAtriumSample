using System;

namespace UnityEngine.AzureSky
{
    /// <summary>Stores a follower and a target transform that will be used by the AzureCoreSystem component.</summary>
    [Serializable]
    public sealed class AzureFollowTarget
    {
        /// <summary>The transform that will follow the target.</summary>
        public Transform follower { get => m_follower; set => m_follower = value; }
        [SerializeField] private Transform m_follower;

        /// <summary>The transform to follow.</summary>
        public Transform target { get => m_target; set => m_target = value; }
        [SerializeField] private Transform m_target;
    }
}