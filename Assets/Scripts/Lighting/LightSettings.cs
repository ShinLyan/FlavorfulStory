using System;
using UnityEngine;

namespace FlavorfulStory.Lightning
{
    /// <summary> Класс настроек освещения, содержащий параметры для солнца и луны. </summary>
    [Serializable]
    public class LightSettings
    {
        /// <summary> Настройки цвета солнца в течение дня (градиент). </summary>
        [Header("Sun Settings")] public Gradient SunColorGradient;

        /// <summary> Кривая интенсивности солнечного света в течение дня. </summary>
        public AnimationCurve SunIntensityCurve;

        /// <summary> Максимальная интенсивность солнечного света. </summary>
        public float MaxSunIntensity = 4f;

        /// <summary> Углы поворота солнца по оси X (утро, полдень, вечер). </summary>
        public Vector3 SunAngleX;

        /// <summary> Углы поворота солнца по оси Y до полудня (утро, полдень). </summary>
        public Vector2 SunAngleYBefore12;

        /// <summary> Углы поворота солнца по оси Y после полудня (полдень, вечер). </summary>
        public Vector2 SunAngleYAfter12;

        /// <summary> Тип теней от солнца (Hard/Soft/None). </summary>
        [Header("Sun Shadow Settings")] public LightShadows SunShadowType = LightShadows.Hard;

        /// <summary> Сила теней от солнца (0-1). </summary>
        [Range(0, 1)] public float SunShadowStrength = 0.9f;


        /// <summary> Настройки цвета луны в течение ночи (градиент). </summary>
        [Header("Moon Settings")] public Gradient MoonColorGradient;

        /// <summary> Кривая интенсивности лунного света в течение ночи. </summary>
        public AnimationCurve MoonIntensityCurve;

        /// <summary> Максимальная интенсивность лунного света. </summary>
        public float MaxMoonIntensity = 2f;

        /// <summary> Тип теней от луны (Hard/Soft/None). </summary>
        [Header("Moon Shadow Settings")] public LightShadows MoonShadowType = LightShadows.Soft;

        /// <summary> Сила теней от луны (0-1). </summary>
        [Range(0, 1)] public float MoonShadowStrength = 0.6f;
    }
}