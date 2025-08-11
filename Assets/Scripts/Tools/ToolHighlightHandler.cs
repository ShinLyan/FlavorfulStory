using System;
using FlavorfulStory.GridSystem;
using FlavorfulStory.Player;
using FlavorfulStory.Toolbar;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.Tools
{
    /// <summary> Отвечает за отображение индикатора на клетке грида при выборе инструмента. </summary>
    public class ToolHighlightHandler : IInitializable, ILateTickable, IDisposable
    {
        /// <summary> Шина сигналов Zenject, используется для подписки на события. </summary>
        private readonly SignalBus _signalBus;

        /// <summary> Сервис отображения индикатора выбранной клетки грида. </summary>
        private readonly GridSelectionService _gridSelectionService;

        /// <summary> Провайдер координат и позиций клеток грида. </summary>
        private readonly GridPositionProvider _gridPositionProvider;

        /// <summary> Сервис, выполняющий проверку возможности применения инструмента. </summary>
        private readonly ToolUsageService _toolUsageService;

        /// <summary> Игрок, относительно которого проверяется дистанция удара и направление взгляда. </summary>
        private readonly PlayerController _player;

        /// <summary> Текущий выбранный инструмент из панели быстрого доступа. </summary>
        private Tool _activeTool;

        /// <summary> Флаг, указывающий, что именно этот класс сейчас управляет отображением индикатора. </summary>
        private bool _ownsIndicator;

        /// <summary> Флаг, блокирующий динамическое обновление индикатора. </summary>
        private bool _isLocked;

        /// <summary> Центр клетки в мировых координатах, на котором зафиксирован индикатор во время удара. </summary>
        private Vector3 _lockedCenterWorld;

        /// <summary> Состояние индикатора, зафиксированное во время удара. </summary>
        private GridIndicatorState _lockedState;

        /// <summary> Конструктор с внедрением зависимостей. </summary>
        /// <param name="signalBus"> Шина сигналов Zenject. </param>
        /// <param name="gridSelectionService"> Сервис отображения индикатора выбранной клетки. </param>
        /// <param name="gridPositionProvider"> Провайдер для получения и конвертации координат клеток грида. </param>
        /// <param name="toolUsageService"> Сервис проверки и применения инструментов. </param>
        /// <param name="player"> Контроллер игрока, необходимый для проверки расстояния до цели. </param>
        public ToolHighlightHandler(SignalBus signalBus, GridSelectionService gridSelectionService,
            GridPositionProvider gridPositionProvider, ToolUsageService toolUsageService, PlayerController player)
        {
            _signalBus = signalBus;
            _gridSelectionService = gridSelectionService;
            _gridPositionProvider = gridPositionProvider;
            _toolUsageService = toolUsageService;
            _player = player;
        }

        /// <summary> Выполняет инициализацию. </summary>
        public void Initialize()
        {
            _signalBus.Subscribe<ToolbarSlotSelectedSignal>(OnToolbarItemChanged);
            _toolUsageService.ToolUseStarted += OnToolUseStarted;
            _toolUsageService.ToolUseFinished += OnToolUseFinished;
        }

        /// <summary> Обрабатывает смену предмета в тулбаре. </summary>
        /// <param name="signal"> Сигнал, содержащий данные о новом выбранном слоте тулбара. </param>
        private void OnToolbarItemChanged(ToolbarSlotSelectedSignal signal)
        {
            _activeTool = signal.SelectedItem as Tool;

            // Если предмет не является инструментом — индикатором управляют другие системы (например, Placement)
            if (!_activeTool) _ownsIndicator = false;
        }

        /// <summary> Обработчик начала использования инструмента. </summary>
        /// <param name="centerWorld"> Центр клетки в мировых координатах. </param>
        /// <param name="state"> Состояние индикатора (валидная / невалидная цель). </param>
        private void OnToolUseStarted(Vector3 centerWorld, GridIndicatorState state)
        {
            if (!_activeTool) return;

            _isLocked = true;
            _lockedCenterWorld = centerWorld;
            _lockedState = state;
        }

        /// <summary> Обработчик завершения использования инструмента. </summary>
        private void OnToolUseFinished()
        {
            _isLocked = false;
            Hide();
        }

        /// <summary> Скрывает выделение сеткой. </summary>
        private void Hide()
        {
            if (!_ownsIndicator) return;

            _gridSelectionService.HideGridIndicator();
            _ownsIndicator = false;
        }

        /// <summary> Отображает индикатор на валидной клетке под курсором. </summary>
        public void LateTick()
        {
            if (_isLocked)
            {
                var lockedGrid = _gridPositionProvider.WorldToGrid(_lockedCenterWorld);
                var lockedCenter = _gridPositionProvider.GetCellCenterWorld(lockedGrid);
                ShowAtCenter(lockedCenter, _lockedState);
                return;
            }

            if (!_activeTool || !_gridPositionProvider.TryGetCursorGridPosition(out var gridPosition))
            {
                Hide();
                return;
            }

            var center = _gridPositionProvider.GetCellCenterWorld(gridPosition);
            if (!_toolUsageService.TryGetValidHitableAt(_activeTool, center, out _) || !IsWithinChebyshevRange(center))
            {
                Hide();
                return;
            }

            ShowAtCenter(center, GridIndicatorState.ValidTarget);
        }

        /// <summary> Проверяет, находится ли цель в пределах допустимой дистанции Чебышева от игрока. </summary>
        /// <param name="targetCenterWorld"> Центр целевой клетки в мировых координатах. </param>
        /// <returns> True, если цель находится в пределах досягаемости. </returns>
        private bool IsWithinChebyshevRange(Vector3 targetCenterWorld)
        {
            var playerGrid = _gridPositionProvider.WorldToGrid(_player.transform.position);
            var targetGrid = _gridPositionProvider.WorldToGrid(targetCenterWorld);

            int dx = Mathf.Abs(targetGrid.x - playerGrid.x);
            int dy = Mathf.Abs(targetGrid.y - playerGrid.y);

            // 2 клетки в любую сторону, включая диагональ
            return Mathf.Max(dx, dy) <= ToolUsageService.MaxDistanceInCells;
        }

        /// <summary> Отображает индикатор в заданном центре клетки с указанным состоянием. </summary>
        /// <param name="centerWorld"> Центр клетки в мировых координатах. </param>
        /// <param name="state"> Состояние индикатора. </param>
        private void ShowAtCenter(Vector3 centerWorld, GridIndicatorState state)
        {
            var half = GridPositionProvider.CellHalfExtents;
            var origin = new Vector3(centerWorld.x - half.x, 0f, centerWorld.z - half.z);
            _gridSelectionService.ShowGridIndicator(origin, Vector2Int.one, state);
            _ownsIndicator = true;
        }

        /// <summary> Освобождает ресурсы. </summary>
        public void Dispose()
        {
            _signalBus.Unsubscribe<ToolbarSlotSelectedSignal>(OnToolbarItemChanged);
            _toolUsageService.ToolUseStarted -= OnToolUseStarted;
            _toolUsageService.ToolUseFinished -= OnToolUseFinished;
        }
    }
}