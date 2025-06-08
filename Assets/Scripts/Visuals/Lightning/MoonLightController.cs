using FlavorfulStory.TimeManagement;
using UnityEngine;

namespace FlavorfulStory.Visuals.Lightning
{
    /// <summary> Контроллер лунного света, управляющий поведением луны в течение ночного цикла.
    /// Реализует логику активации, позиционирования и настройки теней для лунного освещения. </summary>
    public class MoonLightController : ILightController
    {
        /// <summary> Время начала активности луны (17:00 вечера). </summary>
        private const float StartTime = 17f;

        /// <summary> Время окончания активности луны (2:00 ночи следующего дня, представлено как 26:00). </summary>
        private const float EndTime = 26f;

        /// <summary>  Фиксированный угол наклона луны по оси X (30 градусов). </summary>
        private const float MoonAngleX = 30f;

        /// <summary> Начальный угол поворота луны по оси Y (-45 градусов). </summary>
        private const float MoonAngleYStart = -45f;

        /// <summary> Конечный угол поворота луны по оси Y (45 градусов). </summary>
        private const float MoonAngleYEnd = 45f;

        /// <summary>  Смещение нормалей для теней луны (0.4). </summary>
        private const float MoonShadowNormalBias = 0.4f;


        /// <summary> Час начала появления луны на небе (18:00). </summary>
        private const float MoonStartHour = 18f;

        /// <summary> Час достижения луной полной интенсивности освещения (19:00). </summary>
        private const float MoonFullIntensityStartHour = 19f;

        /// <summary> Час начала дневного цикла (06:00).</summary>
        private const float DayStartHour = 6f;

        /// <summary> Текущий час для внутренних расчетов контроллера. </summary>
        private float _currentHour;

        /// <summary>
        /// Определяет, активна ли луна в указанное время. Луна активна с 17:00 до 6:00 (включая переход через полночь).
        /// </summary>
        /// <param name="time"> Текущее игровое время для проверки. </param>
        /// <returns> True, если луна должна быть активной; иначе false. </returns>
        public bool IsActive(DateTime time)
        {
            _currentHour = time.Hour;
            return time.Hour >= 17f || time.Hour < 6f;
        }

        /// <summary> Вычисляет прогресс лунного цикла от появления до исчезновения.
        /// Учитывает переход через полночь, корректируя время для непрерывного расчета.
        /// Возвращает значение от 0 (появление в 17:00) до 1 (исчезновение в 2:00). </summary>
        /// <param name="time"> Текущее игровое время. </param>
        /// <returns> Нормализованный прогресс ночного цикла (0.0 - 1.0). </returns>
        public float CalculateProgress(DateTime time)
        {
            _currentHour = time.Hour;
            float adjustedHour = time.Hour >= 17f ? time.Hour : time.Hour + 24f;
            return Mathf.InverseLerp(StartTime, EndTime, adjustedHour);
        }

        /// <summary> Создает конфигурацию лунного света с настройками цвета, интенсивности, теней и поворота.
        /// Включает стратегии для плавного появления теней вечером и линейного движения луны по небу. </summary>
        /// <param name="time"> Текущее игровое время. </param>
        /// <param name="light"> Источник лунного света для настройки. </param>
        /// <param name="settings"> Настройки освещения, зависящие от погодных условий. </param>
        /// <returns> Полная конфигурация лунного света с параметрами и стратегиями поведения. </returns>
        public LightConfig CreateLightConfig(DateTime time, Light light, LightSettings settings)
        {
            _currentHour = time.Hour;
            return new LightConfig
            {
                LightSource = light,
                ColorGradient = settings.MoonColorGradient,
                IntensityCurve = settings.MoonIntensityCurve,
                MaxIntensity = settings.MaxMoonIntensity,
                ShadowType = settings.MoonShadowType,
                ShadowStrength = settings.MoonShadowStrength,
                ShadowStrategy = progress =>
                {
                    light.shadowNormalBias = MoonShadowNormalBias;

                    if (_currentHour >= DayStartHour && _currentHour < MoonStartHour) { light.shadowStrength = 0f; }
                    else if (_currentHour >= MoonStartHour && _currentHour < MoonFullIntensityStartHour)
                    {
                        float shadowProgress =
                            Mathf.InverseLerp(MoonStartHour, MoonFullIntensityStartHour, _currentHour);
                        light.shadowStrength = Mathf.Lerp(0f, settings.MoonShadowStrength, shadowProgress);
                    }
                    else { light.shadowStrength = settings.MoonShadowStrength; }
                },
                RotationStrategy = progress =>
                {
                    float angleY = Mathf.Lerp(MoonAngleYStart, MoonAngleYEnd, progress);
                    light.transform.rotation = Quaternion.Euler(MoonAngleX, angleY, 0f);
                }
            };
        }
    }
}