using UnityEngine;
using Zenject;

namespace FlavorfulStory.QuestSystem
{
    /// <summary> Компонент для завершения цели квеста, например, при выполнении определенного действия. </summary>
    // TODO: Обобщить с QuestGiver или похожими действиями.
    public class QuestCompletion : MonoBehaviour
    {
        /// <summary> Квест, к которому относится завершаемая цель. </summary>
        [SerializeField] private Quest _quest;

        /// <summary> Ссылка на завершаемую цель. </summary>
        [SerializeField] private string _objective;

        /// <summary> Список всех квестов игрока. </summary>
        private QuestList _questList;

        /// <summary> Внедряет зависимости через Zenject. </summary>
        /// <param name="questList"> Компонент списка квестов. </param>
        [Inject]
        private void Construct(QuestList questList) => _questList = questList;

        /// <summary> Завершает указанную цель квеста в списке активных квестов игрока. </summary>
        public void CompleteObjective()
        {
            // TODO: Добавить проверку на наличие квеста в списке, если потребуется:
            // if (_questList.HasQuest(new QuestStatus(_quest))) ...
            _questList.CompleteObjective(_quest, _objective);
        }
    }
}