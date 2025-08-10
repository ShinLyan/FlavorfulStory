using FlavorfulStory.Actions;
using FlavorfulStory.GridSystem;
using FlavorfulStory.InputSystem;
using FlavorfulStory.Stats;
using FlavorfulStory.Tools;
using Zenject;

namespace FlavorfulStory.InventorySystem.ItemUsage
{
    /// <summary> Контроллер, обрабатывающий использование инструментов игроком. </summary>
    public class ToolUseController : ItemUseController<Tool>
    {
        /// <summary> Сервис, выполняющий логику использования инструментов (атака, проверка попаданий и т. д.). </summary>
        private readonly ToolUsageService _toolUsageService;

        /// <summary> Провайдер для определения позиции курсора в координатах грида. </summary>
        private readonly GridPositionProvider _gridPositionProvider;

        /// <summary> Статы игрока (для проверки стамины перед использованием инструмента). </summary>
        private readonly PlayerStats _playerStats;

        /// <summary> Создаёт контроллер использования инструментов. </summary>
        /// <param name="signalBus"> Шина сигналов для отслеживания смены выбранного предмета. </param>
        /// <param name="toolUsageService"> Сервис для выполнения логики взаимодействия инструментом с окружением. </param>
        /// <param name="gridPositionProvider"> Провайдер получения позиции курсора в гриде. </param>
        /// <param name="playerStats"> Статы игрока для проверки наличия ресурсов (например, стамины). </param>
        public ToolUseController(SignalBus signalBus, ToolUsageService toolUsageService,
            GridPositionProvider gridPositionProvider, PlayerStats playerStats) : base(signalBus)
        {
            _toolUsageService = toolUsageService;
            _gridPositionProvider = gridPositionProvider;
            _playerStats = playerStats;
        }

        /// <summary> Покадровая обработка активного инструмента. </summary>
        /// <param name="tool"> Текущий активный инструмент. </param>
        protected override void TickItem(Tool tool)
        {
            bool leftClick = InputWrapper.GetLeftMouseButton() && tool.UseActionType == UseActionType.LeftClick;
            bool rightClick = InputWrapper.GetRightMouseButton() && tool.UseActionType == UseActionType.RightClick;
            if (!leftClick && !rightClick) return;

            var stamina = _playerStats.GetStat<Stamina>();
            if (stamina == null || stamina.CurrentValue < tool.StaminaCost)
                // TODO: Показываем уведомление / звук о нехватке стамины
                return;

            if (!_gridPositionProvider.TryGetCursorGridPosition(out var gridPosition)) return;

            var cellCenter = _gridPositionProvider.GetCellCenterWorld(gridPosition);
            bool success = _toolUsageService.TryUseTool(tool, cellCenter);
            if (success) stamina.ChangeValue(-tool.StaminaCost);
        }
    }
}