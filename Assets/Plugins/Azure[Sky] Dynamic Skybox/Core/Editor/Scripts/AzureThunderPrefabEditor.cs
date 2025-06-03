using UnityEngine;
using UnityEngine.AzureSky;

namespace UnityEditor.AzureSky
{
    [CustomEditor(typeof(AzureThunderPrefab))]
    public class AzureThunderPrefabEditor : Editor
    {
        public Texture2D iconTexture;
        private AzureThunderPrefab m_target;
        private Rect m_controlRect;

        // Serialized properties
        private SerializedProperty m_lightFrequency;
        private SerializedProperty m_audioDelay;

        private void OnEnable()
        {
            // Get target
            m_target = target as AzureThunderPrefab;

            // Get serialized properties
            m_lightFrequency = serializedObject.FindProperty("m_lightFrequency");
            m_audioDelay = serializedObject.FindProperty("m_audioDelay");
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
            GUI.Label(new Rect(m_controlRect.x + 38f, m_controlRect.y + 3f, m_controlRect.width, 22f), "Azure Thunder Prefab", EditorStyles.whiteLargeLabel);
            GUI.Label(new Rect(m_controlRect.x + 38f, m_controlRect.y + 17f, m_controlRect.width, 22f), "Script Version 1.0.0  |  Asset Version 8.0.4", EditorStyles.whiteMiniLabel);

            // Property fields
            EditorGUILayout.CurveField(m_lightFrequency, Color.green, new Rect(0.0f, 0.0f, 1.0f, 1.0f), new GUIContent("Light Frequency", "The lightning frequency that affect the clouds and scene while this thunder prefab is playing."));
            EditorGUILayout.PropertyField(m_audioDelay, new GUIContent("Audio Delay", "The delay to play the thunder audio clip after this prefab instantiation."));

            // Update the inspector when it has changed
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}