using FlavorfulStory.TimeManagement;
using UnityEngine;

namespace FlavorfulStory.Visuals.Lightning
{
    /// <summary>
    /// Контроллер солнечного света, управляющий поведением солнца в течение дневного цикла.
    /// Реализует логику активации, позиционирования и настройки теней для солнечного освещения.
    /// </summary>
    public class SunLightController : ILightController
    {
        /// <summary> Время начала активности солнца (6:00 утра). </summary>
        private const float StartTime = 6f;

        /// <summary> Время окончания активности солнца (18:00 вечера). </summary>
        private const float EndTime = 18f;

        /// <summary> Точка полудня в нормализованном времени (0.5 от общего дневного цикла). </summary>
        private const float Midday = 0.5f;

        /// <summary> Время начала ослабления теней солнца (17:30). </summary>
        private const float OffShadowsHour = 17.5f;

        /// <summary> Минимальное смещение теней для предотвращения артефактов рендеринга. </summary>
        private const float MinimalShadowBias = 0.5f;

        /// <summary> Текущий час для внутренних расчетов контроллера. </summary>
        private float _currentHour;

        /// <summary> Определяет, активно ли солнце в указанное время. Солнце активно с 6:00 до 18:00. </summary>
        /// <param name="time"> Текущее игровое время для проверки. </param>
        /// <returns> True, если солнце должно быть активным; иначе false. </returns>
        public bool IsActive(DateTime time)
        {
            _currentHour = time.Hour;
            return _currentHour >= StartTime && _currentHour < EndTime;
        }

        /// <summary> Вычисляет прогресс солнечного цикла от восхода до заката. </summary>
        /// <param name="time"> Текущее игровое время. </param>
        /// <returns> Нормализованный прогресс дневного цикла (0.0 - 1.0). </returns>
        public float CalculateProgress(DateTime time) => Mathf.InverseLerp(StartTime, EndTime, time.Hour);

        /// <summary> Создает конфигурацию солнечного света с настройками цвета, интенсивности, теней и поворота.
        /// Включает стратегии для динамического изменения силы теней и позиции солнца в течение дня. </summary>
        /// <param name="time"> Текущее игровое время. </param>
        /// <param name="light"> Источник солнечного света для настройки. </param>
        /// <param name="settings"> Настройки освещения, зависящие от погодных условий. </param>
        /// <returns> Полная конфигурация солнечного света с параметрами и стратегиями поведения. </returns>
        public LightConfig CreateLightConfig(DateTime time, Light light, LightSettings settings)
        {
            _currentHour = time.Hour;
            return new LightConfig
            {
                LightSource = light,
                ColorGradient = settings.SunColorGradient,
                IntensityCurve = settings.SunIntensityCurve,
                MaxIntensity = settings.MaxSunIntensity,
                ShadowType = settings.SunShadowType,
                ShadowStrength = settings.SunShadowStrength,
                ShadowStrategy = progress =>
                {
                    light.shadowBias = MinimalShadowBias;
                    if (_currentHour >= OffShadowsHour)
                    {
                        float shadowProgress = Mathf.InverseLerp(OffShadowsHour, EndTime, _currentHour);
                        light.shadowStrength = Mathf.Lerp(settings.SunShadowStrength, 0f, shadowProgress);
                    }
                    else
                    {
                        light.shadowStrength = settings.SunShadowStrength;
                    }
                },
                RotationStrategy = progress =>
                {
                    float angleX = progress < Midday
                        ? Mathf.Lerp(settings.SunAngleX.x, settings.SunAngleX.y, progress)
                        : Mathf.Lerp(settings.SunAngleX.y, settings.SunAngleX.z, progress);

                    float angleY = progress < Midday
                        ? Mathf.Lerp(settings.SunAngleYBefore12.x, settings.SunAngleYBefore12.y, progress)
                        : Mathf.Lerp(settings.SunAngleYAfter12.x, settings.SunAngleYAfter12.y, progress);

                    light.transform.rotation = Quaternion.Euler(angleX, angleY, 0f);
                }
            };
        }
    }
}