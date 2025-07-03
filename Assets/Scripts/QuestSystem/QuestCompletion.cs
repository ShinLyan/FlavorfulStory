using UnityEngine;
using Zenject;

namespace FlavorfulStory.QuestSystem
{
    // TODO: Обобщить с QuestGiver. НУ или с похожими действиями
    public class QuestCompletion : MonoBehaviour
    {
        [SerializeField] private Quest _quest;

        [SerializeField] private string _objective;

        /// <summary> Список всех квестов игрока. </summary>
        private QuestList _questList;

        /// <summary> Внедряет зависимости через Zenject. </summary>
        /// <param name="questList"> Компонент списка квестов. </param>
        [Inject]
        private void Construct(QuestList questList) => _questList = questList;

        public void CompleteObjective()
        {
            // if (_questList.HasQuest(new QuestStatus(_quest))) 
            _questList.CompleteObjective(_quest, _objective);
        }
    }
}