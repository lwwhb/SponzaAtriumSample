using System;

namespace UnityEngine.AzureSky
{
    [Serializable] public sealed class AzurePropertyData
    {
        /// <summary>The data container for customization the custom property if it is configured to return a float output.</summary>
        public float floatData { get => m_floatData; set => m_floatData = value; }
        [SerializeField] private float m_floatData;

        /// <summary>The data container for customization the custom property if it is configured to return a color output.</summary>
        public Color colorData { get => m_colorData; set => m_colorData = value; }
        [SerializeField] private Color m_colorData;

        /// <summary>The data container for customization the custom property if it is configured to return a dynamic float output based on the timeline.</summary>
        public AnimationCurve curveData { get => m_curveData; set => m_curveData = value; }
        [SerializeField] private AnimationCurve m_curveData;

        /// <summary>The data container for customization the custom property if it is configured to return a dynamic color output based on the timeline.</summary>
        public Gradient gradientData { get => m_gradientData; set => m_gradientData = value; }
        [SerializeField] private Gradient m_gradientData;

        /// <summary>The data container for customization the custom property if it is configured to return a direction or a position output.</summary>
        public Vector3 vector3Data { get => m_vector3Data; set => m_vector3Data = value; }
        [SerializeField] private Vector3 m_vector3Data;

        /// <summary>The reference to the weather property from the core system this data belongs to.</summary>
        public AzureWeatherProperty weatherPropertyOwner { get => m_weatherPropertyOwner; set => m_weatherPropertyOwner = value; }
        [SerializeField] private AzureWeatherProperty m_weatherPropertyOwner;
    }
}