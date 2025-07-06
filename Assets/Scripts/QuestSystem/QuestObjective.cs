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
        [field: SerializeField, TextArea(2, 4)]
        public string Description { get; private set; }

        [field: SerializeField] public ObjectiveType Type { get; private set; }

        [field: SerializeReference] public ObjectiveParamsBase Params { get; private set; }
    }
}