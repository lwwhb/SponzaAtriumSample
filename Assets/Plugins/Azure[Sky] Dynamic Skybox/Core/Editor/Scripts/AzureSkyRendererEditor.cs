using UnityEngine;
using UnityEngine.AzureSky;

namespace UnityEditor.AzureSky
{
    [CustomEditor(typeof(AzureSkyRenderer))]
    public sealed class AzureSkyRendererEditor : Editor
    {
        // Target
        private AzureSkyRenderer m_target;

        // Logo
        public Texture2D iconTexture;
        private Rect m_controlRect, m_foldoutRect;

        // References tab
        private SerializedProperty m_showReferencesTab;
        private SerializedProperty m_showScatteringTab;
        private SerializedProperty m_showOutterSpaceTab;
        private SerializedProperty m_showFogScatteringTab;
        private SerializedProperty m_showDynamicCloudTab;
        private SerializedProperty m_showOptionsTab;
        private SerializedProperty m_sunTransform;
        private SerializedProperty m_moonTransform;
        private SerializedProperty m_starfieldTransform;
        private SerializedProperty m_skyMaterial;
        private SerializedProperty m_fogMaterial;
        private SerializedProperty m_sunTexture;
        private SerializedProperty m_moonTexture;
        private SerializedProperty m_starfieldTexture;
        private SerializedProperty m_constellationTexture;
        private SerializedProperty m_dynamicCloudTexture;
        private SerializedProperty m_emptySkyShader;
        private SerializedProperty m_dynamicCloudShader;

        // Scattering tab
        private SerializedProperty m_wavelength;
        private SerializedProperty m_molecularDensity;
        private SerializedProperty m_kr;
        private SerializedProperty m_km;
        private SerializedProperty m_rayleigh;
        private SerializedProperty m_mie;
        private SerializedProperty m_mieDirectionalityFactor;
        private SerializedProperty m_scattering;
        private SerializedProperty m_skyLuminance;
        private SerializedProperty m_exposure;
        private SerializedProperty m_rayleighColor;
        private SerializedProperty m_mieColor;

        // Outer Space tab
        private SerializedProperty m_sunSize;
        private SerializedProperty m_sunOpacity;
        private SerializedProperty m_sunColor;
        private SerializedProperty m_moonSize;
        private SerializedProperty m_moonOpacity;
        private SerializedProperty m_moonColor;
        private SerializedProperty m_moonRotationOffset;
        private SerializedProperty m_starsIntensity;
        private SerializedProperty m_milkyWayIntensity;
        private SerializedProperty m_starfieldColor;
        private SerializedProperty m_skyExtinction;
        private SerializedProperty m_constellationIntensity;
        private SerializedProperty m_constellationColor;
        private SerializedProperty m_starfieldRotationOffset;

        // Fog Scattering tab
        private SerializedProperty m_mieDistance;
        private SerializedProperty m_globalFogDistance;
        private SerializedProperty m_globalFogSmoothStep;
        private SerializedProperty m_globalFogDensity;
        private SerializedProperty m_heightFogDistance;
        private SerializedProperty m_heightFogSmoothStep;
        private SerializedProperty m_heightFogDensity;
        private SerializedProperty m_heightFogStartAltitude;
        private SerializedProperty m_heightFogEndAltitude;
        private SerializedProperty m_fogBluishDistance;
        private SerializedProperty m_fogBluishIntensity;
        private SerializedProperty m_heightFogScatteringMultiplier;

        // Dynamic Clouds tab
        private SerializedProperty m_dynamicCloudAltitude;
        private SerializedProperty m_dynamicCloudDirection;
        private SerializedProperty m_dynamicCloudSpeed;
        private SerializedProperty m_dynamicCloudDensity;
        private SerializedProperty m_dynamicCloudColor1;
        private SerializedProperty m_dynamicCloudColor2;

        // Options tab
        private SerializedProperty m_updateMode;
        private SerializedProperty m_cloudMode;

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= m_target.UpdateCloudMode;
        }

        private void OnEnable()
        {
            // Get target
            m_target = (AzureSkyRenderer) target;
            Undo.undoRedoPerformed += m_target.UpdateCloudMode;

            // Initializations
            m_target.UpdateCloudMode();
            m_target.InitializeSkySystem();
            m_target.UpdateSkySystem();

            // References tab
            m_showReferencesTab = serializedObject.FindProperty("m_showReferencesTab");
            m_showScatteringTab = serializedObject.FindProperty("m_showScatteringTab");
            m_showOutterSpaceTab = serializedObject.FindProperty("m_showOutterSpaceTab");
            m_showFogScatteringTab = serializedObject.FindProperty("m_showFogScatteringTab");
            m_showDynamicCloudTab = serializedObject.FindProperty("m_showDynamicCloudTab");
            m_showOptionsTab = serializedObject.FindProperty("m_showOptionsTab");
            m_sunTransform = serializedObject.FindProperty("m_sunTransform");
            m_moonTransform = serializedObject.FindProperty("m_moonTransform");
            m_starfieldTransform = serializedObject.FindProperty("m_starfieldTransform");
            m_skyMaterial = serializedObject.FindProperty("m_skyMaterial");
            m_fogMaterial = serializedObject.FindProperty("m_fogMaterial");
            m_sunTexture = serializedObject.FindProperty("m_sunTexture");
            m_moonTexture = serializedObject.FindProperty("m_moonTexture");
            m_starfieldTexture = serializedObject.FindProperty("m_starfieldTexture");
            m_constellationTexture = serializedObject.FindProperty("m_constellationTexture");
            m_dynamicCloudTexture = serializedObject.FindProperty("m_dynamicCloudTexture");
            m_emptySkyShader = serializedObject.FindProperty("m_emptySkyShader");
            m_dynamicCloudShader = serializedObject.FindProperty("m_dynamicCloudShader");

            // Scattering tab
            m_wavelength = serializedObject.FindProperty("m_wavelength");
            m_molecularDensity = serializedObject.FindProperty("m_molecularDensity");
            m_kr = serializedObject.FindProperty("m_kr");
            m_km = serializedObject.FindProperty("m_km");
            m_rayleigh = serializedObject.FindProperty("m_rayleigh");
            m_mie = serializedObject.FindProperty("m_mie");
            m_mieDirectionalityFactor = serializedObject.FindProperty("m_mieDirectionalityFactor");
            m_scattering = serializedObject.FindProperty("m_scattering");
            m_skyLuminance = serializedObject.FindProperty("m_skyLuminance");
            m_exposure = serializedObject.FindProperty("m_exposure");
            m_rayleighColor = serializedObject.FindProperty("m_rayleighColor");
            m_mieColor = serializedObject.FindProperty("m_mieColor");

            // Outer Space tab
            m_sunSize = serializedObject.FindProperty("m_sunSize");
            m_sunOpacity = serializedObject.FindProperty("m_sunOpacity");
            m_sunColor = serializedObject.FindProperty("m_sunColor");
            m_moonSize = serializedObject.FindProperty("m_moonSize");
            m_moonOpacity = serializedObject.FindProperty("m_moonOpacity");
            m_moonColor = serializedObject.FindProperty("m_moonColor");
            m_moonRotationOffset = serializedObject.FindProperty("m_moonRotationOffset");
            m_starsIntensity = serializedObject.FindProperty("m_starsIntensity");
            m_milkyWayIntensity = serializedObject.FindProperty("m_milkyWayIntensity");
            m_starfieldColor = serializedObject.FindProperty("m_starfieldColor");
            m_skyExtinction = serializedObject.FindProperty("m_skyExtinction");
            m_constellationIntensity = serializedObject.FindProperty("m_constellationIntensity");
            m_constellationColor = serializedObject.FindProperty("m_constellationColor");
            m_starfieldRotationOffset = serializedObject.FindProperty("m_starfieldRotationOffset");

            // Fog Scattering tab
            m_mieDistance = serializedObject.FindProperty("m_mieDistance");
            m_globalFogDistance = serializedObject.FindProperty("m_globalFogDistance");
            m_globalFogSmoothStep = serializedObject.FindProperty("m_globalFogSmoothStep");
            m_globalFogDensity = serializedObject.FindProperty("m_globalFogDensity");
            m_heightFogDistance = serializedObject.FindProperty("m_heightFogDistance");
            m_heightFogSmoothStep = serializedObject.FindProperty("m_heightFogSmoothStep");
            m_heightFogDensity = serializedObject.FindProperty("m_heightFogDensity");
            m_heightFogStartAltitude = serializedObject.FindProperty("m_heightFogStartAltitude");
            m_heightFogEndAltitude = serializedObject.FindProperty("m_heightFogEndAltitude");
            m_fogBluishDistance = serializedObject.FindProperty("m_fogBluishDistance");
            m_fogBluishIntensity = serializedObject.FindProperty("m_fogBluishIntensity");
            m_heightFogScatteringMultiplier = serializedObject.FindProperty("m_heightFogScatteringMultiplier");

            // Dynamic Clouds tab
            m_dynamicCloudAltitude = serializedObject.FindProperty("m_dynamicCloudAltitude");
            m_dynamicCloudDirection = serializedObject.FindProperty("m_dynamicCloudDirection");
            m_dynamicCloudSpeed = serializedObject.FindProperty("m_dynamicCloudSpeed");
            m_dynamicCloudDensity = serializedObject.FindProperty("m_dynamicCloudDensity");
            m_dynamicCloudColor1 = serializedObject.FindProperty("m_dynamicCloudColor1");
            m_dynamicCloudColor2 = serializedObject.FindProperty("m_dynamicCloudColor2");

            // Options tab
            m_updateMode = serializedObject.FindProperty("m_updateMode");
            m_cloudMode = serializedObject.FindProperty("m_cloudMode");
        }

        public override void OnInspectorGUI()
        {
            // Start custom inspector
            EditorGUI.BeginChangeCheck();

            // Title
            m_controlRect = EditorGUILayout.GetControlRect(GUILayout.Height(38f));
            EditorGUI.LabelField(new Rect(m_controlRect.x - 14f, m_controlRect.y, m_controlRect.width + 14f, m_controlRect.height), "", "", "selectionRect");
            if (iconTexture) GUI.DrawTexture(new Rect(m_controlRect.x + 3f, m_controlRect.y + 3f, 32f, 32f), iconTexture);
            GUI.Label(new Rect(m_controlRect.x + 38f, m_controlRect.y + 3f, m_controlRect.width, 22f), "Azure Sky Renderer", EditorStyles.whiteLargeLabel);
            GUI.Label(new Rect(m_controlRect.x + 38f, m_controlRect.y + 17f, m_controlRect.width, 22f), "Script Version 1.0.0  |  Asset Version 8.0.4", EditorStyles.whiteMiniLabel);

            // Begin the references tab
            m_controlRect = EditorGUILayout.GetControlRect();
            m_foldoutRect = m_controlRect;
            m_controlRect.x -= 14f;
            m_controlRect.width += 14f;

            if (GUI.Button(m_controlRect, GUIContent.none, "HelpBox")) { m_showReferencesTab.isExpanded = !m_showReferencesTab.isExpanded; }
            EditorGUI.Foldout(m_foldoutRect, m_showReferencesTab.isExpanded, "References");
            if (m_showReferencesTab.isExpanded)
            {
                EditorGUILayout.PropertyField(m_sunTransform, new GUIContent("Sun Transform", "The transform used to represent the position of the sun in the sky."));
                EditorGUILayout.PropertyField(m_moonTransform, new GUIContent("Moon Transform", "The transform used to represent the position of the moon in the sky."));
                EditorGUILayout.PropertyField(m_starfieldTransform, new GUIContent("Starfield Transform", "The transform used to represent the position of the starfield in the sky."));
                EditorGUILayout.PropertyField(m_skyMaterial, new GUIContent("Sky Material", "The material that renders the sky."));
                EditorGUILayout.PropertyField(m_fogMaterial, new GUIContent("Fog Material", "The material that renders the fog scattering effect."));
                EditorGUILayout.PropertyField(m_sunTexture, new GUIContent("Sun Texture", "The cubemap texture used to render the sun sphere."));
                EditorGUILayout.PropertyField(m_moonTexture, new GUIContent("Moon Texture", "The cubemap texture used to render the moon sphere."));
                EditorGUILayout.PropertyField(m_starfieldTexture, new GUIContent("Starfield Texture", "The cubemap texture used to render the regular stars and the Milky Way."));
                EditorGUILayout.PropertyField(m_constellationTexture, new GUIContent("Constellation Texture", "The cubemap texture used to render the sky constellation."));
                EditorGUILayout.PropertyField(m_dynamicCloudTexture, new GUIContent("Dynamic Cloud Texture", "The 2D texture used to render the dynamic clouds."));
                EditorGUILayout.PropertyField(m_emptySkyShader, new GUIContent("Empty Sky Shader", "The shader used to render only the sky without clouds."));
                EditorGUILayout.PropertyField(m_dynamicCloudShader, new GUIContent("Dynamic Cloud Shader", "The shader used to render sky with dynamic clouds."));
                EditorGUILayout.Space();
            }

            // Begin the scattering tab
            m_controlRect = EditorGUILayout.GetControlRect();
            m_foldoutRect = m_controlRect;
            m_controlRect.x -= 14f;
            m_controlRect.width += 14f;

            if (GUI.Button(m_controlRect, GUIContent.none, "HelpBox")) { m_showScatteringTab.isExpanded = !m_showScatteringTab.isExpanded; }
            EditorGUI.Foldout(m_foldoutRect, m_showScatteringTab.isExpanded, "Scattering");
            if (m_showScatteringTab.isExpanded)
            {
                EditorGUILayout.PropertyField(m_wavelength, new GUIContent("Wavelength", "The wavelength of the visible light."));
                EditorGUILayout.PropertyField(m_molecularDensity, new GUIContent("Molecular Density", "The molecular density of the air."));
                EditorGUILayout.PropertyField(m_kr, new GUIContent("Kr", "The rayleigh altitude in meters."));
                EditorGUILayout.PropertyField(m_km, new GUIContent("Km", "The mie altitude in meters."));
                EditorGUILayout.PropertyField(m_rayleigh, new GUIContent("Rayleigh", "The rayleigh scattering multiplier."));
                EditorGUILayout.PropertyField(m_mie, new GUIContent("Mie", "The mie scattering multiplier."));
                EditorGUILayout.PropertyField(m_mieDirectionalityFactor, new GUIContent("Mie Directionality Factor", "The mie directionality factor."));
                EditorGUILayout.PropertyField(m_scattering, new GUIContent("Scattering", "The scattering intensity multiplier."));
                EditorGUILayout.PropertyField(m_skyLuminance, new GUIContent("Sky Luminance", "The luminance of the sky when there is no sun or moon in the sky."));
                EditorGUILayout.PropertyField(m_exposure, new GUIContent("Exposure", "The exposure of the internal sky shader tonemapping."));
                EditorGUILayout.PropertyField(m_rayleighColor, new GUIContent("Rayleigh Color", "The rayleigh color multiplier."));
                EditorGUILayout.PropertyField(m_mieColor, new GUIContent("Mie Color", "The mie color multiplier."));
                EditorGUILayout.Space();
            }

            // Begin the outer space tab
            m_controlRect = EditorGUILayout.GetControlRect();
            m_foldoutRect = m_controlRect;
            m_controlRect.x -= 14f;
            m_controlRect.width += 14f;

            if (GUI.Button(m_controlRect, GUIContent.none, "HelpBox")) { m_showOutterSpaceTab.isExpanded = !m_showOutterSpaceTab.isExpanded; }
            EditorGUI.Foldout(m_foldoutRect, m_showOutterSpaceTab.isExpanded, "Outer Space");
            if (m_showOutterSpaceTab.isExpanded)
            {
                EditorGUILayout.PropertyField(m_sunSize, new GUIContent("Sun Size", "The size of the Sun sphere in the sky."));
                EditorGUILayout.PropertyField(m_sunOpacity, new GUIContent("Sun Opacity", "The opacity of the Sun sphere."));
                EditorGUILayout.PropertyField(m_sunColor, new GUIContent("Sun Color", "The color multiplier of the Sun sphere."));
                EditorGUILayout.PropertyField(m_moonSize, new GUIContent("Moon Size", "The size of the Moon sphere in the sky."));
                EditorGUILayout.PropertyField(m_moonOpacity, new GUIContent("Moon Opacity", "The opacity of the Moon sphere."));
                EditorGUILayout.PropertyField(m_moonColor, new GUIContent("Moon Color", "The color multiplier of the Moon sphere."));
                EditorGUILayout.PropertyField(m_moonRotationOffset, new GUIContent("Moon Rotation Offset", "The rotation offset to adjust the moon cubemap texture in its sphere."));
                EditorGUILayout.PropertyField(m_starsIntensity, new GUIContent("Stars Intensity", "The intensity of the regular stars."));
                EditorGUILayout.PropertyField(m_milkyWayIntensity, new GUIContent("Milky Way Intensity", "The intensity of the Milky Way."));
                EditorGUILayout.PropertyField(m_starfieldColor, new GUIContent("Starfield Color", "The color multiplier of the entire starfield."));
                EditorGUILayout.PropertyField(m_skyExtinction, new GUIContent("Sky Extinction", "The extinction of the light coming from the outer space, caused by the atmosphere density."));
                EditorGUILayout.PropertyField(m_constellationIntensity, new GUIContent("Constellation Intensity", "The intensity of the stars constellation."));
                EditorGUILayout.PropertyField(m_constellationColor, new GUIContent("Constellation Color", "The color of the sky constellation."));
                EditorGUILayout.PropertyField(m_starfieldRotationOffset, new GUIContent("Starfield Rotation Offset", "The rotation offset to adjust the starfield cubemap texture in the sky sphere."));
                EditorGUILayout.Space();
            }

            // Begin the fog scattering tab
            m_controlRect = EditorGUILayout.GetControlRect();
            m_foldoutRect = m_controlRect;
            m_controlRect.x -= 14f;
            m_controlRect.width += 14f;

            if (GUI.Button(m_controlRect, GUIContent.none, "HelpBox")) { m_showFogScatteringTab.isExpanded = !m_showFogScatteringTab.isExpanded; }
            EditorGUI.Foldout(m_foldoutRect, m_showFogScatteringTab.isExpanded, "Fog Scattering");
            if (m_showFogScatteringTab.isExpanded)
            {
                EditorGUILayout.PropertyField(m_mieDistance, new GUIContent("Mie Distance", "The distance of the mie bright influence in the fog scattering effect."));
                EditorGUILayout.PropertyField(m_globalFogDistance, new GUIContent("Global Fog Distance", "The distance of the global fog scattering effect."));
                EditorGUILayout.PropertyField(m_globalFogSmoothStep, new GUIContent("Global Fog Smooth Step", "The smooth step transition from where there is no global fog in the scene to where is completely foggy."));
                EditorGUILayout.PropertyField(m_globalFogDensity, new GUIContent("Global Fog Density", "The density of the global fog scattering effect."));
                EditorGUILayout.PropertyField(m_heightFogDistance, new GUIContent("Height Fog Distance", "The distance of the height fog scattering effect."));
                EditorGUILayout.PropertyField(m_heightFogSmoothStep, new GUIContent("Height Fog Smooth Step", "The smooth step transition from where there is no height fog in the scene to where is completely foggy."));
                EditorGUILayout.PropertyField(m_heightFogDensity, new GUIContent("Height Fog Density", "The density of the height fog scattering effect."));
                EditorGUILayout.PropertyField(m_heightFogStartAltitude, new GUIContent("Height Fog Start Altitude", "The height altitude where the height fog scattering effect should start."));
                EditorGUILayout.PropertyField(m_heightFogEndAltitude, new GUIContent("Height Fog End Altitude", "The height altitude where the height fog scattering effect should end."));
                EditorGUILayout.PropertyField(m_fogBluishDistance, new GUIContent("Fog Bluish Distance", "The distance of the bluish color effect of the fog at distance."));
                EditorGUILayout.PropertyField(m_fogBluishIntensity, new GUIContent("Fog Bluish Intensity", "The intensity of the bluish color effect of the fog at distance."));
                EditorGUILayout.PropertyField(m_heightFogScatteringMultiplier, new GUIContent("Height Fog Scattering Multiplier", "The scattering multiplier based on the height fog."));
                EditorGUILayout.Space();
            }

            // Begin the dynamic cloud tab
            m_controlRect = EditorGUILayout.GetControlRect();
            m_foldoutRect = m_controlRect;
            m_controlRect.x -= 14f;
            m_controlRect.width += 14f;

            if (GUI.Button(m_controlRect, GUIContent.none, "HelpBox")) { m_showDynamicCloudTab.isExpanded = !m_showDynamicCloudTab.isExpanded; }
            EditorGUI.Foldout(m_foldoutRect, m_showDynamicCloudTab.isExpanded, "Dynamic Cloud");
            if (m_showDynamicCloudTab.isExpanded)
            {
                EditorGUILayout.PropertyField(m_dynamicCloudDensity);
                EditorGUILayout.PropertyField(m_dynamicCloudAltitude);
                EditorGUILayout.PropertyField(m_dynamicCloudSpeed);
                EditorGUILayout.PropertyField(m_dynamicCloudDirection);
                EditorGUILayout.PropertyField(m_dynamicCloudColor1);
                EditorGUILayout.PropertyField(m_dynamicCloudColor2);
                EditorGUILayout.Space();
            }

            // Begin the options tab
            m_controlRect = EditorGUILayout.GetControlRect();
            m_foldoutRect = m_controlRect;
            m_controlRect.x -= 14f;
            m_controlRect.width += 14f;

            if (GUI.Button(m_controlRect, GUIContent.none, "HelpBox")) { m_showOptionsTab.isExpanded = !m_showOptionsTab.isExpanded; }
            EditorGUI.Foldout(m_foldoutRect, m_showOptionsTab.isExpanded, "Options");
            if (m_showOptionsTab.isExpanded)
            {
                EditorGUILayout.PropertyField(m_updateMode);
                EditorGUILayout.PropertyField(m_cloudMode);
                EditorGUILayout.Space();
            }

            // Update the inspector when there is a change
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                m_target.UpdateCloudMode();
                m_target.InitializeSkySystem();
                m_target.UpdateSkySystem();
            }
        }
    }
}