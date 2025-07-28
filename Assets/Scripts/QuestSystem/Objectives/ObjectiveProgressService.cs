using System.Linq;
using FlavorfulStory.AI;
using FlavorfulStory.BuildingRepair;
using FlavorfulStory.DialogueSystem;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.QuestSystem.Objectives.Params;
using FlavorfulStory.TimeManagement;
using Zenject;

namespace FlavorfulStory.QuestSystem.Objectives
{
    /// <summary> Сервис отслеживания и проверки выполнения целей квестов. </summary>
    public class ObjectiveProgressService : IInitializable
    {
        /// <summary> Контекст, предоставляющий доступ к системам, необходимым для выполнения действий квеста. </summary>
        private readonly QuestExecutionContext _context;

        /// <summary> Создаёт сервис и сохраняет переданный контекст. </summary>
        /// <param name="questExecutionContext"> Контекст выполнения квестов. </param>
        public ObjectiveProgressService(QuestExecutionContext questExecutionContext) =>
            _context = questExecutionContext;

        /// <summary> Подписывается на события, влияющие на прогресс целей. </summary>
        public void Initialize()
        {
            _context.Inventory.ItemCollected += OnItemCollected;
            _context.PlayerSpeaker.OnDialogueCompleted += OnDialogueCompleted;
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
        /// <param name="repairableBuildingName"> Название ремонтируемого здания. </param>
        private void OnRepairCompleted(RepairableBuildingName repairableBuildingName) =>
            CheckProgressForParamsType<RepairObjectiveParams>(repairableBuildingName);

        /// <summary> Проверяет выполнение всех целей, использующих указанный тип параметров. </summary>
        /// <typeparam name="TParams"> Тип параметров цели. </typeparam>
        private void CheckProgressForParamsType<TParams>(object eventData = null)
            where TParams : ObjectiveParamsBase
        {
            // Делаем копию изначального списка квестов, т.к. во время выполнения список может измениться.
            var questStatuses = _context.QuestList.QuestStatuses.ToList();
            foreach (var questStatus in questStatuses)
            foreach (var objective in questStatus.CurrentObjectives)
                if (objective.Params is TParams paramsInstance)
                {
                    paramsInstance.CheckAndComplete(questStatus, _context, eventData);
                    break; // Если в одном этапе есть два одинаковых Objective
                }
        }
    }
}