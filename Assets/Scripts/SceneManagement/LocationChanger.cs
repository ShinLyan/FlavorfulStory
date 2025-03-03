using System.Collections.Generic;
using UnityEngine;

namespace FlavorfulStory.SceneManagement
{
    public static class LocationChanger
    {
        private static LocationType _currentLocation;
        private static Dictionary<LocationType, Location> _locationsDictionary;

        public static void InitializeLocations()
        {
            var locations = Resources.FindObjectsOfTypeAll<Location>();
            _locationsDictionary = new Dictionary<LocationType, Location>();

            foreach (var location in locations)
            {
                var type = location.LocationType;
                if (!_locationsDictionary.TryAdd(type, location))
                    Debug.LogWarning($"Duplicate LocationType found: {type} in {location.name}");
            }
        }

        public static void EnableLocation(LocationType newLocation)
        {
            if (_locationsDictionary.TryGetValue(newLocation, out Location location))
                location.gameObject.SetActive(true);
            else
                Debug.LogError($"Location {newLocation} not found!");
        }

        public static void DisableLocation(LocationType location)
        {
            if (_locationsDictionary.TryGetValue(location, out Location locationObj))
                locationObj.gameObject.SetActive(false);
            else
                Debug.LogError($"Location {location} not found!");
        }
    }
}