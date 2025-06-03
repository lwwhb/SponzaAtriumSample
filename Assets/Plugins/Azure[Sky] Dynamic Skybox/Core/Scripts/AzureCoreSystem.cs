using UnityEngine.Events;
using UnityEngine.Rendering;
using System.Collections.Generic;

namespace UnityEngine.AzureSky
{
    /// <summary>This class is the main component that handles the entire system.</summary>
    [ExecuteInEditMode]
    [AddComponentMenu("Azure[Sky] Dynamic Skybox/Azure Core System")]
    public sealed class AzureCoreSystem : MonoBehaviour
    {
        /// <summary>Instance of the time system class used to handle the time stuff in this component.</summary>
        public AzureTimeSystem timeSystem { get => m_timeSystem; set => m_timeSystem = value; }
        [SerializeField] private AzureTimeSystem m_timeSystem = new AzureTimeSystem();

        /// <summary>Instance of the weather system class used to handle the weather stuff in this component.</summary>
        public AzureWeatherSystem weatherSystem { get => m_weatherSystem; set => m_weatherSystem = value; }
        [SerializeField] private AzureWeatherSystem m_weatherSystem = new AzureWeatherSystem();

        /// <summary>Instance of the event system class used to handle the time and date events.</summary>
        public AzureEventSystem eventSystem { get => m_eventSystem; set => m_eventSystem = value; }
        [SerializeField] private AzureEventSystem m_eventSystem = new AzureEventSystem();

        /// <summary>The way the Core System should be updated.</summary>
        public AzureCoreUpdateMode coreUpdateMode { get => m_coreUpdateMode; set => m_coreUpdateMode = value; }
        [SerializeField] private AzureCoreUpdateMode m_coreUpdateMode = AzureCoreUpdateMode.EveryFrame;

        /// <summary>The time passed since the last core system update.</summary>
        public float timeSinceLastUpdate => m_timeSinceLastUpdate;
        [SerializeField] private float m_timeSinceLastUpdate = 0.0f;

        /// <summary>The time interval used to update the core system.</summary>
        public float refreshRate { get => m_refreshRate; set => m_refreshRate = value; }
        [SerializeField] private float m_refreshRate = 0.1f;

        /// <summary>The event that is invoked every time the core system is updated.</summary>
        public UnityEvent onCoreUpdateEvent { get => m_onCoreUpdateEvent; set => m_onCoreUpdateEvent = value; }
        [SerializeField] private UnityEvent m_onCoreUpdateEvent = new UnityEvent();

        /// <summary>The reference to the reflection probe handled by this core system component.</summary>
        public ReflectionProbe reflectionProbe { get => m_reflectionProbe; set => m_reflectionProbe = value; }
        [SerializeField] private ReflectionProbe m_reflectionProbe = null;

        /// <summary>List of the follow targets.</summary>
        public List<AzureFollowTarget> followTargetList { get => m_followTargetList; set => m_followTargetList = value; }
        [SerializeField] private List<AzureFollowTarget> m_followTargetList = new List<AzureFollowTarget>();

        /// <summary>Called when the component is reseted in the Inspector.</summary>
        private void Reset()
        {
            m_timeSystem.UpdateCalendar();
        }

        //private void Awake()
        //{
        //    //Debug.Log(typeof(AzureCelestialBody.CelestialBodyType).AssemblyQualifiedName);
        //}

        private void Start()
        {
            m_timeSystem.Start();
            m_weatherSystem.evaluationTime = m_timeSystem.evaluationTime;
            m_weatherSystem.Start();
            UpdateFollowTargets();
            m_onCoreUpdateEvent?.Invoke();
        }

        /// <summary>Register the EventSystem's events when this GameObject is enabled.</summary>
        private void OnEnable()
        {
            m_eventSystem.RegisterEvents();
        }

        /// <summary>Unregister the EventSystem's events when this GameObject is disabled.</summary>
        private void OnDisable()
        {
            m_eventSystem.UnregisterEvents();
        }

        /// <summary>Update is called every frame, if the MonoBehaviour is enabled.</summary>
        private void Update()
        {
            // Only in gameplay
            if (Application.isPlaying)
            {
                m_timeSystem.UpdateTimeOfDay();
                m_weatherSystem.evaluationTime = m_timeSystem.evaluationTime;

                if (m_coreUpdateMode == AzureCoreUpdateMode.EveryFrame)
                {
                    m_timeSystem.UpdateCelestialBodies();
                    m_weatherSystem.UpdateWeatherSystem();
                    m_weatherSystem.OverrideTargetProperties();
                    m_onCoreUpdateEvent?.Invoke();
                }
                else
                {
                    m_timeSinceLastUpdate += Time.deltaTime;

                    if (m_timeSinceLastUpdate >= m_refreshRate)
                    {
                        m_timeSystem.UpdateCelestialBodies();
                        m_weatherSystem.UpdateWeatherSystem();
                        m_weatherSystem.OverrideTargetProperties();
                        m_onCoreUpdateEvent?.Invoke();
                        m_timeSinceLastUpdate = 0.0f;
                    }
                }

                UpdateFollowTargets();
            }

            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                m_timeSystem.OnEditorUpdate();
                m_weatherSystem.evaluationTime = m_timeSystem.evaluationTime;
                m_weatherSystem.OnEditorUpdate();
                m_weatherSystem.OverrideTargetProperties();
            }
            #endif
        }

        /// <summary>Set the position of the follower transforms to be the same as the target transforms.</summary>
        private void UpdateFollowTargets()
        {
            if (m_followTargetList.Count <= 0) return;

            for (int i = 0; i < m_followTargetList.Count; i++)
            {
                if (!m_followTargetList[i].follower) continue;
                if (!m_followTargetList[i].target) continue;

                if (m_followTargetList[i].follower.position != m_followTargetList[i].target.position)
                {
                    m_followTargetList[i].follower.position = m_followTargetList[i].target.position;
                }
            }
        }

        /// <summary>
        /// Update the reflection probe attached in the options tab.
        /// Note: It can be very slow if called every frame!!!
        /// </summary>
        public void UpdateReflectionProbe()
        {
            if (m_reflectionProbe)
            {
                if (m_reflectionProbe.refreshMode == ReflectionProbeRefreshMode.ViaScripting)
                {
                    m_reflectionProbe.RenderProbe();
                }
            }
        }

        /// <summary>
        /// Update the environment cubemap texture from DynamicGI.
        /// Note: It can be very slow if called every frame!!!
        /// </summary>
        public void UpdateDynamicGI()
        {
            DynamicGI.UpdateEnvironment();
        }
    }
}