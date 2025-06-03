using System;

namespace UnityEngine.AzureSky
{
    [AddComponentMenu("Azure[Sky] Dynamic Skybox/Azure Weather Zone")]
    public sealed class AzureWeatherZone : MonoBehaviour
    {
        /// <summary>The weather preset for this local weather zone.</summary>
        public AzureWeatherPreset weatherPreset { get => m_weatherPreset; set => m_weatherPreset = value; }
        [SerializeField] private AzureWeatherPreset m_weatherPreset;

        /// <summary>The collider used as volume by this local weather zone.</summary>
        public Collider colliderVolume { get => m_colliderVolume; set => m_colliderVolume = value; }
        [SerializeField] private Collider m_colliderVolume;

        /// <summary>How smooth is the weather change when entering a local weather zone.</summary>
        public float blendDistance { get => m_blendDistance; set => m_blendDistance = value; }
        [SerializeField] private float m_blendDistance = 0f;

        private void Awake()
        {
            if (!m_colliderVolume) m_colliderVolume = GetComponent<Collider>();
        }

        #if UNITY_EDITOR
        /// <summary>The color used to draw the volume gizmo.</summary>
        private Color m_gizmosColor1 = new Color(0, 1, 0, 0.25f);

        /// <summary>Draws the zone collider gizmos. Based on Unity's PostProcessVolume.cs.</summary>
        private void OnDrawGizmos()
        {
            if (!m_colliderVolume) m_colliderVolume = GetComponent<Collider>();
            if (m_colliderVolume == null) return;

            if (m_colliderVolume.enabled)
            {
                Vector3 scale = transform.lossyScale;
                Vector3 invScale = new Vector3(1f / scale.x, 1f / scale.y, 1f / scale.z);
                Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, scale);

                Type type = m_colliderVolume.GetType();
                if (type == typeof(BoxCollider))
                {
                    BoxCollider c = (BoxCollider)m_colliderVolume;
                    Gizmos.color = m_gizmosColor1;
                    Gizmos.DrawCube(c.center, c.size);
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireCube(c.center, c.size + invScale * m_blendDistance * 4f);
                }
                else if (type == typeof(SphereCollider))
                {
                    SphereCollider c = (SphereCollider)m_colliderVolume;
                    Gizmos.color = m_gizmosColor1;
                    Gizmos.DrawSphere(c.center, c.radius);
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(c.center, c.radius + invScale.x * m_blendDistance * 2f);
                }
                else if (type == typeof(MeshCollider))
                {
                    MeshCollider c = (MeshCollider)m_colliderVolume;

                    // Only convex mesh collider are allowed
                    if (!c.convex)
                        c.convex = true;

                    // Mesh pivot should be centered or this won't work
                    Gizmos.color = m_gizmosColor1;
                    Gizmos.DrawMesh(c.sharedMesh);
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireMesh(c.sharedMesh, Vector3.zero, Quaternion.identity, Vector3.one + invScale * m_blendDistance * 4f);
                }
            }
        }
        #endif
    }
}