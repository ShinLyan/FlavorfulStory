using System.Linq;
using FlavorfulStory.PlacementSystem;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.Shop;
using UnityEngine;

namespace FlavorfulStory.TimeManagement
{
    /// <summary> Сервис для определения точки спавна игрока. </summary>
    public class PlayerSpawnService
    {
        /// <summary> Провайдер размещаемых объектов. </summary>
        private readonly IPlaceableObjectProvider _placeableObjectProvider;

        /// <summary> Менеджер локаций. </summary>
        private readonly LocationManager _locationManager;

        /// <summary> Последняя использованная кровать. </summary>
        private SleepTrigger _lastUsedBed;

        /// <summary> Инициализирует сервис спавна игрока. </summary>
        /// <param name="placeableObjectProvider"> Провайдер объектов. </param>
        /// <param name="locationManager"> Менеджер локаций. </param>
        public PlayerSpawnService(IPlaceableObjectProvider placeableObjectProvider, LocationManager locationManager)
        {
            _placeableObjectProvider = placeableObjectProvider;
            _locationManager = locationManager;
        }

        /// <summary> Регистрирует последнюю использованную кровать. </summary>
        /// <param name="sleepTrigger"> Триггер сна. </param>
        public void RegisterLastUsedBed(SleepTrigger sleepTrigger) => _lastUsedBed = sleepTrigger;

        /// <summary> Получает позицию для спавна игрока. </summary>
        /// <returns> Позиция спавна. </returns>
        public Vector3 GetSpawnPosition()
        {
            var allBeds = _placeableObjectProvider.GetObjectsOfType<SleepTrigger>();

            if (_lastUsedBed && allBeds.Contains(_lastUsedBed)) return _lastUsedBed.transform.position;

            return ((ShopLocation)_locationManager.GetLocationByName(LocationName.NewShop)).EntryPoint.position;
        }
    }
}