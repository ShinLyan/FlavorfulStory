using FlavorfulStory.DialogueSystem;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.InventorySystem.DropSystem;

namespace FlavorfulStory.QuestSystem
{
    /// <summary> Контекст, предоставляющий доступ к системам, необходимым для выполнения действий квеста. </summary>
    public class QuestExecutionContext
    {
        /// <summary> Список квестов игрока. </summary>
        public QuestList QuestList { get; }

        /// <summary> Инвентарь игрока. </summary>
        public Inventory Inventory { get; }

        /// <summary> Компонент диалогов игрока. </summary>
        public PlayerSpeaker PlayerSpeaker { get; }

        /// <summary> Сервис, отвечающий за выброс предметов из инвентаря в игровом мире. </summary>
        public IItemDropService ItemDropService { get; }

        /// <summary> Создаёт контекст выполнения цели. </summary>
        /// <param name="questList"> Список квестов. </param>
        /// <param name="inventory"> Инвентарь игрока. </param>
        /// <param name="playerSpeaker"> Компонент диалогов игрока. </param>
        /// <param name="itemDropService"> Сервис, отвечающий за выброс предметов из инвентаря в игровом мире. </param>
        public QuestExecutionContext(QuestList questList, Inventory inventory, PlayerSpeaker playerSpeaker,
            IItemDropService itemDropService)
        {
            QuestList = questList;
            Inventory = inventory;
            PlayerSpeaker = playerSpeaker;
            ItemDropService = itemDropService;
        }
    }
}