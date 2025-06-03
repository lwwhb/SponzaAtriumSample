using System;
using System.Reflection;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AzureSky;

namespace UnityEditor.AzureSky
{
    [CustomEditor(typeof(AzureCoreSystem))]
    public sealed class AzureCoreSystemEditor : Editor
    {
        // Logo
        public Texture2D iconTexture;

        private AzureCoreSystem m_target;
        private Rect m_controlRect, m_minLengthRect, m_maxLengthRect;
        private Color32 m_blueColor = new Color32(38, 86, 189, 255);
        private Color32 m_yellowColor = new Color32(232, 153, 12, 255);

        // Header toolbox
        private static int m_headerToolbarIndex = 0;
        private GUIContent[] m_headerToolbarContent = new[]
        {
            new GUIContent("Time System", ""),
            new GUIContent("Weather System", ""),
            new GUIContent("Weather Properties", ""),
            new GUIContent("Event System", ""),
            new GUIContent("Options", "")
        };

        // Calendar styles
        private string[] m_calendarHeaderStringArray = new[] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
        private GUIStyle m_calendarHeaderStringStyle;
        private GUIStyle m_dayNightLengthsBackgroundStyle;

        // Utilities
        private int[] m_hourMinute;
        private int[] m_dayMonthYear;
        GUIContent[] m_hourMinuteContents = new GUIContent[] { new GUIContent("Hour"), new GUIContent("Minute") };
        GUIContent[] m_dayMonthYearContents = new GUIContent[] { new GUIContent("Day"), new GUIContent("Month"), new GUIContent("Year") };

        // Serialized properties
        private SerializedProperty m_selectedCalendarDay;
        private SerializedProperty m_day, m_month, m_year;
        private SerializedProperty m_sunTransform, m_moonTransform, m_starfieldTransform, m_directionalLight;
        private SerializedProperty m_timeMode, m_timeDirection, m_timeLoop;
        private SerializedProperty m_timeline, m_latitude, m_longitude, m_utc;
        private SerializedProperty m_startTime, m_dayLength, m_minLightAltitude, m_dawnTime, m_duskTime;
        private SerializedProperty m_celestialBodiesList, m_weatherPropertyGroupList, m_weatherPropertyList, m_globalWeatherList, m_weatherZoneList, m_thunderSettingsList;
        private SerializedProperty m_weatherZoneTrigger;
        private SerializedProperty m_weatherTransitionProgress;

        private SerializedProperty m_coreUpdateMode;
        private SerializedProperty m_refreshRate;
        private SerializedProperty m_reflectionProbe;
        private SerializedProperty m_onCoreUpdateEvent;
        private SerializedProperty m_followTargetList;

        private SerializedProperty m_onMinuteChangeEvent;
        private SerializedProperty m_onHourChangeEvent;
        private SerializedProperty m_onDayChangeEvent;
        private SerializedProperty m_onMonthChangeEvent;
        private SerializedProperty m_onYearChangeEvent;
        private SerializedProperty m_customEventScanMode;
        private SerializedProperty m_customEventList;

        // Reorderable lists
        private ReorderableList m_reorderableCelestialBodiesList;
        private ReorderableList m_reorderableWeatherPropertyGroupList;
        private ReorderableList m_reorderableWeatherPropertyList;
        private ReorderableList m_reorderableFollowTargetList;
        private ReorderableList m_reorderableGlobalWeatherList;
        private ReorderableList m_reorderableWeatherZoneList;
        private ReorderableList m_reorderableThunderSettingsList;
        private ReorderableList m_customEventReorderableList;
        private static int m_propertyGroupListIndex = 0;

        private void OnEnable()
        {
            // Get target
            m_target = target as AzureCoreSystem;
            //m_target.timeSystem.Start();
            //m_target.weatherSystem.Start();
            m_headerToolbarIndex = PlayerPrefs.GetInt("AzureHeaderToolbarIndex", 0);
            serializedObject.Update();

            // Get serialized properties
            m_selectedCalendarDay = serializedObject.FindProperty("m_timeSystem.m_selectedCalendarDay");
            m_day = serializedObject.FindProperty("m_timeSystem.m_day");
            m_month = serializedObject.FindProperty("m_timeSystem.m_month");
            m_year = serializedObject.FindProperty("m_timeSystem.m_year");
            m_sunTransform = serializedObject.FindProperty("m_timeSystem.m_sunTransform");
            m_moonTransform = serializedObject.FindProperty("m_timeSystem.m_moonTransform");
            m_starfieldTransform = serializedObject.FindProperty("m_timeSystem.m_starfieldTransform");
            m_directionalLight = serializedObject.FindProperty("m_timeSystem.m_directionalLight");
            m_timeMode = serializedObject.FindProperty("m_timeSystem.m_timeMode");
            m_timeDirection = serializedObject.FindProperty("m_timeSystem.m_timeDirection");
            m_timeLoop = serializedObject.FindProperty("m_timeSystem.m_timeLoop");
            m_timeline = serializedObject.FindProperty("m_timeSystem.m_timeline");
            m_latitude = serializedObject.FindProperty("m_timeSystem.m_latitude");
            m_longitude = serializedObject.FindProperty("m_timeSystem.m_longitude");
            m_utc = serializedObject.FindProperty("m_timeSystem.m_utc");
            m_startTime = serializedObject.FindProperty("m_timeSystem.m_startTime");
            m_dayLength = serializedObject.FindProperty("m_timeSystem.m_dayLength");
            m_minLightAltitude = serializedObject.FindProperty("m_timeSystem.m_minLightAltitude");
            m_dawnTime = serializedObject.FindProperty("m_timeSystem.m_dawnTime");
            m_duskTime = serializedObject.FindProperty("m_timeSystem.m_duskTime");
            m_celestialBodiesList = serializedObject.FindProperty("m_timeSystem.m_celestialBodiesList");
            m_weatherPropertyGroupList = serializedObject.FindProperty("m_weatherSystem.m_weatherPropertyGroupList");

            m_coreUpdateMode = serializedObject.FindProperty("m_coreUpdateMode");
            m_refreshRate = serializedObject.FindProperty("m_refreshRate");
            m_reflectionProbe = serializedObject.FindProperty("m_reflectionProbe");
            m_onCoreUpdateEvent = serializedObject.FindProperty("m_onCoreUpdateEvent");
            m_followTargetList = serializedObject.FindProperty("m_followTargetList");

            m_globalWeatherList = serializedObject.FindProperty("m_weatherSystem.m_globalWeatherList");
            m_weatherTransitionProgress = serializedObject.FindProperty("m_weatherSystem.m_weatherTransitionProgress");
            m_weatherZoneTrigger = serializedObject.FindProperty("m_weatherSystem.m_weatherZoneTrigger");
            m_weatherZoneList = serializedObject.FindProperty("m_weatherSystem.m_weatherZoneList");
            m_thunderSettingsList = serializedObject.FindProperty("m_weatherSystem.m_thunderSettingsList");

            m_onMinuteChangeEvent = serializedObject.FindProperty("m_eventSystem.m_onMinuteChangeEvent");
            m_onHourChangeEvent = serializedObject.FindProperty("m_eventSystem.m_onHourChangeEvent");
            m_onDayChangeEvent = serializedObject.FindProperty("m_eventSystem.m_onDayChangeEvent");
            m_onMonthChangeEvent = serializedObject.FindProperty("m_eventSystem.m_onMonthChangeEvent");
            m_onYearChangeEvent = serializedObject.FindProperty("m_eventSystem.m_onYearChangeEvent");
            m_customEventScanMode = serializedObject.FindProperty("m_eventSystem.m_customEventScanMode");
            m_customEventList = serializedObject.FindProperty("m_eventSystem.m_customEventList");

            if (m_weatherPropertyGroupList.arraySize > 0)
            {
                if (m_propertyGroupListIndex >= m_weatherPropertyGroupList.arraySize)
                {
                    m_propertyGroupListIndex = 0;
                }

                // Close all group elements
                for (int i = 0; i < m_weatherPropertyGroupList.arraySize; i++)
                {
                    SerializedProperty item = m_weatherPropertyGroupList.GetArrayElementAtIndex(i);
                    item.isExpanded = false;
                }

                SerializedProperty element = m_weatherPropertyGroupList.GetArrayElementAtIndex(m_propertyGroupListIndex);
                m_weatherPropertyList = element.FindPropertyRelative("m_weatherPropertyList");
                element.isExpanded = true;
            }

            // Create the reorderable lists
            CreateCelestialBodiesList();
            CreateWeatherPropertiesGroupsList();
            CreateWeatherPropertiesList();
            CreateFollowTargetList();
            CreateGlobalWeatherList();
            CreateWeatherZoneList();
            CreateThunderList();
            CreateCustomEventList();
        }

        public override void OnInspectorGUI()
        {
            // Start custom inspector
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            // Title
            m_controlRect = EditorGUILayout.GetControlRect(GUILayout.Height(38f));
            EditorGUI.LabelField(new Rect(m_controlRect.x - 14f, m_controlRect.y, m_controlRect.width + 14f, m_controlRect.height), "", "", "selectionRect");
            if (iconTexture) GUI.DrawTexture(new Rect(m_controlRect.x + 3f, m_controlRect.y + 3f, 32f, 32f), iconTexture);
            GUI.Label(new Rect(m_controlRect.x + 38f, m_controlRect.y + 3f, m_controlRect.width, 22f), "Azure Core System", EditorStyles.whiteLargeLabel);
            GUI.Label(new Rect(m_controlRect.x + 38f, m_controlRect.y + 17f, m_controlRect.width, 22f), "Script Version 1.0.0  |  Asset Version 8.0.4", EditorStyles.whiteMiniLabel);

            // Component header toolbar
            m_headerToolbarIndex = GUILayout.Toolbar(m_headerToolbarIndex, m_headerToolbarContent, GUILayout.Height(32));

            // Time and Version
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(m_target.timeSystem.GetDayOfWeekString() + ", " + m_target.timeSystem.GetTimeOfDayString(), "MiniLabel");
            GUILayout.Label("Current Weather: " + m_target.weatherSystem.currentWeatherPreset?.name, "MiniLabel");
            GUILayout.Label(m_target.timeSystem.GetDateString(), "MiniLabel", GUILayout.ExpandWidth(false));
            EditorGUILayout.EndHorizontal();

            // Component content
            switch (m_headerToolbarIndex)
            {
                case 0:
                    TimeSystemGUI();
                    break;

                    case 1:
                    WeatherSystemGUI();
                    break;

                case 2:
                    WeatherPropertiesGUI();
                    break;

                case 3:
                    EventSystemGUI();
                    break;

                case 4:
                    OptionsGUI();
                    break;
            }

            // Update the inspector when it has changed
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();

                m_target.timeSystem.UpdateCalendar();
                m_target.timeSystem.UpdateTimeLengthCurve();
                m_target.weatherSystem.InitializePropertyTargets();

                // Setting the timeline property here to force the call of its "Set" method when changing it using the Ispector slider.
                // The Ispector timeline slider is referenced to the private serialied field, and changing it does not automatically calls the "Set" method of its "Property".
                // The set will trigger the "OnTimelineChanged()" event, required to update the sky when the time changes.
                m_target.timeSystem.timeline = m_timeline.floatValue;

                PlayerPrefs.SetInt("AzureHeaderToolbarIndex", m_headerToolbarIndex);
                PlayerPrefs.Save();
            }

            //// Updates the calendar if the undo command is performed
            //if (Event.current.commandName == "UndoRedoPerformed")
            //{
            //
            //}
        }

        /// <summary>Creates the celestial bodies reorderable list.</summary>
        private void CreateCelestialBodiesList()
        {
            m_reorderableCelestialBodiesList = new ReorderableList(serializedObject, m_celestialBodiesList, true, true, true, true)
            {
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    rect.y += 2f;
                    float half = rect.width / 2f;
                    Rect fieldRect = new Rect(rect.x, rect.y, half, EditorGUIUtility.singleLineHeight);

                    SerializedProperty element = m_celestialBodiesList.GetArrayElementAtIndex(index);
                    SerializedProperty transform = element.FindPropertyRelative("m_transform");
                    SerializedProperty type = element.FindPropertyRelative("m_type");

                    // Celestial body transform
                    EditorGUI.PropertyField(fieldRect, transform, GUIContent.none);

                    // Celestial body type
                    fieldRect = new Rect(rect.x + half + 5f, rect.y, half - 5f, EditorGUIUtility.singleLineHeight);
                    EditorGUI.PropertyField(fieldRect, type, GUIContent.none);
                },

                drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, "Celestial Bodies", EditorStyles.boldLabel);
                },

                drawElementBackgroundCallback = (rect, index, active, focused) =>
                {
                    if (active)
                        GUI.Box(new Rect(rect.x + 2f, rect.y - 1f, rect.width - 4f, rect.height + 1f), "", "selectionRect");
                }
            };
        }

        /// <summary>Creates the weather property group reorderable list.</summary>
        private void CreateWeatherPropertiesGroupsList()
        {
            m_reorderableWeatherPropertyGroupList = new ReorderableList(serializedObject, m_weatherPropertyGroupList, true, true, true, true)
            {
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    rect.y += 2f;
                    Rect fieldRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                    Rect foldoutRect = new Rect(rect.x + 14f, rect.y, rect.width - 14f, EditorGUIUtility.singleLineHeight);

                    SerializedProperty element = m_weatherPropertyGroupList.GetArrayElementAtIndex(index);
                    SerializedProperty name = element.FindPropertyRelative("m_name");
                    SerializedProperty isEnabled = element.FindPropertyRelative("m_isEnabled");

                    if (GUI.Button(fieldRect, GUIContent.none, "HelpBox"))
                    {
                        // Close all group elements
                        for (int i = 0; i < m_weatherPropertyGroupList.arraySize; i++)
                        {
                            SerializedProperty item = m_weatherPropertyGroupList.GetArrayElementAtIndex(i);
                            item.isExpanded = false;
                        }

                        // Select the clicked group in the list
                        element.isExpanded = true;
                        m_reorderableWeatherPropertyGroupList.Select(index);

                        // Update the weather property list according to the new selected group
                        m_weatherPropertyList = element.FindPropertyRelative("m_weatherPropertyList");
                        CreateWeatherPropertiesList();
                    }
                    EditorGUI.Foldout(foldoutRect, element.isExpanded, index + " - " + name.stringValue);

                    if (element.isExpanded)
                    {
                        fieldRect.y += 20f;
                        EditorGUI.PropertyField(fieldRect, name, new GUIContent("Group Name", "The name of this property group element."));
                        fieldRect.y += 20f;
                        EditorGUI.PropertyField(fieldRect, isEnabled, new GUIContent("Is Enabled", "The active state of this weather group, is this group current Enabled or Disabled?"));
                    }

                    m_propertyGroupListIndex = m_reorderableWeatherPropertyGroupList.index;
                },

                onSelectCallback = (ReorderableList l) =>
                {
                    SerializedProperty element = m_weatherPropertyGroupList.GetArrayElementAtIndex(l.index);
                    m_weatherPropertyList = element.FindPropertyRelative("m_weatherPropertyList");
                    CreateWeatherPropertiesList();
                },

                onAddCallback = (ReorderableList l) =>
                {
                    // Create item
                    Undo.RecordObject(m_target, "Add Weather Property Group");
                    AzureWeatherPropertyGroup newGroup = new AzureWeatherPropertyGroup();
                    m_target.weatherSystem.weatherPropertyGroupList.Add(newGroup);
                    serializedObject.Update();

                    // Update list Inspector
                    for (int i = 0; i < m_weatherPropertyGroupList.arraySize; i++)
                    {
                        // Close all group elements
                        SerializedProperty item = m_weatherPropertyGroupList.GetArrayElementAtIndex(i);
                        item.isExpanded = false;

                        if (i == m_weatherPropertyGroupList.arraySize - 1)
                        {
                            // Select the clicked group in the list
                            item.isExpanded = true;
                            m_reorderableWeatherPropertyGroupList.Select(i);

                            // Update the weather property list according to the new selected group
                            m_weatherPropertyList = item.FindPropertyRelative("m_weatherPropertyList");
                            CreateWeatherPropertiesList();
                        }
                    }

                    AzureNotificationCenterEditor.Invoke.AddWeatherPropertyGroupCallback(m_target);
                },

                onRemoveCallback = (ReorderableList l) =>
                {
                    AzureNotificationCenterEditor.Invoke.RemoveWeatherPropertyGroupCallback(m_target, l.index);
                    ReorderableList.defaultBehaviours.DoRemoveButton(l);

                    if (l.count > 0)
                    {
                        SerializedProperty element = m_weatherPropertyGroupList.GetArrayElementAtIndex(l.index);
                        m_weatherPropertyList = element.FindPropertyRelative("m_weatherPropertyList");
                        CreateWeatherPropertiesList();
                    }
                },

                onReorderCallbackWithDetails = (ReorderableList l, int oldIndex, int newIndex) =>
                {
                    Undo.RecordObject(m_target, "Reorder Weather Property Group");
                    AzureNotificationCenterEditor.Invoke.ReorderWeatherPropertyGroupCallback(m_target, oldIndex, newIndex);
                },

                drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, "Weather Property Groups", EditorStyles.boldLabel);
                },

                elementHeightCallback = (int index) =>
                {
                    if (m_weatherPropertyGroupList.arraySize > 0)
                    {
                        SerializedProperty element = m_weatherPropertyGroupList.GetArrayElementAtIndex(index);
                        if (element.isExpanded)
                        {
                            return 60f;
                        }
                    }

                    return 22f;
                },

                //drawElementBackgroundCallback = (rect, index, active, focused) =>
                //{
                //    if (active)
                //        GUI.Box(new Rect(rect.x + 2f, rect.y - 1f, rect.width - 4f, rect.height + 1f), "", "selectionRect");
                //}
            };

            m_reorderableWeatherPropertyGroupList.Select(m_propertyGroupListIndex);
        }

        /// <summary>Creates the weather property reorderable list.</summary>
        private void CreateWeatherPropertiesList()
        {
            m_reorderableWeatherPropertyList = new ReorderableList(serializedObject, m_weatherPropertyList, true, true, true, true)
            {
                index = -1,

                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    if (m_weatherPropertyList.arraySize > 0)
                    {
                        rect.y += 2f;
                        float height = 20f;
                        Rect fieldRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                        Rect foldoutRect = new Rect(rect.x + 14f, rect.y, rect.width - 18f, EditorGUIUtility.singleLineHeight);
                        int grouptIndex = m_reorderableWeatherPropertyGroupList.index;

                        SerializedProperty element = m_weatherPropertyList.GetArrayElementAtIndex(index);
                        SerializedProperty name = element.FindPropertyRelative("m_name");

                        if (m_target.weatherSystem.weatherPropertyGroupList[grouptIndex].weatherPropertyList.Count <= 0) return;

                        if (GUI.Button(fieldRect, GUIContent.none, "HelpBox")) { element.isExpanded = !element.isExpanded; }
                        EditorGUI.Foldout(foldoutRect, element.isExpanded, index + " - " + name.stringValue);
                        if (element.isExpanded)
                        {
                            SerializedProperty propertyType = element.FindPropertyRelative("m_propertyType");
                            SerializedProperty minValue = element.FindPropertyRelative("m_minValue");
                            SerializedProperty maxValue = element.FindPropertyRelative("m_maxValue");
                            SerializedProperty defaultFloatValue = element.FindPropertyRelative("m_defaultFloatValue");
                            SerializedProperty defaultColorValue = element.FindPropertyRelative("m_defaultColorValue");
                            SerializedProperty defaultCurveValue = element.FindPropertyRelative("m_defaultCurveValue");
                            SerializedProperty defaultGradientValue = element.FindPropertyRelative("m_defaultGradientValue");
                            SerializedProperty defaultVector3Value = element.FindPropertyRelative("m_defaultVector3Value");
                            SerializedProperty overrideMode = element.FindPropertyRelative("m_overrideMode");
                            SerializedProperty targetType = element.FindPropertyRelative("m_targetType");
                            SerializedProperty targetObject = element.FindPropertyRelative("m_targetObject");
                            SerializedProperty targetMaterial = element.FindPropertyRelative("m_targetMaterial");
                            SerializedProperty targetComponentName = element.FindPropertyRelative("m_targetComponentName");
                            SerializedProperty targetPropertyName = element.FindPropertyRelative("m_targetPropertyName");

                            // Property Type
                            fieldRect.y += height;
                            EditorGUI.PropertyField(fieldRect, propertyType);

                            // Name
                            fieldRect.y += height;
                            EditorGUI.PropertyField(fieldRect, name);

                            // Default values
                            switch (propertyType.enumValueIndex)
                            {
                                case 0: // Float
                                    fieldRect.y += height;
                                    EditorGUI.PropertyField(fieldRect, minValue);

                                    fieldRect.y += height;
                                    EditorGUI.PropertyField(fieldRect, maxValue);

                                    fieldRect.y += height;
                                    EditorGUI.PropertyField(fieldRect, defaultFloatValue);
                                    break;

                                case 1: // Color
                                    fieldRect.y += height;
                                    EditorGUI.PropertyField(fieldRect, defaultColorValue);
                                    break;

                                case 2: // Curve
                                    fieldRect.y += height;
                                    EditorGUI.PropertyField(fieldRect, minValue);

                                    fieldRect.y += height;
                                    EditorGUI.PropertyField(fieldRect, maxValue);

                                    fieldRect.y += height;
                                    EditorGUI.CurveField(fieldRect, defaultCurveValue, Color.green, new Rect(0.0f, minValue.floatValue, 24.0f, maxValue.floatValue));
                                    break;

                                case 3: // Gradient
                                    fieldRect.y += height;
                                    EditorGUI.PropertyField(fieldRect, defaultGradientValue);
                                    break;

                                case 4: // Direction
                                case 5: // Position
                                    fieldRect.y += height;
                                    EditorGUI.PropertyField(fieldRect, defaultVector3Value);
                                    break;
                            }

                            // Override Mode
                            fieldRect.y += height;
                            EditorGUI.PropertyField(fieldRect, overrideMode);

                            if (overrideMode.enumValueIndex == 1)
                            {
                                fieldRect.y += height;
                                EditorGUI.PropertyField(fieldRect, targetType);

                                // Avoid "the index was out of the list range" error
                                if (m_target.weatherSystem.weatherPropertyGroupList.Count - 1 < grouptIndex) return;
                                if (m_target.weatherSystem.weatherPropertyGroupList[grouptIndex].weatherPropertyList.Count - 1 < index) return;

                                switch (targetType.enumValueIndex)
                                {
                                    case 0: // Property
                                        fieldRect.y += height;
                                        GUI.backgroundColor = (m_target.weatherSystem.weatherPropertyGroupList[grouptIndex].weatherPropertyList[index].targetObject != null) ? Color.green : Color.red;
                                        EditorGUI.PropertyField(fieldRect, targetObject);

                                        fieldRect.y += height;
                                        GUI.backgroundColor = (m_target.weatherSystem.weatherPropertyGroupList[grouptIndex].weatherPropertyList[index].targetComponent != null) ? Color.green : Color.red;
                                        EditorGUI.DelayedTextField(fieldRect, targetComponentName);

                                        fieldRect.y += height;
                                        GUI.backgroundColor = CheckTargetMatch(propertyType.enumValueIndex, m_target.weatherSystem.weatherPropertyGroupList[grouptIndex].weatherPropertyList[index].propertyInfo) ? Color.green : Color.red;
                                        EditorGUI.DelayedTextField(fieldRect, targetPropertyName);
                                        break;

                                    case 1: // Field
                                        fieldRect.y += height;
                                        GUI.backgroundColor = (m_target.weatherSystem.weatherPropertyGroupList[grouptIndex].weatherPropertyList[index].targetObject != null) ? Color.green : Color.red;
                                        EditorGUI.PropertyField(fieldRect, targetObject);

                                        fieldRect.y += height;
                                        GUI.backgroundColor = (m_target.weatherSystem.weatherPropertyGroupList[grouptIndex].weatherPropertyList[index].targetComponent != null) ? Color.green : Color.red;
                                        EditorGUI.DelayedTextField(fieldRect, targetComponentName);

                                        fieldRect.y += height;
                                        GUI.backgroundColor = CheckTargetMatch(propertyType.enumValueIndex, m_target.weatherSystem.weatherPropertyGroupList[grouptIndex].weatherPropertyList[index].fieldInfo) ? Color.green : Color.red;
                                        EditorGUI.DelayedTextField(fieldRect, targetPropertyName);
                                        break;

                                    case 2: // Material
                                        fieldRect.y += height;
                                        GUI.backgroundColor = (m_target.weatherSystem.weatherPropertyGroupList[grouptIndex].weatherPropertyList[index].targetMaterial != null) ? Color.green : Color.red;
                                        EditorGUI.PropertyField(fieldRect, targetMaterial);

                                        fieldRect.y += height;
                                        if (targetMaterial.objectReferenceValue)
                                        {
                                            GUI.backgroundColor = ((Material)targetMaterial.objectReferenceValue).HasProperty(targetPropertyName.stringValue) ? Color.green : Color.red;
                                        }
                                        EditorGUI.DelayedTextField(fieldRect, targetPropertyName);
                                        break;

                                    case 3: // Global Shader Uniform
                                        fieldRect.y += height;
                                        EditorGUI.DelayedTextField(fieldRect, targetPropertyName);
                                        break;

                                    case 4: // Global Property
                                        fieldRect.y += height;
                                        GUI.backgroundColor = (m_target.weatherSystem.weatherPropertyGroupList[grouptIndex].weatherPropertyList[index].targetGlobalType != null) ? Color.green : Color.red;
                                        EditorGUI.DelayedTextField(fieldRect, targetComponentName);

                                        fieldRect.y += height;
                                        GUI.backgroundColor = CheckTargetMatch(propertyType.enumValueIndex, m_target.weatherSystem.weatherPropertyGroupList[grouptIndex].weatherPropertyList[index].propertyInfo) ? Color.green : Color.red;
                                        EditorGUI.DelayedTextField(fieldRect, targetPropertyName);
                                        break;

                                    case 5: // Global Field
                                        fieldRect.y += height;
                                        GUI.backgroundColor = (m_target.weatherSystem.weatherPropertyGroupList[grouptIndex].weatherPropertyList[index].targetGlobalType != null) ? Color.green : Color.red;
                                        EditorGUI.DelayedTextField(fieldRect, targetComponentName);

                                        fieldRect.y += height;
                                        GUI.backgroundColor = CheckTargetMatch(propertyType.enumValueIndex, m_target.weatherSystem.weatherPropertyGroupList[grouptIndex].weatherPropertyList[index].fieldInfo) ? Color.green : Color.red;
                                        EditorGUI.DelayedTextField(fieldRect, targetPropertyName);
                                        break;
                                }
                            }
                        }

                        GUI.backgroundColor = Color.white;
                    }
                },

                elementHeightCallback = (int index) =>
                {
                    if (m_weatherPropertyList.arraySize > 0)
                    {
                        SerializedProperty element = m_weatherPropertyList.GetArrayElementAtIndex(index);
                        if (element.isExpanded)
                        {
                            SerializedProperty propertyType = element.FindPropertyRelative("m_propertyType");
                            SerializedProperty overrideMode = element.FindPropertyRelative("m_overrideMode");
                            SerializedProperty targetType = element.FindPropertyRelative("m_targetType");

                            switch (propertyType.enumValueIndex)
                            {
                                case 0: // Float
                                case 2: // Curve
                                    if (overrideMode.enumValueIndex == 0)
                                    {
                                        return 141f;
                                    }
                                    else
                                    {
                                        switch (targetType.enumValueIndex)
                                        {
                                            case 0: // Property
                                            case 1: // Field
                                                return 221f;

                                            case 2: // Material
                                                return 201f;

                                            case 3: // Global Shader Uniform
                                                return 181f;

                                            case 4: // Global Property
                                            case 5: // Global Field
                                                return 201f;
                                        }
                                    }
                                    break;

                                case 1: // Color
                                case 3: // Gradient
                                case 4: // Direction
                                case 5: // Position
                                    if (overrideMode.enumValueIndex == 0)
                                    {
                                        return 101f;
                                    }
                                    else
                                    {
                                        switch (targetType.enumValueIndex)
                                        {
                                            case 0: // Property
                                            case 1: // Field
                                                return 181f;

                                            case 2: // Material
                                                return 161f;

                                            case 3: // Global Shader Uniform
                                                return 141f;

                                            case 4: // Global Property
                                            case 5: // Global Field
                                                return 161f;
                                        }
                                    }
                                    break;
                            }

                            return 181f;
                        }
                    }

                    return 22f;
                },

                onAddCallback = (ReorderableList l) =>
                {
                    int groupIndex = m_reorderableWeatherPropertyGroupList.index;

                    Undo.RecordObject(m_target, "Add Weather Property");
                    AzureWeatherProperty newProperty = new AzureWeatherProperty();
                    m_target.weatherSystem.weatherPropertyGroupList[groupIndex].weatherPropertyList.Add(newProperty);
                    serializedObject.Update();

                    SerializedProperty element = m_weatherPropertyList.GetArrayElementAtIndex(m_weatherPropertyList.arraySize - 1);
                    element.isExpanded = true;
                    AzureNotificationCenterEditor.Invoke.AddWeatherPropertyCallback(m_target, groupIndex);
                },

                onRemoveCallback = (ReorderableList l) =>
                {
                    int groupIndex = m_reorderableWeatherPropertyGroupList.index;

                    Undo.RecordObject(m_target, "Remove Weather Property");
                    m_target.weatherSystem.weatherPropertyGroupList[groupIndex].weatherPropertyList.RemoveAt(l.index);
                    serializedObject.Update();

                    AzureNotificationCenterEditor.Invoke.RemoveWeatherPropertyCallback(m_target, groupIndex, l.index);
                },

                onReorderCallbackWithDetails = (ReorderableList l, int oldIndex, int newIndex) =>
                {
                    int groupIndex = m_reorderableWeatherPropertyGroupList.index;
                    Undo.RecordObject(m_target, "Reorder Weather Property");
                    AzureNotificationCenterEditor.Invoke.ReorderWeatherPropertyCallback(m_target, groupIndex, oldIndex, newIndex);
                },

                drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, "Weather Property List", EditorStyles.boldLabel);
                },
            };
        }

        /// <summary>Creates the follow target reorderable list.</summary>
        private void CreateFollowTargetList()
        {
            m_reorderableFollowTargetList = new ReorderableList(serializedObject, m_followTargetList, true, true, true, true)
            {
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    rect.y += 2f;
                    Rect fieldRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);

                    SerializedProperty element = m_followTargetList.GetArrayElementAtIndex(index);
                    SerializedProperty follower = element.FindPropertyRelative("m_follower");
                    SerializedProperty target = element.FindPropertyRelative("m_target");

                    EditorGUI.PropertyField(fieldRect, follower);
                    fieldRect.y += 20f;
                    EditorGUI.PropertyField(fieldRect, target);
                },

                elementHeightCallback = (int index) =>
                {
                    if (m_followTargetList.arraySize > 0)
                    {
                        return 41f;
                    }

                    return 22f;
                },

                drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, "Follow Target List", EditorStyles.boldLabel);
                },

                drawElementBackgroundCallback = (rect, index, active, focused) =>
                {
                    if (active)
                        GUI.Box(new Rect(rect.x + 2f, rect.y - 1f, rect.width - 4f, rect.height + 1f), "", "selectionRect");
                }
            };
        }

        /// <summary>Creates the global weather reorderable list.</summary>
        private void CreateGlobalWeatherList()
        {
            m_reorderableGlobalWeatherList = new ReorderableList(serializedObject, m_globalWeatherList, true, true, true, true)
            {
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    rect.y += 2f;
                    Rect fieldRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                    Rect foldoutRect = new Rect(rect.x + 14f, rect.y, rect.width - 18f, EditorGUIUtility.singleLineHeight);

                    string presetName = m_target.weatherSystem.globalWeatherList[index].weatherPreset == null ? "None" : m_target.weatherSystem.globalWeatherList[index].weatherPreset.name;
                    SerializedProperty element = m_globalWeatherList.GetArrayElementAtIndex(index);

                    if (GUI.Button(fieldRect, GUIContent.none, "HelpBox")) { element.isExpanded = !element.isExpanded; }
                    EditorGUI.Foldout(foldoutRect, element.isExpanded, "Preset " + index + " - (" + presetName + ")");
                    if (element.isExpanded)
                    {
                        SerializedProperty weatherPreset = element.FindPropertyRelative("m_weatherPreset");
                        SerializedProperty transitionTime = element.FindPropertyRelative("m_transitionTime");

                        fieldRect.y += 20f;
                        EditorGUI.PropertyField(fieldRect, weatherPreset);
                        fieldRect.y += 20f;
                        EditorGUI.PropertyField(fieldRect, transitionTime);
                        fieldRect.y += 20f;
                        if (GUI.Button(fieldRect, "Go"))
                        {
                            if (Application.isPlaying)
                            {
                                m_target.weatherSystem.SetGlobalWeather(index);
                            }
                            else
                            {
                                Debug.Log("To perform a weather transition, the application must be playing.");
                            }
                        }
                    }
                },

                elementHeightCallback = (int index) =>
                {
                    if (m_globalWeatherList.arraySize > 0)
                    {
                        SerializedProperty element = m_globalWeatherList.GetArrayElementAtIndex(index);

                        if (element.isExpanded)
                        {
                            return 81f;
                        }
                    }

                    return 22f;
                },

                onAddCallback = (ReorderableList l) =>
                {
                    Undo.RecordObject(m_target, "Add Global Weather");
                    AzureGlobalWeather newGlobalWeather = new AzureGlobalWeather(10f);
                    m_target.weatherSystem.globalWeatherList.Add(newGlobalWeather);
                    serializedObject.Update();

                    SerializedProperty element = m_globalWeatherList.GetArrayElementAtIndex(m_globalWeatherList.arraySize - 1);
                    element.isExpanded = true;
                    //AzureNotificationCenterEditor.Invoke.AddGlobalWeatherCallback(m_target);
                },

                drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, "Global Weather List", EditorStyles.boldLabel);
                },
            };
        }

        /// <summary>Creates the weather zones reorderable list.</summary>
        private void CreateWeatherZoneList()
        {
            m_reorderableWeatherZoneList = new ReorderableList(serializedObject, m_weatherZoneList, true, true, true, true)
            {
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    rect.y += 2f;
                    Rect fieldRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                    SerializedProperty element = m_weatherZoneList.GetArrayElementAtIndex(index);
                    EditorGUI.PropertyField(fieldRect, element, new GUIContent("Priority  " + index.ToString()));
                },

                drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, "Weather Zone List", EditorStyles.boldLabel);
                },

                drawElementBackgroundCallback = (rect, index, active, focused) =>
                {
                    if (active)
                        GUI.Box(new Rect(rect.x + 2f, rect.y - 1f, rect.width - 4f, rect.height + 1f), "", "selectionRect");
                }
            };
        }

        /// <summary>Creates the thunder reorderable list.</summary>
        private void CreateThunderList()
        {
            m_reorderableThunderSettingsList = new ReorderableList(serializedObject, m_thunderSettingsList, true, true, true, true)
            {
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    rect.y += 2f;
                    Rect fieldRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);

                    SerializedProperty element = m_thunderSettingsList.GetArrayElementAtIndex(index);
                    SerializedProperty transform = element.FindPropertyRelative("m_transform");
                    SerializedProperty position = element.FindPropertyRelative("m_position");

                    // Prefab transform field
                    EditorGUI.PropertyField(fieldRect, transform, new GUIContent("Thunder Prefab - Index " + index));

                    // Instantiation position
                    fieldRect.y += 20f;
                    EditorGUI.PropertyField(fieldRect, position, GUIContent.none);

                    // Instantiate button
                    fieldRect.y += 20f;
                    if (GUI.Button(fieldRect, new GUIContent("Instantiate", "Instantiate the thunder prefab in the position specified in the Vector3 field above.")))
                    {
                        if (Application.isPlaying)
                        {
                            m_target.weatherSystem.InstantiateThunderPrefab(index);
                        }
                        else { Debug.Log("The application must be playing to instantiate a thunder prefab."); }
                    }
                },

                elementHeightCallback = (int index) =>
                {
                    return 62f;
                },

                drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, "Thunder Settings List", EditorStyles.boldLabel);
                },

                //drawElementBackgroundCallback = (rect, index, active, focused) =>
                //{
                //    if (active)
                //        GUI.Box(new Rect(rect.x + 2f, rect.y - 1f, rect.width - 4f, rect.height + 1f), "", "selectionRect");
                //}
            };
        }

        /// <summary>Creates the custom events reorderable list.</summary>
        private void CreateCustomEventList()
        {
            // Create the custom event list
            m_customEventReorderableList = new ReorderableList(serializedObject, m_customEventList, true, true, true, true)
            {
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    // Utilities
                    Rect fieldRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                    Rect foldoutRect = new Rect(rect.x + 14f, rect.y, rect.width - 18f, EditorGUIUtility.singleLineHeight);
                    SerializedProperty element = m_customEventList.GetArrayElementAtIndex(index);

                    if (GUI.Button(fieldRect, GUIContent.none, "HelpBox")) { element.isExpanded = !element.isExpanded; }
                    EditorGUI.Foldout(foldoutRect, element.isExpanded, "Custom Event: " + index.ToString());
                    if (element.isExpanded)
                    {
                        // Getting element properties
                        //SerializedProperty element = m_customEventList.GetArrayElementAtIndex(index);
                        SerializedProperty minute = element.FindPropertyRelative("m_minute");
                        SerializedProperty hour = element.FindPropertyRelative("m_hour");
                        SerializedProperty day = element.FindPropertyRelative("m_day");
                        SerializedProperty month = element.FindPropertyRelative("m_month");
                        SerializedProperty year = element.FindPropertyRelative("m_year");
                        SerializedProperty unityEvent = element.FindPropertyRelative("m_unityEvent");


                        // Hour and Minute
                        fieldRect.y += 20f;
                        m_hourMinute = new int[] { hour.intValue, minute.intValue };
                        EditorGUI.MultiIntField(fieldRect, m_hourMinuteContents, m_hourMinute);
                        hour.intValue = m_hourMinute[0];
                        minute.intValue = m_hourMinute[1];


                        // Day Month and Year
                        fieldRect.y += 20f;
                        m_dayMonthYear = new int[] { day.intValue, month.intValue, year.intValue };
                        EditorGUI.MultiIntField(fieldRect, m_dayMonthYearContents, m_dayMonthYear);
                        day.intValue = m_dayMonthYear[0];
                        month.intValue = m_dayMonthYear[1];
                        year.intValue = m_dayMonthYear[2];


                        // Unity Event
                        fieldRect.y += 20f;
                        EditorGUI.PropertyField(fieldRect, unityEvent);
                    }
                },


                drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, "Custom Events ()", EditorStyles.boldLabel);
                },


                elementHeightCallback = (int index) =>
                {
                    if (m_customEventList.arraySize > 0)
                    {
                        SerializedProperty element = m_customEventList.GetArrayElementAtIndex(index);

                        if (element.isExpanded)
                        {
                            if (m_target.eventSystem.customEventList[index].eventListenersCount > 0)
                            {
                                return m_target.eventSystem.customEventList[index].eventListenersCount * 49f + 120f;
                            }

                            return 169f;
                        }
                    }

                    return 22f;
                },


                drawElementBackgroundCallback = (rect, index, active, focused) =>
                {
                    if (active)
                        GUI.Label(new Rect(rect.x + 2f, rect.y - 2.5f, rect.width - 4f, rect.height), "", "selectionRect");
                }
            };
        }

        /// <summary>Renders the ui content when the time system toolbar is selected.</summary>
        private void TimeSystemGUI()
        {
            m_calendarHeaderStringStyle = new GUIStyle("WhiteMiniLabel")
            {
                alignment = TextAnchor.MiddleCenter
            };

            m_dayNightLengthsBackgroundStyle = new GUIStyle();
            m_dayNightLengthsBackgroundStyle.normal.background = Texture2D.whiteTexture;

            // Calendar header buttons
            EditorGUILayout.BeginHorizontal("box");
            if (GUILayout.Button(new GUIContent("<<", "Decrease one year in the calendar."), EditorStyles.miniButtonLeft, GUILayout.Width(25))) { DecreaseYear(); }
            if (GUILayout.Button(new GUIContent("<", "Decrease one month in the calendar."), EditorStyles.miniButtonMid, GUILayout.Width(25))) { DecreaseMonth(); }
            if (GUILayout.Button(m_target.timeSystem.GetDateString(), EditorStyles.miniButtonMid)) { }
            if (GUILayout.Button(new GUIContent(">", "Increase one month in the calendar."), EditorStyles.miniButtonMid, GUILayout.Width(25))) { IncreaseMonth(); }
            if (GUILayout.Button(new GUIContent(">>", "Increase one year in the calendar."), EditorStyles.miniButtonRight, GUILayout.Width(25))) { IncreaseYear(); }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(-6);

            // Calendar week header
            EditorGUILayout.BeginVertical("box");
            GUILayout.SelectionGrid(-1, m_calendarHeaderStringArray, 7, m_calendarHeaderStringStyle);
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(-6);

            // Creates the calendar selectable grid
            EditorGUILayout.BeginVertical("Box");
            m_selectedCalendarDay.intValue = GUILayout.SelectionGrid(m_selectedCalendarDay.intValue, m_target.timeSystem.DayNumberList, 7);
            m_day.intValue = m_selectedCalendarDay.intValue + 1 - GetDayOfWeek(m_year.intValue, m_month.intValue, 1);
            EditorGUILayout.EndVertical();

            // Property fields
            EditorGUILayout.PropertyField(m_sunTransform, new GUIContent("Sun Transform", "The transform that will represent the position of the sun in the sky."));
            EditorGUILayout.PropertyField(m_moonTransform, new GUIContent("Moon Transform", "The transform that will represent the position of the moon in the sky."));
            EditorGUILayout.PropertyField(m_starfieldTransform, new GUIContent("Starfield Transform", "The transform that will represent the position of the starfield in the sky."));
            EditorGUILayout.PropertyField(m_directionalLight, new GUIContent("Directional Light", "The directional light that will apply the sun and moon lighting to the scene."));
            EditorGUILayout.PropertyField(m_timeMode, new GUIContent("Time Mode", "The time mode used to perform position of the sun and moon in the sky according to the time of day."));
            EditorGUILayout.PropertyField(m_timeDirection, new GUIContent("Time Direction", "The direction in which the time of day will flow."));
            EditorGUILayout.PropertyField(m_timeLoop, new GUIContent("Time Loop", "The time repeat mode cycle."));
            EditorGUILayout.Slider(m_timeline, 0.0f, 24.0f, new GUIContent("Timeline", "The timeline that represents the day cycle."));
            EditorGUILayout.Slider(m_latitude, -90.0f, 90.0f, new GUIContent("Latitude", "The north-south angle of a position on the Earth's surface."));
            EditorGUILayout.Slider(m_longitude, -180.0f, 180.0f, new GUIContent("Longitude", "The east-west angle of a position on the Earth's surface."));
            EditorGUILayout.Slider(m_utc, -12.0f, 12.0f, new GUIContent("Utc", "Universal Time Coordinated (UTC) also known as Greenwich Mean Time (GMT)."));
            EditorGUILayout.PropertyField(m_day, new GUIContent("Day", "Represents the day in the calendar."));
            EditorGUILayout.PropertyField(m_month, new GUIContent("Month", "Represents the month in the calendar."));
            EditorGUILayout.PropertyField(m_year, new GUIContent("Year", "Represents the year in the calendar."));
            EditorGUILayout.PropertyField(m_startTime, new GUIContent("Start Time", "The value the timeline should start the scene when entering the play mode."));
            EditorGUILayout.PropertyField(m_dayLength, new GUIContent("Day Length", "The duration of the day cycle in minutes."));
            EditorGUILayout.PropertyField(m_minLightAltitude, new GUIContent("Min Light Altitude", "The minimmun directional light angle according to the horizon line."));
            EditorGUILayout.PropertyField(m_dawnTime, new GUIContent("Dawn Time", "The time that marks the transition from nighttime to sunrise in the timeline cycle."));
            EditorGUILayout.PropertyField(m_duskTime, new GUIContent("Dusk Time", "The time that marks the transition from sunset to nighttime in the timeline cycle."));

            // Day and Night Lengths
            m_controlRect = EditorGUILayout.GetControlRect(GUILayout.Height(16));
            m_minLengthRect = m_controlRect;
            m_maxLengthRect = m_controlRect;
            float step = m_controlRect.width / 24f;

            m_dawnTime.floatValue = Mathf.Clamp(m_dawnTime.floatValue, 1.0f, 11.0f);
            m_duskTime.floatValue = Mathf.Clamp(m_duskTime.floatValue, 13.0f, 23.0f);

            m_minLengthRect.width = step * m_dawnTime.floatValue;
            m_maxLengthRect.x += step * m_duskTime.floatValue;
            m_maxLengthRect.width -= step * m_duskTime.floatValue - 1f;

            GUI.backgroundColor = m_yellowColor;
            GUI.Box(m_controlRect, "", m_dayNightLengthsBackgroundStyle);

            GUI.backgroundColor = m_blueColor;
            GUI.Box(m_minLengthRect, "", m_dayNightLengthsBackgroundStyle);
            GUI.Box(m_maxLengthRect, "", m_dayNightLengthsBackgroundStyle);
            GUI.Label(m_controlRect, "Day and Night Lengths", m_calendarHeaderStringStyle);
            GUI.backgroundColor = Color.white;

            // Time markers
            m_controlRect = EditorGUILayout.GetControlRect();
            m_controlRect.y -= 4f;

            EditorGUILayout.BeginHorizontal();
            m_minLengthRect = m_controlRect;
            m_minLengthRect.x -= 3f;
            GUI.Label(m_minLengthRect, "0", "MiniLabel");

            m_minLengthRect = m_controlRect;
            m_minLengthRect.x += step * 6f - 6f;
            GUI.Label(m_minLengthRect, "6", "MiniLabel");

            m_minLengthRect = m_controlRect;
            m_minLengthRect.x += step * 12f - 8f;
            GUI.Label(m_minLengthRect, "12", "MiniLabel");

            m_minLengthRect = m_controlRect;
            m_minLengthRect.x += step * 18f - 8f;
            GUI.Label(m_minLengthRect, "18", "MiniLabel");

            m_minLengthRect = m_controlRect;
            m_minLengthRect.x = step * 24f + 5f;
            GUI.Label(m_minLengthRect, "24", "MiniLabel");
            EditorGUILayout.EndHorizontal();

            if (m_timeMode.enumValueIndex == 1)
            {
                m_reorderableCelestialBodiesList.DoLayoutList();
            }
        }

        /// <summary>Renders the ui content when the weather system toolbar is selected.</summary>
        private void WeatherSystemGUI()
        {
            // Progress bar
            m_controlRect = EditorGUILayout.GetControlRect(GUILayout.Height(18f));
            EditorGUI.ProgressBar(m_controlRect, m_weatherTransitionProgress.floatValue, "Transition Progress");

            m_reorderableGlobalWeatherList.DoLayoutList();
            EditorGUILayout.Space(18);
            EditorGUILayout.PropertyField(m_weatherZoneTrigger, new GUIContent("Weather Zone Trigger", "The trigger used to detect if it is entering a local weather zone."));
            m_reorderableWeatherZoneList.DoLayoutList();
            EditorGUILayout.Space(18);
            m_reorderableThunderSettingsList.DoLayoutList();
        }

        /// <summary>Renders the ui content when the weather properties toolbar is selected.</summary>
        private void WeatherPropertiesGUI()
        {
            // Draw the weather properties groups list
            m_reorderableWeatherPropertyGroupList.DoLayoutList();

            // Draw the weather properties list if it exists in the group list
            if (m_reorderableWeatherPropertyGroupList.count > 0)
            {
                if (m_reorderableWeatherPropertyGroupList.selectedIndices.Count > 0)
                {
                    if (m_reorderableWeatherPropertyGroupList.index < m_weatherPropertyGroupList.arraySize)
                    {
                        EditorGUILayout.Space(18);
                        m_reorderableWeatherPropertyList.DoLayoutList();
                    }
                }
            }
        }

        /// <summary>Renders the ui content when the events toolbar is selected.</summary>
        private void EventSystemGUI()
        {
            // On Minute Change Event
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_onMinuteChangeEvent);

            // On Hour Change Event
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_onHourChangeEvent);

            // On Day Change Event
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_onDayChangeEvent);

            // On Month Change Event
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_onMonthChangeEvent);

            // On Year Change Event
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_onYearChangeEvent);

            // On Custom Event
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_customEventScanMode);
            m_customEventReorderableList.DoLayoutList();
        }

        /// <summary>Renders the ui content when the options toolbar is selected.</summary>
        private void OptionsGUI()
        {
            EditorGUILayout.PropertyField(m_coreUpdateMode, new GUIContent("Core Update Mode", "The way the Core System should be updated."));

            if (m_coreUpdateMode.enumValueIndex == 1)
            {
                EditorGUILayout.PropertyField(m_refreshRate, new GUIContent("Refresh Rate", "The time interval used to update the core system."));
            }

            EditorGUILayout.PropertyField(m_reflectionProbe, new GUIContent("Reflection Probe", "The reflection probe handled by this core system component."));
            EditorGUILayout.PropertyField(m_onCoreUpdateEvent, new GUIContent("On Core Update Event", "The event that is invoked every time the core system is updated."));

            EditorGUILayout.Space();

            m_reorderableFollowTargetList.DoLayoutList();
        }

        private void DecreaseMonth()
        {
            Undo.RecordObject(m_target, "Undo Azure Time Controller");
            m_target.timeSystem.DecreaseMonth();
        }

        private void IncreaseMonth()
        {
            Undo.RecordObject(m_target, "Undo Azure Time Controller");
            m_target.timeSystem.IncreaseMonth();
        }

        private void DecreaseYear()
        {
            Undo.RecordObject(m_target, "Undo Azure Time Controller");
            m_target.timeSystem.DecreaseYear();
        }

        private void IncreaseYear()
        {
            Undo.RecordObject(m_target, "Undo Azure Time Controller");
            m_target.timeSystem.IncreaseYear();
        }

        /// <summary>Gets the day of the week from a custom date and returns an integer between 0 and 6.</summary>
        private int GetDayOfWeek(int year, int month, int day)
        {
            DateTime dateTime = new DateTime(year, month, day);
            return (int)dateTime.DayOfWeek;
        }

        /// <summary>The type list used to check if the type of the target property to override match the custom property type.</summary>
        private Type[] m_customPropertyTypes = new Type[]
        {
            typeof(float),   // Float
            typeof(Color),   // Color
            typeof(float),   // Curve
            typeof(Color),   // Gradient
            typeof(Vector3), // Direction
            typeof(Vector3)  // Position
        };


        /// <summary>Returns true if the custom property type is the same type as the target property.</summary>
        private bool CheckTargetMatch(int index, PropertyInfo propertyInfo)
        {
            if (propertyInfo == null) return false;
            if (propertyInfo.PropertyType == m_customPropertyTypes[(int)index])
            {
                return true;
            }
            else return false;
        }


        /// <summary>Returns true if the custom property type is the same type as the target property.</summary>
        private bool CheckTargetMatch(int index, FieldInfo fieldInfo)
        {
            if (fieldInfo == null) return false;
            if (fieldInfo.FieldType == m_customPropertyTypes[(int)index])
            {
                return true;
            }
            else return false;
        }
    }
}