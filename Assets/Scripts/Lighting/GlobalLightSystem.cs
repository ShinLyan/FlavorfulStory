using FlavorfulStory.TimeManagement;
using UnityEngine;
using DateTime = FlavorfulStory.TimeManagement.DateTime;

namespace FlavorfulStory.Lightning
{
    /// <summary> Система глобального освещения, управляющая поведением солнечного и лунного света в зависимости от
    /// времени суток и погодных условий. </summary>  
    public class GlobalLightSystem : MonoBehaviour
    {
        /// <summary> Источник света, представляющий солнце. </summary> 
        [SerializeField] private Light _sunLight;

        /// <summary> Источник света, представляющий луну. </summary> 
        [SerializeField] private Light _moonLight;

        /// <summary> Массив настроек освещения для различных погодных условий. </summary> 
        [SerializeField] private WeatherLightSettings[] _weatherLightSettings;

        /// <summary> Текущие настройки освещения в зависимости от погоды. </summary> 
        private LightSettings _currentWeatherLightSettings;

        /// <summary> Время начала дня (6:00). </summary> 
        private const float DayStartTime = 6f;

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

        private const float MoonAngleYStart = -45f;

        private const float MoonAngleYEnd = 45f;

        private const float MoonAngleX = 30f;


        /// <summary> Подписывается на событие обновления времени при активации. </summary> 
        private void OnEnable() => WorldTime.OnTimeUpdated += UpdateLighting;

        /// <summary> Отписывается от события обновления времени при деактивации. </summary> 
        private void OnDisable() => WorldTime.OnTimeUpdated -= UpdateLighting;

        /// <summary> Инициализирует начальные настройки освещения. </summary> 
        private void Start()
        {
            _currentWeatherLightSettings = _weatherLightSettings[0].LightSettings;
            //TODO: добавить определение текущей погоды из стороннего скрипта
        }

        /// <summary> Обновляет освещение в соответствии с текущим игровым временем. </summary> 
        /// <param name="gameTime"> Текущее игровое время. </param>
        private void UpdateLighting(DateTime gameTime)
        {
            UpdateSun(gameTime);
            UpdateMoon(gameTime);
        }

        /// <summary> Обновляет параметры солнечного света. </summary> 
        /// <param name="gameTime"> Текущее игровое время. </param>
        private void UpdateSun(DateTime gameTime)
        {
            float currentTimeInHours = gameTime.Hour;

            bool isSunActive = currentTimeInHours >= DayStartTime && currentTimeInHours < NightStartTime;
            _sunLight.enabled = isSunActive;

            if (!isSunActive) return;

            SetSunShadows(currentTimeInHours);

            float sunProgress = Mathf.InverseLerp(DayStartTime, NightStartTime, currentTimeInHours);
            RotateSun(sunProgress);
            ColorizeSun(sunProgress);
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

        /// <summary> Изменяет цвет и интенсивность солнечного света. </summary> 
        /// <param name="sunProgress"> Прогресс дня от 0 до 1. </param>
        private void ColorizeSun(float sunProgress)
        {
            _sunLight.color = _currentWeatherLightSettings.SunColorGradient.Evaluate(sunProgress);
            _sunLight.intensity =
                _currentWeatherLightSettings.SunIntensityCurve.Evaluate(sunProgress) *
                _currentWeatherLightSettings.MaxSunIntensity;
        }

        /// <summary> Обновляет параметры лунного света. </summary> 
        /// <param name="gameTime"> Текущее игровое время. </param>
        private void UpdateMoon(DateTime gameTime)
        {
            float currentTimeInHours = gameTime.Hour;

            bool isMoonActive = currentTimeInHours >= MoonStartTime || currentTimeInHours < DayStartTime;
            _moonLight.enabled = isMoonActive;

            if (!isMoonActive) return;

            SetMoonShadows(currentTimeInHours);

            float moonProgress = Mathf.InverseLerp(MoonStartTime, HoursInDay + 2f,
                currentTimeInHours >= MoonStartTime ? currentTimeInHours : currentTimeInHours + HoursInDay);
            RotateMoon(moonProgress);
            ColorizeMoon(moonProgress);
        }

        /// <summary> Устанавливает параметры теней для луны. </summary> 
        /// <param name="currentTimeInHours"> Текущее время в часах. </param>
        private void SetMoonShadows(float currentTimeInHours)
        {
            _moonLight.shadows = _currentWeatherLightSettings.MoonShadowType;
            _moonLight.shadowNormalBias = MoonShadowNormalBias;

            float nightWithShadowTime = NightStartTime + 1f;

            if (DayStartTime < currentTimeInHours && currentTimeInHours < NightStartTime)
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

        /// <summary> Изменяет цвет и интенсивность лунного света. </summary> 
        /// <param name="moonProgress"> Прогресс ночи от 0 до 1. </param>
        private void ColorizeMoon(float moonProgress)
        {
            _moonLight.color = _currentWeatherLightSettings.MoonColorGradient.Evaluate(moonProgress);
            _moonLight.intensity =
                _currentWeatherLightSettings.MoonIntensityCurve.Evaluate(moonProgress) *
                _currentWeatherLightSettings.MaxMoonIntensity;
        }
    }
}