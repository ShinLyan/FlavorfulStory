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

        /// <summary> Событие при смене локации. </summary>
        public event Action<Location> OnLocationChanged;

        /// <summary> Конструктор с внедрением зависимостей. </summary>
        /// <param name="playerController"> Контроллер игрока. </param>
        /// <param name="locations"> Список всех доступных локаций. </param>
        public LocationManager(PlayerController playerController, List<Location> locations)
        {
            _playerController = playerController;
            _locations = locations;
        }

        /// <summary> При инициализации активирует текущую локацию игрока. </summary>
        public void Initialize() => UpdateActiveLocation();

        /// <summary> Активирует локацию, в которой находится игрок, и деактивирует все остальные. </summary>
        public void UpdateActiveLocation()
        {
            var playerPosition = _playerController.transform.position;
            var currentLocation = FindLocationByPosition(playerPosition);
            foreach (var location in _locations) location.SetActive(location == currentLocation);

            if (currentLocation) OnLocationChanged?.Invoke(currentLocation);
        }

        private Location FindLocationByPosition(Vector3 position) =>
            _locations.SingleOrDefault(location => location.IsPositionInLocation(position));
    }
}