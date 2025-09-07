using System;
using System.Linq;
using FlavorfulStory.AI;
using FlavorfulStory.DialogueSystem.Selectors;
using FlavorfulStory.QuestSystem;
using Zenject;

namespace FlavorfulStory.DialogueSystem
{
    /// <summary> Сервис для получения диалогов с использованием цепочки селекторов. </summary>
    public class DialogueService : IInitializable, IDisposable
    {
        /// <summary> Массив селекторов диалогов в порядке приоритета. </summary>
        private readonly IDialogueSelector[] _selectors;

        /// <summary> Инициализирует сервис с необходимыми зависимостями. </summary>
        /// <param name="questList"> Список квестов. </param>
        public DialogueService(QuestList questList) => _selectors = new IDialogueSelector[]
        {
            new QuestDialogueSelector(questList), new GreetingDialogueSelector(), new ContextDialogueSelector()
        };

        /// <summary> Инициализирует все инициализируемые селекторы. </summary>
        public void Initialize()
        {
            foreach (var selector in _selectors.OfType<IInitializableSelector>()) selector.Initialize();
        }

        /// <summary> Освобождает ресурсы всех инициализируемых селекторов. </summary>
        public void Dispose()
        {
            foreach (var selector in _selectors.OfType<IInitializableSelector>()) selector.Dispose();
        }

        /// <summary> Получает наиболее подходящий диалог для NPC. </summary>
        /// <returns> Найденный диалог или null. </returns>
        public Dialogue GetDialogue(NpcInfo npcInfo) =>
            _selectors.Select(selector => selector.SelectDialogue(npcInfo)).FirstOrDefault(dialogue => dialogue);
    }
}