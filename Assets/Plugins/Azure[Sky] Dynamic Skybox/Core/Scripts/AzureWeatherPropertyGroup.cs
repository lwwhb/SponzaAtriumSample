using System;
using System.Collections.Generic;

namespace UnityEngine.AzureSky
{
    [Serializable] public sealed class AzureWeatherPropertyGroup
    {
        /// <summary>The name of this property group.</summary>
        public string name { get => m_name; set => m_name = value; }
        [SerializeField] private string m_name = "GroupName";

        /// <summary>The list containing all the weather properties in this group.</summary>
        public List<AzureWeatherProperty> weatherPropertyList { get => m_weatherPropertyList; set => m_weatherPropertyList = value; }
        [SerializeField] private List<AzureWeatherProperty> m_weatherPropertyList = new List<AzureWeatherProperty>();

        /// <summary>The active state of this weather group, is this group current Enabled or Disabled?</summary>
        public bool isEnabled { get => m_isEnabled; set => m_isEnabled = value; }
        [SerializeField] private bool m_isEnabled = true;

        ///// <summary>Constructor.</summary>
        //public AzureWeatherPropertyGroup()
        //{
        //    m_name = "MyGroupName";
        //    m_isEnabled = true;
        //}
    }
}