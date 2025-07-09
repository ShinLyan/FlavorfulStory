using FlavorfulStory.AI;
using FlavorfulStory.BuildingRepair;
using FlavorfulStory.DialogueSystem;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.QuestSystem.Objectives.Params;
using FlavorfulStory.TimeManagement;

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
        /// <param name="playerSpeaker"> Компонент диалогов игрока. </param>
        public ObjectiveProgressService(QuestList questList, Inventory inventory, PlayerSpeaker playerSpeaker)
        {
            _context = new ObjectiveExecutionContext(questList, inventory);

            inventory.ItemCollected += OnItemCollected;
            playerSpeaker.OnDialogueCompleted += OnDialogueCompleted;
            WorldTime.OnDayEnded += OnDayEnded;
            RepairableBuilding.OnRepairCompleted += OnRepairCompleted;
        }

        /// <summary> Обработчик события сбора предмета. </summary>
        /// <param name="item"> Собранный предмет. </param>
        private void OnItemCollected(InventoryItem item) => CheckProgressForParamsType<HaveObjectiveParams>(item);

        /// <summary> Обработчик завершения диалога — завершает взаимодействие с NPC. </summary>
        /// <param name="npcName"> Имя NPC. </param>
        /// <param name="dialogue"> Диалог, который завершился. </param>
        private void OnDialogueCompleted(NpcName npcName, Dialogue dialogue) =>
            CheckProgressForParamsType<TalkObjectiveParams>((npcName, dialogue));

        /// <summary> Обработчик события завершения дня. </summary>
        /// <param name="dateTime"> Игровое время. </param>
        private void OnDayEnded(DateTime dateTime) => CheckProgressForParamsType<SleepObjectiveParams>(dateTime);

        /// <summary> Обработчик события завершения ремонта здания. </summary>
        /// <param name="repairableBuildingType"> Тип ремонтируемого здания. </param>
        private void OnRepairCompleted(RepairableBuildingType repairableBuildingType) =>
            CheckProgressForParamsType<RepairObjectiveParams>(repairableBuildingType);

        /// <summary> Проверяет выполнение всех целей, использующих указанный тип параметров. </summary>
        /// <typeparam name="TParams"> Тип параметров цели. </typeparam>
        private void CheckProgressForParamsType<TParams>(object eventData = null)
            where TParams : ObjectiveParamsBase
        {
            foreach (var questStatus in _context.QuestList.QuestStatuses)
            foreach (var objective in questStatus.Quest.Objectives)
                if (objective.Params is TParams paramsInstance)
                    paramsInstance.CheckAndComplete(questStatus, _context, eventData);
        }
    }
}