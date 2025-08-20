using System.Linq;
using FlavorfulStory.AI;
using FlavorfulStory.DialogueSystem.Selectors;
using FlavorfulStory.QuestSystem;

namespace FlavorfulStory.DialogueSystem
{
    /// <summary> Сервис для получения диалогов с использованием цепочки селекторов. </summary>
    public class DialogueService
    {
        /// <summary> Массив селекторов диалогов в порядке приоритета. </summary>
        private readonly IDialogueSelector[] _selectors;

        /// <summary> Инициализирует сервис с необходимыми зависимостями. </summary>
        /// <param name="questContext"> Контекст выполнения квестов. </param>
        public DialogueService(QuestExecutionContext questContext)
        {
            _selectors = new IDialogueSelector[]
            {
                new QuestDialogueSelector(questContext),
                new GreetingDialogueSelector(),
                new ContextDialogueSelector()
            };
        }

        /// <summary> Получает наиболее подходящий диалог для NPC. </summary>
        /// <param name="npcName"> Имя NPC. </param>
        /// <returns> Найденный диалог или null. </returns>
        public Dialogue GetDialogue(NpcName npcName) =>
            _selectors.Select(selector => selector.SelectDialogue(npcName)).FirstOrDefault(dialogue => dialogue);
    }
}