using System;
using System.Collections.Generic;

namespace UnityEngine.AzureSky
{
    /// <summary>Class that handles the time, date and location stuff.</summary>
    [Serializable]
    public sealed class AzureTimeSystem
    {
        /// <summary>The transform that will represent the position of the sun in the sky.</summary>
        public Transform sunTransform { get => m_sunTransform; set => m_sunTransform = value; }
        [SerializeField] private Transform m_sunTransform = null;

        /// <summary>Stores the sun elevation in the sky in the Range(-1.0, 1.0).</summary>
        public float sunElevation => m_sunElevation;
        private float m_sunElevation = 0.0f;

        /// <summary>The sun elevation converted from Range(-1.0, 1.0) to Range(0.0, 1.0).</summary>
        private float m_sunElevationTime = 0.0f;

        /// <summary>The transform that will represent the position of the moon in the sky.</summary>
        public Transform moonTransform { get => m_moonTransform; set => m_moonTransform = value; }
        [SerializeField] private Transform m_moonTransform = null;

        /// <summary>Stores the moon elevation in the sky in the Range(-1.0, 1.0).</summary>
        public float moonElevation => m_moonElevation;
        private float m_moonElevation = 0.0f;

        /// <summary>The transform that will represent the position of the starfield in the sky.</summary>
        public Transform starfieldTransform { get => m_starfieldTransform; set => m_starfieldTransform = value; }
        [SerializeField] private Transform m_starfieldTransform = null;

        /// <summary>The directional light that will apply the sun and moon lighting to the scene.</summary>
        public Transform directionalLight { get => m_directionalLight; set => m_directionalLight = value; }
        [SerializeField] private Transform m_directionalLight = null;

        /// <summary>Used internally to compute the directional light direction.</summary>
        private Vector3 m_directionalLightDirection = Vector3.zero;

        /// <summary>The time mode used to perform position of the sun and moon in the sky according to the time of day.</summary>
        public AzureTimeMode timeMode { get => m_timeMode; set => m_timeMode = value; }
        [SerializeField] private AzureTimeMode m_timeMode = AzureTimeMode.Simple;

        /// <summary>The direction in which the time of day will flow.</summary>
        public AzureTimeDirection timeDirection { get => m_timeDirection; set => m_timeDirection = value; }
        [SerializeField] private AzureTimeDirection m_timeDirection = AzureTimeDirection.Forward;

        /// <summary>The time repeat mode cycle.</summary>
        public AzureTimeLoop timeLoop { get => m_timeLoop; set => m_timeLoop = value; }
        [SerializeField] private AzureTimeLoop m_timeLoop = AzureTimeLoop.Off;

        /// <summary>
        /// The timeline that represents the day cycle.
        /// Do not confuse with the time of day, they may not be the same thing depending on the configuration.
        /// </summary>
        public float timeline { get => m_timeline; set { m_timeline = value; OnTimelineChanged(); } }
        [SerializeField] private float m_timeline = 6.5f;

        /// <summary>The north-south angle of a position on the Earth's surface.</summary>
        public float latitude { get => m_latitude; set => m_latitude = value; }
        [SerializeField] private float m_latitude = 0f;

        /// <summary>The east-west angle of a position on the Earth's surface.</summary>
        public float longitude { get => m_longitude; set => m_longitude = value; }
        [SerializeField] private float m_longitude = 0f;

        /// <summary>
        /// Prior to 1972, this time was called Greenwich Mean Time (GMT).
        /// But is now referred to as Coordinated Universal Time or Universal Time Coordinated (UTC).
        /// </summary>
        public float utc { get => m_utc; set => m_utc = value; }
        [SerializeField] private float m_utc = 0;

        /// <summary>Represents the day in the calendar.</summary>
        public int day
        {
            get => m_day;

            set
            {
                m_day = value;
                UpdateCalendar();

                #if UNITY_EDITOR
                AzureNotificationCenterEditor.Invoke.RequestCalendarUpdateCallback();
                #endif
            }
        }
        [SerializeField] private int m_day = 1;

        /// <summary>Represents the month in the calendar.</summary>
        public int month
        {
            get => m_month;

            set
            {
                m_month = value;
                UpdateCalendar();

                #if UNITY_EDITOR
                AzureNotificationCenterEditor.Invoke.RequestCalendarUpdateCallback();
                #endif
            }
        } 
        [SerializeField] private int m_month = 1;

        /// <summary>Represents the year in the calendar.</summary>
        public int year
        {
            get => m_year;

            set
            {
                m_year = value;
                UpdateCalendar();

                #if UNITY_EDITOR
                AzureNotificationCenterEditor.Invoke.RequestCalendarUpdateCallback();
                #endif
            }
        }
        [SerializeField] private int m_year = 2024;

        /// <summary>The value the timeline should start the scene when entering the play mode.</summary>
        public float startTime { get => m_startTime; set => m_startTime = value; }
        [SerializeField] private float m_startTime = 6.5f;

        /// <summary>The duration of the day cycle in minutes.</summary>
        public float dayLength { get => m_dayLength; set { m_dayLength = value; ComputeTimeProgressionStep(); } }
        [SerializeField] private float m_dayLength = 24.0f;

        /// <summary>The minimmun directional light angle according to the horizon line.</summary>
        /// Useful for saving performance by avoiding stretched shadows when the light is horizontally tilted relative to the scene.
        public float minLightAltitude { get => m_minLightAltitude; set => m_minLightAltitude = value; }
        [SerializeField] private float m_minLightAltitude = 0.0f;

        /// <summary>The time that marks the transition from nighttime to sunrise in the timeline cycle.</summary>
        public float dawnTime
        {
            get => m_dawnTime;

            set
            {
                m_dawnTime = value;
                UpdateTimeLengthCurve();
            }
        }
        [SerializeField] private float m_dawnTime = 6f;

        /// <summary>The time that marks the transition from sunset to nighttime in the timeline cycle.</summary>
        public float duskTime
        {
            get => m_duskTime;

            set
            {
                m_duskTime = value;
                UpdateTimeLengthCurve();
            }
        }
        [SerializeField] private float m_duskTime = 18.0f;

        /// <summary>The current time of day inside the timeline cycle.</summary>
        public float timeOfDay => m_timeOfDay;
        [SerializeField] private float m_timeOfDay = 6.5f;

        /// <summary>The hour according to the time of day.</summary>
        public int hour => m_hour;
        [SerializeField] private int m_hour = 6;

        /// <summary>The minute according to the time of day.</summary>
        public int minute => m_minute;
        [SerializeField] private int m_minute = 0;

        /// <summary>The time used to evaluate the curves and gradients.</summary>
        public float evaluationTime => m_evaluationTime;
        [SerializeField] private float m_evaluationTime = 6.5f;

        /// <summary>The curve used to evaluate the daytime and nightime length.</summary>
        public AnimationCurve timeLengthCurve { get => m_timeLengthCurve; set => m_timeLengthCurve = value; }
        [SerializeField] private AnimationCurve m_timeLengthCurve = AnimationCurve.Linear(0f, 0f, 24f, 24f);

        /// <summary>The list of the celistial bodies the system can simulate other than the sun and moon.</summary>
        public List<AzureCelestialBody> celestialBodiesList { get => m_celestialBodiesList; set => m_celestialBodiesList = value; }
        [SerializeField] private List<AzureCelestialBody> m_celestialBodiesList = new List<AzureCelestialBody>();

        /// <summary>String array that stores the name of each day in the week.</summary>
        public string[] weekNameList => m_weekNameList;
        private readonly string[] m_weekNameList = new string[]
        {
            "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"
        };

        /// <summary>String array that stores the name of each month.</summary>
        public string[] monthNameList => m_monthNameList;
        private readonly string[] m_monthNameList = new string[]
        {
            "January", "February", "March", "April", "May", "June",
            "July", "August", "September", "October", "November", "December"
        };

        /// <summary>Array with 42 numeric strings used to fill a calendar.</summary>
        public string[] DayNumberList { get => m_dayNumberList; set => m_dayNumberList = value; }
        [SerializeField] private string[] m_dayNumberList = new string[]
        {
            "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10",
            "11", "12", "13", "14", "15", "16", "17", "18", "19", "20",
            "21", "22", "23", "24", "25", "26", "27", "28", "29", "30",
            "31", "32", "33", "34", "35", "36", "37", "38", "39", "40", "41"
        };

        /// <summary>The current selected day in the calendar.</summary>
        public int selectedCalendarDay => m_selectedCalendarDay;
        [SerializeField] private int m_selectedCalendarDay = 1;

        /// <summary>The number that represents the current selected day in the week (0 - 6).</summary>
        public int dayOfWeek => m_dayOfWeek;
        private int m_dayOfWeek = 0;

        /// <summary>Stores the total number of days in the current month.</summary>
        public int daysInMonth => m_daysInMonth;
        private int m_daysInMonth = 30;

        /// <summary>Used internally to update the calendar.</summary>
        private DateTime m_dateTime;

        /// <summary>Used internally to stores the total number of days in the previous month, usefull when the Time Direction is defined to Backward.</summary>
        private int m_daysInPreviousMonth = 30;

        /// <summary>Used internally to stores the previous month in the calendar, usefull when the Time Direction is defined to Backward.</summary>
        private int m_previousMonth = 1;

        /// <summary>The time progression step used to move the time of day.</summary>
        private float m_timeProgressionStep = 0f;

        /// <summary>Used internally to trigger the OnMinuteChange event.</summary>
        private int m_previousMinute = 0;

        /// <summary>Used internally to trigger the OnHourChange event.</summary>
        private int m_previousHour = 6;

        /// <summary>The exactly Time.time the timeline transition started.</summary>
        private float m_timelineTransitionStartTime = 0.0f;

        /// <summary>The start timeline point where a timeline transition should starte.</summary>
        private float m_timelineTransitionStartPoint = 0.0f;

        /// <summary>The end timeline point where a timeline transition should end.</summary>
        private float m_timelineTransitionEndPoint = 0.0f;

        /// <summary>The duration in seconds of the timeline transition.</summary>
        private float m_timelineTransitionDuration = 0.0f;

        /// <summary>The smooth step of the timeline transition.</summary>
        private float m_timelineTransitionStep = 0.0f;

        /// <summary>The temporary timeline value used by the timeline transition.</summary>
        private float m_temporaryTimeline = 0.0f;

        /// <summary>Is a timeline transition current running?</summary>
        public bool isPlayingTimelineTransition => m_isPlayingTimelineTransition;
        private bool m_isPlayingTimelineTransition = false;

        /// <summary>Called just before the first Update() of the AzureCoreSystem script.</summary>
        public void Start()
        {
            if (Application.isPlaying)
            {
                m_timeline = m_startTime;
            }

            // Get the correct time of day from the timeline cycle
            m_timeOfDay = m_timeLengthCurve.Evaluate(m_timeline);

            // Convert the time of day to hour and minute
            m_hour = (int)Mathf.Floor(m_timeOfDay);
            m_minute = (int)Mathf.Floor(m_timeOfDay * 60 % 60);

            m_previousMinute = m_minute;
            m_previousHour = m_hour;

            ComputeEvaluationTime();
            UpdateTimeLengthCurve();
            ComputeTimeProgressionStep();

            // First update of the calendar to sync with the start time and date
            UpdateCalendar();

            // First update of the sun, moon and directional light transforms
            UpdateCelestialBodies();
        }

        /// <summary>
        /// Moves the time of day to make all the time dependent things work.
        /// These calculations need to be done every frame.
        /// </summary>
        public void UpdateTimeOfDay()
        {
            if (!m_isPlayingTimelineTransition)
            {
                if (m_dayLength > 0)
                {
                    // Moves the timeline forward
                    if (m_timeDirection == AzureTimeDirection.Forward)
                    {
                        m_timeline += m_timeProgressionStep * Time.deltaTime;

                        // Change to the next day in the calendar
                        if (m_timeline > 24.0f)
                        {
                            IncreaseDay();
                            m_timeline = 0.0f;
                        }
                    }
                    // Moves the timeline backward
                    else
                    {
                        m_timeline -= m_timeProgressionStep * Time.deltaTime;

                        // Change to the previous day in the calendar
                        if (m_timeline < 0.0f)
                        {
                            DecreaseDay();
                            m_timeline = 24.0f;
                        }
                    }

                    OnTimelineChanged();
                }
            }
            else
            {
                ApplyTimelineTransition();
                OnTimelineChanged();
            }
        }

        #if UNITY_EDITOR
        /// <summary>Performs calculations that are required in edit mode only.</summary>
        public void OnEditorUpdate()
        {
            // Get the correct time of day from the timeline cycle
            m_timeOfDay = m_timeLengthCurve.Evaluate(m_timeline);

            // Convert the time of day to hours and minutes
            m_hour = (int)Mathf.Floor(m_timeOfDay);
            m_minute = (int)Mathf.Floor(m_timeOfDay * 60 % 60);

            UpdateCelestialBodies();
            ComputeEvaluationTime();
        }
        #endif

        /// <summary>Computes the time progression step according to the day length value.</summary>
        private void ComputeTimeProgressionStep()
        {
            if (m_dayLength > 0.0f)
            {
                m_timeProgressionStep = (24.0f / 60.0f) / m_dayLength;
            }
            else
            {
                m_timeProgressionStep = 0.0f;
            }
        }

        /// <summary>Updates the transforms of the sun, moon and directional light according to the time of day.</summary>
        public void UpdateCelestialBodies()
        {
            // Compute the direction of the transforms
            if (m_timeMode == AzureTimeMode.Simple)
            {
                m_sunTransform.rotation = Quaternion.Euler(0.0f, m_longitude, -m_latitude) * Quaternion.Euler(((m_timeOfDay - m_utc) * 360.0f / 24.0f) - 90.0f, 180.0f, 0.0f);
                m_moonTransform.rotation = m_sunTransform.rotation * Quaternion.Euler(0.0f, -180.0f, 0.0f);
                m_starfieldTransform.rotation = m_sunTransform.rotation;
            }
            else
            {
                // Initializations
                float hour = m_timeOfDay - m_utc;
                float rad = Mathf.Deg2Rad;
                float deg = Mathf.Rad2Deg;
                float latitude = m_latitude * rad;

                // The time scale
                float d = 367 * m_year - 7 * (m_year + (m_month + 9) / 12) / 4 + 275 * m_month / 9 + m_day - 730530;
                d = d + (hour / 24.0f);

                // Obliquity of the ecliptic: The tilt of earth's axis of rotation
                float ecl = 23.4393f - 3.563E-7f * d;
                ecl *= rad;

                // Orbital elements of the sun
                float N = 0.0f;
                float i = 0.0f;
                float w = 282.9404f + 4.70935E-5f * d;
                float a = 1.000000f;
                float e = 0.016709f - 1.151E-9f * d;
                float M = 356.0470f + 0.9856002585f * d;

                // Eccentric anomaly
                M *= rad;
                float E = M + e * Mathf.Sin(M) * (1f + e * Mathf.Cos(M));

                // Sun's distance (r) and its true anomaly (v)
                float xv = Mathf.Cos(E) - e;
                float yv = Mathf.Sqrt(1.0f - (e * e)) * Mathf.Sin(E);
                float v = Mathf.Atan2(yv, xv) * deg;
                float r = Mathf.Sqrt((xv * xv) + (yv * yv));

                // Sun's true longitude
                float lonsun = (v + w) * rad;

                // Convert lonsun,r to ecliptic rectangular geocentric coordinates xs,ys:
                float xs = r * Mathf.Cos(lonsun);
                float ys = r * Mathf.Sin(lonsun);
                //    zs = 0;

                // To convert this to equatorial, rectangular, geocentric coordinates, compute:
                float xe = xs;
                float ye = ys * Mathf.Cos(ecl);
                float ze = ys * Mathf.Sin(ecl);

                // Sun's right ascension (RA) and declination (Decl):
                float RA = Mathf.Atan2(ye, xe);
                float Decl = Mathf.Atan2(ze, Mathf.Sqrt((xe * xe) + (ye * ye)));

                // The sidereal time
                float Ls = v + w;
                float GMST0 = Ls + 180.0f;
                float GMST = GMST0 + (hour * 15.0f);
                float LST = (GMST + m_longitude) * rad;

                // Azimuthal coordinates
                float HA = LST - RA;

                float x = Mathf.Cos(HA) * Mathf.Cos(Decl);
                float y = Mathf.Sin(HA) * Mathf.Cos(Decl);
                float z = Mathf.Sin(Decl);

                float xhor = (x * Mathf.Sin(latitude)) - (z * Mathf.Cos(latitude));
                float yhor = y;
                float zhor = (x * Mathf.Cos(latitude)) + (z * Mathf.Sin(latitude));

                float azimuth = Mathf.Atan2(yhor, xhor);
                float altitude = Mathf.Asin(zhor);

                // Gets the celestial rotation
                Vector3 celestialRotation;
                celestialRotation.x = altitude * deg;
                celestialRotation.y = azimuth * deg;
                celestialRotation.z = 0.0f;

                //m_sunTransform.rotation = Quaternion.Euler(celestialRotation);
                m_starfieldTransform.rotation = Quaternion.Euler(90.0f - m_latitude, 0.0f, 0.0f) * Quaternion.Euler(0.0f, m_longitude, 0.0f) * Quaternion.Euler(0.0f, LST * deg, 0.0f);
                m_sunTransform.position = Quaternion.Euler(celestialRotation) * new Vector3(0.0f, 0.0f, 1.0f);
                m_sunTransform.rotation = Quaternion.LookRotation(m_sunTransform.position, m_starfieldTransform.up);

                // Orbital elements of the Moon
                N = 125.1228f - 0.0529538083f * d;
                i = 5.1454f;
                w = 318.0634f + 0.1643573223f * d;
                //a = 0.002566882112227f; (AU)
                a = 60.2666f; // Earth radius
                e = 0.054900f;
                M = 115.3654f + 13.0649929509f * d;

                // Eccentric anomaly
                M *= rad;
                E = M + e * Mathf.Sin(M) * (1f + e * Mathf.Cos(M));

                // Moon's distance and true anomaly
                xv = a * (Mathf.Cos(E) - e);
                yv = a * (Mathf.Sqrt(1f - e * e) * Mathf.Sin(E));
                v = Mathf.Atan2(yv, xv) * deg;
                r = Mathf.Sqrt(xv * xv + yv * yv);

                // Moon's true longitude
                lonsun = (v + w) * rad;
                float sinLongitude = Mathf.Sin(lonsun);
                float cosLongitude = Mathf.Cos(lonsun);

                // The position in space - for the planets
                // Geocentric (Earth-centered) coordinates - for the moon
                N *= rad;
                i *= rad;

                float xh = r * (Mathf.Cos(N) * cosLongitude - Mathf.Sin(N) * sinLongitude * Mathf.Cos(i));
                float yh = r * (Mathf.Sin(N) * cosLongitude + Mathf.Cos(N) * sinLongitude * Mathf.Cos(i));
                float zh = r * (sinLongitude * Mathf.Sin(i));

                // Geocentric (Earth-centered) coordinates
                // For the moon this is the same as the position in space, there is no need to calculate again
                // float xg = xh;
                // float yg = yh;
                // float zg = zh;

                // Equatorial coordinates
                xe = xh;
                ye = yh * Mathf.Cos(ecl) - zh * Mathf.Sin(ecl);
                ze = yh * Mathf.Sin(ecl) + zh * Mathf.Cos(ecl);

                // Moon's right ascension (RA) and declination (Decl)
                RA = Mathf.Atan2(ye, xe);
                Decl = Mathf.Atan2(ze, Mathf.Sqrt(xe * xe + ye * ye));

                // The sidereal time
                // It is already calculated for the sun, there is no need to calculate again

                // Azimuthal coordinates
                HA = LST - RA;

                x = Mathf.Cos(HA) * Mathf.Cos(Decl);
                y = Mathf.Sin(HA) * Mathf.Cos(Decl);
                z = Mathf.Sin(Decl);

                xhor = x * Mathf.Sin(latitude) - z * Mathf.Cos(latitude);
                yhor = y;
                zhor = x * Mathf.Cos(latitude) + z * Mathf.Sin(latitude);

                azimuth = Mathf.Atan2(yhor, xhor);
                altitude = Mathf.Asin(zhor);

                // Gets the celestial rotation
                celestialRotation.x = altitude * deg;
                celestialRotation.y = azimuth * deg;
                celestialRotation.z = 0.0f;

                //m_moonTransform.localRotation = Quaternion.Euler(celestialRotation);
                //m_moonDistance = r * 6371f;
                m_moonTransform.position = Quaternion.Euler(celestialRotation) * new Vector3(0.0f, 0.0f, 1.0f);
                m_moonTransform.rotation = Quaternion.LookRotation(m_moonTransform.position, m_starfieldTransform.up);

                // Planets
                if (m_celestialBodiesList.Count > 0)
                {
                    AzureCelestialBody celestialBody;
                    for (int index = 0; index < m_celestialBodiesList.Count; index++)
                    {
                        celestialBody = m_celestialBodiesList[index];
                        if (!celestialBody.transform) continue;

                        // Orbital elements of the planets
                        switch (celestialBody.type)
                        {
                            case AzureCelestialBodyType.Mercury:
                                N = 48.3313f + 3.24587E-5f * d;
                                i = 7.0047f + 5.00E-8f * d;
                                w = 29.1241f + 1.01444E-5f * d;
                                a = 0.387098f;
                                e = 0.205635f + 5.59E-10f * d;
                                M = 168.6562f + 4.0923344368f * d;
                                break;

                            case AzureCelestialBodyType.Venus:
                                N = 76.6799f + 2.46590E-5f * d;
                                i = 3.3946f + 2.75E-8f * d;
                                w = 54.8910f + 1.38374E-5f * d;
                                a = 0.723330f;
                                e = 0.006773f - 1.302E-9f * d;
                                M = 48.0052f + 1.6021302244f * d;
                                break;

                            case AzureCelestialBodyType.Mars:
                                N = 49.5574f + 2.11081E-5f * d;
                                i = 1.8497f - 1.78E-8f * d;
                                w = 286.5016f + 2.92961E-5f * d;
                                a = 1.523688f;
                                e = 0.093405f + 2.516E-9f * d;
                                M = 18.6021f + 0.5240207766f * d;
                                break;

                            case AzureCelestialBodyType.Jupiter:
                                N = 100.4542f + 2.76854E-5f * d;
                                i = 1.3030f - 1.557E-7f * d;
                                w = 273.8777f + 1.64505E-5f * d;
                                a = 5.20256f;
                                e = 0.048498f + 4.469E-9f * d;
                                M = 19.8950f + 0.0830853001f * d;
                                break;

                            case AzureCelestialBodyType.Saturn:
                                N = 113.6634f + 2.38980E-5f * d;
                                i = 2.4886f - 1.081E-7f * d;
                                w = 339.3939f + 2.97661E-5f * d;
                                a = 9.55475f;
                                e = 0.055546f - 9.499E-9f * d;
                                M = 316.9670f + 0.0334442282f * d;
                                break;

                            case AzureCelestialBodyType.Uranus:
                                N = 74.0005f + 1.3978E-5f * d;
                                i = 0.7733f + 1.9E-8f * d;
                                w = 96.6612f + 3.0565E-5f * d;
                                a = 19.18171f - 1.55E-8f * d;
                                e = 0.047318f + 7.45E-9f * d;
                                M = 142.5905f + 0.011725806f * d;
                                break;

                            case AzureCelestialBodyType.Neptune:
                                N = 131.7806f + 3.0173E-5f * d;
                                i = 1.7700f - 2.55E-7f * d;
                                w = 272.8461f - 6.027E-6f * d;
                                a = 30.05826f + 3.313E-8f * d;
                                e = 0.008606f + 2.15E-9f * d;
                                M = 260.2471f + 0.005995147f * d;
                                break;

                            // No analytical theory has ever been constructed for the planet Pluto.
                            // Our most accurate representation of the motion of this planet is from numerical integrations.
                            case AzureCelestialBodyType.Pluto:
                                float S = 50.03f + 0.033459652f * d;
                                float P = 238.95f + 0.003968789f * d;

                                S *= rad;
                                P *= rad;

                                float pluto_lonecl = 238.9508f + 0.00400703f * d
                                                   - 19.799f * Mathf.Sin(P) + 19.848f * Mathf.Cos(P)
                                                   + 0.897f * Mathf.Sin(2 * P) - 4.956f * Mathf.Cos(2 * P)
                                                   + 0.610f * Mathf.Sin(3 * P) + 1.211f * Mathf.Cos(3 * P)
                                                   - 0.341f * Mathf.Sin(4 * P) - 0.190f * Mathf.Cos(4 * P)
                                                   + 0.128f * Mathf.Sin(5 * P) - 0.034f * Mathf.Cos(5 * P)
                                                   - 0.038f * Mathf.Sin(6 * P) + 0.031f * Mathf.Cos(6 * P)
                                                   + 0.020f * Mathf.Sin(S - P) - 0.010f * Mathf.Cos(S - P);

                                float pluto_latecl = -3.9082f
                                                   - 5.453f * Mathf.Sin(P) - 14.975f * Mathf.Cos(P)
                                                   + 3.527f * Mathf.Sin(2 * P) + 1.673f * Mathf.Cos(2 * P)
                                                   - 1.051f * Mathf.Sin(3 * P) + 0.328f * Mathf.Cos(3 * P)
                                                   + 0.179f * Mathf.Sin(4 * P) - 0.292f * Mathf.Cos(4 * P)
                                                   + 0.019f * Mathf.Sin(5 * P) + 0.100f * Mathf.Cos(5 * P)
                                                   - 0.031f * Mathf.Sin(6 * P) - 0.026f * Mathf.Cos(6 * P)
                                                                               + 0.011f * Mathf.Cos(S - P);

                                float pluto_r = 40.72f
                                              + 6.68f * Mathf.Sin(P) + 6.90f * Mathf.Cos(P)
                                              - 1.18f * Mathf.Sin(2 * P) - 0.03f * Mathf.Cos(2 * P)
                                              + 0.15f * Mathf.Sin(3 * P) - 0.14f * Mathf.Cos(3 * P);

                                // Geocentric (Earth-centered) coordinates
                                pluto_lonecl *= rad;
                                pluto_latecl *= rad;
                                float pluto_cosLatecl = Mathf.Cos(pluto_latecl);
                                xh = pluto_r * Mathf.Cos(pluto_lonecl) * pluto_cosLatecl;
                                yh = pluto_r * Mathf.Sin(pluto_lonecl) * pluto_cosLatecl;
                                zh = pluto_r * Mathf.Sin(pluto_latecl);

                                // From the sun computation
                                // xs = r * cosLongitude;
                                // ys = r * sinLongitude;

                                float pluto_xg = xh + xs;
                                float pluto_yg = yh + ys;
                                float pluto_zg = zh;

                                // Equatorial coordinates
                                xe = pluto_xg;
                                ye = pluto_yg * Mathf.Cos(ecl) - pluto_zg * Mathf.Sin(ecl);
                                ze = pluto_yg * Mathf.Sin(ecl) + pluto_zg * Mathf.Cos(ecl);

                                // Moon's right ascension (RA) and declination (Decl)
                                RA = Mathf.Atan2(ye, xe);
                                Decl = Mathf.Atan2(ze, Mathf.Sqrt(xe * xe + ye * ye));

                                // The sidereal time
                                // It is already calculated for the sun, there is no need to calculate again

                                // Azimuthal coordinates
                                HA = LST - RA;

                                x = Mathf.Cos(HA) * Mathf.Cos(Decl);
                                y = Mathf.Sin(HA) * Mathf.Cos(Decl);
                                z = Mathf.Sin(Decl);

                                xhor = x * Mathf.Sin(latitude) - z * Mathf.Cos(latitude);
                                yhor = y;
                                zhor = x * Mathf.Cos(latitude) + z * Mathf.Sin(latitude);

                                azimuth = Mathf.Atan2(yhor, xhor);
                                altitude = Mathf.Asin(zhor);

                                // Gets the celestial rotation
                                celestialRotation.x = altitude * deg;
                                celestialRotation.y = azimuth * deg;
                                celestialRotation.z = 0.0f;

                                //celestialBody.transform.rotation = Quaternion.Euler(celestialRotation);
                                celestialBody.transform.position = Quaternion.Euler(celestialRotation) * new Vector3(0.0f, 0.0f, 1.0f);
                                celestialBody.transform.rotation = Quaternion.LookRotation(celestialBody.transform.position, m_starfieldTransform.up);
                                continue;
                        }

                        // Eccentric anomaly
                        M *= rad;
                        E = M + e * Mathf.Sin(M) * (1f + e * Mathf.Cos(M));

                        // Planet's distance and true anomaly
                        xv = a * (Mathf.Cos(E) - e);
                        yv = a * (Mathf.Sqrt(1f - e * e) * Mathf.Sin(E));
                        v = Mathf.Atan2(yv, xv) * deg;
                        r = Mathf.Sqrt(xv * xv + yv * yv);

                        // Planet's true longitude
                        lonsun = (v + w) * rad;
                        cosLongitude = Mathf.Cos(lonsun);
                        sinLongitude = Mathf.Sin(lonsun);

                        // The position in space - heliocentric (Sun-centered) position
                        N *= rad;
                        i *= rad;

                        xh = r * (Mathf.Cos(N) * cosLongitude - Mathf.Sin(N) * sinLongitude * Mathf.Cos(i));
                        yh = r * (Mathf.Sin(N) * cosLongitude + Mathf.Cos(N) * sinLongitude * Mathf.Cos(i));
                        zh = r * (sinLongitude * Mathf.Sin(i));

                        float lonecl = Mathf.Atan2(yh, xh);
                        float latecl = Mathf.Atan2(zh, Mathf.Sqrt(xh * xh + yh * yh));

                        // Geocentric (Earth-centered) coordinates
                        float cosLatecl = Mathf.Cos(latecl);
                        xh = r * Mathf.Cos(lonecl) * cosLatecl;
                        yh = r * Mathf.Sin(lonecl) * cosLatecl;
                        zh = r * Mathf.Sin(latecl);

                        // From the sun computation
                        // xs = r * cosLongitude;
                        // ys = r * sinLongitude;

                        float xg = xh + xs;
                        float yg = yh + ys;
                        float zg = zh;

                        // Equatorial coordinates
                        xe = xg;
                        ye = yg * Mathf.Cos(ecl) - zg * Mathf.Sin(ecl);
                        ze = yg * Mathf.Sin(ecl) + zg * Mathf.Cos(ecl);

                        // Planet's right ascension (RA) and declination (Decl)
                        RA = Mathf.Atan2(ye, xe);
                        Decl = Mathf.Atan2(ze, Mathf.Sqrt(xe * xe + ye * ye));

                        // The sidereal time
                        // It is already calculated for the sun, there is no need to calculate again

                        // Azimuthal coordinates
                        HA = LST - RA;

                        x = Mathf.Cos(HA) * Mathf.Cos(Decl);
                        y = Mathf.Sin(HA) * Mathf.Cos(Decl);
                        z = Mathf.Sin(Decl);

                        xhor = x * Mathf.Sin(latitude) - z * Mathf.Cos(latitude);
                        yhor = y;
                        zhor = x * Mathf.Cos(latitude) + z * Mathf.Sin(latitude);

                        azimuth = Mathf.Atan2(yhor, xhor);
                        altitude = Mathf.Asin(zhor);

                        // Gets the celestial rotation
                        celestialRotation.x = altitude * deg;
                        celestialRotation.y = azimuth * deg;
                        celestialRotation.z = 0.0f;

                        //celestialBody.transform.rotation = Quaternion.Euler(celestialRotation);
                        celestialBody.transform.position = Quaternion.Euler(celestialRotation) * new Vector3(0.0f, 0.0f, 1.0f);
                        celestialBody.transform.rotation = Quaternion.LookRotation(celestialBody.transform.position, m_starfieldTransform.up);
                    }
                }
            }

            // Computes sun and moon elevation
            m_sunElevation = Vector3.Dot(-m_sunTransform.forward, Vector3.up);
            m_moonElevation = Vector3.Dot(-m_moonTransform.forward, Vector3.up);

            // Set the directional light direction
            m_directionalLight.localRotation = Quaternion.LookRotation(m_sunElevation >= 0.0f ? m_sunTransform.forward : m_moonTransform.forward);
            
            // Avoid the directional light to get close to the horizon line
            m_directionalLightDirection = m_directionalLight.localEulerAngles;
            if (m_directionalLightDirection.x <= m_minLightAltitude) { m_directionalLightDirection.x = m_minLightAltitude; }
            m_directionalLight.localEulerAngles = m_directionalLightDirection;
        }

        /// <summary>Increases a day in the calendar.</summary>
        public void IncreaseDay()
        {
            if (m_timeLoop != AzureTimeLoop.Dayly)
            {
                m_day++;

                if (m_day > m_daysInMonth)
                {
                    m_day = 1;
                    IncreaseMonth();
                    AzureNotificationCenter.Invoke.OnDayChangedCallback(this);
                    return;
                }
                else
                {
                    AzureNotificationCenter.Invoke.OnDayChangedCallback(this);
                }
            }

            UpdateCalendar();
        }

        /// <summary>Decreases a day in the calendar.</summary>
        public void DecreaseDay()
        {
            if (m_timeLoop != AzureTimeLoop.Dayly)
            {
                m_day--;

                m_previousMonth = m_timeLoop == AzureTimeLoop.Monthly ? m_month : m_month - 1;
                if (m_previousMonth < 1) m_previousMonth = 12;
                m_daysInPreviousMonth = DateTime.DaysInMonth(m_year, m_previousMonth);

                if (m_day < 1)
                {
                    m_day = m_daysInPreviousMonth;
                    DecreaseMonth();
                    AzureNotificationCenter.Invoke.OnDayChangedCallback(this);
                    return;
                }
                else
                {
                    AzureNotificationCenter.Invoke.OnDayChangedCallback(this);
                }
            }

            UpdateCalendar();
        }

        /// <summary>Increases a month in the calendar.</summary>
        public void IncreaseMonth()
        {
            if (m_timeLoop != AzureTimeLoop.Monthly)
            {
                m_month++;

                if (m_month > 12)
                {
                    m_month = 1;
                    IncreaseYear();
                    AzureNotificationCenter.Invoke.OnMonthChangedCallback(this);
                    return;
                }
                else
                {
                    AzureNotificationCenter.Invoke.OnMonthChangedCallback(this);
                }
            }

            UpdateCalendar();
        }

        /// <summary>Decreases a month in the calendar.</summary>
        public void DecreaseMonth()
        {
            if (m_timeLoop != AzureTimeLoop.Monthly)
            {
                m_month--;

                if (m_month < 1)
                {
                    m_month = 12;
                    DecreaseYear();
                    AzureNotificationCenter.Invoke.OnMonthChangedCallback(this);
                    return;
                }
                else
                {
                    AzureNotificationCenter.Invoke.OnMonthChangedCallback(this);
                }
            }

            UpdateCalendar();
        }

        /// <summary>Increases a year in the calendar.</summary>
        public void IncreaseYear()
        {
            if (m_timeLoop != AzureTimeLoop.Yearly)
            {
                m_year++;
                if (m_year > 9999) m_year = 0;
                AzureNotificationCenter.Invoke.OnYearChangedCallback(this);
            }

            UpdateCalendar();
        }

        /// <summary>Decreases a year in the calendar.</summary>
        public void DecreaseYear()
        {
            if (m_timeLoop != AzureTimeLoop.Yearly)
            {
                m_year--;
                if (m_year < 0) m_year = 9999;
                AzureNotificationCenter.Invoke.OnYearChangedCallback(this);
            }

            UpdateCalendar();
        }

        /// <summary>The proper way for setting a custom date.</summary>
        public void SetDate(int day, int month, int year)
        {
            m_year = year;
            m_month = month;
            m_day = day;
            UpdateCalendar();

            #if UNITY_EDITOR
            AzureNotificationCenterEditor.Invoke.RequestCalendarUpdateCallback();
            #endif
        }

        /// <summary>Returns the current time of day as a Vector2Int(hours, minutes).</summary>
        public Vector2Int GetTimeOfDayVector()
        {
            return new Vector2Int(m_hour, m_minute);
        }

        /// <summary>Returns the current time of day as a string ("00:00").</summary>
        public string GetTimeOfDayString()
        {
            return hour.ToString("00") + ":" + minute.ToString("00");
        }

        /// <summary>Returns the current date as a Vector3Int(year, month, day).</summary>
        public Vector3Int GetDateVector()
        {
            return new Vector3Int(m_year, m_month, m_day);
        }

        /// <summary>Returns the current date converted to string using the default date format used by Azure ("January 01, 2024").</summary>
        public string GetDateString()
        {
            // Format: "MMMM dd, yyyy"
            return m_monthNameList[m_month - 1] + " " + m_day.ToString("00") + ", " + m_year.ToString("0000");
        }

        /// <summary>Returns the current date converted to string using a custom format as parameter.</summary>
        public string GetDateString(string format)
        {
            m_dateTime = new DateTime(m_year, m_month, m_day);
            return m_dateTime.ToString(format);
        }

        /// <summary>Returns the current day of the week as string.</summary>
        public string GetDayOfWeekString()
        {
            m_dateTime = new DateTime(m_year, m_month, m_day);
            return m_weekNameList[(int)m_dateTime.DayOfWeek];
        }

        /// <summary>Returns the current day of the week as an integer number between 0 and 6.</summary>
        public int GetDayOfWeekInteger()
        {
            DateTime dateTime = new DateTime(m_year, m_month, m_day);
            return (int)dateTime.DayOfWeek;
        }

        /// <summary>Adjust the calendar when there is a change in the date.</summary>
        public void UpdateCalendar()
        {
            // Avoid selecting a date that does not exist
            m_year = Mathf.Clamp(year, 0, 9999);
            m_month = Mathf.Clamp(month, 1, 12);
            m_daysInMonth = DateTime.DaysInMonth(m_year, m_month);
            m_day = Mathf.Clamp(day, 1, m_daysInMonth);

            // Creates a custom DateTime at the first day of the current month
            m_dateTime = new DateTime(m_year, m_month, 1);

            // Gets the day of week corresponding to this custom DateTime
            m_dayOfWeek = (int)m_dateTime.DayOfWeek;

            // Keeps the same day selected in the calendar even when the date is changed externally
            m_selectedCalendarDay = m_day - 1 + m_dayOfWeek;

            for (int i = 0; i < m_dayNumberList.Length; i++)
            {
                // Set the strings of all calendar's spots to empty
                if (i < m_dayOfWeek || i >= (m_dayOfWeek + m_daysInMonth))
                {
                    m_dayNumberList[i] = "";
                    continue;
                }

                // Sets the day number only to the valid calendar's spots of the current month in use by the calendar
                m_dateTime = new DateTime(m_year, m_month, (i - m_dayOfWeek) + 1);
                m_dayNumberList[i] = m_dateTime.Day.ToString();
            }
        }

        /// <summary>Starts a timeline transition to a desired time.</summary>
        public void StartTimelineTransition(float targetTime, float transitionTime)
        {
            Mathf.Clamp(targetTime, 0.0f, 24.0f);
            m_timelineTransitionStartTime = Time.time;
            m_timelineTransitionStartPoint = m_timeline;
            m_timelineTransitionDuration = transitionTime;

            if (m_timeDirection == AzureTimeDirection.Forward)
            {
                if (targetTime < m_timeline)
                {
                    m_timelineTransitionEndPoint = m_timeline + (targetTime - m_timeline + 24f);
                }
                else { m_timelineTransitionEndPoint = targetTime; }
            }
            else
            {
                if (targetTime > m_timeline)
                {
                    m_timelineTransitionEndPoint = m_timeline - (24f - targetTime + m_timeline);
                }
                else { m_timelineTransitionEndPoint = targetTime; }
            }

            m_isPlayingTimelineTransition = true;
        }

        /// <summary>Used to Update the time length curve when there is a change in the dawn and dusk time.</summary>
        public void UpdateTimeLengthCurve()
        {
            m_timeLengthCurve = AnimationCurve.Linear(0f, 0f, 24f, 24f);
            m_timeLengthCurve.AddKey(m_dawnTime, 6.0f);
            m_timeLengthCurve.AddKey(m_duskTime, 18.0f);
        }

        /// <summary>Updates the time length curve based on a custom dawn and dusk time.</summary>
        public void UpdateTimeLengthCurve(float dawn, float dusk)
        {
            m_timeLengthCurve = AnimationCurve.Linear(0f, 0f, 24f, 24f);
            m_timeLengthCurve.AddKey(dawn, 6.0f);
            m_timeLengthCurve.AddKey(dusk, 18.0f);
        }

        /// <summary>Computes the evaluation time values used by the Weather System to evaluate its curves and gradients.</summary>
        private void ComputeEvaluationTime()
        {
            if (m_timeMode == AzureTimeMode.Simple)
            {
                m_evaluationTime = m_timeOfDay;
            }
            else
            {
                m_sunElevationTime = Mathf.InverseLerp(-1.0f, 1.0f, m_sunElevation);
                m_evaluationTime = Mathf.Lerp(0.0f, 12.0f, m_sunElevationTime);
            }
        }

        /// <summary>Performs the timeline transition feature.</summary>
        private void ApplyTimelineTransition()
        {
            m_timelineTransitionStep = (Time.time - m_timelineTransitionStartTime) / m_timelineTransitionDuration;
            m_temporaryTimeline = Mathf.SmoothStep(m_timelineTransitionStartPoint, m_timelineTransitionEndPoint, m_timelineTransitionStep);

            m_timeline = m_temporaryTimeline;

            if (m_timeDirection == AzureTimeDirection.Forward)
            {
                // Change to the next day in the calendar
                if (m_timeline > 24.0f)
                {
                    IncreaseDay();
                    m_timeline = 0.0f;
                    m_timelineTransitionStartPoint -= 24f;
                    m_timelineTransitionEndPoint -= 24f;
                }

                if (m_timeline >= m_timelineTransitionEndPoint)
                {
                    m_isPlayingTimelineTransition = false;
                }
            }
            else
            {
                // Change to the previous day in the calendar
                if (m_timeline < 0.0f)
                {
                    DecreaseDay();
                    m_timeline = 24.0f;
                    m_timelineTransitionStartPoint += 24f;
                    m_timelineTransitionEndPoint += 24f;
                }

                if (m_timeline <= m_timelineTransitionEndPoint)
                {
                    m_isPlayingTimelineTransition = false;
                }
            }
        }

        /// <summary>Internal method called every time the timeline property changes.</summary>
        private void OnTimelineChanged()
        {
            // Get the correct time of day from the timeline cycle
            m_timeOfDay = m_timeLengthCurve.Evaluate(m_timeline);

            // Convert the time of day to hour and minute
            m_hour = (int)Mathf.Floor(m_timeOfDay);
            m_minute = (int)Mathf.Floor(m_timeOfDay * 60 % 60);

            ComputeEvaluationTime();

            // On minute change event
            if (m_previousMinute != m_minute)
            {
                m_previousMinute = m_minute;
                AzureNotificationCenter.Invoke.OnMinuteChangedCallback(this);
            }

            // On hour change event
            if (m_previousHour != m_hour)
            {
                m_previousHour = m_hour;
                AzureNotificationCenter.Invoke.OnHourChangedCallback(this);
            }

            AzureNotificationCenter.Invoke.OnTimelineChangedCallback(this);
        }
    }
}