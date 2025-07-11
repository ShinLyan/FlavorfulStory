using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary> При инициализации активирует текущую локацию игрока. </summary>
        public void Initialize()
        {
            foreach (var location in _locations)
                if (!_locationByName.TryAdd(location.LocationName, location))
                    Debug.LogError($"Дубликат локации: {location.LocationName} в {location.name}");

            UpdateActiveLocation();
        }

        /// <summary> Активирует локацию, в которой находится игрок, и деактивирует все остальные. </summary>
        public void UpdateActiveLocation()
        {
            var playerPosition = _playerController.transform.position;
            var currentLocation = FindLocationByPosition(playerPosition);
            foreach (var location in _locations) location.SetActive(location == currentLocation);

            if (currentLocation) OnLocationChanged?.Invoke(currentLocation);
        }

        /// <summary> Найти локацию по текущей позиции игрока в мире. </summary>
        /// <param name="position"> Позиция игрока в мире. </param>
        /// <returns> Локация, в которой находится игрок. </returns>
        private Location FindLocationByPosition(Vector3 position) =>
            _locations.SingleOrDefault(location => location.IsPositionInLocation(position));

        /// <summary> Включить локацию по имени. </summary>
        /// <param name="name"> Имя локации, которую требуется включить. </param>
        public void EnableLocation(LocationName name)
        {
            if (_locationByName.TryGetValue(name, out var location))
                location.SetActive(true);
            else
                Debug.LogError($"Локации {name} не существует!");
        }

        /// <summary> Отключить локацию по имени. </summary>
        /// <param name="name"> Имя локации, которую требуется отключить. </param>
        public void DisableLocation(LocationName name)
        {
            if (_locationByName.TryGetValue(name, out var location))
                location.SetActive(false);
            else
                Debug.LogError($"Локации {name} не существует!");
        }
    }
}