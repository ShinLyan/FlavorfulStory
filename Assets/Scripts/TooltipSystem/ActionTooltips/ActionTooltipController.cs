using System.Collections.Generic;
using FlavorfulStory.Actions;
using FlavorfulStory.InteractionSystem;
using FlavorfulStory.Toolbar;
using Zenject;

namespace FlavorfulStory.TooltipSystem.ActionTooltips
{
    /// <summary> Контроллер, управляющий отображением тултипов действий на основе игровых сигналов. </summary>
    /// <remarks> Реализует бизнес-логику выбора подходящего тултипа и делегирует его отображение во View. </remarks>
    public class ActionTooltipController
    {
        /// <summary> Активные тултипы по источникам. </summary>
        private readonly Dictionary<ActionTooltipSource, ActionTooltipData> _activeTooltips;

        /// <summary> Сервис для отображения/удаления тултипов во View. </summary>
        private readonly IActionTooltipViewSpawner _tooltipSpawner;

        /// <summary> Подписка на сигналы и инициализация зависимостей. </summary>
        /// <param name="signalBus"> Шина сигналов для подписки на игровые события. </param>
        /// <param name="tooltipSpawner"> Сервис, отвечающий за отображение тултипов. </param>
        public ActionTooltipController(SignalBus signalBus, IActionTooltipViewSpawner tooltipSpawner)
        {
            _activeTooltips = new Dictionary<ActionTooltipSource, ActionTooltipData>();
            _tooltipSpawner = tooltipSpawner;

            signalBus.Subscribe<ToolbarSlotSelectedSignal>(OnToolbarSlotSelected);
            signalBus.Subscribe<ClosestInteractableChangedSignal>(OnClosestInteractableChanged);
        }

        /// <summary> Обработка сигнала выбора предмета в тулбаре. </summary>
        /// <param name="signal"> Сигнал, содержащий информацию о выбранном предмете. </param>
        private void OnToolbarSlotSelected(ToolbarSlotSelectedSignal signal)
        {
            const ActionTooltipSource sourceKey = ActionTooltipSource.Toolbar;
            RemoveTooltip(sourceKey);

            var item = signal.SelectedItem;
            if (!item || !item.CanBeDropped) return;

            var tooltip = new ActionTooltipData("G", ActionType.Drop, item.ItemName, ActionTooltipSource.Toolbar);
            AddTooltip(sourceKey, tooltip);
        }

        /// <summary> Обработка сигнала смены ближайшего взаимодействуемого объекта. </summary>
        /// <param name="signal"> Сигнал, содержащий ссылку на ближайший объект для взаимодействия. </param>
        private void OnClosestInteractableChanged(ClosestInteractableChangedSignal signal)
        {
            const ActionTooltipSource sourceKey = ActionTooltipSource.Interactable;
            RemoveTooltip(sourceKey);

            var interactable = signal.ClosestInteractable;
            if (interactable == null) return;

            var tooltip = interactable.ActionTooltip;
            AddTooltip(sourceKey, tooltip);
        }

        /// <summary> Добавить тултип и отобразить его через View. </summary>
        /// <param name="source"> Источник тултипа (например, Toolbar или Interactable). </param>
        /// <param name="data"> Данные тултипа для отображения. </param>
        private void AddTooltip(ActionTooltipSource source, ActionTooltipData data)
        {
            _activeTooltips[source] = data;
            _tooltipSpawner.Add(data);
        }

        /// <summary> Удалить тултип и скрыть его через View. </summary>
        /// <param name="source"> Источник тултипа, который необходимо удалить. </param>
        private void RemoveTooltip(ActionTooltipSource source)
        {
            if (!_activeTooltips.TryGetValue(source, out var actionTooltipData)) return;

            _tooltipSpawner.Remove(actionTooltipData);
            _activeTooltips.Remove(source);
        }
    }
}