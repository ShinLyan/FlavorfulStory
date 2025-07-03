using System.Collections.Generic;
using FlavorfulStory.InventorySystem.UI;
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
        [SerializeField] private InventorySlotView _rewardPrefab;

        [SerializeField] private GameObject _noQuest;

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
            // TODO: Сделать проверку.
            // Может возникнуть ситуация, когда все квесты выполнились и нужно будет такое показывать
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

        private void SetupQuestTexts(Quest quest)
        {
            _questNameText.text = quest.QuestName;
            _npcIcon.sprite = quest.QuestGiver.Icon;
            _npcNameText.text = quest.QuestGiver.NpcName.ToString();
            _descriptionText.text = quest.QuestDescription;
        }

        private void SetupObjectives(QuestStatus questStatus)
        {
            foreach (var objective in questStatus.Quest.Objectives)
            {
                var instance = Instantiate(_objectivePrefab, _objectivesContainer);
                instance.Setup(objective.Description, questStatus.IsObjectiveComplete(objective.Reference));
            }
        }

        private void SetupRewards(IEnumerable<QuestReward> rewards)
        {
            foreach (var reward in rewards)
            {
                var instance = Instantiate(_rewardPrefab, _rewardsContainer);
                instance.Setup(reward.Item, reward.Number);
                // instance.
            }
        }
    }
}