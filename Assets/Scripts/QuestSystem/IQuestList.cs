using System;
using System.Collections.Generic;

namespace FlavorfulStory.QuestSystem
{
    /// <summary> Интерфейс, определяющий поведение списка квестов игрока. </summary>
    public interface IQuestList
    {
        /// <summary> Событие, вызываемое при изменении списка квестов. </summary>
        event Action OnQuestListUpdated;

        /// <summary> Коллекция текущих статусов квестов игрока. </summary>
        IEnumerable<QuestStatus> QuestStatuses { get; }
    }
}