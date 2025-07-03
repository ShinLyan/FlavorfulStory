using System.Collections.Generic;
using UnityEngine;

namespace FlavorfulStory.QuestSystem
{
    /// <summary> Хранит список активных квестов игрока. </summary>
    public class QuestList : MonoBehaviour
    {
        /// <summary> Массив статусов всех квестов игрока. </summary>
        [SerializeField] private QuestStatus[] _questStatuses;

        /// <summary> Перечисление всех статусов квестов. </summary>
        public IEnumerable<QuestStatus> QuestStatuses => _questStatuses;
    }
}