using System.Collections.Generic;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.TimeManagement;
using FlavorfulStory.Visuals.Lightning;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.Visuals.Weather
{
    /// <summary> Генератор погоды, управляющий погодой и их влиянием на освещение. </summary>
    public class WeatherGenerator : MonoBehaviour
    {
        /// <summary> Массив настроек погоды с их вероятностями появления. </summary>
        [SerializeField]
        private WeatherSettings[] _weatherSettings;

        /// <summary> Родительский объект партиклов. </summary>
        [SerializeField] private GameObject _particleParent;

        /// <summary> Текущие активные погодные условия для сегодняшнего дня. </summary>
        private WeatherSettings _currentWeather;

        /// <summary> Словарь, связывающий дни с конкретными погодными условиями. </summary>
        private readonly Dictionary<int, WeatherSettings> _weatherByDay = new();

        /// <summary> Система глобального освещения. </summary>
        private GlobalLightSystem _globalLightSystem;

        /// <summary> Менеджер локаций. </summary>
        private LocationManager _locationManager;

        /// <summary> Внедряет зависимости Zenject. </summary>
        /// <param name="globalLightSystem"> Система глобального освещения. </param>
        /// <param name="locationManager"> Менеджер локаций. </param>
        [Inject]
        private void Construct(GlobalLightSystem globalLightSystem, LocationManager locationManager)
        {
            _globalLightSystem = globalLightSystem;
            _locationManager = locationManager;
        }

        /// <summary> Подписывается на события. </summary>
        private void Awake()
        {
            _locationManager.OnLocationChanged += UpdateParticleVisibility;
            WorldTime.OnDayEnded += ApplyWeatherForDay;
        }

        /// <summary> Применяет погоду сразу после старта сцены. </summary>
        private void Start() => ApplyWeatherForDay(WorldTime.CurrentGameTime);

        /// <summary> Отписывается от событий при уничтожении компонента. </summary>
        private void OnDestroy()
        {
            WorldTime.OnDayEnded -= ApplyWeatherForDay;
            _locationManager.OnLocationChanged -= UpdateParticleVisibility;
        }

        /// <summary> Применяет погоду на конкретный день, генерируя её при необходимости. </summary>
        /// <param name="dateTime"> Дата текущего дня. </param>
        private void ApplyWeatherForDay(DateTime dateTime)
        {
            int day = dateTime.TotalDays;
            if (!_weatherByDay.ContainsKey(day)) _weatherByDay[day] = GenerateRandomWeather();

            _currentWeather = _weatherByDay[day];

            _globalLightSystem.SetLightSettings(_currentWeather.WeatherType);
            UpdateWeatherParticles();
        }

        /// <summary> Активирует или отключает партиклы в зависимости от текущей погоды. </summary>
        private void UpdateWeatherParticles()
        {
            foreach (var weather in _weatherSettings)
                if (weather.ParticleObject)
                    weather.ParticleObject.gameObject.SetActive(weather == _currentWeather);
        }

        /// <summary> Включает или отключает партиклы в зависимости от помещения. </summary>
        /// <param name="location"> Локация, в которой находится игрок. </param>
        private void UpdateParticleVisibility(Location location)
        {
            if (_particleParent) _particleParent.SetActive(!location.IsRoom);
        }

        /// <summary> Генерирует случайную погоду на основе вероятностей в настройках. </summary>
        /// <remarks> Использует взвешенную случайную выборку для создания разнообразных погодных условий. </remarks>
        /// <returns> Случайно выбранные настройки погоды согласно заданным вероятностям. </returns>
        private WeatherSettings GenerateRandomWeather()
        {
            float randomValue = Random.value;
            float cumulativeProbability = 0f;
            foreach (var weatherSetting in _weatherSettings)
            {
                cumulativeProbability += weatherSetting.Probability;
                if (randomValue <= cumulativeProbability) return weatherSetting;
            }

            return _weatherSettings[0];
        }
    }
}