using System;
using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.Saving;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.TimeManagement;
using FlavorfulStory.Visuals.Lightning;
using UnityEngine;
using Zenject;
using DateTime = FlavorfulStory.TimeManagement.DateTime;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace FlavorfulStory.Visuals.Weather
{
    /// <summary> Сервис управления погодными условиями в игре. </summary>
    public class WeatherService : IInitializable, IDisposable, ISaveableService
    {
        /// <summary> Конфигурация погодных условий. </summary>
        private readonly WeatherConfig _config;

        /// <summary> Система глобального освещения. </summary>
        private readonly GlobalLightSystem _lightSystem;

        /// <summary> Менеджер локаций. </summary>
        private readonly LocationManager _locationManager;

        /// <summary> Родительский объект для частиц. </summary>
        private readonly Transform _particleParent;

        /// <summary> Словарь погодных условий по дням. </summary>
        private readonly Dictionary<int, WeatherSettings> _weatherByDay = new();

        /// <summary> Текущие погодные условия. </summary>
        private WeatherSettings _currentWeather;

        /// <summary> Активная система частиц. </summary>
        private ParticleSystem _activeParticle;

        /// <summary> Получает текущий тип погоды. </summary>
        public static WeatherType CurrentWeatherType { get; private set; }

        /// <summary> Инициализирует сервис погоды. </summary>
        /// <param name="config"> Конфигурация погоды. </param>
        /// <param name="particleParent"> Родитель для частиц. </param>
        /// <param name="lightSystem"> Система освещения. </param>
        /// <param name="locationManager"> Менеджер локаций. </param>
        public WeatherService(WeatherConfig config, Transform particleParent, GlobalLightSystem lightSystem,
            LocationManager locationManager)
        {
            _config = config;
            _particleParent = particleParent;
            _lightSystem = lightSystem;
            _locationManager = locationManager;
        }

        /// <summary> Инициализирует сервис. </summary>
        public void Initialize()
        {
            _locationManager.OnLocationChanged += UpdateParticleVisibility;
            WorldTime.OnDayEnded += ApplyWeatherForDay;

            ApplyWeatherForDay(WorldTime.CurrentGameTime);
        }

        /// <summary> Освобождает ресурсы сервиса. </summary>
        public void Dispose()
        {
            _locationManager.OnLocationChanged -= UpdateParticleVisibility;
            WorldTime.OnDayEnded -= ApplyWeatherForDay;
        }

        /// <summary> Применяет погодные условия для дня. </summary>
        /// <param name="dateTime"> Дата и время. </param>
        private void ApplyWeatherForDay(DateTime dateTime)
        {
            int day = dateTime.TotalDays;
            if (!_weatherByDay.ContainsKey(day)) _weatherByDay[day] = GenerateRandomWeather();

            _currentWeather = _weatherByDay[day];
            CurrentWeatherType = _currentWeather.WeatherType;
            _lightSystem.SetLightSettings(_currentWeather.WeatherType);
            UpdateWeatherParticles();
        }

        /// <summary> Обновляет погодные частицы. </summary>
        private void UpdateWeatherParticles()
        {
            if (_activeParticle) Object.Destroy(_activeParticle.gameObject);

            if (_currentWeather.ParticleObject)
                _activeParticle = Object.Instantiate(_currentWeather.ParticleObject, _particleParent);
        }

        /// <summary> Обновляет видимость частиц при смене локации. </summary>
        /// <param name="location"> Новая локация. </param>
        private void UpdateParticleVisibility(Location location) =>
            _particleParent.gameObject.SetActive(!location.IsRoom);

        /// <summary> Генерирует случайные погодные условия. </summary>
        /// <returns> Настройки погоды. </returns>
        private WeatherSettings GenerateRandomWeather()
        {
            float randomValue = Random.value;
            float cumulativeProbability = 0f;
            foreach (var weatherSetting in _config.WeatherSettings)
            {
                cumulativeProbability += weatherSetting.Probability;
                if (randomValue <= cumulativeProbability) return weatherSetting;
            }

            return _config.WeatherSettings[0];
        }

        #region ISaveableService

        /// <summary> Сохраняет состояние сервиса. </summary>
        /// <returns> Сохраненное состояние. </returns>
        public object CaptureState()
        {
            var save = new Dictionary<int, WeatherType>();
            foreach (var kvp in _weatherByDay) save[kvp.Key] = kvp.Value.WeatherType;

            return save;
        }

        /// <summary> Восстанавливает состояние сервиса. </summary>
        /// <param name="state"> Сохраненное состояние. </param>
        public void RestoreState(object state)
        {
            if (state is not Dictionary<int, WeatherType> records) return;

            _weatherByDay.Clear();
            foreach (var kvp in records)
            {
                var setting = _config.WeatherSettings.FirstOrDefault(weatherSettings =>
                    weatherSettings.WeatherType == kvp.Value);
                if (setting != null) _weatherByDay[kvp.Key] = setting;
            }

            ApplyWeatherForDay(WorldTime.CurrentGameTime);
        }

        #endregion
    }
}