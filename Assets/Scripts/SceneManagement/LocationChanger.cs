using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace FlavorfulStory.SceneManagement
{
    /// <summary> Управляет активацией и деактивацией локаций. </summary>
    public static class LocationChanger
    {
        /// <summary> Хранилище всех локаций, найденных в сцене. </summary>
        private static readonly Dictionary<LocationName, Location> _locations = new();

        /// <summary> Событие, вызываемое при изменении активной локации. </summary>
        public static Action<Location> OnLocationChanged;

        /// <summary> Статический конструктор — подписка на загрузку сцены. </summary>
        static LocationChanger() => SceneManager.sceneLoaded += (_, _) => InitializeLocations();

        /// <summary> Инициализация всех локаций. </summary>
        /// <remarks> Находит все <see cref="Location"/> в сцене и добавляет их в словарь.</remarks>
        private static void InitializeLocations()
        {
            _locations.Clear();

            foreach (var location in Object.FindObjectsByType<Location>(
                         FindObjectsInactive.Include, FindObjectsSortMode.None))
                if (!_locations.TryAdd(location.LocationName, location))
                    Debug.LogError($"Найден дубликат локации: {location.LocationName} в {location.name}");
        }

        /// <summary> Активирует локацию, в которой находится игрок, и деактивирует все остальные. </summary>
        public static void ActivatePlayerCurrentLocation()
        {
            var player = GameObject.FindWithTag("Player");
            if (!player) return;

            var playerPosition = player.transform.position;
            foreach (var location in _locations.Values)
                if (location.IsPositionInLocation(playerPosition))
                    location.Enable();
                else
                    location.Disable();
        }

        /// <summary> Включает указанную локацию. </summary>
        /// <param name="location"> Имя локации, которую нужно включить. </param>
        public static void EnableLocation(LocationName location)
        {
            if (_locations.TryGetValue(location, out var locationObject))
            {
                locationObject.Enable();
                OnLocationChanged?.Invoke(locationObject);
            }
            else { Debug.LogError($"Локации {location} не существует!"); }
        }

        /// <summary> Выключает указанную локацию. </summary>
        /// <param name="location"> Имя локации, которую нужно выключить. </param>
        public static void DisableLocation(LocationName location)
        {
            if (_locations.TryGetValue(location, out var locationObject))
                locationObject.Disable();
            else
                Debug.LogError($"Локации {location} не существует!");
        }
    }
}