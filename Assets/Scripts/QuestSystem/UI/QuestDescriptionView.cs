using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlavorfulStory.QuestSystem
{
    /// <summary> Представление описания выбранного квеста в UI. </summary>
    public class QuestDescriptionView : MonoBehaviour
    {
        #region Fields

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
        [SerializeField] private QuestObjectiveView _questObjectivePrefab;

        /// <summary> Контейнер для наград квеста. </summary>
        [SerializeField] private Transform _rewardsContainer;

        /// <summary> Префаб для отображения награды. </summary>
        [SerializeField] private QuestRewardSlotView _rewardPrefab;

        /// <summary> Элемент, отображаемый при отсутствии выбранного квеста. </summary>
        [SerializeField] private GameObject _noQuest;

        #endregion

        /// <summary> Очищает отображение при инициализации. </summary>
        private void Awake()
        {
            _noQuest.SetActive(true);
            ClearView();
        }

        /// <summary> Настраивает отображение информации о квесте на основе его состояния. </summary>
        /// <param name="questStatus"> Статус квеста для отображения. </param>
        public void UpdateView(QuestStatus questStatus)
        {
            // TODO: Сделать проверку. Может возникнуть ситуация, когда все квесты выполнились, и нужно будет такое показывать
            _noQuest.SetActive(false);

            ClearView();
            SetupQuestTexts(questStatus.Quest);
            SetupObjectives(questStatus);
            SetupRewards(questStatus.Quest.Rewards);
        }

        /// <summary> Очищает контейнеры целей и наград от предыдущего контента. </summary>
        private void ClearView()
        {
            foreach (Transform child in _objectivesContainer) Destroy(child.gameObject);
            foreach (Transform child in _rewardsContainer) Destroy(child.gameObject);
        }

        /// <summary> Устанавливает тексты с основной информацией о квесте. </summary>
        /// <param name="quest"> Квест для отображения. </param>
        private void SetupQuestTexts(Quest quest)
        {
            _questNameText.text = quest.QuestName;
            _npcIcon.sprite = quest.QuestGiver.Icon;
            _npcNameText.text = quest.QuestGiver.NpcName.ToString();
            _descriptionText.text = quest.QuestDescription;
        }

        /// <summary> Создает и отображает цели квеста. </summary>
        /// <param name="questStatus"> Статус квеста с прогрессом целей. </param>
        private void SetupObjectives(QuestStatus questStatus)
        {
            foreach (var objective in questStatus.CurrentObjectives)
            {
                var instance = Instantiate(_questObjectivePrefab, _objectivesContainer);
                instance.Setup(objective.Description, questStatus.IsObjectiveComplete(objective));
            }
        }

        /// <summary> Создает и отображает награды за квест. </summary>
        /// <param name="rewards"> Список наград. </param>
        private void SetupRewards(IEnumerable<QuestReward> rewards)
        {
            foreach (var reward in rewards)
            {
                var instance = Instantiate(_rewardPrefab, _rewardsContainer);
                instance.UpdateView(reward.Item, reward.Number);
            }
        }
    }
}