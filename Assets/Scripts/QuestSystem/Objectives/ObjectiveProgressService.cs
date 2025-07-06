using FlavorfulStory.InventorySystem;
using FlavorfulStory.QuestSystem.Objectives.Params;

namespace FlavorfulStory.QuestSystem.Objectives
{
    /// <summary> Сервис отслеживания и проверки выполнения целей квестов. </summary>
    public class ObjectiveProgressService
    {
        /// <summary> Контекст выполнения целей, содержащий ссылки на список квестов и инвентарь. </summary>
        private readonly ObjectiveExecutionContext _context;

        /// <summary> Создаёт экземпляр сервиса и подписывается на событие сбора предметов. </summary>
        /// <param name="questList"> Список активных квестов игрока. </param>
        /// <param name="inventory"> Инвентарь игрока. </param>
        public ObjectiveProgressService(QuestList questList, Inventory inventory)
        {
            _context = new ObjectiveExecutionContext(questList, inventory);

            inventory.ItemCollected += OnItemCollected;
        }

        /// <summary> Обработчик события сбора предмета. </summary>
        /// <param name="item"> Собранный предмет. </param>
        private void OnItemCollected(InventoryItem item) => CheckProgressForParamsType<CollectObjectiveParams>();

        /// <summary> Проверяет выполнение всех целей, использующих указанный тип параметров. </summary>
        /// <typeparam name="TParams"> Тип параметров цели. </typeparam>
        private void CheckProgressForParamsType<TParams>() where TParams : ObjectiveParamsBase
        {
            foreach (var questStatus in _context.QuestList.QuestStatuses)
            foreach (var objective in questStatus.Quest.Objectives)
                if (objective.Params is TParams paramsInstance)
                    paramsInstance.CheckAndComplete(questStatus, _context);
        }
    }
}