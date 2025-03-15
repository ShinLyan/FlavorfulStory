using System.Collections.Generic;
using UnityEngine;

namespace FlavorfulStory.SceneManagement
{
    public static class LocationChanger
    {
        private static LocationName _currentLocation;
        private static Dictionary<LocationName, Location> _locationsDictionary;

        public static void InitializeLocations()
        {
            var locations = Object.FindObjectsByType<Location>(FindObjectsSortMode.None);
            _locationsDictionary = new Dictionary<LocationName, Location>();

            foreach (var location in locations)
            {
                var type = location.LocationName;
                if (!_locationsDictionary.TryAdd(type, location))
                    Debug.LogWarning($"Duplicate LocationType found: {type} in {location.name}");
            }
        }

        public static void EnableLocation(LocationName newLocation)
        {
            if (_locationsDictionary.TryGetValue(newLocation, out var location))
                location.gameObject.SetActive(true);
            else
                Debug.LogError($"Location {newLocation} not found!");
        }

        public static void DisableLocation(LocationName location)
        {
            if (_locationsDictionary.TryGetValue(location, out var locationObj))
                locationObj.gameObject.SetActive(false);
            else
                Debug.LogError($"Location {location} not found!");
        }
    }
}