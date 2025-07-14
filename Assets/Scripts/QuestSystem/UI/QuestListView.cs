using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.Infrastructure;
using FlavorfulStory.Infrastructure.Factories;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.QuestSystem
{
    /// <summary> Отвечает за отображение и управление списком кнопок квестов. </summary>
    public class QuestListView : MonoBehaviour
    {
        /// <summary> Фабрика для создания кнопок квестов. </summary>
        private IGameFactory<QuestListButton> _questListButtonFactory;

        /// <summary> Список всех квестов игрока. </summary>
        private IQuestList _questList;

        /// <summary> Пул объектов кнопок квестов. </summary>
        private ObjectPool<QuestListButton> _pool;

        /// <summary> Словарь сопоставлений между статусами квестов и их кнопками в UI. </summary>
        private readonly Dictionary<QuestStatus, QuestListButton> _questButtonMap = new();

        /// <summary> Внедряет зависимости через Zenject. </summary>
        /// <param name="factory"> Фабрика кнопок квестов. </param>
        /// <param name="questList"> Компонент списка квестов. </param>
        [Inject]
        private void Construct(IGameFactory<QuestListButton> factory, IQuestList questList)
        {
            _questListButtonFactory = factory;
            _questList = questList;
        }

        /// <summary> Удаляем всех детей и подписываемся на обновление списка квестов при инициализации. </summary>
        private void Awake()
        {
            foreach (Transform child in transform)
                if (child.TryGetComponent<QuestListButton>(out _))
                    Destroy(child.gameObject);

            _questList.OnQuestListUpdated += UpdateView;
        }

        /// <summary> Заполняет список кнопок при старте. </summary>
        private void Start()
        {
            _pool = new ObjectPool<QuestListButton>(
                () => _questListButtonFactory.Create(transform),
                questListButton => questListButton.gameObject.SetActive(true),
                questListButton => questListButton.gameObject.SetActive(false)
            );

            UpdateView();
        }

        /// <summary> Обновляет список кнопок квестов, пересоздавая их на основе текущих данных. </summary>
        private void UpdateView()
        {
            var currentStatuses = new HashSet<QuestStatus>(_questList.QuestStatuses);
            RemoveObsoleteButtons(currentStatuses);
            AddNewButtons(currentStatuses);
        }

        /// <summary> Удаляет кнопки для квестов, которых больше нет в текущем списке. </summary>
        /// <param name="currentStatuses"> Актуальный набор статусов квестов. </param>
        private void RemoveObsoleteButtons(HashSet<QuestStatus> currentStatuses)
        {
            var toRemove = new List<QuestStatus>();
            foreach (var kvp in _questButtonMap.Where(kvp => !currentStatuses.Contains(kvp.Key)))
            {
                _pool.Release(kvp.Value);
                toRemove.Add(kvp.Key);
            }

            foreach (var key in toRemove) _questButtonMap.Remove(key);
        }

        /// <summary> Добавляет новые кнопки квестов, которых ещё нет в отображаемом списке. </summary>
        /// <param name="currentStatuses"> Актуальный набор статусов квестов. </param>
        private void AddNewButtons(HashSet<QuestStatus> currentStatuses)
        {
            foreach (var status in currentStatuses)
            {
                if (_questButtonMap.ContainsKey(status)) continue;

                var button = _pool.Get();
                button.Setup(status);
                button.OnClick = () => OnQuestButtonClicked(button);
                _questButtonMap[status] = button;
            }
        }

        /// <summary> Обрабатывает нажатие на кнопку квеста, делая её неактивной, а остальные — активными. </summary>
        /// <param name="clickedButton"> Кнопка квеста, на которую был сделан клик. </param>
        private void OnQuestButtonClicked(QuestListButton clickedButton)
        {
            foreach (var button in _questButtonMap.Values) button.Interactable = button != clickedButton;
        }

        /// <summary> При активации окна автоматически выбирает первую кнопку квеста, если она есть. </summary>
        private void OnEnable() => SelectFirstAvailableButton();

        /// <summary> Выбирает первую доступную кнопку в списке, если она существует. </summary>
        private void SelectFirstAvailableButton()
        {
            if (_questButtonMap.Count <= 0) return;

            var firstButton = _questButtonMap.Select(pair => pair.Value).FirstOrDefault();
            firstButton?.Select();
            OnQuestButtonClicked(firstButton);
        }
    }
}