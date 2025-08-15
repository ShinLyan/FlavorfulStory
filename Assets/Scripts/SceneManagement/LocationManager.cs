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
        /// <summary> Провайдер позиции игрока. </summary>
        private readonly IPlayerPositionProvider _playerPositionProvider;

        /// <summary> Список всех локаций в сцене. </summary>
        private readonly List<Location> _locations;

        /// <summary> Словарь локаций по имени для быстрого доступа. </summary>
        private readonly Dictionary<LocationName, Location> _locationByName;

        /// <summary> Событие при смене локации. </summary>
        public event Action<Location> OnLocationChanged;

        /// <summary> Создать менеджер локаций. </summary>
        /// <param name="playerPositionProvider">  Провайдер позиции игрока. </param>
        /// <param name="locations"> Список всех доступных локаций. </param>
        public LocationManager(IPlayerPositionProvider playerPositionProvider, List<Location> locations)
        {
            _playerPositionProvider = playerPositionProvider;
            _locations = locations;
            _locationByName = new Dictionary<LocationName, Location>();
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
            var playerPosition = _playerPositionProvider.GetPlayerPosition();
            var currentLocation = FindLocationByPosition(playerPosition);
            foreach (var location in _locations) location.SetActive(location == currentLocation);

            if (currentLocation) OnLocationChanged?.Invoke(currentLocation);
        }

        /// <summary> Найти локацию по позиции в мировом пространстве. </summary>
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

        /// <summary> Получает локацию по её имени. </summary>
        /// <param name="name"> Имя локации для поиска. </param>
        /// <returns> Объект локации, если найден; иначе — <c>null</c>. </returns> 
        public Location GetLocationByName(LocationName name)
        {
            if (_locationByName.TryGetValue(name, out var location)) return location;

            Debug.LogError($"Локации {name} не существует!");
            return null;
        }

        /// <summary> Проверяет, находится ли игрок в указанной локации. </summary>
        /// <param name="location"> Локация для проверки. </param>
        /// <returns> Возвращает <c>true</c>, если игрок находится в локации, иначе <c>false</c>. </returns>
        public bool IsPlayerInLocation(LocationName locationName)
        {
            var location = GetLocationByName(locationName);
            return location.IsPositionInLocation(_playerController.transform.position);
        }
    }
}