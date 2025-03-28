using System.Collections.Generic;
using UnityEngine;

namespace FlavorfulStory.SceneManagement
{
    /// <summary> Класс для активации и деактивации локаций в сцене. </summary>
    public static class LocationChanger
    {
        /// <summary> Словарь с локациями и связанными с ними объектами, которые нужно включать/выключать. </summary>
        private static readonly Dictionary<LocationName, Location> _locations = new();

        /// <summary> Текущая активная локация. </summary>
        private static Location _currentLocation;

        /// <summary> Инициализирует систему локаций, собирая все объекты типа <see cref="Location"/> в сцене. </summary>
        public static void Initialize()
        {
            _locations.Clear();

            foreach (var location in Object.FindObjectsByType<Location>(
                         FindObjectsInactive.Include, FindObjectsSortMode.None))
                if (!_locations.TryAdd(location.LocationName, location))
                    Debug.LogWarning($"Найден дубликат локации: {location.LocationName} в {location.name}");

            // TODO: Выключать все локации, кроме _currentLocation
            // foreach (var location in _locations.Values) location.Disable();
            // _currentLocation.Enable();
        }

        /// <summary> Включает указанную локацию. </summary>
        /// <param name="location"> Имя локации, которую нужно включить. </param>
        public static void EnableLocation(LocationName location)
        {
            if (!_locations.TryGetValue(location, out var locationObject))
            {
                Debug.LogError($"Локации {location} не существует!");
                return;
            }

            locationObject.Enable();
        }

        /// <summary> Выключает указанную локацию. </summary>
        /// <param name="location"> Имя локации, которую нужно выключить. </param>
        public static void DisableLocation(LocationName location)
        {
            if (!_locations.TryGetValue(location, out var locationObject))
            {
                Debug.LogError($"Локации {location} не существует!");
                return;
            }

            locationObject.Disable();
        }
    }
}