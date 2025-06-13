using System;
using System.Collections.Generic;
using FlavorfulStory.Player;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.SceneManagement
{
    /// <summary> Менеджер локаций. </summary>
    /// <remarks> Отвечает за активацию и деактивацию локаций. </remarks>
    public class LocationManager : IInitializable
    {
        /// <summary> Контроллер игрока. </summary>
        private readonly PlayerController _playerController;

        /// <summary> Список всех локаций в сцене. </summary>
        private readonly List<Location> _locations;

        /// <summary> Словарь локаций по имени для быстрого доступа. </summary>
        private readonly Dictionary<LocationName, Location> _locationByName;

        /// <summary> Событие при смене локации. </summary>
        public event Action<Location> OnLocationChanged;

        /// <summary> Конструктор с внедрением зависимостей. </summary>
        /// <param name="playerController"> Контроллер игрока. </param>
        /// <param name="locations"> Список всех доступных локаций. </param>
        public LocationManager(PlayerController playerController, List<Location> locations)
        {
            _playerController = playerController;
            _locations = locations;
            _locationByName = new Dictionary<LocationName, Location>();
        }

        /// <summary> Инициализирует словарь локаций и активирует текущую локацию игрока. </summary>
        public void Initialize()
        {
            foreach (var location in _locations)
                if (!_locationByName.TryAdd(location.LocationName, location))
                    Debug.LogError($"Дубликат локации: {location.LocationName} в {location.name}");

            ActivatePlayerCurrentLocation();
        }

        /// <summary> Активирует локацию, в которой находится игрок, и деактивирует все остальные. </summary>
        public void ActivatePlayerCurrentLocation()
        {
            var playerPosition = _playerController.transform.position;
            foreach (var location in _locations)
                if (location.IsPositionInLocation(playerPosition))
                    location.Enable();
                else
                    location.Disable();
        }

        /// <summary> Включить локацию по имени. </summary>
        /// <param name="name"> Имя локации, которую требуется включить. </param>
        public void EnableLocation(LocationName name)
        {
            if (_locationByName.TryGetValue(name, out var location))
            {
                location.Enable();
                OnLocationChanged?.Invoke(location);
            }
            else
            {
                Debug.LogError($"Локации {name} не существует!");
            }
        }

        /// <summary> Отключить локацию по имени. </summary>
        /// <param name="name"> Имя локации, которую требуется отключить. </param>
        public void DisableLocation(LocationName name)
        {
            if (_locationByName.TryGetValue(name, out var location))
                location.Disable();
            else
                Debug.LogError($"Локации {name} не существует!");
        }
    }
}