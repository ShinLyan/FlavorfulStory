using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using FlavorfulStory.Player;
using FlavorfulStory.Saving;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.SceneManagement
{
    /// <summary> Менеджер локаций. </summary>
    /// <remarks> Отвечает за активацию и деактивацию локаций. </remarks>
    public class LocationManager : IInitializable, IDisposable
    {
        /// <summary> Список всех локаций в сцене. </summary>
        private readonly List<Location> _locations;

        /// <summary> Словарь локаций по имени для быстрого доступа. </summary>
        private readonly Dictionary<LocationName, Location> _locationByName;

        /// <summary> Провайдер позиции игрока. </summary>
        private readonly IPlayerPositionProvider _playerPositionProvider;

        /// <summary> Событие при смене локации. </summary>
        public event Action<Location> OnLocationChanged;

        /// <summary> Создать менеджер локаций. </summary>
        /// <param name="locations"> Список всех доступных локаций. </param>
        /// <param name="playerPositionProvider">  Провайдер позиции игрока. </param>
        public LocationManager(List<Location> locations, IPlayerPositionProvider playerPositionProvider)
        {
            _locations = locations;
            _locationByName = new Dictionary<LocationName, Location>();
            _playerPositionProvider = playerPositionProvider;
        }

        /// <summary> Инициализировать менеджер. </summary>
        public void Initialize()
        {
            foreach (var location in _locations)
                if (!_locationByName.TryAdd(location.LocationName, location))
                    Debug.LogError($"Дубликат локации: {location.LocationName} в {location.name}");

            SavingSystem.OnLoadCompleted += OnLoadCompleted;
        }

        /// <summary> Отписаться от событий и освободить ресурсы. </summary>
        public void Dispose() => SavingSystem.OnLoadCompleted -= OnLoadCompleted;

        /// <summary> Обработать завершение загрузки состояния. </summary>
        private void OnLoadCompleted() => OnLoadCompletedAsync().Forget();

        /// <summary> Асинхронно активировать текущую локацию после загрузки. </summary>
        private async UniTaskVoid OnLoadCompletedAsync()
        {
            await UniTask.Yield();
            UpdateActiveLocation();
        }

        /// <summary> Активировать локацию, в которой находится игрок, и отключить остальные. </summary>
        public void UpdateActiveLocation()
        {
            var currentLocation = FindLocationByPosition(_playerPositionProvider.GetPlayerPosition());
            if (!currentLocation)
            {
                Debug.LogError("Location by position not found!");
                return;
            }

            foreach (var location in _locations) location.SetActive(location == currentLocation);

            OnLocationChanged?.Invoke(currentLocation);
        }

        /// <summary> Найти локацию по позиции в мировом пространстве. </summary>
        /// <param name="position"> Позиция игрока в мире. </param>
        /// <returns> Локация, в которой находится игрок. </returns>
        private Location FindLocationByPosition(Vector3 position) =>
            _locations.SingleOrDefault(location => location.IsPositionInLocation(position));

        /// <summary> Включить локацию по имени. </summary>
        /// <param name="name"> Имя локации, которую требуется включить. </param>
        public void EnableLocation(LocationName name) => SetLocationActive(name, true);

        /// <summary> Отключить локацию по имени. </summary>
        /// <param name="name"> Имя локации, которую требуется отключить. </param>
        public void DisableLocation(LocationName name) => SetLocationActive(name, false);

        /// <summary> Активирует или деактивирует указанную локацию по имени. </summary>
        /// <param name="name"> Имя локации. </param>
        /// <param name="isActive"> <c>true</c> — активировать; <c>false</c> — деактивировать. </param>
        private void SetLocationActive(LocationName name, bool isActive)
        {
            var location = GetLocationByName(name);
            if (location) location.SetActive(isActive);
        }

        /// <summary> Получает локацию по её имени. </summary>
        /// <param name="name"> Имя локации для поиска. </param>
        /// <returns> Объект локации, если найден; иначе — <c>null</c>. </returns> 
        public Location GetLocationByName(LocationName name)
        {
            if (_locationByName.TryGetValue(name, out var location)) return location;

            Debug.LogError($"Локация с именем {name} не найдена!");
            return null;
        }

        /// <summary> Проверяет, находится ли игрок в указанной локации. </summary>
        /// <param name="locationName"> Имя локации для проверки. </param>
        /// <returns> Возвращает <c>true</c>, если игрок находится в локации, иначе <c>false</c>. </returns>
        public bool IsPlayerInLocation(LocationName locationName)
        {
            var location = GetLocationByName(locationName);
            return location.IsPositionInLocation(_playerPositionProvider.GetPlayerPosition());
        }
    }
}