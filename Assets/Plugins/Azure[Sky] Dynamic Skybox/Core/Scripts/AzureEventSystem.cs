using System;
using UnityEngine.Events;
using System.Collections.Generic;

namespace UnityEngine.AzureSky
{
    [Serializable]
    public sealed class AzureEventSystem
    {
        /// <summary>The event triggered when the minute change in the Time System component.</summary>
        public UnityEvent onMinuteChangeEvent => m_onMinuteChangeEvent;
        [SerializeField] private UnityEvent m_onMinuteChangeEvent;

        /// <summary>The event triggered when the hour change in the Time System component.</summary>
        public UnityEvent onHourChangeEvent => m_onHourChangeEvent;
        [SerializeField] private UnityEvent m_onHourChangeEvent;

        /// <summary>The event triggered when the day change in the Time System component.</summary>
        public UnityEvent onDayChangeEvent => m_onDayChangeEvent;
        [SerializeField] private UnityEvent m_onDayChangeEvent;

        /// <summary>The event triggered when the month change in the Time System component.</summary>
        public UnityEvent onMonthChangeEvent => m_onMonthChangeEvent;
        [SerializeField] private UnityEvent m_onMonthChangeEvent;

        /// <summary>The event triggered when the year change in the Time System component.</summary>
        public UnityEvent onYearChangeEvent => m_onYearChangeEvent;
        [SerializeField] private UnityEvent m_onYearChangeEvent;

        /// <summary>The interval time the custom event should be scanned.</summary>
        public AzureCustomEventUpdateMode customEventScanMode { get => m_customEventScanMode; set => m_customEventScanMode = value; }
        [SerializeField] private AzureCustomEventUpdateMode m_customEventScanMode = AzureCustomEventUpdateMode.ByMinute;

        /// <summary>The custom event list.</summary>
        public List<AzureCustomEvent> customEventList => m_customEventList;
        [SerializeField] private List<AzureCustomEvent> m_customEventList = new List<AzureCustomEvent>();

        /// <summary>Register the events when the GameObject is enabled.</summary>
        public void RegisterEvents()
        {
            AzureNotificationCenter.OnMinuteChanged += OnMinuteChange;
            AzureNotificationCenter.OnHourChanged += OnHourChange;
            AzureNotificationCenter.OnDayChanged += OnDayChange;
            AzureNotificationCenter.OnMonthChanged += OnMonthChange;
            AzureNotificationCenter.OnYearChanged += OnYearChange;
        }

        /// <summary>Register the events when the GameObject is disable.</summary>
        public void UnregisterEvents()
        {
            AzureNotificationCenter.OnMinuteChanged -= OnMinuteChange;
            AzureNotificationCenter.OnHourChanged -= OnHourChange;
            AzureNotificationCenter.OnDayChanged -= OnDayChange;
            AzureNotificationCenter.OnMonthChanged -= OnMonthChange;
            AzureNotificationCenter.OnYearChanged -= OnYearChange;
        }

        /// <summary>Triggers the UnityEvent linked to the OnMinuteChange event from the TimeSystem component.</summary>
        private void OnMinuteChange(AzureTimeSystem timeSystem)
        {
            m_onMinuteChangeEvent?.Invoke();

            if (m_customEventScanMode == AzureCustomEventUpdateMode.ByMinute)
            {
                ScanCustomEventList(timeSystem);
            }
        }

        /// <summary>Triggers the UnityEvent linked to the OnHourChange event from the TimeSystem component.</summary>
        private void OnHourChange(AzureTimeSystem timeSystem)
        {
            m_onHourChangeEvent?.Invoke();

            if (m_customEventScanMode == AzureCustomEventUpdateMode.ByHour)
            {
                ScanCustomEventList(timeSystem);
            }
        }

        /// <summary>Triggers the UnityEvent linked to the OnDayChange event from the TimeSystem component.</summary>
        private void OnDayChange(AzureTimeSystem timeSystem)
        {
            m_onDayChangeEvent?.Invoke();
        }

        /// <summary>Triggers the UnityEvent linked to the OnMonthChange event from the TimeSystem component.</summary>
        private void OnMonthChange(AzureTimeSystem timeSystem)
        {
            m_onMonthChangeEvent?.Invoke();
        }

        /// <summary>Triggers the UnityEvent linked to the OnYearChange event from the TimeSystem component.</summary>
        private void OnYearChange(AzureTimeSystem timeSystem)
        {
            m_onYearChangeEvent?.Invoke();
        }

        /// <summary>Scans the custom event list and perform the event that match with the current date and time from the TymeSystem component.</summary>
        private void ScanCustomEventList(AzureTimeSystem timeSystem)
        {
            for (int i = 0; i < m_customEventList.Count; i++)
            {
                if (m_customEventList[i].eventListenersCount <= 0)
                    continue;

                if (m_customEventList[i].year != timeSystem.year && m_customEventList[i].year != -1)
                    continue;

                if (m_customEventList[i].month != timeSystem.month && m_customEventList[i].month != -1)
                    continue;

                if (m_customEventList[i].day != timeSystem.day && m_customEventList[i].day != -1)
                    continue;

                if (timeSystem.hour != m_customEventList[i].executedHour) m_customEventList[i].isAlreadyExecutedOnThisHour = false;
                if (m_customEventList[i].hour != timeSystem.hour && m_customEventList[i].hour != -1)
                    continue;

                if (m_customEventList[i].minute == -1)
                {
                    m_customEventList[i].Invoke();
                }
                else
                {
                    if (!m_customEventList[i].isAlreadyExecutedOnThisHour)
                    {
                        if (timeSystem.minute >= m_customEventList[i].minute)
                        {
                            m_customEventList[i].executedHour = timeSystem.hour;
                            m_customEventList[i].isAlreadyExecutedOnThisHour = true;
                            m_customEventList[i].Invoke();
                        }
                    }
                }
            }
        }
    }
}