using System;
using FlavorfulStory.QuestSystem.Objectives;
using FlavorfulStory.QuestSystem.Objectives.Params;
using UnityEngine;

namespace FlavorfulStory.QuestSystem
{
    /// <summary> Представляет цель квеста с уникальной ссылкой и описанием. </summary>
    [Serializable]
    public class QuestObjective
    {
        /// <summary> Уникальная ссылка на цель квеста. </summary>
        [field: SerializeField] public string Reference { get; private set; }

        /// <summary> Описание цели квеста, отображаемое игроку. </summary>
        [field: Tooltip("Описание цели квеста, отображаемое игроку."), SerializeField, TextArea(2, 4)]
        public string Description { get; private set; }

        /// <summary> Тип цели квеста. </summary>
        [field: Tooltip("Тип цели квеста."), SerializeField]
        public ObjectiveType Type { get; private set; }

        /// <summary> Параметры цели квеста. </summary>
        [field: SerializeReference] public ObjectiveParamsBase Params { get; private set; }
    }
}