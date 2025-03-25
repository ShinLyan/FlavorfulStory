using System.Collections.Generic;
using UnityEngine;

namespace FlavorfulStory.SceneManagement
{
    public static class LocationChanger
    {
        private static LocationName _currentLocation;
        private static Dictionary<LocationName, GameObject> _locationsDictionary;

        public static void InitializeLocations()
        {
            var locations = Object.FindObjectsByType<Location>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            _locationsDictionary = new Dictionary<LocationName, GameObject>();

            foreach (var location in locations)
            {
                var name = location.LocationName;
                if (!_locationsDictionary.TryAdd(name, location.ObjectsToDisable))
                    Debug.LogWarning($"Найден дупликат локации: {name} in {location.name}");
            }
        }

        public static void EnableLocation(LocationName newLocation)
        {
            if (_locationsDictionary.TryGetValue(newLocation, out var locationObjects))
                locationObjects.SetActive(true);
            else
                Debug.LogError($"Локации {newLocation} не существует!");
        }

        public static void DisableLocation(LocationName oldLocation)
        {
            if (_locationsDictionary.TryGetValue(oldLocation, out var locationObjects))
                locationObjects.SetActive(false);
            else
                Debug.LogError($"Локации {oldLocation} не существует!");
        }
    }
}