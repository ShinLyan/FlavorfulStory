using System;
using UnityEngine;

namespace FlavorfulStory.Visuals.Lightning
{
    /// <summary> Класс настроек освещения, содержащий параметры для солнца и луны. </summary>
    [Serializable]
    public class LightSettings
    {
        /// <summary> Настройки цвета солнца в течение дня (градиент). </summary>
        [field: Header("Sun Settings")]
        [field: Tooltip("Настройки цвета солнца в течение дня (градиент)."), SerializeField]
        public Gradient SunColorGradient { get; private set; }

        /// <summary> Кривая интенсивности солнечного света в течение дня. </summary>
        [field: Tooltip("Кривая интенсивности солнечного света в течение дня."), SerializeField]
        public AnimationCurve SunIntensityCurve { get; private set; }

        /// <summary> Максимальная интенсивность солнечного света. </summary>
        [field: Tooltip("Максимальная интенсивность солнечного света."), SerializeField]
        public float MaxSunIntensity { get; private set; } = 4f;

        /// <summary> Углы поворота солнца по оси X (утро, полдень, вечер). </summary>
        [field: Tooltip("Углы поворота солнца по оси X (утро, полдень, вечер)."), SerializeField]
        public Vector3 SunAngleX { get; private set; }

        /// <summary> Углы поворота солнца по оси Y до полудня (утро, полдень). </summary>
        [field: Tooltip("Углы поворота солнца по оси Y до полудня (утро, полдень)."), SerializeField]
        public Vector2 SunAngleYBefore12 { get; private set; }

        /// <summary> Углы поворота солнца по оси Y после полудня (полдень, вечер). </summary>
        [field: Tooltip("Углы поворота солнца по оси Y после полудня (полдень, вечер)."), SerializeField]
        public Vector2 SunAngleYAfter12 { get; private set; }

        /// <summary> Тип теней от солнца (Hard/Soft/None). </summary>
        [field: Header("Sun Shadow Settings")] [field: Tooltip("Тип теней от солнца (Hard/Soft/None)."), SerializeField]
        public LightShadows SunShadowType = LightShadows.Hard;

        /// <summary> Сила теней от солнца (0-1). </summary>
        [field: Tooltip("Сила теней от солнца (0-1)."), SerializeField, Range(0, 1)]
        public float SunShadowStrength { get; private set; } = 0.9f;

        /// <summary> Настройки цвета луны в течение ночи (градиент). </summary>
        [field: Header("Moon Settings")]
        [field: Tooltip("Настройки цвета луны в течение ночи (градиент)."), SerializeField]
        public Gradient MoonColorGradient { get; private set; }

        /// <summary> Кривая интенсивности лунного света в течение ночи. </summary>
        [field: Tooltip("Кривая интенсивности лунного света в течение ночи."), SerializeField]
        public AnimationCurve MoonIntensityCurve { get; private set; }

        /// <summary> Максимальная интенсивность лунного света. </summary>
        [field: Tooltip("Максимальная интенсивность лунного света."), SerializeField]
        public float MaxMoonIntensity { get; private set; } = 2f;

        /// <summary> Тип теней от луны (Hard/Soft/None). </summary>
        [field: Header("Moon Shadow Settings")]
        [field: Tooltip("Тип теней от луны (Hard/Soft/None)."), SerializeField]
        public LightShadows MoonShadowType { get; private set; } = LightShadows.Soft;

        /// <summary> Сила теней от луны (0-1). </summary>
        [field: Tooltip("Сила теней от луны (0-1)."), SerializeField, Range(0, 1)]
        public float MoonShadowStrength { get; private set; } = 0.6f;
    }
}