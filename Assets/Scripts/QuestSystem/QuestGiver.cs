using UnityEngine;
using Zenject;

namespace FlavorfulStory.QuestSystem
{
    public class QuestGiver : MonoBehaviour
    {
        [SerializeField] private Quest _quest;

        /// <summary> Список всех квестов игрока. </summary>
        private QuestList _questList;

        /// <summary> Внедряет зависимости через Zenject. </summary>
        /// <param name="questList"> Компонент списка квестов. </param>
        [Inject]
        private void Construct(QuestList questList) => _questList = questList;

        public void GiveQuest() => _questList.AddQuest(_quest);
    }
}