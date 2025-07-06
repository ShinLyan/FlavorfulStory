using FlavorfulStory.InventorySystem;

namespace FlavorfulStory.QuestSystem.Objectives
{
    /// <summary> Контекст выполнения цели квеста, содержащий доступ к различным системам. </summary>
    public class ObjectiveExecutionContext
    {
        /// <summary> Список квестов игрока. </summary>
        public QuestList QuestList { get; }

        /// <summary> Инвентарь игрока. </summary>
        public Inventory Inventory { get; }

        /// <summary> Создаёт контекст выполнения цели. </summary>
        /// <param name="questList"> Список квестов. </param>
        /// <param name="inventory"> Инвентарь игрока. </param>
        public ObjectiveExecutionContext(QuestList questList, Inventory inventory)
        {
            QuestList = questList;
            Inventory = inventory;
        }
    }
}