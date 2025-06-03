using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AzureSky;

namespace UnityEditor.AzureSky
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AzureWeatherPreset))]
    public class AzureWeatherPresetEditor : Editor
    {
        // Logo
        public Texture2D iconTexture;

        private AzureWeatherPreset m_target;
        private Rect m_controlRect;

        // Serialized properties
        private SerializedProperty m_azureCoreSystem;
        private SerializedProperty m_propertyGroupDataList;

        // Reorderable list
        private ReorderableList m_reorderableList;

        private void OnEnable()
        {
            // Get target
            m_target = target as AzureWeatherPreset;
            UpdateFoldoutGroupNames();
            UpdateOwnerProperty();

            // Get serialized properties
            m_azureCoreSystem = serializedObject.FindProperty("m_azureCoreSystem");
            m_propertyGroupDataList = serializedObject.FindProperty("m_propertyGroupDataList");

            m_reorderableList = new ReorderableList(serializedObject, m_propertyGroupDataList, false, true, false, false)
            {
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    float height = 20f;
                    Rect fieldRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                    Rect foldoutRect = new Rect(rect.x + 14f, rect.y, rect.width - 18f, EditorGUIUtility.singleLineHeight);
                    SerializedProperty groupElement = m_propertyGroupDataList.GetArrayElementAtIndex(index);
                    SerializedProperty groupName = groupElement.FindPropertyRelative("m_name");

                    if (GUI.Button(fieldRect, GUIContent.none, "HelpBox")) { groupElement.isExpanded = !groupElement.isExpanded; }
                    EditorGUI.Foldout(foldoutRect, groupElement.isExpanded, index + " - " + groupName.stringValue);
                    if (groupElement.isExpanded)
                    {
                        fieldRect.y += height;
                        fieldRect.x += 14f;
                        fieldRect.width -= 14f;
                        SerializedProperty propertyDataList = groupElement.FindPropertyRelative("m_propertyDataList");

                        if (propertyDataList.arraySize == 0)
                        {
                            EditorGUI.HelpBox(fieldRect, "Group is Empty!", MessageType.Warning);
                        }
                        else
                        {
                            SerializedProperty rampCurve = groupElement.FindPropertyRelative("m_rampCurve");
                            EditorGUI.CurveField(fieldRect, rampCurve, Color.yellow, new Rect(0f, 0f, 1f, 1f));
                            fieldRect.y += height;

                            for (int i = 0; i < propertyDataList.arraySize; i++)
                            {
                                SerializedProperty dataElement = propertyDataList.GetArrayElementAtIndex(i);
                                SerializedProperty weatherPropertyOwner = dataElement.FindPropertyRelative("m_weatherPropertyOwner");
                                SerializedProperty name = weatherPropertyOwner.FindPropertyRelative("m_name");
                                SerializedProperty propertyType = weatherPropertyOwner.FindPropertyRelative("m_propertyType");
                                SerializedProperty min = weatherPropertyOwner.FindPropertyRelative("m_minValue");
                                SerializedProperty max = weatherPropertyOwner.FindPropertyRelative("m_maxValue");

                                switch (propertyType.enumValueIndex)
                                {
                                    case 0: // Float
                                        EditorGUI.Slider(fieldRect, dataElement.FindPropertyRelative("m_floatData"), min.floatValue, max.floatValue, i + " - " + name.stringValue);
                                        break;

                                    case 1: // Color
                                        EditorGUI.PropertyField(fieldRect, dataElement.FindPropertyRelative("m_colorData"), new GUIContent(i + " - " + name.stringValue));
                                        break;

                                    case 2: // Curve
                                        EditorGUI.CurveField(fieldRect, dataElement.FindPropertyRelative("m_curveData"), Color.green, new Rect(0.0f, min.floatValue, 24.0f, max.floatValue), new GUIContent(i + " - " + name.stringValue));
                                        break;

                                    case 3: // Gradient
                                        EditorGUI.PropertyField(fieldRect, dataElement.FindPropertyRelative("m_gradientData"), new GUIContent(i + " - " + name.stringValue));
                                        break;

                                    case 4: // Direction
                                    case 5: // Position
                                        EditorGUI.PropertyField(fieldRect, dataElement.FindPropertyRelative("m_vector3Data"), new GUIContent(i + " - " + name.stringValue));
                                        break;
                                }

                                fieldRect.y += height;
                            }
                        }
                    }
                },

                drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, "Custom Properties (Data)", EditorStyles.boldLabel);
                },

                elementHeightCallback = (int index) =>
                {
                    if (m_propertyGroupDataList.arraySize > 0)
                    {
                        float height = 20f;
                        SerializedProperty groupElement = m_propertyGroupDataList.GetArrayElementAtIndex(index);

                        if (groupElement.isExpanded)
                        {
                            SerializedProperty propertyDataList = groupElement.FindPropertyRelative("m_propertyDataList");

                            if (propertyDataList.arraySize == 0)
                            {
                                height += 20f;
                            }
                            else
                            {
                                height += 20f;

                                for (int i = 0; i < propertyDataList.arraySize; i++)
                                {
                                    height += 20f;
                                }
                            }

                            return height;
                        }
                    }

                    return 18f;
                },

                drawElementBackgroundCallback = (rect, index, active, focused) =>
                {
                    //if (active)
                    //    GUI.Box(new Rect(rect.x + 2f, rect.y - 1f, rect.width - 4f, rect.height + 1f), "", "selectionRect");
                }
            };
        }

        public override void OnInspectorGUI()
        {
            // Start custom inspector
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            // Title
            m_controlRect = EditorGUILayout.GetControlRect(GUILayout.Height(38f));
            EditorGUI.LabelField(m_controlRect, "", "", "selectionRect");
            if (iconTexture) GUI.DrawTexture(new Rect(m_controlRect.x + 3f, m_controlRect.y + 3f, 32f, 32f), iconTexture);
            GUI.Label(new Rect(m_controlRect.x + 38f, m_controlRect.y + 3f, m_controlRect.width, m_controlRect.height), "Azure Weather Preset", EditorStyles.whiteLargeLabel);
            GUI.Label(new Rect(m_controlRect.x + 38f, m_controlRect.y + 17f, m_controlRect.width, 22f), "Script Version 1.0.0  |  Asset Version 8.0.4", EditorStyles.whiteMiniLabel);

            EditorGUILayout.PropertyField(m_azureCoreSystem);
            m_reorderableList.DoLayoutList();

            // Update the inspector when it changes
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void UpdateFoldoutGroupNames()
        {
            if (m_target.azureCoreSystem != null)
            {
                for (int i = 0; i < m_target.propertyGroupDataList.Count; i++)
                {
                    m_target.propertyGroupDataList[i].name = m_target.azureCoreSystem.weatherSystem.weatherPropertyGroupList[i].name;
                }
            }
        }

        private void UpdateOwnerProperty()
        {
            if (m_target.azureCoreSystem != null)
            {
                if (m_target.azureCoreSystem.weatherSystem.weatherPropertyGroupList.Count == m_target.propertyGroupDataList.Count)
                {
                    for (int i = 0; i < m_target.propertyGroupDataList.Count; i++)
                    {
                        if (m_target.azureCoreSystem.weatherSystem.weatherPropertyGroupList[i].weatherPropertyList.Count == m_target.propertyGroupDataList[i].propertyDataList.Count)
                        {
                            for (int j = 0; j < m_target.propertyGroupDataList[i].propertyDataList.Count; j++)
                            {
                                m_target.propertyGroupDataList[i].propertyDataList[j].weatherPropertyOwner = m_target.azureCoreSystem.weatherSystem.weatherPropertyGroupList[i].weatherPropertyList[j];
                            }
                        }
                    }
                }
            }
        }
    }
}