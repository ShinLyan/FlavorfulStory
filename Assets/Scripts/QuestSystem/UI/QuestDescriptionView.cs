using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.Infrastructure;
using FlavorfulStory.InventorySystem;
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

        /// <summary> Пул для повторного использования элементов целей квеста. </summary>
        private ObjectPool<QuestObjectiveView> _objectivePool;

        /// <summary> Пул для повторного использования слотов наград квеста. </summary>
        private ObjectPool<QuestRewardSlotView> _rewardPool;

        /// <summary> Активные элементы целей, отображаемые в данный момент. </summary>
        private readonly List<QuestObjectiveView> _activeObjectives = new();

        /// <summary> Активные элементы наград, отображаемые в данный момент. </summary>
        private readonly List<QuestRewardSlotView> _activeRewards = new();

        #endregion

        /// <summary> Инициализирует пулы для целей и наград квеста, регистрирует уже существующие элементы, 
        /// очищает представление и деактивирует объект до первого обновления UI. </summary>
        private void Awake()
        {
            InitializePools();

            RegisterExistingViews(_objectivesContainer, _objectivePool);
            RegisterExistingViews(_rewardsContainer, _rewardPool);

            ClearView();

            gameObject.SetActive(false);
        }

        /// <summary> Инициализация пулов. </summary>
        private void InitializePools()
        {
            _objectivePool = new ObjectPool<QuestObjectiveView>(
                () => Instantiate(_questObjectivePrefab, _objectivesContainer),
                view =>
                {
                    view.gameObject.SetActive(true);
                    view.transform.SetParent(_objectivesContainer, false);
                },
                view => view.gameObject.SetActive(false)
            );

            _rewardPool = new ObjectPool<QuestRewardSlotView>(
                () => Instantiate(_rewardPrefab, _rewardsContainer),
                view =>
                {
                    view.gameObject.SetActive(true);
                    view.transform.SetParent(_rewardsContainer, false);
                },
                view => view.gameObject.SetActive(false)
            );
        }

        /// <summary> Регистрирует уже существующие дочерние элементы в переданном контейнере
        /// как неактивные и добавляет их в пул. </summary>
        /// <typeparam name="T"> Тип компонента, который нужно зарегистрировать. </typeparam>
        /// <param name="parent"> Родительский трансформ, содержащий компоненты. </param>
        /// <param name="pool"> Пул объектов, в который добавляются компоненты. </param>
        private static void RegisterExistingViews<T>(Transform parent, ObjectPool<T> pool) where T : Component
        {
            foreach (Transform child in parent)
                if (child.TryGetComponent<T>(out var component))
                {
                    component.gameObject.SetActive(false);
                    pool.Release(component);
                }
        }

        /// <summary> Настраивает отображение информации о квесте на основе его состояния. </summary>
        /// <param name="questStatus"> Статус квеста для отображения. </param>
        public void UpdateView(QuestStatus questStatus)
        {
            gameObject.SetActive(true);

            ClearView();
            SetupQuestTexts(questStatus.Quest);
            SetupObjectives(questStatus);
            SetupRewards(questStatus.Quest.Rewards);
        }

        /// <summary> Очищает контейнеры целей и наград от предыдущего контента. </summary>
        private void ClearView()
        {
            foreach (var objective in _activeObjectives) _objectivePool.Release(objective);
            _activeObjectives.Clear();

            foreach (var reward in _activeRewards) _rewardPool.Release(reward);
            _activeRewards.Clear();
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
            var stages = questStatus.Quest.Stages.ToList();
            int lastVisibleStageIndex = questStatus.IsComplete
                ? stages.Count - 1
                : questStatus.CurrentStageIndex;

            // Обходим от текущего этапа вниз до самого первого
            for (int stageIndex = lastVisibleStageIndex; stageIndex >= 0; stageIndex--)
                foreach (var objective in stages[stageIndex].Objectives)
                {
                    var instance = _objectivePool.Get();
                    instance.transform.SetAsLastSibling();
                    bool isCompleted = questStatus.IsStageComplete(stageIndex) ||
                                       questStatus.IsObjectiveComplete(objective);
                    instance.Setup(objective.Description, isCompleted);
                    _activeObjectives.Add(instance);
                }
        }

        /// <summary> Создает и отображает награды за квест. </summary>
        /// <param name="rewards"> Список наград. </param>
        private void SetupRewards(IEnumerable<ItemStack> rewards)
        {
            foreach (var reward in rewards)
            {
                var instance = _rewardPool.Get();
                instance.transform.SetAsLastSibling();
                instance.UpdateView(reward);
                _activeRewards.Add(instance);
            }
        }
    }
}