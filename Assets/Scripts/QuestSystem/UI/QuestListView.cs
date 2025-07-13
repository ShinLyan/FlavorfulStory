using System.Collections.Generic;
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
        private QuestList _questList; // TODO: переделать так, чтобы VIEW про модель не знала

        /// <summary> Список созданных кнопок квестов. </summary>
        private readonly List<QuestListButton> _questListButtons = new();

        /// <summary> Внедряет зависимости через Zenject. </summary>
        /// <param name="factory"> Фабрика кнопок квестов. </param>
        /// <param name="questList"> Компонент списка квестов. </param>
        [Inject]
        private void Construct(IGameFactory<QuestListButton> factory, QuestList questList)
        {
            _questListButtonFactory = factory;
            _questList = questList;
        }

        /// <summary> Подписывается на обновление списка квестов при инициализации. </summary>
        private void Awake() => _questList.OnQuestListUpdated += UpdateView;

        /// <summary> Заполняет список кнопок при старте. </summary>
        private void Start() => UpdateView();

        /// <summary> При активации окна автоматически выбирает первую кнопку квеста, если она есть. </summary>
        private void OnEnable()
        {
            if (_questListButtons.Count > 0) _questListButtons[0].Select();
        }

        /// <summary> Обновляет список кнопок квестов, пересоздавая их на основе текущих данных. </summary>
        private void UpdateView()
        {
            // TODO: Крч как-то нужно оптимизировать эту хрень. Проверять, был ли квест до этого, если да,
            // то ничего не делаем, если нет, то добавляем, если закончился квест, то удалить
            foreach (Transform child in transform) Destroy(child.gameObject); // TODO: Переделать на ObjectPool
            _questListButtons.Clear();

            foreach (var status in _questList.QuestStatuses)
            {
                var instance = _questListButtonFactory.Create(transform);
                instance.Setup(status);
                instance.OnClick = () => OnQuestButtonClicked(instance);
                _questListButtons.Add(instance);
            }
        }

        /// <summary> Обрабатывает нажатие на кнопку квеста, делая её неактивной, а остальные — активными. </summary>
        /// <param name="clickedButton"> Кнопка квеста, на которую был сделан клик. </param>
        private void OnQuestButtonClicked(QuestListButton clickedButton)
        {
            foreach (var button in _questListButtons) button.Interactable = button != clickedButton;
        }
    }
}