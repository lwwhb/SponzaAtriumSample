namespace UnityEngine.AzureSky
{
    /// <summary>Sets the mode used to perform celestial bodies positions in the sky.</summary>
    public enum AzureTimeMode
    {
        Simple,
        Realistic
    }

    /// <summary>Sets the direction in which the time of day will flow.</summary>
    public enum AzureTimeDirection
    {
        Forward,
        Backward
    }

    /// <summary>Sets the time repeat mode.</summary>
    public enum AzureTimeLoop
    {
        Off,
        Dayly,
        Monthly,
        Yearly
    }

    /// <summary>
    /// Sets if the scene will start using the local time defined in the 'Start Time' field.
    /// Or whether the scene will start using the global time to keep the current time of day between scene transitions.
    /// </summary>
    public enum AzureTimeSource
    {
        LocalTime,
        GlobalTime
    }

    /// <summary>The way the core features should be updated.</summary>
    public enum AzureCoreUpdateMode
    {
        EveryFrame,
        TimeInterval
    }

    /// <summary>The way a component with this attribute should be updated.</summary>
    public enum AzureUpdateMode
    {
        LocallyEveryFrame,
        ExternalCall
    }

    /// <summary>The way used to render the clouds.</summary>
    public enum AzureCloudMode
    {
        Off,
        Dynamic
    }

    /// <summary>A preset of celestial bodies that the core system can simulate.</summary>
    public enum AzureCelestialBodyType
    {
        Mercury,
        Venus,
        Mars,
        Jupiter,
        Saturn,
        Uranus,
        Neptune,
        Pluto
    }

    /// <summary>The types supported by the weather properties.</summary>
    public enum AzureWeatherPropertyType
    {
        Float,
        Color,
        Curve,
        Gradient,
        Direction,
        Position
    }

    /// <summary>The override mode feature of a weather property.</summary>
    public enum AzureWeatherPropertyOverrideMode
    {
        Off,
        On
    }

    /// <summary>The target type that the weather property should override.</summary>
    public enum AzureWeatherPropertyTargetType
    {
        Property,
        Field,
        MaterialProperty,
        GlobalShaderUniform,
        GlobalProperty,
        GlobalField
    }

    /// <summary>The interval time the custom event will be scanned.</summary>
    public enum AzureCustomEventUpdateMode
    {
        ByMinute,
        ByHour
    }
}