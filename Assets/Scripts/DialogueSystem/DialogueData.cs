using System;
using System.Collections.Generic;
using FlavorfulStory.AI;

namespace FlavorfulStory.DialogueSystem
{
    /// <summary> Данные для отображения диалога. </summary>
    [Serializable]
    public struct DialogueData
    {
        /// <summary> Текст текущей реплики. </summary>
        public string Text { get; }

        /// <summary> Информация о говорящем персонаже. </summary>
        public NpcInfo SpeakerInfo { get; }

        /// <summary> Находится ли игрок в режиме выбора ответа? </summary>
        public bool IsChoosing { get; }

        /// <summary> Список доступных вариантов ответа. </summary>
        public IEnumerable<DialogueNode> Choices { get; }

        /// <summary> Конструктор структуры диалога. </summary>
        /// <param name="text"> Текст реплики. </param>
        /// <param name="speaker"> Информация о говорящем. </param>
        /// <param name="isChoosing"> Флаг выбора ответа. </param>
        /// <param name="choices"> Доступные варианты выбора. </param>
        public DialogueData(string text, NpcInfo speaker, bool isChoosing, IEnumerable<DialogueNode> choices)
        {
            Text = text;
            SpeakerInfo = speaker;
            IsChoosing = isChoosing;
            Choices = choices;
        }
    }
}