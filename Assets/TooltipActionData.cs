using System;
using FlavorfulStory.Actions;
using UnityEngine;

namespace FlavorfulStory
{
    /// <summary> Содержит данные для отображения действия в тултипе, включая клавишу и описание действия. </summary>
    [Serializable]
    public class TooltipActionData
    {
        /// <summary> Текст клавиши действия (например, "G"). </summary>
        [SerializeField] private string _keyText;
    
        /// <summary> Описание действия (что делает и над чем выполняется). </summary>
        [SerializeField] private ActionDescription _actionDescription;

        /// <summary> Текст клавиши действия. </summary>
        public string KeyText => _keyText;
        /// <summary> Описание действия. </summary>
        public ActionDescription ActionDescription => _actionDescription;
        
        public TooltipActionData(string keyText, string action, string target)
        {
            _keyText = keyText;
            _actionDescription = new ActionDescription { Action = action, Target = target };
        }
        
        /// <summary> Проверяет равенство двух TooltipActionData по содержимому. </summary>
        public override bool Equals(object obj)
        {
            if (obj is not TooltipActionData other) return false;
            return KeyText == other.KeyText &&
                   ActionDescription.Action == other.ActionDescription.Action &&
                   ActionDescription.Target == other.ActionDescription.Target;
        }

        /// <summary> Генерирует хэш-код на основе полей. </summary>
        /// <remarks> Необходимо для сравнения и прооверки на равенство. </remarks>
        public override int GetHashCode()
        {
            return HashCode.Combine(KeyText, ActionDescription.Action, ActionDescription.Target);
        }
    }
}