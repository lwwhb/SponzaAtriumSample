using System;
using System.Collections.Generic;

namespace UnityEngine.AzureSky
{
    [Serializable] public sealed class AzurePropertyGroupData
    {
        /// <summary>The list containing all the properties data in this group.</summary>
        public List<AzurePropertyData> propertyDataList { get => m_propertyDataList; set => m_propertyDataList = value; }
        [SerializeField] private List<AzurePropertyData> m_propertyDataList = new List<AzurePropertyData>();

        /// <summary>The curve used to define the stage in the weather transition this group transition will start/end.</summary>
        public AnimationCurve rampCurve { get => m_rampCurve; set => m_rampCurve = value; }
        [SerializeField] private AnimationCurve m_rampCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        /// <summary>The name of this property group data.</summary>
        public string name { get => m_name; set => m_name = value; }
        [SerializeField] private string m_name;
    }
}