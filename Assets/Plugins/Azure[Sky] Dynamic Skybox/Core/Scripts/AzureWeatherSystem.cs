using System;
using Unity.VisualScripting;
using System.Collections.Generic;

namespace UnityEngine.AzureSky
{
    /// <summary>Class that handles the weather features.</summary>
    [Serializable]
    public sealed class AzureWeatherSystem
    {
        /// <summary>The list containing the weather property groups.</summary>
        public List<AzureWeatherPropertyGroup> weatherPropertyGroupList { get => m_weatherPropertyGroupList; set => m_weatherPropertyGroupList = value; }
        [SerializeField] private List<AzureWeatherPropertyGroup> m_weatherPropertyGroupList = new List<AzureWeatherPropertyGroup>();

        /// <summary>The list containing the global weather presets.</summary>
        public List<AzureGlobalWeather> globalWeatherList { get => m_globalWeatherList; set => m_globalWeatherList = value; }
        [SerializeField] private List<AzureGlobalWeather> m_globalWeatherList = new List<AzureGlobalWeather>();

        /// <summary>List of local weather zones arranged according to its priorities.</summary>
        public List<AzureWeatherZone> weatherZoneList { get => m_weatherZoneList; set => m_weatherZoneList = value; }
        [SerializeField] private List<AzureWeatherZone> m_weatherZoneList = new List<AzureWeatherZone>();

        /// <summary>The list containing the settings for instantiation of the thunder prefabs.</summary>
        public List<AzureThunderSettings> thunderSettingsList { get => m_thunderSettingsList; set => m_thunderSettingsList = value; }
        [SerializeField] private List<AzureThunderSettings> m_thunderSettingsList = new List<AzureThunderSettings>();

        /// <summary>The current weather preset.</summary>
        public AzureWeatherPreset currentWeatherPreset { get => m_currentWeatherPreset; set => m_currentWeatherPreset = value; }
        [SerializeField] private AzureWeatherPreset m_currentWeatherPreset = null;

        /// <summary>Stores the target weather preset when runing a global weather transition.</summary>
        public AzureWeatherPreset targetWeatherPreset => m_targetWeatherPreset;
        private AzureWeatherPreset m_targetWeatherPreset = null;

        /// <summary>The trigger used to detect if it is entering a local weather zone.</summary>
        public Transform weatherZoneTrigger { get => m_weatherZoneTrigger; set => m_weatherZoneTrigger = value; }
        [SerializeField] private Transform m_weatherZoneTrigger = null;

        /// <summary>The time used to evaluate the curves and gradients.</summary>
        public float evaluationTime { get => m_evaluationTime; set => m_evaluationTime = value; }
        [SerializeField] private float m_evaluationTime = 6.5f;
        [SerializeField] private float m_evaluationTimeGradient = 0.27f; // 6.5f / 24f

        /// <summary>There is a global weather transition in progress?</summary>
        public bool isWeatherChanging => m_isWeatherChanging;
        private bool m_isWeatherChanging = false;

        /// <summary>Stores the current global weather index in use.</summary>
        public int globalWeatherIndex => m_globalWeatherIndex;
        private int m_globalWeatherIndex = 0;

        /// <summary>Stores the progress of a global weather transition.</summary>
        public float weatherTransitionProgress => m_weatherTransitionProgress;
        [SerializeField] private float m_weatherTransitionProgress = 0.0f;

        /// <summary>Used internally to perform a global weather transition.</summary>
        private float m_weatherTransitionStart = 0.0f;

        /// <summary>Used internally to perform a global weather transition.</summary>
        private float m_weatherTransitionLength = 0.0f;

        /// <summary>Used internally to perform a local weather zone transition.</summary>
        private Vector3 m_weatherZoneTriggerPosition = Vector3.zero;

        /// <summary>Used internally to perform a local weather zone transition.</summary>
        private Collider m_weatherZoneCollider = null;

        /// <summary>Used internally to perform a local weather zone transition.</summary>
        private float m_weatherZoneClosestDistanceSqr = 0.0f;

        /// <summary>Used internally to perform a local weather zone transition.</summary>
        private Vector3 m_weatherZoneClosestPoint = Vector3.zero;

        /// <summary>Used internally to perform a local weather zone transition.</summary>
        private float m_weatherZoneDistance = 0.0f;

        /// <summary>Used internally to perform a local weather zone transition.</summary>
        private float m_weatherZoneBlendDistanceSqr = 0.0f;

        /// <summary>Used internally to perform a local weather zone transition.</summary>
        private float m_weatherZoneInterpolationFactor = 0.0f;

        /// <summary>Used internally to perform the transition ramp curve of each weather group.</summary>
        private float m_weatherGroupRampTime = 0.0f;

        /// <summary>Used internally to perform the direction and position weather transitions.</summary>
        private Vector3 m_vector3Angle;

        /// <summary>Called just before the first Update() of the AzureCoreSystem script.</summary>
        public void Start()
        {
            InitializeCurrentWeatherPreset();
            InitializePropertyTargets();
            UpdateWeatherSystem();
            OverrideTargetProperties();
        }

        /// <summary>Performs a complete update in the weather system. Handled by the Update() of the AzureCoreSystem script.</summary>
        public void UpdateWeatherSystem()
        {
            AzureNotificationCenter.Invoke.OnBeforeWeatherSystemUpdateCallback(this);

            m_evaluationTimeGradient = m_evaluationTime / 24f;

            if (!m_isWeatherChanging)
            {
                EvaluateCurrentWeather();
            }
            else
            {
                // Performs the global weather transition
                m_weatherTransitionProgress = Mathf.Clamp01((Time.time - m_weatherTransitionStart) / m_weatherTransitionLength);
                EvaluateGlobalWeatherTransition(m_currentWeatherPreset, m_targetWeatherPreset, m_weatherTransitionProgress);

                // Ends the global weather transition
                if (Mathf.Abs(m_weatherTransitionProgress - 1.0f) <= 0.0f)
                {
                    m_isWeatherChanging = false;
                    m_weatherTransitionProgress = 0.0f;
                    m_weatherTransitionStart = 0.0f;
                    m_currentWeatherPreset = m_targetWeatherPreset;
                    AzureNotificationCenter.Invoke.OnWeatherTransitionEndCallback(this);
                }
            }

            // Computes weather zones influence
            // Based on Unity's Post Processing v2
            if (!m_weatherZoneTrigger)
            {
                AzureNotificationCenter.Invoke.OnAfterWeatherSystemUpdateCallback(this);
                return;
            }

            m_weatherZoneTriggerPosition = m_weatherZoneTrigger.position;

            // Traverse all weather zones in the weather zone list
            foreach (AzureWeatherZone weatherZone in m_weatherZoneList)
            {
                // Skip if the list index is null
                if (weatherZone == null)
                    continue;

                // If weather zone has no collider, skip it as it's useless
                m_weatherZoneCollider = weatherZone.colliderVolume;
                if (!m_weatherZoneCollider)
                    continue;

                // Skip if the collider or the game object is disabled
                if (!m_weatherZoneCollider.enabled || !m_weatherZoneCollider.gameObject.activeInHierarchy)
                    continue;

                // Find closest distance to weather zone, 0 means it's inside it
                m_weatherZoneClosestDistanceSqr = float.PositiveInfinity;
                m_weatherZoneClosestPoint = m_weatherZoneCollider.ClosestPoint(m_weatherZoneTriggerPosition); // 5.6-only API
                m_weatherZoneDistance = ((m_weatherZoneClosestPoint - m_weatherZoneTriggerPosition) / 2f).sqrMagnitude;

                if (m_weatherZoneDistance < m_weatherZoneClosestDistanceSqr)
                    m_weatherZoneClosestDistanceSqr = m_weatherZoneDistance;

                m_weatherZoneCollider = null;
                m_weatherZoneBlendDistanceSqr = weatherZone.blendDistance * weatherZone.blendDistance;

                // Weather zone has no influence, ignore it
                // Note: Weather zone doesn't do anything when `closestDistanceSqr = blendDistSqr` but
                // we can't use a >= comparison as blendDistSqr could be set to 0 in which
                // case weather zone would have total influence
                if (m_weatherZoneClosestDistanceSqr > m_weatherZoneBlendDistanceSqr)
                    continue;

                // Weather zone has influence
                m_weatherZoneInterpolationFactor = 1f;
                if (m_weatherZoneBlendDistanceSqr > 0f)
                    m_weatherZoneInterpolationFactor = 1f - (m_weatherZoneClosestDistanceSqr / m_weatherZoneBlendDistanceSqr);

                // No need to clamp01 the interpolation factor as it'll always be in [0 <-> 1] range
                EvaluateWeatherZonesInfluence(weatherZone.weatherPreset, m_weatherZoneInterpolationFactor);
            }

            AzureNotificationCenter.Invoke.OnAfterWeatherSystemUpdateCallback(this);
        }

        /// <summary>Gets the output values from the weather properties and uses them to override the targets.</summary>
        public void OverrideTargetProperties()
        {
            AzureNotificationCenter.Invoke.OnBeforeOverrideUpdateCallback(this);

            for (int i = 0; i < m_weatherPropertyGroupList.Count; i++)
            {
                if (m_weatherPropertyGroupList[i].isEnabled == false)
                    continue;

                for (int j = 0; j < m_weatherPropertyGroupList[i].weatherPropertyList.Count; j++)
                {
                    if (m_weatherPropertyGroupList[i].weatherPropertyList[j].overrideMode == AzureWeatherPropertyOverrideMode.Off)
                        continue;

                    switch (m_weatherPropertyGroupList[i].weatherPropertyList[j].propertyType)
                    {
                        case AzureWeatherPropertyType.Float:
                        case AzureWeatherPropertyType.Curve:
                            switch (m_weatherPropertyGroupList[i].weatherPropertyList[j].targetType)
                            {
                                case AzureWeatherPropertyTargetType.Property:
                                    m_weatherPropertyGroupList[i].weatherPropertyList[j].propertyInfo?.SetValueOptimized(m_weatherPropertyGroupList[i].weatherPropertyList[j].targetComponent, m_weatherPropertyGroupList[i].weatherPropertyList[j].floatOutput);
                                    break;
                                case AzureWeatherPropertyTargetType.Field:
                                    m_weatherPropertyGroupList[i].weatherPropertyList[j].fieldInfo?.SetValueOptimized(m_weatherPropertyGroupList[i].weatherPropertyList[j].targetComponent, m_weatherPropertyGroupList[i].weatherPropertyList[j].floatOutput);
                                    break;
                                case AzureWeatherPropertyTargetType.MaterialProperty:
                                    m_weatherPropertyGroupList[i].weatherPropertyList[j].targetMaterial?.SetFloat(m_weatherPropertyGroupList[i].weatherPropertyList[j].targetPropertyName, m_weatherPropertyGroupList[i].weatherPropertyList[j].floatOutput);
                                    break;
                                case AzureWeatherPropertyTargetType.GlobalShaderUniform:
                                    Shader.SetGlobalFloat(m_weatherPropertyGroupList[i].weatherPropertyList[j].targetPropertyName, m_weatherPropertyGroupList[i].weatherPropertyList[j].floatOutput);
                                    break;
                                case AzureWeatherPropertyTargetType.GlobalProperty:
                                    m_weatherPropertyGroupList[i].weatherPropertyList[j].propertyInfo?.SetValue(m_weatherPropertyGroupList[i].weatherPropertyList[j].targetGlobalType, m_weatherPropertyGroupList[i].weatherPropertyList[j].floatOutput);
                                    break;
                                case AzureWeatherPropertyTargetType.GlobalField:
                                    m_weatherPropertyGroupList[i].weatherPropertyList[j].fieldInfo?.SetValue(m_weatherPropertyGroupList[i].weatherPropertyList[j].targetGlobalType, m_weatherPropertyGroupList[i].weatherPropertyList[j].floatOutput);
                                    break;
                            }
                            break;

                        case AzureWeatherPropertyType.Color:
                        case AzureWeatherPropertyType.Gradient:
                            switch (m_weatherPropertyGroupList[i].weatherPropertyList[j].targetType)
                            {
                                case AzureWeatherPropertyTargetType.Property:
                                    m_weatherPropertyGroupList[i].weatherPropertyList[j].propertyInfo?.SetValueOptimized(m_weatherPropertyGroupList[i].weatherPropertyList[j].targetComponent, m_weatherPropertyGroupList[i].weatherPropertyList[j].colorOutput);
                                    break;
                                case AzureWeatherPropertyTargetType.Field:
                                    m_weatherPropertyGroupList[i].weatherPropertyList[j].fieldInfo?.SetValueOptimized(m_weatherPropertyGroupList[i].weatherPropertyList[j].targetComponent, m_weatherPropertyGroupList[i].weatherPropertyList[j].colorOutput);
                                    break;
                                case AzureWeatherPropertyTargetType.MaterialProperty:
                                    m_weatherPropertyGroupList[i].weatherPropertyList[j].targetMaterial?.SetColor(m_weatherPropertyGroupList[i].weatherPropertyList[j].targetPropertyName, m_weatherPropertyGroupList[i].weatherPropertyList[j].colorOutput);
                                    break;
                                case AzureWeatherPropertyTargetType.GlobalShaderUniform:
                                    Shader.SetGlobalColor(m_weatherPropertyGroupList[i].weatherPropertyList[j].targetPropertyName, m_weatherPropertyGroupList[i].weatherPropertyList[j].colorOutput);
                                    break;
                                case AzureWeatherPropertyTargetType.GlobalProperty:
                                    m_weatherPropertyGroupList[i].weatherPropertyList[j].propertyInfo?.SetValue(m_weatherPropertyGroupList[i].weatherPropertyList[j].targetGlobalType, m_weatherPropertyGroupList[i].weatherPropertyList[j].colorOutput);
                                    break;
                                case AzureWeatherPropertyTargetType.GlobalField:
                                    m_weatherPropertyGroupList[i].weatherPropertyList[j].fieldInfo?.SetValue(m_weatherPropertyGroupList[i].weatherPropertyList[j].targetGlobalType, m_weatherPropertyGroupList[i].weatherPropertyList[j].colorOutput);
                                    break;
                            }
                            break;

                        case AzureWeatherPropertyType.Direction:
                        case AzureWeatherPropertyType.Position:
                            switch (m_weatherPropertyGroupList[i].weatherPropertyList[j].targetType)
                            {
                                case AzureWeatherPropertyTargetType.Property:
                                    m_weatherPropertyGroupList[i].weatherPropertyList[j].propertyInfo?.SetValueOptimized(m_weatherPropertyGroupList[i].weatherPropertyList[j].targetComponent, m_weatherPropertyGroupList[i].weatherPropertyList[j].vector3Output);
                                    break;
                                case AzureWeatherPropertyTargetType.Field:
                                    m_weatherPropertyGroupList[i].weatherPropertyList[j].fieldInfo?.SetValueOptimized(m_weatherPropertyGroupList[i].weatherPropertyList[j].targetComponent, m_weatherPropertyGroupList[i].weatherPropertyList[j].vector3Output);
                                    break;
                                case AzureWeatherPropertyTargetType.MaterialProperty:
                                    m_weatherPropertyGroupList[i].weatherPropertyList[j].targetMaterial?.SetVector(m_weatherPropertyGroupList[i].weatherPropertyList[j].targetPropertyName, m_weatherPropertyGroupList[i].weatherPropertyList[j].vector3Output);
                                    break;
                                case AzureWeatherPropertyTargetType.GlobalShaderUniform:
                                    Shader.SetGlobalVector(m_weatherPropertyGroupList[i].weatherPropertyList[j].targetPropertyName, m_weatherPropertyGroupList[i].weatherPropertyList[j].vector3Output);
                                    break;
                                case AzureWeatherPropertyTargetType.GlobalProperty:
                                    m_weatherPropertyGroupList[i].weatherPropertyList[j].propertyInfo?.SetValue(m_weatherPropertyGroupList[i].weatherPropertyList[j].targetGlobalType, m_weatherPropertyGroupList[i].weatherPropertyList[j].vector3Output);
                                    break;
                                case AzureWeatherPropertyTargetType.GlobalField:
                                    m_weatherPropertyGroupList[i].weatherPropertyList[j].fieldInfo?.SetValue(m_weatherPropertyGroupList[i].weatherPropertyList[j].targetGlobalType, m_weatherPropertyGroupList[i].weatherPropertyList[j].vector3Output);
                                    break;
                            }
                            break;
                    }
                }
            }

            AzureNotificationCenter.Invoke.OnAfterOverrideUpdateCallback(this);
        }

        #if UNITY_EDITOR
        public void OnEditorUpdate()
        {
            m_evaluationTimeGradient = m_evaluationTime / 24f;
            InitializeCurrentWeatherPreset();
            EvaluateCurrentWeather();
        }
        #endif

        /// <summary>Instantiates a thunder prefab in the scene using as position the especification from the thunder settings list. When the thunder sound is over, the instance is automatically deleted.</summary>
        /// Note that in this case, you only need to pass the element index as parameter, the position will the one you set in the Inspector of the thunder settings list.
		public void InstantiateThunderPrefab(int index)
        {
            if (m_thunderSettingsList.Count <= 0) return;
            if (m_thunderSettingsList[index].transform == null) return;
            Object.Instantiate(m_thunderSettingsList[index].transform, m_thunderSettingsList[index].position, Quaternion.identity);
        }

        /// <summary>Instantiates a thunder prefab in the scene using a Vector3 parameter as position. When the thunder sound is over, the instance is automatically deleted.</summary>
        ///  Note that in this case, you also need to pass the instantiation position as parameter, the position in the Inspector of the thunder settings list will be ignored.
		public void InstantiateThunderPrefab(int index, Vector3 position)
        {
            if (m_thunderSettingsList.Count <= 0) return;
            if (m_thunderSettingsList[index].transform == null) return;
            Object.Instantiate(m_thunderSettingsList[index].transform, position, Quaternion.identity);
        }

        /// <summary>Starts a global weather transition using the preset from the global weather list. It changes the weather index to the selected one.</summary>
        public void SetGlobalWeather(int index)
        {
            index = Mathf.Clamp(index, 0, m_globalWeatherList.Count);
            if (m_globalWeatherList[index].weatherPreset == null) return;
            if (m_globalWeatherIndex == index) return;
            if (m_isWeatherChanging) return;

            m_targetWeatherPreset = m_globalWeatherList[index].weatherPreset;
            m_weatherTransitionLength = m_globalWeatherList[index].transitionTime;
            m_weatherTransitionProgress = 0.0f;
            m_weatherTransitionStart = Time.time;
            m_globalWeatherIndex = index;
            m_isWeatherChanging = true;
        }

        /// <summary>Starts a global weather transition using a custom preset and transition time, without changing the weather index.</summary>
        public void SetGlobalWeather(AzureWeatherPreset preset, float transitionTime)
        {
            m_targetWeatherPreset = preset;
            m_weatherTransitionLength = transitionTime;
            m_weatherTransitionProgress = 0.0f;
            m_weatherTransitionStart = Time.time;
            m_isWeatherChanging = true;
        }

        /// <summary>Instantly changes the global weather without a transition, and without changing the weather index.</summary>
        public void SetGlobalWeather(AzureWeatherPreset preset)
        {
            m_currentWeatherPreset = preset;
        }

        /// <summary>Enable all the weather groups.</summary>
        public void WeatherGroupsEnableAll()
        {
            for (int i = 0; i < m_weatherPropertyGroupList.Count; i++)
            {
                m_weatherPropertyGroupList[i].isEnabled = true;
            }
        }

        /// <summary>Disable all the weather groups.</summary>
        public void WeatherGroupsDisableAll()
        {
            for (int i = 0; i < m_weatherPropertyGroupList.Count; i++)
            {
                m_weatherPropertyGroupList[i].isEnabled = false;
            }
        }

        /// <summary>Enable a weather group at a given index.</summary>
        public void WeatherGroupsEnableAt(int index)
        {
            m_weatherPropertyGroupList[index].isEnabled = true;
        }

        /// <summary>Disable a weather group at a given index.</summary>
        public void WeatherGroupsDisableAt(int index)
        {
            m_weatherPropertyGroupList[index].isEnabled = false;
        }

        /// <summary>Set the active state of a weather group giving its index and state.</summary>
        public void WeatherGroupsSetStateAt(int index, bool state)
        {
            m_weatherPropertyGroupList[index].isEnabled = state;
        }

        /// <summary>Returns the float output of a custom weather property.</summary>
        public float GetFloatOutput(int groupIndex, int propertyIndex)
        {
            return m_weatherPropertyGroupList[groupIndex].weatherPropertyList[propertyIndex].floatOutput;
        }

        /// <summary>Returns the color output of a custom weather property.</summary>
        public Color GetColorOutput(int groupIndex, int propertyIndex)
        {
            return m_weatherPropertyGroupList[groupIndex].weatherPropertyList[propertyIndex].colorOutput;
        }

        /// <summary>Returns the Vector3 output of a custom weather property.</summary>
        public Vector3 GetVector3Output(int groupIndex, int propertyIndex)
        {
            return m_weatherPropertyGroupList[groupIndex].weatherPropertyList[propertyIndex].vector3Output;
        }

        /// <summary>PropertInfo and FieldInfo are not serializable, so the targets must always be reassigned when the scene starts.</summary>
        public void InitializePropertyTargets()
        {
            for (int i = 0; i < m_weatherPropertyGroupList.Count; i++)
            {
                for (int j = 0; j < m_weatherPropertyGroupList[i].weatherPropertyList.Count; j++)
                {
                    m_weatherPropertyGroupList[i].weatherPropertyList[j].propertyInfo = null;
                    m_weatherPropertyGroupList[i].weatherPropertyList[j].fieldInfo = null;

                    switch (m_weatherPropertyGroupList[i].weatherPropertyList[j].targetType)
                    {
                        case AzureWeatherPropertyTargetType.Property:
                            if (m_weatherPropertyGroupList[i].weatherPropertyList[j].targetObject == null) continue;
                            GameObject obj = m_weatherPropertyGroupList[i].weatherPropertyList[j].targetObject;
                            string componentName = m_weatherPropertyGroupList[i].weatherPropertyList[j].targetComponentName;
                            string propertyName = m_weatherPropertyGroupList[i].weatherPropertyList[j].targetPropertyName;
                            m_weatherPropertyGroupList[i].weatherPropertyList[j].targetComponent = obj.GetComponent(componentName);
                            m_weatherPropertyGroupList[i].weatherPropertyList[j].propertyInfo = m_weatherPropertyGroupList[i].weatherPropertyList[j].targetComponent?.GetType().GetProperty(propertyName);
                            break;

                        case AzureWeatherPropertyTargetType.Field:
                            if (m_weatherPropertyGroupList[i].weatherPropertyList[j].targetObject == null) continue;
                            obj = m_weatherPropertyGroupList[i].weatherPropertyList[j].targetObject;
                            componentName = m_weatherPropertyGroupList[i].weatherPropertyList[j].targetComponentName;
                            propertyName = m_weatherPropertyGroupList[i].weatherPropertyList[j].targetPropertyName;
                            m_weatherPropertyGroupList[i].weatherPropertyList[j].targetComponent = obj.GetComponent(componentName);
                            m_weatherPropertyGroupList[i].weatherPropertyList[j].fieldInfo = m_weatherPropertyGroupList[i].weatherPropertyList[j].targetComponent?.GetType().GetField(propertyName);
                            break;

                        case AzureWeatherPropertyTargetType.GlobalProperty:
                            componentName = m_weatherPropertyGroupList[i].weatherPropertyList[j].targetComponentName;
                            propertyName = m_weatherPropertyGroupList[i].weatherPropertyList[j].targetPropertyName;
                            m_weatherPropertyGroupList[i].weatherPropertyList[j].targetGlobalType = Type.GetType(componentName);
                            m_weatherPropertyGroupList[i].weatherPropertyList[j].propertyInfo = m_weatherPropertyGroupList[i].weatherPropertyList[j].targetGlobalType?.GetProperty(propertyName);
                            break;

                        case AzureWeatherPropertyTargetType.GlobalField:
                            componentName = m_weatherPropertyGroupList[i].weatherPropertyList[j].targetComponentName;
                            propertyName = m_weatherPropertyGroupList[i].weatherPropertyList[j].targetPropertyName;
                            m_weatherPropertyGroupList[i].weatherPropertyList[j].targetGlobalType = Type.GetType(componentName);
                            m_weatherPropertyGroupList[i].weatherPropertyList[j].fieldInfo = m_weatherPropertyGroupList[i].weatherPropertyList[j].targetGlobalType?.GetField(propertyName);
                            break;
                    }
                }
            }
        }

        /// <summary>Initialize the current weather preset using the first global weather as reference.</summary>
        private void InitializeCurrentWeatherPreset()
        {
            if (m_globalWeatherList.Count <= 0) return;
            if (m_globalWeatherList[0] == null) return;
            m_currentWeatherPreset = m_globalWeatherList[0].weatherPreset;
        }

        /// <summary>Evaluate the weather system according to the current weather preset selected.</summary>
        private void EvaluateCurrentWeather()
        {
            if (m_currentWeatherPreset == null) return;

            for (int i = 0; i < m_weatherPropertyGroupList.Count; i++)
            {
                if (m_weatherPropertyGroupList[i].isEnabled == false)
                    continue;

                for (int j = 0; j < m_weatherPropertyGroupList[i].weatherPropertyList.Count; j++)
                {
                    switch (m_weatherPropertyGroupList[i].weatherPropertyList[j].propertyType)
                    {
                        case AzureWeatherPropertyType.Float:
                            m_weatherPropertyGroupList[i].weatherPropertyList[j].floatOutput = m_currentWeatherPreset.propertyGroupDataList[i].propertyDataList[j].floatData;
                            break;

                        case AzureWeatherPropertyType.Color:
                            m_weatherPropertyGroupList[i].weatherPropertyList[j].colorOutput = m_currentWeatherPreset.propertyGroupDataList[i].propertyDataList[j].colorData;
                            break;

                        case AzureWeatherPropertyType.Curve:
                            m_weatherPropertyGroupList[i].weatherPropertyList[j].floatOutput = m_currentWeatherPreset.propertyGroupDataList[i].propertyDataList[j].curveData.Evaluate(m_evaluationTime);
                            break;

                        case AzureWeatherPropertyType.Gradient:
                            m_weatherPropertyGroupList[i].weatherPropertyList[j].colorOutput = m_currentWeatherPreset.propertyGroupDataList[i].propertyDataList[j].gradientData.Evaluate(m_evaluationTimeGradient);
                            break;

                        case AzureWeatherPropertyType.Direction:
                        case AzureWeatherPropertyType.Position:
                            m_weatherPropertyGroupList[i].weatherPropertyList[j].vector3Output = m_currentWeatherPreset.propertyGroupDataList[i].propertyDataList[j].vector3Data;
                            break;
                    }
                }
            }
        }

        /// <summary>Performs a global weather transition.</summary>
        private void EvaluateGlobalWeatherTransition(AzureWeatherPreset from, AzureWeatherPreset to, float t)
        {
            if (from == null || to == null) return;

            for (int i = 0; i < m_weatherPropertyGroupList.Count; i++)
            {
                if (m_weatherPropertyGroupList[i].isEnabled == false)
                    continue;

                m_weatherGroupRampTime = to.propertyGroupDataList[i].rampCurve.Evaluate(t);

                for (int j = 0; j < m_weatherPropertyGroupList[i].weatherPropertyList.Count; j++)
                {
                    switch (m_weatherPropertyGroupList[i].weatherPropertyList[j].propertyType)
                    {
                        case AzureWeatherPropertyType.Float:
                            m_weatherPropertyGroupList[i].weatherPropertyList[j].floatOutput = FloatInterpolation(from.propertyGroupDataList[i].propertyDataList[j].floatData, to.propertyGroupDataList[i].propertyDataList[j].floatData, m_weatherGroupRampTime);
                            break;

                        case AzureWeatherPropertyType.Color:
                            m_weatherPropertyGroupList[i].weatherPropertyList[j].colorOutput = ColorInterpolation(from.propertyGroupDataList[i].propertyDataList[j].colorData, to.propertyGroupDataList[i].propertyDataList[j].colorData, m_weatherGroupRampTime);
                            break;

                        case AzureWeatherPropertyType.Curve:
                            m_weatherPropertyGroupList[i].weatherPropertyList[j].floatOutput = FloatInterpolation(from.propertyGroupDataList[i].propertyDataList[j].curveData.Evaluate(m_evaluationTime), to.propertyGroupDataList[i].propertyDataList[j].curveData.Evaluate(m_evaluationTime), m_weatherGroupRampTime);
                            break;

                        case AzureWeatherPropertyType.Gradient:
                            m_weatherPropertyGroupList[i].weatherPropertyList[j].colorOutput = ColorInterpolation(from.propertyGroupDataList[i].propertyDataList[j].gradientData.Evaluate(m_evaluationTimeGradient), to.propertyGroupDataList[i].propertyDataList[j].gradientData.Evaluate(m_evaluationTimeGradient), m_weatherGroupRampTime);
                            break;

                        case AzureWeatherPropertyType.Direction:
                            m_vector3Angle.x = Mathf.LerpAngle(from.propertyGroupDataList[i].propertyDataList[j].vector3Data.x, to.propertyGroupDataList[i].propertyDataList[j].vector3Data.x, m_weatherGroupRampTime);
                            m_vector3Angle.y = Mathf.LerpAngle(from.propertyGroupDataList[i].propertyDataList[j].vector3Data.y, to.propertyGroupDataList[i].propertyDataList[j].vector3Data.y, m_weatherGroupRampTime);
                            m_vector3Angle.z = Mathf.LerpAngle(from.propertyGroupDataList[i].propertyDataList[j].vector3Data.z, to.propertyGroupDataList[i].propertyDataList[j].vector3Data.z, m_weatherGroupRampTime);
                            m_weatherPropertyGroupList[i].weatherPropertyList[j].vector3Output = m_vector3Angle;
                            break;

                        case AzureWeatherPropertyType.Position:
                            m_weatherPropertyGroupList[i].weatherPropertyList[j].vector3Output = Vector3.Lerp(from.propertyGroupDataList[i].propertyDataList[j].vector3Data, to.propertyGroupDataList[i].propertyDataList[j].vector3Data, m_weatherGroupRampTime);
                            break;
                    }
                }
            }
        }

        /// <summary>Evaluate the weather while inside any weather zone influence.</summary>
        private void EvaluateWeatherZonesInfluence(AzureWeatherPreset target, float blendFactor)
        {
            if (target == null) return;

            for (int i = 0; i < m_weatherPropertyGroupList.Count; i++)
            {
                if (m_weatherPropertyGroupList[i].isEnabled == false)
                    continue;

                m_weatherGroupRampTime = target.propertyGroupDataList[i].rampCurve.Evaluate(blendFactor);

                for (int j = 0; j < m_weatherPropertyGroupList[i].weatherPropertyList.Count; j++)
                {
                    switch (m_weatherPropertyGroupList[i].weatherPropertyList[j].propertyType)
                    {
                        case AzureWeatherPropertyType.Float:
                            m_weatherPropertyGroupList[i].weatherPropertyList[j].floatOutput = FloatInterpolation(m_weatherPropertyGroupList[i].weatherPropertyList[j].floatOutput, target.propertyGroupDataList[i].propertyDataList[j].floatData, m_weatherGroupRampTime);
                            break;

                        case AzureWeatherPropertyType.Color:
                            m_weatherPropertyGroupList[i].weatherPropertyList[j].colorOutput = ColorInterpolation(m_weatherPropertyGroupList[i].weatherPropertyList[j].colorOutput, target.propertyGroupDataList[i].propertyDataList[j].colorData, m_weatherGroupRampTime);
                            break;

                        case AzureWeatherPropertyType.Curve:
                            m_weatherPropertyGroupList[i].weatherPropertyList[j].floatOutput = FloatInterpolation(m_weatherPropertyGroupList[i].weatherPropertyList[j].floatOutput, target.propertyGroupDataList[i].propertyDataList[j].curveData.Evaluate(m_evaluationTime), m_weatherGroupRampTime);
                            break;

                        case AzureWeatherPropertyType.Gradient:
                            m_weatherPropertyGroupList[i].weatherPropertyList[j].colorOutput = ColorInterpolation(m_weatherPropertyGroupList[i].weatherPropertyList[j].colorOutput, target.propertyGroupDataList[i].propertyDataList[j].gradientData.Evaluate(m_evaluationTimeGradient), m_weatherGroupRampTime);
                            break;

                        case AzureWeatherPropertyType.Direction:
                            m_vector3Angle.x = Mathf.LerpAngle(m_weatherPropertyGroupList[i].weatherPropertyList[j].vector3Output.x, target.propertyGroupDataList[i].propertyDataList[j].vector3Data.x, m_weatherGroupRampTime);
                            m_vector3Angle.y = Mathf.LerpAngle(m_weatherPropertyGroupList[i].weatherPropertyList[j].vector3Output.y, target.propertyGroupDataList[i].propertyDataList[j].vector3Data.y, m_weatherGroupRampTime);
                            m_vector3Angle.z = Mathf.LerpAngle(m_weatherPropertyGroupList[i].weatherPropertyList[j].vector3Output.z, target.propertyGroupDataList[i].propertyDataList[j].vector3Data.z, m_weatherGroupRampTime);
                            m_weatherPropertyGroupList[i].weatherPropertyList[j].vector3Output = m_vector3Angle;
                            break;

                        case AzureWeatherPropertyType.Position:
                            m_weatherPropertyGroupList[i].weatherPropertyList[j].vector3Output = Vector3.Lerp(m_weatherPropertyGroupList[i].weatherPropertyList[j].vector3Output, target.propertyGroupDataList[i].propertyDataList[j].vector3Data, m_weatherGroupRampTime);
                            break;
                    }
                }
            }
        }

        /// <summary>Interpolates between two values given an interpolation factor.</summary>
        private float FloatInterpolation(float from, float to, float t)
        {
            return from + (to - from) * t;
        }

        /// <summary>Interpolates between two colors given an interpolation factor.</summary>
        private Color ColorInterpolation(Color from, Color to, float t)
        {
            Color ret;
            ret.r = from.r + (to.r - from.r) * t;
            ret.g = from.g + (to.g - from.g) * t;
            ret.b = from.b + (to.b - from.b) * t;
            ret.a = from.a + (to.a - from.a) * t;
            return ret;
        }
    }
}