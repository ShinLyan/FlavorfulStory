using System;
using System.Collections.Generic;
using FlavorfulStory.GridSystem;
using FlavorfulStory.Utils.Factories;
using FlavorfulStory.InputSystem;
using FlavorfulStory.TimeManagement;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.PlacementSystem
{
    /// <summary> Контроллер режима размещения и удаления объектов на гриде. </summary>
    public class PlacementController : ITickable
    {
        /// <summary> Провайдер координат курсора на гриде. </summary>
        private readonly GridPositionProvider _gridPositionProvider;

        /// <summary> Сервис отображения предпросмотра размещаемого или удаляемого объекта. </summary>
        private readonly PlacementPreview _placementPreview;

        /// <summary> Родительский контейнер для размещаемых объектов. </summary>
        private readonly Transform _placeableContainer;

        /// <summary> Фабрика для создания объектов размещения. </summary>
        private readonly IPrefabFactory<PlaceableObject> _placeableFactory;

        /// <summary> Слои грида с размещёнными объектами. </summary>
        private readonly Dictionary<PlacementLayer, PlacementGridData> _gridLayers = new();

        /// <summary> Доступные режимы размещения/удаления, сопоставленные с их типами. </summary>
        private readonly Dictionary<PlacementModeType, IPlacementMode> _modes = new();

        /// <summary> Провайдер размещаемых объектов. </summary>
        private readonly IPlaceableObjectProvider _placeableObjectProvider;

        /// <summary> Текущий активный режим размещения или удаления. </summary>
        private IPlacementMode _currentMode;

        /// <summary> Последняя позиция курсора на гриде, для оптимизации обновления предпросмотра. </summary>
        private Vector3Int _lastGridPosition;

        /// <summary> Действие, вызываемое при успешном применении действия (размещение или удаление). </summary>
        private Action _onApplySuccess;

        /// <summary> Конструктор с параметрами. </summary>
        /// <param name="gridPositionProvider"> Провайдер координат грида. </param>
        /// <param name="placementPreview"> Сервис предпросмотра размещения. </param>
        /// <param name="placeableContainer"> Родительский контейнер для размещаемых объектов. </param>
        /// <param name="placeableFactory"> Фабрика для создания объектов размещения. </param>
        /// <param name="placeableObjectProvider"> Провайдер размещаемых объектов. </param>
        public PlacementController(GridPositionProvider gridPositionProvider, PlacementPreview placementPreview,
            Transform placeableContainer, IPrefabFactory<PlaceableObject> placeableFactory,
            IPlaceableObjectProvider placeableObjectProvider)
        {
            _gridPositionProvider = gridPositionProvider;
            _placementPreview = placementPreview;
            _placeableContainer = placeableContainer;
            _placeableFactory = placeableFactory;
            _placeableObjectProvider = placeableObjectProvider;

            InitializeGridLayers();
            InitializeModes();
        }

        /// <summary> Создаёт пустые контейнеры данных для каждого слоя грида. </summary>
        private void InitializeGridLayers()
        {
            foreach (PlacementLayer layer in Enum.GetValues(typeof(PlacementLayer)))
                _gridLayers[layer] = new PlacementGridData();
        }

        /// <summary> Инициализирует доступные режимы (размещение и удаление). </summary>
        private void InitializeModes()
        {
            _modes[PlacementModeType.Place] = new PlacementMode(_gridPositionProvider, _placementPreview, _gridLayers,
                _placeableContainer, _placeableFactory, _placeableObjectProvider);
            _modes[PlacementModeType.Remove] = new RemovingMode(_gridPositionProvider, _placementPreview, _gridLayers,
                _placeableObjectProvider);
        }

        /// <summary> Включает указанный режим работы системы размещения объектов. </summary>
        /// <param name="modeType"> Тип режима (размещение или удаление). </param>
        /// <param name="placeableObject"> Объект для размещения (актуально только для режима размещения). </param>
        /// <param name="onApplySuccess"> Действие, вызываемое при успешном применении. </param>
        public void EnterPlacementMode(PlacementModeType modeType, PlaceableObject placeableObject,
            Action onApplySuccess)
        {
            if (!_modes.TryGetValue(modeType, out var modeInstance)) return;

            if (placeableObject && modeInstance is PlacementMode placementMode)
                placementMode.PlaceableObject = placeableObject;

            _onApplySuccess = onApplySuccess;
            ActivateMode(modeInstance);
        }

        /// <summary> Активирует переданный режим, предварительно деактивировав текущий. </summary>
        /// <param name="mode"> Новый режим для активации. </param>
        private void ActivateMode(IPlacementMode mode)
        {
            ExitCurrentMode();

            _currentMode = mode;
            _currentMode.Enter();
        }

        /// <summary> Выключает текущий режим размещения/удаления. </summary>
        public void ExitCurrentMode()
        {
            if (_currentMode == null) return;

            _currentMode.Exit();
            _currentMode = null;
            _lastGridPosition = Vector3Int.zero;
        }

        /// <summary> Попробовать удалить объект в ячейке. </summary>
        /// <param name="cellCenter"> Центр ячейки, в которой нужно удалить объект. </param>
        /// <param name="removedPlaceable"> Удаленный объект. </param>
        /// <returns> <c>true</c> - если объект был удалён, <c>false</c> - не был удалён. </returns>
        public bool TryRemoveAt(Vector3 cellCenter, out PlaceableObject removedPlaceable)
        {
            var gridPosition = _gridPositionProvider.WorldToGrid(cellCenter);
            removedPlaceable = null;

            if (_modes.TryGetValue(PlacementModeType.Remove, out var mode) && mode is RemovingMode removing)
                return removing.TryRemoveAtSilent(gridPosition, out removedPlaceable);

            return false;
        }

        /// <summary> Регистрирует объект в системе размещения по мировой позиции. </summary>
        /// <param name="worldPosition"> Позиция в мировых координатах. </param>
        /// <param name="placeableObject"> Объект для регистрации. </param>
        public void RegisterPlacedObject(Vector3 worldPosition, PlaceableObject placeableObject)
        {
            var gridPosition = _gridPositionProvider.WorldToGrid(worldPosition);
            _gridLayers[placeableObject.Layer].AddObjectAt(gridPosition, placeableObject);
            _placeableObjectProvider.Register(placeableObject);
        }

        /// <summary> Удаляет объект из системы размещения. </summary>
        /// <param name="worldPosition"> Позиция в мировых координатах. </param>
        /// <param name="placeableObject"> Объект для удаления. </param>
        public void UnregisterPlacedObject(Vector3 worldPosition, PlaceableObject placeableObject)
        {
            var gridPosition = _gridPositionProvider.WorldToGrid(worldPosition);
            _gridLayers[placeableObject.Layer].RemoveObjectAt(gridPosition);
            _placeableObjectProvider.Unregister(placeableObject);
        }

        /// <summary> Обрабатывает обновление предпросмотра и клик размещения. </summary>
        public void Tick()
        {
            if (WorldTime.IsPaused || _currentMode == null ||
                !_gridPositionProvider.TryGetCursorGridPosition(out var gridPosition))
                return;

            RefreshCursor(gridPosition);

            if (InputWrapper.GetLeftMouseButtonDown() && _currentMode.TryApply(gridPosition)) _onApplySuccess?.Invoke();
        }

        /// <summary> Обновляет предпросмотр размещения, если позиция курсора изменилась. </summary>
        /// <param name="gridPosition"> Новая позиция курсора на гриде. </param>
        private void RefreshCursor(Vector3Int gridPosition)
        {
            if (_lastGridPosition == gridPosition) return;

            _currentMode.Refresh(gridPosition);
            _lastGridPosition = gridPosition;
        }
    }
}