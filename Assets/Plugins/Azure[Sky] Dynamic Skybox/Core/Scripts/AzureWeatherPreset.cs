using UnityEditor;
using System.Collections.Generic;

namespace UnityEngine.AzureSky
{
    [ExecuteInEditMode]
    [AddComponentMenu("Azure[Sky] Dynamic Skybox/Azure Weather Preset")]
    public sealed class AzureWeatherPreset : MonoBehaviour
    {
        /// <summary>The reference to the core system this preset belong to.</summary>
        public AzureCoreSystem azureCoreSystem { get => m_azureCoreSystem; set => m_azureCoreSystem = value; }
        [SerializeField] private AzureCoreSystem m_azureCoreSystem;

        /// <summary>The list containing all the property groups data of this preset.</summary>
        public List<AzurePropertyGroupData> propertyGroupDataList { get => m_propertyGroupDataList; set => m_propertyGroupDataList = value; }
        [SerializeField] private List<AzurePropertyGroupData> m_propertyGroupDataList = new List<AzurePropertyGroupData>();

        private void Reset()
        {
            #if UNITY_EDITOR
            Undo.RecordObject(this, "Reset Weather Preset");
            InitPropertyList();
            #endif
        }

        /// <summary>Registering to the events.</summary>
        private void Awake()
        {
            #if UNITY_EDITOR
            AzureNotificationCenterEditor.OnAddWeatherPropertyGroup += OnAddPropertyGroup;
            AzureNotificationCenterEditor.OnAddWeatherProperty += OnAddWeatherProperty;
            AzureNotificationCenterEditor.OnRemoveWeatherPropertyGroup += OnRemoveWeatherPropertyGroup;
            AzureNotificationCenterEditor.OnRemoveWeatherProperty += OnRemoveWeatherProperty;
            AzureNotificationCenterEditor.OnReorderWeatherPropertyGroupList += OnReorderWeatherPropertyGroupList;
            AzureNotificationCenterEditor.OnReorderWeatherPropertyList += OnReorderWeatherPropertyList;
            #endif
        }

        /// <summary>Unregistering to the events.</summary>
        private void OnDestroy()
        {
            #if UNITY_EDITOR
            AzureNotificationCenterEditor.OnAddWeatherPropertyGroup -= OnAddPropertyGroup;
            AzureNotificationCenterEditor.OnAddWeatherProperty -= OnAddWeatherProperty;
            AzureNotificationCenterEditor.OnRemoveWeatherPropertyGroup -= OnRemoveWeatherPropertyGroup;
            AzureNotificationCenterEditor.OnRemoveWeatherProperty -= OnRemoveWeatherProperty;
            AzureNotificationCenterEditor.OnReorderWeatherPropertyGroupList -= OnReorderWeatherPropertyGroupList;
            AzureNotificationCenterEditor.OnReorderWeatherPropertyList -= OnReorderWeatherPropertyList;
            #endif
        }

        /// <summary>Initialize the property list to match with the custom property list from the core system.</summary>
        public void InitPropertyList()
        {
            // If the preset game object is a child of the Azure Core System
            m_azureCoreSystem = GetComponentInParent<AzureCoreSystem>();

            // If the m_azureCoreSystem is still null, get the first game object in the scene using the AzureCoreSystem component
            if (m_azureCoreSystem == null)
            {
                m_azureCoreSystem = FindFirstObjectByType<AzureCoreSystem>();
            }

            // If the m_azureCoreSystem is still null, just give up
            // We can still use the Unity's component copy/paste feature to initialize it manually...
            // from another weather preset already configured in the scene
            if (m_azureCoreSystem == null)
            {
                return;
            }

            if (m_azureCoreSystem)
            {
                if (m_azureCoreSystem.weatherSystem.weatherPropertyGroupList.Count == m_propertyGroupDataList.Count)
                {
                    for (int i = 0; i < m_propertyGroupDataList.Count; i++)
                    {
                        if (m_azureCoreSystem.weatherSystem.weatherPropertyGroupList[i].weatherPropertyList.Count == m_propertyGroupDataList[i].propertyDataList.Count)
                        {
                            for (int j = 0; j < m_propertyGroupDataList[i].propertyDataList.Count; j++)
                            {
                                m_propertyGroupDataList[i].propertyDataList[j].weatherPropertyOwner = m_azureCoreSystem.weatherSystem.weatherPropertyGroupList[i].weatherPropertyList[j];
                            }
                        }
                        else
                        {
                            for (int j = 0; j < m_azureCoreSystem.weatherSystem.weatherPropertyGroupList[i].weatherPropertyList.Count; j++)
                            {
                                if (m_propertyGroupDataList[i].propertyDataList.Count < m_azureCoreSystem.weatherSystem.weatherPropertyGroupList[i].weatherPropertyList.Count)
                                {
                                    AzurePropertyData newPropertyData = new AzurePropertyData();
                                    m_propertyGroupDataList[i].propertyDataList.Add(newPropertyData);
                                    newPropertyData.weatherPropertyOwner = m_azureCoreSystem.weatherSystem.weatherPropertyGroupList[i].weatherPropertyList[j];
                                    InitNewPropertyData(newPropertyData);
                                }
                                else
                                {
                                    m_propertyGroupDataList[i].propertyDataList.RemoveAt(j);
                                }

                                m_propertyGroupDataList[i].propertyDataList[j].weatherPropertyOwner = m_azureCoreSystem.weatherSystem.weatherPropertyGroupList[i].weatherPropertyList[j];
                            }
                        }

                        m_propertyGroupDataList[i].name = m_azureCoreSystem.weatherSystem.weatherPropertyGroupList[i].name;
                    }
                }
                else
                {
                    for (int i = 0; i < m_azureCoreSystem.weatherSystem.weatherPropertyGroupList.Count; i++)
                    {
                        if (m_propertyGroupDataList.Count < m_azureCoreSystem.weatherSystem.weatherPropertyGroupList.Count)
                        {
                            AzurePropertyGroupData newPropertyGroup = new AzurePropertyGroupData();
                            m_propertyGroupDataList.Add(newPropertyGroup);

                            for (int j = 0; j < m_azureCoreSystem.weatherSystem.weatherPropertyGroupList[i].weatherPropertyList.Count; j++)
                            {
                                AzurePropertyData newPropertyData = new AzurePropertyData();
                                newPropertyGroup.propertyDataList.Add(newPropertyData);
                                newPropertyData.weatherPropertyOwner = m_azureCoreSystem.weatherSystem.weatherPropertyGroupList[i].weatherPropertyList[j];
                                InitNewPropertyData(newPropertyData);
                            }
                        }
                        else
                        {
                            m_propertyGroupDataList.RemoveAt(i);
                        }

                        m_propertyGroupDataList[i].name = m_azureCoreSystem.weatherSystem.weatherPropertyGroupList[i].name;
                    }
                }
            }
        }

        //#if UNITY_EDITOR
        //[ContextMenu("CheckOwners()")]
        //private void CheckOwners()
        //{
        //    for (int i = 0; i < m_propertyGroupDataList.Count; i++)
        //    {
        //        for (int j = 0; j < m_propertyGroupDataList[i].propertyDataList.Count; j++)
        //        {
        //            if (m_propertyGroupDataList[i].propertyDataList[j].weatherPropertyOwner != m_azureCoreSystem.weatherSystem.weatherPropertyGroupList[i].weatherPropertyList[j])
        //            {
        //                Debug.Log("The custom property: " + m_propertyGroupDataList[i].propertyDataList[j].weatherPropertyOwner.name + " is missing its owner!");
        //            }
        //
        //            if (m_propertyGroupDataList[i].propertyDataList[j].weatherPropertyOwner == null)
        //            {
        //                Debug.Log("The custom property is missing its owner! Group Index: " + i + " Property Index: " + j);
        //            }
        //        }
        //    }
        //}
        //#endif

        /// <summary>Initialize the new property data according to its owner default values.</summary>
        private void InitNewPropertyData(AzurePropertyData propertyData)
        {
            propertyData.floatData = propertyData.weatherPropertyOwner.defaultFloatValue;
            propertyData.colorData = propertyData.weatherPropertyOwner.defaultColorValue;
            propertyData.curveData = new AnimationCurve();
            propertyData.curveData.CopyFrom(propertyData.weatherPropertyOwner.defaultCurveValue);
            propertyData.gradientData = new Gradient();
            propertyData.gradientData.SetKeys(propertyData.weatherPropertyOwner.defaultGradientValue.colorKeys, propertyData.weatherPropertyOwner.defaultGradientValue.alphaKeys);
            propertyData.vector3Data = propertyData.weatherPropertyOwner.defaultVector3Value;
        }

        #if UNITY_EDITOR
        /// <summary>Add a new element to the weather property group list.</summary>
        private void OnAddPropertyGroup(AzureCoreSystem azureCoreSystem)
        {
            if (m_azureCoreSystem == azureCoreSystem)
            {
                Undo.RecordObject(this, "Add Weather Property Group");
                AzurePropertyGroupData newPropertyGroup = new AzurePropertyGroupData();
                int lastIndex = m_azureCoreSystem.weatherSystem.weatherPropertyGroupList.Count - 1;
                newPropertyGroup.name = m_azureCoreSystem.weatherSystem.weatherPropertyGroupList[lastIndex].name;
                m_propertyGroupDataList.Add(newPropertyGroup);
                //InitPropertyList();
            }
        }

        /// <summary>Add a new element to the weather property list.</summary>
        private void OnAddWeatherProperty(AzureCoreSystem azureCoreSystem, int groupIndex)
        {
            if (m_azureCoreSystem == azureCoreSystem)
            {
                Undo.RecordObject(this, "Add Weather Property");
                AzurePropertyData newPropertyData = new AzurePropertyData();
                int lastIndex = m_azureCoreSystem.weatherSystem.weatherPropertyGroupList[groupIndex].weatherPropertyList.Count - 1;
                newPropertyData.weatherPropertyOwner = m_azureCoreSystem.weatherSystem.weatherPropertyGroupList[groupIndex].weatherPropertyList[lastIndex];
                InitNewPropertyData(newPropertyData);
                m_propertyGroupDataList[groupIndex].propertyDataList.Add(newPropertyData);
                //InitPropertyList();
            }
        }

        /// <summary>Remove an element of the weather property group list.</summary>
        private void OnRemoveWeatherPropertyGroup(AzureCoreSystem azureCoreSystem, int groupIndex)
        {
            if (m_azureCoreSystem == azureCoreSystem)
            {
                Undo.RecordObject(this, "Remove Weather Property Group");
                AzurePropertyGroupData data = m_propertyGroupDataList[groupIndex];
                m_propertyGroupDataList.RemoveAt(groupIndex);
                data = null;
                //InitPropertyList();
            }
        }

        /// <summary>Remove an element from a weather property list.</summary>
        private void OnRemoveWeatherProperty(AzureCoreSystem azureCoreSystem, int groupIndex, int propertyIndex)
        {
            if (m_azureCoreSystem == azureCoreSystem)
            {
                Undo.RecordObject(this, "Remove Weather Property");
                AzurePropertyData data = m_propertyGroupDataList[groupIndex].propertyDataList[propertyIndex];
                m_propertyGroupDataList[groupIndex].propertyDataList.RemoveAt(propertyIndex);
                data = null;
                //InitPropertyList();
            }
        }

        /// <summary>Reorder the weather property group list.</summary>
        private void OnReorderWeatherPropertyGroupList(AzureCoreSystem azureCoreSystem, int oldIndex, int newIndex)
        {
            if (m_azureCoreSystem == azureCoreSystem)
            {
                Undo.RecordObject(this, "Reorder Weather Property Group");
                AzurePropertyGroupData item = m_propertyGroupDataList[oldIndex];
                m_propertyGroupDataList.RemoveAt(oldIndex);
                m_propertyGroupDataList.Insert(newIndex, item);
                //InitPropertyList();
            }
        }

        /// <summary>Reorder the weather property list.</summary>
        private void OnReorderWeatherPropertyList(AzureCoreSystem azureCoreSystem, int groupIndex, int oldIndex, int newIndex)
        {
            if (m_azureCoreSystem == azureCoreSystem)
            {
                Undo.RecordObject(this, "Reorder Weather Property");
                AzurePropertyData item = m_propertyGroupDataList[groupIndex].propertyDataList[oldIndex];
                m_propertyGroupDataList[groupIndex].propertyDataList.RemoveAt(oldIndex);
                m_propertyGroupDataList[groupIndex].propertyDataList.Insert(newIndex, item);
                //InitPropertyList();
            }
        }
        #endif
    }
}