using System;
using FlavorfulStory.Actions;

namespace FlavorfulStory.TooltipSystem.ActionTooltips
{
    /// <summary> Содержит данные для отображения действия в тултипе, включая клавишу и описание действия. </summary>
    public readonly struct ActionTooltipData : IEquatable<ActionTooltipData>
    {
        /// <summary> Текст клавиши действия, отображаемый в тултипе (например, "E"). </summary>
        public string KeyText { get; }

        /// <summary> Тип действия, которое выполняется. </summary>
        public ActionType Action { get; }

        /// <summary> Название объекта, над которым выполняется действие. </summary>
        public string Target { get; }

        /// <summary> Источник вызова тултипа действия. </summary>
        public ActionTooltipSource Source { get; }

        /// <summary> Конструктор, инициализирующий данные для тултипа. </summary>
        /// <param name="keyText"> Текст клавиши действия, отображаемый в тултипе (например, "E"). </param>
        /// <param name="action"> Тип действия, которое выполняется. </param>
        /// <param name="target"> Название объекта, над которым выполняется действие. </param>
        /// <param name="source"> Источник вызова тултипа действия. </param>
        public ActionTooltipData(string keyText, ActionType action, string target,
            ActionTooltipSource source = ActionTooltipSource.Interactable)
        {
            KeyText = keyText;
            Action = action;
            Target = target;
            Source = source;
        }

        /// <summary> Проверяет равенство двух TooltipActionData по содержимому. </summary>
        /// <param name="obj"> Объект для сравнения. </param>
        /// <returns> <c>true</c>, если объекты эквивалентны; иначе — <c>false</c>. </returns>
        public override bool Equals(object obj) => obj is ActionTooltipData other && Equals(other);

        /// <summary> Проверяет равенство двух TooltipActionData по содержимому. </summary>
        /// <param name="other"> Объект для сравнения. </param>
        /// <returns> <c>true</c>, если объекты эквивалентны; иначе — <c>false</c>. </returns>
        public bool Equals(ActionTooltipData other) =>
            KeyText == other.KeyText && Action == other.Action && Target == other.Target && Source == other.Source;

        /// <summary> Генерирует хэш-код на основе полей. </summary>
        /// <returns> Хэш-код объекта. </returns>
        /// <remarks> Необходимо для сравнения и проверки на равенство. </remarks>
        public override int GetHashCode() => HashCode.Combine(KeyText, Action, Target, Source);
    }
}