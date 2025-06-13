using System.Collections.Generic;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.TimeManagement;
using FlavorfulStory.Visuals.Lightning;
using UnityEngine;
using Zenject;
using DateTime = FlavorfulStory.TimeManagement.DateTime;
using Random = UnityEngine.Random;

namespace FlavorfulStory.Visuals.Weather
{
    /// <summary>Генератор погоды, управляющий ежедневной генерацией погодных условий и их влиянием на освещение.
    /// Использует вероятностную систему для создания разнообразных погодных условий и синхронизации
    /// с системой глобального освещения. </summary>
    public class WeatherGenerator : MonoBehaviour
    {
        /// <summary>Массив настроек погоды с их вероятностями появления.
        /// Каждый элемент содержит тип погоды, вероятность и связанные эффекты.</summary>
        [Header("Weather Settings")] [SerializeField]
        private WeatherSettings[] _weatherSettings;

        /// <summary> Ссылка на родительский обхект партиклов. </summary>
        [SerializeField] private GameObject _particleParent;

        /// <summary>Текущие активные погодные условия для сегодняшнего дня. </summary>
        private WeatherSettings _currentWeather;

        /// <summary> Кэш сгенерированной погоды по дням для обеспечения консистентности.
        /// Ключ - номер дня, значение - настройки погоды для этого дня. </summary>
        private readonly Dictionary<int, WeatherSettings> _dailyWeather = new();

        /// <summary> Ссылка на систему глобального освещения для синхронизации погодных эффектов. </summary>
        private GlobalLightSystem _globalLightSystem;

        /// <summary> Менеджер локаций. </summary>
        private LocationManager _locationManager;

        [Inject]
        private void Construct(LocationManager locationManager, GlobalLightSystem globalLightSystem)
        {
            _locationManager = locationManager;
            _globalLightSystem = globalLightSystem;
        }

        /// <summary> Подписывается на событие окончания дня для генерации погоды следующего дня. </summary>
        private void OnEnable()
        {
            WorldTime.OnDayEnded += GenerateDailyWeather;
            SubscribeToLocationEvent();
        }

        /// <summary> Отписывается от событий при отключении компонента. </summary>
        private void OnDisable()
        {
            WorldTime.OnDayEnded -= GenerateDailyWeather;
            UnsubscribeFromLocationEvent();
        }

        /// <summary> Генерирует погоду для текущего дня при запуске. </summary>
        private void Awake() => GenerateDailyWeather(WorldTime.CurrentGameTime);

        /// <summary> Включатель партиклов. </summary>
        /// <param name="location"> Локация, в которой находится игрок. </param>
        private void ToggleParticles(Location location)
        {
            if (!_particleParent) return;

            _particleParent.SetActive(location.IsRoom == false);
        }


        /// <summary> Генерирует и активирует погоду для указанного дня.
        /// Обновляет систему освещения и управляет видимостью погодных эффектов. </summary>
        /// <param name="gameTime"> Игровое время для генерации погоды. </param>
        private void GenerateDailyWeather(DateTime gameTime)
        {
            GenerateWeatherForDay(gameTime);
            _currentWeather = _dailyWeather[(int)gameTime.TotalDays];
            _globalLightSystem.SetNewLightSettings(_currentWeather.WeatherType);

            foreach (var weather in _weatherSettings)
                if (weather.ParticleObject)
                    weather.ParticleObject.SetActive(false);

            if (_currentWeather.ParticleObject) _currentWeather.ParticleObject.SetActive(true);
        }

        /// <summary> Генерирует погодные условия для конкретного дня, если они еще не существуют.
        /// Использует кэширование для обеспечения консистентности погоды в течение дня. </summary>
        /// <param name="date"> Дата для генерации погоды. </param>
        private void GenerateWeatherForDay(DateTime date)
        {
            int newDate = (int)date.TotalDays;
            _dailyWeather.TryAdd(newDate, GenerateRandomWeather());
        }

        /// <summary> Генерирует случайную погоду на основе вероятностей в настройках.
        /// Использует взвешенную случайную выборку для создания разнообразных погодных условий. </summary>
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

        private void SubscribeToLocationEvent()
        {
            if (_locationManager == null) return;
            _locationManager.OnLocationChanged += ToggleParticles;
        }

        private void UnsubscribeFromLocationEvent()
        {
            if (_locationManager == null) return;
            _locationManager.OnLocationChanged -= ToggleParticles;
        }
    }
}