using System;
using System.Linq;
using FlavorfulStory.AI;
using FlavorfulStory.DialogueSystem.Selectors;
using Zenject;

namespace FlavorfulStory.DialogueSystem
{
    /// <summary> Сервис для получения диалогов с использованием цепочки селекторов. </summary>
    public class DialogueService : IDialogueService, IInitializable, IDisposable
    {
        /// <summary> Массив селекторов диалогов в порядке приоритета. </summary>
        private readonly IDialogueSelector[] _selectors;

        /// <summary> Инициализирует сервис с необходимыми зависимостями. </summary>
        /// <param name="selectors"> Селекторы. </param>
        public DialogueService(IDialogueSelector[] selectors) =>
            _selectors = selectors.OrderBy(GetSelectorOrder).ToArray();

        /// <summary> Определяет приоритет селектора на основе его типа. </summary>
        /// <param name="selector"> Селектор, приоритет которого нужно определить. </param>
        /// <returns> Целочисленный приоритет (меньше = выше приоритет). </returns>
        private static int GetSelectorOrder(IDialogueSelector selector) => selector switch
        {
            QuestDialogueSelector => 0,
            GreetingDialogueSelector => 1,
            ContextDialogueSelector => 2,
            _ => 100
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

        /// <summary> Возвращает наиболее подходящий диалог для заданного NPC. </summary>
        /// <param name="npcInfo"> Информация об NPC, для которого подбирается диалог. </param>
        /// <returns> Найденный диалог или null, если подходящего не найдено. </returns>
        public Dialogue GetDialogue(NpcInfo npcInfo) =>
            _selectors.Select(selector => selector.SelectDialogue(npcInfo)).FirstOrDefault(dialogue => dialogue);
    }
}