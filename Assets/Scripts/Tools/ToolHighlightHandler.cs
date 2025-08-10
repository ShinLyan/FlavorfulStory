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

        private readonly GridPositionProvider _gridPositionProvider;

        /// <summary> Сервис, выполняющий проверку возможности применения инструмента. </summary>
        private readonly ToolUsageService _toolUsageService;

        private readonly PlayerController _player;

        /// <summary> Текущий выбранный инструмент из панели быстрого доступа. </summary>
        private Tool _activeTool;

        /// <summary> Флаг, указывающий, что именно этот класс сейчас управляет отображением индикатора. </summary>
        private bool _ownsIndicator;

        private bool _isLocked;

        private Vector3 _lockedCenterWorld;

        /// <summary> Конструктор с внедрением зависимостей. </summary>
        /// <param name="signalBus"> Шина сигналов Zenject. </param>
        /// <param name="gridSelectionService"> Сервис отображения индикатора выбранной клетки. </param>
        /// <param name="gridPositionProvider"></param>
        /// <param name="toolUsageService"> Сервис проверки и применения инструментов. </param>
        /// <param name="player"></param>
        public ToolHighlightHandler(SignalBus signalBus, GridSelectionService gridSelectionService,
            GridPositionProvider gridPositionProvider, ToolUsageService toolUsageService, PlayerController player)
        {
            _signalBus = signalBus;
            _gridSelectionService = gridSelectionService;
            _gridPositionProvider = gridPositionProvider;
            _toolUsageService = toolUsageService;
            _player = player;
        }

        /// <summary> Выполняет инициализацию — подписку на событие выбора предмета в тулбаре. </summary>
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

        private void OnToolUseStarted(Vector3 centerWorld)
        {
            if (!_activeTool) return;

            _isLocked = true;
            _lockedCenterWorld = centerWorld;
        }

        private void OnToolUseFinished()
        {
            _isLocked = false;
            Hide();
        }

        private void Hide()
        {
            if (!_ownsIndicator) return;

            _gridSelectionService.HideGridIndicator();
            _ownsIndicator = false;
        }

        /// <summary> Отображает индикатор на валидной клетке под курсором. </summary>
        public void LateTick()
        {
            // Во время удара — держим индикатор на зафиксированном центре
            if (_isLocked)
            {
                ShowAtCenter(_lockedCenterWorld, GetIndicatorState(_lockedCenterWorld));
                return;
            }

            // В обычном режиме подсветка только если цель валидна и в радиусе
            if (!_activeTool || !_gridPositionProvider.TryGetCursorGridPosition(out var gridPosition))
            {
                Hide();
                return;
            }

            var center = _gridPositionProvider.GetCellCenterWorld(gridPosition);

            // 1) Подходит ли инструмент к цели?
            bool canHitByType = _toolUsageService.TryGetValidHitableAt(_activeTool, center, out _);
            if (!canHitByType)
            {
                Hide();
                return;
            }

            // 2) В радиусе ли?
            float maxDist = GridPositionProvider.CellsToWorldDistance(ToolUsageService.MaxDistanceInCells);
            var delta = center - _player.transform.position;
            delta.y = 0f;
            if (delta.sqrMagnitude > maxDist * maxDist)
            {
                Hide();
                return;
            }

            // ОК — показываем как ValidTarget (белый)
            ShowAtCenter(center, GridIndicatorState.ValidTarget);
        }

        private void ShowAtCenter(Vector3 centerWorld, GridIndicatorState state)
        {
            var half = GridPositionProvider.CellHalfExtents;
            var origin = new Vector3(centerWorld.x - half.x, 0f, centerWorld.z - half.z);
            _gridSelectionService.ShowGridIndicator(origin, Vector2Int.one, state);
            _ownsIndicator = true;
        }

        private GridIndicatorState GetIndicatorState(Vector3 center)
        {
            if (!_toolUsageService.TryGetValidHitableAt(_activeTool, center, out _))
                return GridIndicatorState.InvalidTarget;

            float maxDist = GridPositionProvider.CellsToWorldDistance(ToolUsageService.MaxDistanceInCells);
            var delta = center - _player.transform.position;
            delta.y = 0f;

            return delta.sqrMagnitude <= maxDist * maxDist
                ? GridIndicatorState.ValidTarget
                : GridIndicatorState.InvalidTarget;
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<ToolbarSlotSelectedSignal>(OnToolbarItemChanged);
            _toolUsageService.ToolUseStarted -= OnToolUseStarted;
            _toolUsageService.ToolUseFinished -= OnToolUseFinished;
        }
    }
}