using UnityEngine;
using Zenject;

namespace FlavorfulStory.QuestSystem
{
    // TODO: Возможно нужно удалить, если нигде не будет использоваться.
    // Раньше использовался для проигрывания через диалог
    /// <summary> Компонент, представляющий NPC или объект, который выдает квест игроку. </summary>
    public class QuestGiver : MonoBehaviour
    {
        /// <summary> Квест, который будет выдан игроку. </summary>
        [SerializeField] private Quest _quest;

        /// <summary> Список всех квестов игрока. </summary>
        private QuestList _questList;

        /// <summary> Внедряет зависимости через Zenject. </summary>
        /// <param name="questList"> Компонент списка квестов. </param>
        [Inject]
        private void Construct(QuestList questList) => _questList = questList;

        /// <summary> Добавляет квест игроку, добавляя его в активный список квестов. </summary>
        public void GiveQuest() => _questList.AddQuest(_quest);
    }
}