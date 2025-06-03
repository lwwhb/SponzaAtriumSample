using System;
using UnityEngine.Events;

namespace UnityEngine.AzureSky
{
    /// <summary>Base class for setting a custom time event.</summary>
    [Serializable]
    public sealed class AzureCustomEvent
    {
        /// <summary>The minute value of this custom time event.</summary>
        public int minute { get => m_minute; set => m_minute = value; }
        [SerializeField] private int m_minute = 0;

        /// <summary>The hour value of this custom time event.</summary>
        public int hour { get => m_hour; set => m_hour = value; }
        [SerializeField] private int m_hour = 0;

        /// <summary>The day value of this custom time event.</summary>
        public int day { get => m_day; set => m_day = value; }
        [SerializeField] private int m_day = 0;

        /// <summary>The month value of this custom time event.</summary>
        public int month { get => m_month; set => m_month = value; }
        [SerializeField] private int m_month = 0;

        /// <summary>The year value of this custom time event.</summary>
        public int year { get => m_year; set => m_year = value; }
        [SerializeField] private int m_year = 0;

        /// <summary>Stores the hour when the event was executed, used to avoid the event being invoked multiple times in the same hour.</summary>
        public int executedHour { get => m_executedHour; set => m_executedHour = value; }
        private int m_executedHour = -1;

        /// <summary>Is the event already executed on the current hour?</summary>
        public bool isAlreadyExecutedOnThisHour { get => m_isAlreadyExecutedOnThisHour; set => m_isAlreadyExecutedOnThisHour = value; }
        private bool m_isAlreadyExecutedOnThisHour = false;

        /// <summary>Returns the number of listeners attached to this custom event.</summary>
        public int eventListenersCount => m_unityEvent.GetPersistentEventCount();

        /// <summary>The UnityEvent associated to this custom event.</summary>
        public UnityEvent unityEvent { get => m_unityEvent; set => m_unityEvent = value; }
        [SerializeField] private UnityEvent m_unityEvent;

        /// <summary>Invoke the Unity event attached to this custom event.</summary>
        public void Invoke()
        {
            m_unityEvent?.Invoke();
        }

    }
}