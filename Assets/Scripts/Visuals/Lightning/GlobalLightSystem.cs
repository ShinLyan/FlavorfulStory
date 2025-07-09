using FlavorfulStory.TimeManagement;
using FlavorfulStory.Visuals.Weather;
using UnityEngine;

namespace FlavorfulStory.Visuals.Lightning
{
    /// <summary> Система глобального освещения, управляющая поведением солнечного и лунного света в зависимости от
    /// времени суток и погодных условий. </summary>  
    public class GlobalLightSystem : MonoBehaviour
    {
        /// <summary> Источник света, представляющий солнце в игровом мире. </summary>
        [SerializeField] private Light _sunLight;

        /// <summary> Источник света, представляющий луну в игровом мире. </summary>
        [SerializeField] private Light _moonLight;

        /// <summary> Массив настроек освещения для различных типов погоды.</summary>
        [SerializeField] private WeatherLightSettings[] _weatherLightSettings;

        /// <summary> Текущие настройки освещения, соответствующие активному типу погоды. </summary>
        private LightSettings _currentWeatherLightSettings;

        /// <summary> Контроллер, управляющий поведением солнечного света. </summary>
        private ILightController _sunController;

        /// <summary> Контроллер, управляющий поведением лунного света. </summary>
        private ILightController _moonController;

        /// <summary> Инициализирует контроллеры солнечного и лунного света при создании объекта. </summary>
        private void Awake()
        {
            _sunController = new SunLightController();
            _moonController = new MoonLightController();
        }

        /// <summary> Подписывается на событие обновления времени при активации объекта. </summary>
        private void OnEnable() => WorldTime.OnTimeUpdated += UpdateLighting;

        /// <summary> Отписывается от события обновления времени при деактивации объекта. </summary>
        private void OnDisable() => WorldTime.OnTimeUpdated -= UpdateLighting;

        /// <summary> Устанавливает новые настройки освещения в соответствии с указанным типом погоды. </summary>
        /// <param name="weatherType"> Тип погоды для применения соответствующих настроек освещения. </param>
        public void SetLightSettings(WeatherType weatherType)
        {
            foreach (var weather in _weatherLightSettings)
                if (weather.WeatherType == weatherType)
                    _currentWeatherLightSettings = weather.LightSettings;
        }

        /// <summary> Обновляет освещение солнца и луны в соответствии с текущим игровым временем.
        /// Вызывается при каждом обновлении времени в игре. </summary>
        /// <param name="gameTime"> Текущее игровое время. </param>
        private void UpdateLighting(DateTime gameTime)
        {
            ApplyLighting(_sunController, _sunLight, gameTime);
            ApplyLighting(_moonController, _moonLight, gameTime);
        }

        /// <summary> Применяет настройки освещения к указанному источнику света
        /// с использованием соответствующего контроллера. </summary>
        /// <param name="controller"> Контроллер света (солнца или луны). </param>
        /// <param name="lightSource"> Источник света для настройки. </param>
        /// <param name="time"> Текущее время для расчета параметров освещения. </param>
        private void ApplyLighting(ILightController controller, Light lightSource, DateTime time)
        {
            if (_currentWeatherLightSettings == null) return;

            bool isActive = controller.IsActive(time);
            float progress = controller.CalculateProgress(time);
            var config = controller.CreateLightConfig(time, lightSource, _currentWeatherLightSettings);
            UpdateLight(config, progress, isActive);
        }

        /// <summary> Обновляет параметры источника света в соответствии с переданной конфигурацией.
        /// Устанавливает состояние активности, тени, цвет и интенсивность света. </summary>
        /// <param name="config"> Конфигурация света с настройками цвета, интенсивности, теней и стратегий. </param>
        /// <param name="progress"> Прогресс времени от 0 до 1 для интерполяции параметров. </param>
        /// <param name="isActive"> Определяет, должен ли источник света быть активным. </param>
        private static void UpdateLight(LightConfig config, float progress, bool isActive)
        {
            var light = config.LightSource;
            light.enabled = isActive;
            if (!isActive) return;

            light.shadows = config.ShadowType;
            config.ShadowStrategy?.Invoke(progress);
            config.RotationStrategy?.Invoke(progress);

            light.color = config.ColorGradient.Evaluate(progress);
            light.intensity = config.IntensityCurve.Evaluate(progress) * config.MaxIntensity;
        }
    }
}