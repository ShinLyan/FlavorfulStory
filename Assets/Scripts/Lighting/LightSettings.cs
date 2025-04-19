using System;
using UnityEngine;

namespace FlavorfulStory.Lightning
{
    [Serializable]
    public class LightSettings
    {
        [Header("Sun Settings")] public Gradient SunColorGradient;
        public AnimationCurve SunIntensityCurve;
        public float MaxSunIntensity = 4f;
        public Vector3 SunAngleX;
        public Vector2 SunAngleYBefore12;
        public Vector2 SunAngleYAfter12;

        [Header("Sun Shadow Settings")] public LightShadows SunShadowType = LightShadows.Hard;
        [Range(0, 1)] public float SunShadowStrength = 0.9f;


        [Header("Moon Settings")] public Gradient MoonColorGradient;
        public AnimationCurve MoonIntensityCurve;
        public float MaxMoonIntensity = 2f;

        [Header("Moon Shadow Settings")] public LightShadows MoonShadowType = LightShadows.Soft;
        [Range(0, 1)] public float MoonShadowStrength = 0.6f;
    }
}