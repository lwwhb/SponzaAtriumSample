using System;
using System.Reflection;

namespace UnityEngine.AzureSky
{
    [Serializable] public sealed class AzureWeatherProperty
    {
        /// <summary>The name of this weather property.</summary>
        public string name { get => m_name; set => m_name = value; }
        [SerializeField] private string m_name = "MyPropertyName";

        /// <summary>The type in which this weather property will be displayed in weather profiles for customization.</summary>
        public AzureWeatherPropertyType propertyType { get => m_propertyType; set => m_propertyType = value; }
        [SerializeField] private AzureWeatherPropertyType m_propertyType = AzureWeatherPropertyType.Float;

        /// <summary>The min value allowed in case the weather property is a float.</summary>
        public float minValue { get => m_minValue; set => m_minValue = value; }
        [SerializeField] private float m_minValue = 0.0f;

        /// <summary>The max value allowed in case the weather property is a float.</summary>
        public float maxValue { get => m_maxValue; set => m_maxValue = value; }
        [SerializeField] private float m_maxValue = 1.0f;

        /// <summary>The default float value used when a new weather preset is created.</summary>
        public float defaultFloatValue { get => m_defaultFloatValue; set => m_defaultFloatValue = value; }
        [SerializeField] private float m_defaultFloatValue = 0.0f;

        /// <summary>The default color value used when a new weather preset is created.</summary>
        public Color defaultColorValue { get => m_defaultColorValue; set => m_defaultColorValue = value; }
        [SerializeField] private Color m_defaultColorValue = Color.white;

        /// <summary>The default curve value used when a new weather preset is created.</summary>
        public AnimationCurve defaultCurveValue { get => m_defaultCurveValue; set => m_defaultCurveValue = value; }
        [SerializeField] private AnimationCurve m_defaultCurveValue = AnimationCurve.Linear(0f, 0.5f, 24f, 0.5f);

        /// <summary>The default gradient value used when a new weather preset is created.</summary>
        public Gradient defaultGradientValue { get => m_defaultGradientValue; set => m_defaultGradientValue = value; }
        [SerializeField] private Gradient m_defaultGradientValue = new Gradient();

        /// <summary>The default vector value used when a new weather preset is created.</summary>
        public Vector3 defaultVector3Value { get => m_defaultVector3Value; set => m_defaultVector3Value = value; }
        [SerializeField] private Vector3 m_defaultVector3Value = Vector3.zero;

        /// <summary>Enables or disables the override feature of this weather property.</summary>
        public AzureWeatherPropertyOverrideMode overrideMode { get => m_overrideMode; set => m_overrideMode = value; }
        [SerializeField] private AzureWeatherPropertyOverrideMode m_overrideMode = AzureWeatherPropertyOverrideMode.Off;

        /// <summary>The type of the target this weather property is going to overwrite.</summary>
        public AzureWeatherPropertyTargetType targetType { get => m_targetType; set => m_targetType = value; }
        [SerializeField] private AzureWeatherPropertyTargetType m_targetType = AzureWeatherPropertyTargetType.Property;

        /// <summary>The global target type to access and get the global target property that will be overridden.</summary>
        public Type targetGlobalType { get => m_targetGlobalType; set => m_targetGlobalType = value; }
        [SerializeField] private Type m_targetGlobalType;

        /// <summary>The target game object to access and get the target component.</summary>
        public GameObject targetObject { get => m_targetObject; set => m_targetObject = value; }
        [SerializeField] private GameObject m_targetObject;

        /// <summary>The target component to access and get the target property.</summary>
        public Component targetComponent { get => m_targetComponent; set => m_targetComponent = value; }
        [SerializeField] private Component m_targetComponent;

        /// <summary>The target material to access and set the target property.</summary>
        public Material targetMaterial { get => m_targetMaterial; set => m_targetMaterial = value; }
        [SerializeField] private Material m_targetMaterial;

        /// <summary>Stores the name used to get the target component.</summary>
        public string targetComponentName { get => m_targetComponentName; set => m_targetComponentName = value; }
        [SerializeField] private string m_targetComponentName;

        /// <summary>Stores the name used to get the target property.</summary>
        public string targetPropertyName { get => m_targetPropertyName; set => m_targetPropertyName = value; }
        [SerializeField] private string m_targetPropertyName;

        /// <summary>The field info that stores the target property field.</summary>
        public FieldInfo fieldInfo { get => m_fieldInfo; set => m_fieldInfo = value; }
        private FieldInfo m_fieldInfo;

        /// <summary>The property info that stores the target property.</summary>
        public PropertyInfo propertyInfo { get => m_propertyInfo; set => m_propertyInfo = value; }
        private PropertyInfo m_propertyInfo;

        /// <summary>The float output of this weather property after performing the blend transition.</summary>
        public float floatOutput { get => m_floatOutput; set => m_floatOutput = value; }
        private float m_floatOutput;

        /// <summary>The color output of this weather property after performing the blend transition.</summary>
        public Color colorOutput { get => m_colorOutput; set => m_colorOutput = value; }
        private Color m_colorOutput;

        /// <summary>The vector output of this weather property after performing the blend transition.</summary>
        public Vector3 vector3Output { get => m_vector3Output; set => m_vector3Output = value; }
        private Vector3 m_vector3Output;

        ///// <summary>Constructor.</summary>
        //public AzureWeatherProperty()
        //{
        //    m_name = "MyPropertyName";
        //}
    }
}