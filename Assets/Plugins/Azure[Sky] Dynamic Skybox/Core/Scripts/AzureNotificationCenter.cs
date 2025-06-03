namespace UnityEngine.AzureSky
{
    /// <summary>Class that handles events and global communications in the system.</summary>
    public static class AzureNotificationCenter
    {
        public static class Invoke
        {
            /// <summary>Invokes the OnTimelineChanged event.</summary>
            public static void OnTimelineChangedCallback(AzureTimeSystem timeSystem) { OnTimelineChanged?.Invoke(timeSystem); }

            /// <summary>Invokes the OnMinuteChanged event.</summary>
            public static void OnMinuteChangedCallback(AzureTimeSystem timeSystem) { OnMinuteChanged?.Invoke(timeSystem); }

            /// <summary>Invokes the OnHourChanged event.</summary>
            public static void OnHourChangedCallback(AzureTimeSystem timeSystem) { OnHourChanged?.Invoke(timeSystem); }

            /// <summary>Invokes the OnDayChanged event.</summary>
            public static void OnDayChangedCallback(AzureTimeSystem timeSystem) { OnDayChanged?.Invoke(timeSystem); }

            /// <summary>Invokes the OnMonthChanged event.</summary>
            public static void OnMonthChangedCallback(AzureTimeSystem timeSystem) { OnMonthChanged?.Invoke(timeSystem); }

            /// <summary>Invokes the OnYearChanged event.</summary>
            public static void OnYearChangedCallback(AzureTimeSystem timeSystem) { OnYearChanged?.Invoke(timeSystem); }

            /// <summary>Invokes the OnBeforeOverrideUpdate event.</summary>
            public static void OnBeforeOverrideUpdateCallback(AzureWeatherSystem azureWeatherSystem) { OnBeforeOverrideUpdate?.Invoke(azureWeatherSystem); }

            /// <summary>Invokes the OnAfterOverrideUpdate event.</summary>
            public static void OnAfterOverrideUpdateCallback(AzureWeatherSystem azureWeatherSystem) { OnAfterOverrideUpdate?.Invoke(azureWeatherSystem); }

            /// <summary>Invokes the OnBeforeWeatherSystemUpdate event.</summary>
            public static void OnBeforeWeatherSystemUpdateCallback(AzureWeatherSystem azureWeatherSystem) { OnBeforeWeatherSystemUpdate?.Invoke(azureWeatherSystem); }

            /// <summary>Invokes the OnAfterWeatherSystemUpdate event.</summary>
            public static void OnAfterWeatherSystemUpdateCallback(AzureWeatherSystem azureWeatherSystem) { OnAfterWeatherSystemUpdate?.Invoke(azureWeatherSystem); }

            /// <summary>Invokes the OnWeatherTransitionEnd event.</summary>
            public static void OnWeatherTransitionEndCallback(AzureWeatherSystem azureWeatherSystem) { OnWeatherTransitionEnd?.Invoke(azureWeatherSystem); }

            /// <summary>Invokes the OnVolumetricLightPreRender event.</summary>
            public static void OnVolumetricLightPreRenderCallback(AzureVolumetricLightRenderer renderer) { OnVolumetricLightPreRender?.Invoke(renderer); }
        }

        public delegate void TimeSystemDelegate(AzureTimeSystem timeSystem);
        public static event TimeSystemDelegate OnTimelineChanged, OnMinuteChanged, OnHourChanged, OnDayChanged, OnMonthChanged, OnYearChanged;

        public delegate void WeatherSystemDelegate(AzureWeatherSystem azureWeatherSystem);
        public static event WeatherSystemDelegate OnBeforeOverrideUpdate, OnAfterOverrideUpdate, OnBeforeWeatherSystemUpdate, OnAfterWeatherSystemUpdate, OnWeatherTransitionEnd;

        public delegate void VolumetriLightPreRenderDelegate(AzureVolumetricLightRenderer renderer);
        public static event VolumetriLightPreRenderDelegate OnVolumetricLightPreRender;
    }

    // Editor only
    #if UNITY_EDITOR
    public static class AzureNotificationCenterEditor
    {
        public static class Invoke
        {
            /// <summary>Invokes the OnRequestCalendarUpdate event.</summary>
            public static void RequestCalendarUpdateCallback()
            {
                OnRequestCalendarUpdate?.Invoke();
            }

            /// <summary>Triggers the OnAddWeatherPropertyGroup callback.</summary>
            public static void AddWeatherPropertyGroupCallback(AzureCoreSystem azureCoreSystem)
            {
                OnAddWeatherPropertyGroup?.Invoke(azureCoreSystem);
            }

            /// <summary>Triggers the OnRemoveWeatherPropertyGroup callback.</summary>
            public static void RemoveWeatherPropertyGroupCallback(AzureCoreSystem azureCoreSystem, int index)
            {
                OnRemoveWeatherPropertyGroup?.Invoke(azureCoreSystem, index);
            }

            /// <summary>Triggers the OnReorderWeatherPropertyGroupList callback.</summary>
            public static void ReorderWeatherPropertyGroupCallback(AzureCoreSystem azureCoreSystem, int oldIndex, int newIndex)
            {
                OnReorderWeatherPropertyGroupList?.Invoke(azureCoreSystem, oldIndex, newIndex);
            }

            /// <summary>Triggers the OnAddWeatherProperty callback.</summary>
            public static void AddWeatherPropertyCallback(AzureCoreSystem azureCoreSystem, int groupIndex)
            {
                OnAddWeatherProperty?.Invoke(azureCoreSystem, groupIndex);
            }

            /// <summary>Triggers the OnRemoveWeatherProperty callback.</summary>
            public static void RemoveWeatherPropertyCallback(AzureCoreSystem azureCoreSystem, int groupIndex, int propertyIndex)
            {
                OnRemoveWeatherProperty?.Invoke(azureCoreSystem, groupIndex, propertyIndex);
            }

            /// <summary>Triggers the OnReorderWeatherPropertyList callback.</summary>
            public static void ReorderWeatherPropertyCallback(AzureCoreSystem azureCoreSystem, int groupIndex, int oldIndex, int newIndex)
            {
                OnReorderWeatherPropertyList?.Invoke(azureCoreSystem, groupIndex, oldIndex, newIndex);
            }
        }

        public delegate void AzureEditorDelegate();
        public static event AzureEditorDelegate OnRequestCalendarUpdate;

        public delegate void AddWeatherPropertyGroupDelegate(AzureCoreSystem azureCoreSystem);
        public static event AddWeatherPropertyGroupDelegate OnAddWeatherPropertyGroup;

        public delegate void RemoveWeatherPropertyGroupDelegate(AzureCoreSystem azureCoreSystem, int index);
        public static event RemoveWeatherPropertyGroupDelegate OnRemoveWeatherPropertyGroup;

        public delegate void ReorderWeatherPropertyGroupDelegate(AzureCoreSystem azureCoreSystem, int oldIndex, int newIndex);
        public static event ReorderWeatherPropertyGroupDelegate OnReorderWeatherPropertyGroupList;

        public delegate void AddWeatherPropertyDelegate(AzureCoreSystem azureCoreSystem, int groupIndex);
        public static event AddWeatherPropertyDelegate OnAddWeatherProperty;

        public delegate void RemoveWeatherPropertyDelegate(AzureCoreSystem azureCoreSystem, int groupIndex, int propertyIndex);
        public static event RemoveWeatherPropertyDelegate OnRemoveWeatherProperty;

        public delegate void ReorderWeatherPropertyDelegate(AzureCoreSystem azureCoreSystem, int groupIndex, int oldIndex, int newIndex);
        public static event ReorderWeatherPropertyDelegate OnReorderWeatherPropertyList;

    }
    #endif
}