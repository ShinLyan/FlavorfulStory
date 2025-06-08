using System;
using System.Collections.Generic;
using FlavorfulStory.TimeManagement;
using UnityEngine;
using DateTime = FlavorfulStory.TimeManagement.DateTime;
using Random = UnityEngine.Random;

namespace FlavorfulStory.Lightning
{
    public class DailyWeatherGenerator : MonoBehaviour
    {
        [Serializable]
        public struct WeatherSettings
        {
            public WeatherType WeatherType;
            [Range(0f, 1f)] public float Probability;
            public GameObject Particles;
        }

        [Header("Weather Probabilities")] [SerializeField]
        private WeatherSettings[] _weatherSettings;

        private WeatherSettings _currentWeather;

        private readonly Dictionary<int, WeatherSettings> _dailyWeather = new();

        [SerializeField] private GlobalLightSystem _globalLightSystem;

        private void OnEnable() => WorldTime.OnDayEnded += GenerateDailyWeather;

        private void OnDisable() => WorldTime.OnDayEnded -= GenerateDailyWeather;


        private void Awake() => GenerateDailyWeather(WorldTime.CurrentGameTime);

        public WeatherType GetCurrentWeatherType() => _currentWeather.WeatherType;

        private void GenerateDailyWeather(DateTime gameTime)
        {
            GenerateWeatherForDay(gameTime);
            _currentWeather = _dailyWeather[(int)gameTime.TotalDays];
            _globalLightSystem.SetNewLightSettings(_currentWeather.WeatherType);

            foreach (var weather in _weatherSettings)
                if (weather.Particles)
                    weather.Particles.SetActive(false);

            if (_currentWeather.Particles) _currentWeather.Particles.SetActive(true);
        }

        private void GenerateWeatherForDay(DateTime date)
        {
            int newDate = (int)date.TotalDays;
            _dailyWeather.TryAdd(newDate, GenerateRandomWeather());
        }

        private WeatherSettings GenerateRandomWeather()
        {
            float randomValue = Random.value;
            float cumulativeProbability = 0f;

            foreach (var wp in _weatherSettings)
            {
                cumulativeProbability += wp.Probability;
                if (randomValue <= cumulativeProbability) return wp;
            }

            return _weatherSettings[0];
        }
    }
}