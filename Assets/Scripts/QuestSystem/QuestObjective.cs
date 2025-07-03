using System;
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
        [field: SerializeField] public string Description { get; private set; }
    }
}