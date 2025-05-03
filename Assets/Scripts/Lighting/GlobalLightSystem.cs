using System;
using FlavorfulStory.TimeManagement;
using UnityEngine;
using DateTime = FlavorfulStory.TimeManagement.DateTime;

namespace FlavorfulStory.Lightning
{
    /// <summary> Система глобального освещения, управляющая поведением солнечного и лунного света в зависимости от
    /// времени суток и погодных условий. </summary>  
    public class GlobalLightSystem : MonoBehaviour
    {
        #region Fields

        /// <summary> Источник света, представляющий солнце. </summary> 
        [SerializeField] private Light _sunLight;

        /// <summary> Источник света, представляющий луну. </summary> 
        [SerializeField] private Light _moonLight;

        /// <summary> Массив настроек освещения для различных погодных условий. </summary> 
        [SerializeField] private WeatherLightSettings[] _weatherLightSettings;

        /// <summary> Текущие настройки освещения в зависимости от погоды. </summary> 
        private LightSettings _currentWeatherLightSettings;

        /// <summary> Время начала дня (6:00). </summary> 
        private const float SunStartTime = 6f;

        /// <summary> Время начала ночи (18:00). </summary> 
        private const float NightStartTime = 18f;

        /// <summary> Время появления луны (за 1 час до наступления ночи). </summary> 
        private const float MoonStartTime = NightStartTime - 1f;

        /// <summary> Постоянное смещение теневого отображения. </summary>
        private const float MinimalShadowBias = 0.01f;

        /// <summary> Время выключения теней от солнца. </summary>
        private const float OffSunShadowsHour = 17.5f;

        /// <summary> Полдень. </summary>
        private const float Midday = 0.5f;

        /// <summary> Количество часов в сутках. </summary>
        private const float HoursInDay = 24f;

        /// <summary> Смещение, основанное на нормальном отображении теней. </summary>
        private const float MoonShadowNormalBias = 0.4f;

        /// <summary> Начальный угол поворота луны по оси Y. </summary>
        private const float MoonAngleYStart = -45f;

        /// <summary> Конечный угол поворота луны по оси Y. </summary>
        private const float MoonAngleYEnd = 45f;

        /// <summary> Угол поворота луны по оси X. </summary>
        private const float MoonAngleX = 30f;

        #endregion

        /// <summary> Подписывается на событие обновления времени при активации. </summary> 
        private void OnEnable() => WorldTime.OnTimeUpdated += UpdateLighting;

        /// <summary> Отписывается от события обновления времени при деактивации. </summary> 
        private void OnDisable() => WorldTime.OnTimeUpdated -= UpdateLighting;

        /// <summary> Инициализирует начальные настройки освещения. </summary> 
        private void Awake() // в Awake из-за того что если в Старте, то возникает NullRefException
        {
            _currentWeatherLightSettings = _weatherLightSettings[0].LightSettings;
            //TODO: добавить определение текущей погоды из стороннего скрипта
        }

        /// <summary> Обновляет параметры освещения в зависимости от времени суток. </summary>
        /// <param name="gameTime">Текущее игровое время.</param>
        private void UpdateLighting(DateTime gameTime)
        {
            UpdateLight(
                _sunLight,
                gameTime,
                gameTime.Hour >= SunStartTime && gameTime.Hour < NightStartTime,
                SunStartTime,
                NightStartTime,
                0f,
                _currentWeatherLightSettings.SunColorGradient,
                _currentWeatherLightSettings.SunIntensityCurve,
                _currentWeatherLightSettings.MaxSunIntensity,
                _ => SetSunShadows(gameTime.Hour),
                RotateSun,
                _currentWeatherLightSettings.SunShadowType
            );

            UpdateLight(
                _moonLight,
                gameTime,
                gameTime.Hour >= MoonStartTime || gameTime.Hour < SunStartTime,
                MoonStartTime,
                HoursInDay + 2f,
                gameTime.Hour >= MoonStartTime ? 0f : HoursInDay,
                _currentWeatherLightSettings.MoonColorGradient,
                _currentWeatherLightSettings.MoonIntensityCurve,
                _currentWeatherLightSettings.MaxMoonIntensity,
                _ => SetMoonShadows(gameTime.Hour),
                RotateMoon,
                _currentWeatherLightSettings.MoonShadowType
            );
        }

        /// <summary> Обновляет параметры источника света. </summary>
        /// <param name="light"> Источник света для обновления. </param>
        /// <param name="gameTime"> Текущее игровое время. </param>
        /// <param name="isActiveCondition"> Условие активности света. </param>
        /// <param name="startTime"> Время начала активности. </param>
        /// <param name="endTime"> Время окончания активности. </param>
        /// <param name="progressOffset"> Смещение прогресса. </param>
        /// <param name="colorGradient"> Градиент цвета света. </param>
        /// <param name="intensityCurve"> Кривая интенсивности света. </param>
        /// <param name="maxIntensity"> Максимальная интенсивность света. </param>
        /// <param name="shadowSetup"> Метод настройки теней. </param>
        /// <param name="rotationSetup"> Метод настройки вращения. </param>
        /// <param name="shadowType"> Тип теней. </param>
        private static void UpdateLight(
            Light light,
            DateTime gameTime,
            bool isActiveCondition,
            float startTime,
            float endTime,
            float progressOffset,
            Gradient colorGradient,
            AnimationCurve intensityCurve,
            float maxIntensity,
            Action<Light> shadowSetup,
            Action<float> rotationSetup,
            LightShadows shadowType)
        {
            light.enabled = isActiveCondition;
            if (!isActiveCondition) return;

            light.shadows = shadowType;
            shadowSetup(light);

            float currentTime = gameTime.Hour + progressOffset;
            float progress = Mathf.InverseLerp(startTime, endTime, currentTime);

            rotationSetup(progress);
            ColorizeLight(light, progress, colorGradient, intensityCurve, maxIntensity);
        }

        /// <summary> Устанавливает параметры теней для солнца. </summary> 
        /// <param name="currentTimeInHours"> Текущее время в часах. </param>
        private void SetSunShadows(float currentTimeInHours)
        {
            _sunLight.shadows = _currentWeatherLightSettings.SunShadowType;
            _sunLight.shadowBias = MinimalShadowBias;

            if (currentTimeInHours >= OffSunShadowsHour) // убираем тени если время 17:30 - 18:00
            {
                float shadowProgress = Mathf.InverseLerp(OffSunShadowsHour, NightStartTime, currentTimeInHours);
                _sunLight.shadowStrength =
                    Mathf.Lerp(_currentWeatherLightSettings.SunShadowStrength, 0f, shadowProgress);
            }
            else
            {
                _sunLight.shadowStrength = _currentWeatherLightSettings.SunShadowStrength;
            }
        }

        /// <summary> Вращает солнце в зависимости от времени дня. </summary> 
        /// <param name="sunProgress"> Прогресс дня от 0 до 1. </param>
        private void RotateSun(float sunProgress)
        {
            float sunAngleX;
            float sunAngleY;

            if (sunProgress < Midday) // поворачиваем до 12:00
            {
                sunAngleX = Mathf.Lerp(_currentWeatherLightSettings.SunAngleX.x,
                    _currentWeatherLightSettings.SunAngleX.y, sunProgress);
                sunAngleY = Mathf.Lerp(_currentWeatherLightSettings.SunAngleYBefore12.x,
                    _currentWeatherLightSettings.SunAngleYBefore12.y, sunProgress);
            }
            else // поворачиваем после 12:00
            {
                sunAngleX = Mathf.Lerp(_currentWeatherLightSettings.SunAngleX.y,
                    _currentWeatherLightSettings.SunAngleX.z, sunProgress);
                sunAngleY = Mathf.Lerp(_currentWeatherLightSettings.SunAngleYAfter12.x,
                    _currentWeatherLightSettings.SunAngleYAfter12.y, sunProgress);
            }

            _sunLight.transform.rotation = Quaternion.Euler(sunAngleX, sunAngleY, 0f);
        }

        /// <summary> Устанавливает параметры теней для луны. </summary> 
        /// <param name="currentTimeInHours"> Текущее время в часах. </param>
        private void SetMoonShadows(float currentTimeInHours)
        {
            _moonLight.shadows = _currentWeatherLightSettings.MoonShadowType;
            _moonLight.shadowNormalBias = MoonShadowNormalBias;

            float nightWithShadowTime = NightStartTime + 1f;

            if (SunStartTime < currentTimeInHours && currentTimeInHours < NightStartTime)
            {
                // 17:00 - 18:00 теней от луны нету
                _moonLight.shadowStrength = 0f;
            }
            else if (currentTimeInHours >= NightStartTime && currentTimeInHours < nightWithShadowTime)
            {
                // плавно включаем тени если время 18:00 - 19:00
                float shadowProgress = Mathf.InverseLerp(NightStartTime, nightWithShadowTime, currentTimeInHours);
                _moonLight.shadowStrength =
                    Mathf.Lerp(0f, _currentWeatherLightSettings.MoonShadowStrength, shadowProgress);
            }
            else
            {
                _moonLight.shadowStrength = _currentWeatherLightSettings.MoonShadowStrength;
            }
        }

        /// <summary> Вращает луну в зависимости от времени ночи. </summary> 
        /// <param name="moonProgress"> Прогресс ночи от 0 до 1. </param>
        private void RotateMoon(float moonProgress)
        {
            float moonAngleY = Mathf.Lerp(MoonAngleYStart, MoonAngleYEnd, moonProgress);
            _moonLight.transform.rotation = Quaternion.Euler(MoonAngleX, moonAngleY, 0f);
        }

        /// <summary> Изменяет цвет и интенсивность источника света. </summary>
        /// <param name="light"> Источник света (солнце или луна). </param>
        /// <param name="progress"> Прогресс дня/ночи от 0 до 1. </param>
        /// <param name="colorGradient"> Градиент цвета для изменения. </param>
        /// <param name="intensityCurve"> Кривая интенсивности света. </param>
        /// <param name="maxIntensity"> Максимальная интенсивность света. </param>
        private static void ColorizeLight(
            Light light,
            float progress,
            Gradient colorGradient,
            AnimationCurve intensityCurve,
            float maxIntensity)
        {
            light.color = colorGradient.Evaluate(progress);
            light.intensity = intensityCurve.Evaluate(progress) * maxIntensity;
        }
    }
}