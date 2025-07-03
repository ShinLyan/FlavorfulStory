using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlavorfulStory.QuestSystem
{
    /// <summary> Представление описания выбранного квеста в UI. </summary>
    public class QuestDescriptionView : MonoBehaviour
    {
        /// <summary> Текстовое поле с названием квеста. </summary>
        [SerializeField] private TMP_Text _questNameText;

        /// <summary> Иконка NPC, который выдал квест. </summary>
        [SerializeField] private Image _npcIcon;

        /// <summary> Текстовое поле с именем NPC. </summary>
        [SerializeField] private TMP_Text _npcNameText;

        /// <summary> Текстовое поле с описанием квеста. </summary>
        [SerializeField] private TMP_Text _descriptionText;

        /// <summary> Контейнер для целей квеста. </summary>
        [SerializeField] private Transform _objectivesContainer;

        /// <summary> Префаб для отображения одной цели квеста. </summary>
        [SerializeField] private ObjectiveView _objectivePrefab;

        /// <summary> Контейнер для наград квеста. </summary>
        [SerializeField] private Transform _rewardsContainer;

        /// <summary> Префаб для отображения одной награды. </summary>
        [SerializeField] private GameObject _rewardPrefab;

        /// <summary> Очищает отображение при инициализации. </summary>
        private void Awake() => ClearView();

        /// <summary> Настраивает отображение информации о квесте на основе его состояния. </summary>
        /// <param name="questStatus"> Статус квеста для отображения. </param>
        public void Setup(QuestStatus questStatus)
        {
            ClearView();

            var quest = questStatus.Quest;
            _questNameText.text = quest.QuestName;
            _npcIcon.sprite = quest.QuestGiver.Icon;
            _npcNameText.text = quest.QuestGiver.NpcName.ToString();
            _descriptionText.text = quest.QuestDescription;

            foreach (string objective in quest.Objectives)
            {
                var instance = Instantiate(_objectivePrefab, _objectivesContainer);
                instance.Setup(objective, questStatus.IsObjectiveComplete(objective));
            }
        }

        /// <summary> Очищает контейнеры целей и наград от предыдущего контента. </summary>
        private void ClearView()
        {
            foreach (Transform child in _objectivesContainer) Destroy(child.gameObject);
            foreach (Transform child in _rewardsContainer) Destroy(child.gameObject);
        }
    }
}