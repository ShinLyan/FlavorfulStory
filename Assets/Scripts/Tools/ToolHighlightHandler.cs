using FlavorfulStory.GridSystem;
using FlavorfulStory.TimeManagement;
using FlavorfulStory.Toolbar;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.Tools
{
    /// <summary> Отвечает за отображение индикатора на клетке грида при выборе инструмента. </summary>
    public class ToolHighlightHandler : IInitializable, ILateTickable
    {
        /// <summary> Шина сигналов Zenject, используется для подписки на события. </summary>
        private readonly SignalBus _signalBus;

        /// <summary> Сервис отображения индикатора выбранной клетки грида. </summary>
        private readonly GridSelectionService _gridSelectionService;

        /// <summary> Провайдер для получения позиции курсора в координатах грида. </summary>
        private readonly GridPositionProvider _gridPositionProvider;

        /// <summary> Сервис, выполняющий проверку возможности применения инструмента. </summary>
        private readonly ToolUsageService _toolUsageService;

        /// <summary> Текущий выбранный инструмент из панели быстрого доступа. </summary>
        private Tool _activeTool;

        /// <summary> Флаг, указывающий, что именно этот класс сейчас управляет отображением индикатора. </summary>
        private bool _ownsIndicator;

        /// <summary> Конструктор с внедрением зависимостей. </summary>
        /// <param name="signalBus"> Шина сигналов Zenject. </param>
        /// <param name="gridSelectionService"> Сервис отображения индикатора выбранной клетки. </param>
        /// <param name="gridPositionProvider"> Провайдер координат грида. </param>
        /// <param name="toolUsageService"> Сервис проверки и применения инструментов. </param>
        public ToolHighlightHandler(SignalBus signalBus, GridSelectionService gridSelectionService,
            GridPositionProvider gridPositionProvider, ToolUsageService toolUsageService)
        {
            _signalBus = signalBus;
            _gridSelectionService = gridSelectionService;
            _gridPositionProvider = gridPositionProvider;
            _toolUsageService = toolUsageService;
        }

        /// <summary> Выполняет инициализацию — подписку на событие выбора предмета в тулбаре. </summary>
        public void Initialize() => _signalBus.Subscribe<ToolbarSlotSelectedSignal>(OnToolbarItemChanged);

        /// <summary> Обрабатывает смену предмета в тулбаре. </summary>
        /// <param name="signal"> Сигнал, содержащий данные о новом выбранном слоте тулбара. </param>
        private void OnToolbarItemChanged(ToolbarSlotSelectedSignal signal)
        {
            _activeTool = signal.SelectedItem as Tool;

            // Если предмет не является инструментом — индикатором управляют другие системы (например, Placement)
            if (!_activeTool) _ownsIndicator = false;
        }

        /// <summary> Отображает индикатор на валидной клетке под курсором. </summary>
        public void LateTick()
        {
            if (WorldTime.IsPaused || !_activeTool ||
                !_gridPositionProvider.TryGetCursorGridPosition(out var gridPosition))
            {
                if (!_ownsIndicator) return;

                _gridSelectionService.HideGridIndicator();
                _ownsIndicator = false;
                return;
            }

            var cellCenter = _gridPositionProvider.GridToWorld(gridPosition);
            bool canHit = _toolUsageService.TryGetValidHitableAt(_activeTool, cellCenter, out _);
            if (canHit)
            {
                _gridSelectionService.ShowGridIndicator(cellCenter, Vector2Int.one, true);
                _ownsIndicator = true;
            }
            else
            {
                _gridSelectionService.HideGridIndicator();
                _ownsIndicator = false;
            }
        }
    }
}