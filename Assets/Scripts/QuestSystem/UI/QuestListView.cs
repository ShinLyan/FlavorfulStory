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
        private QuestList _questList;

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

        /// <summary> Создаёт кнопки для всех квестов игрока и выбирает первую кнопку по умолчанию. </summary>
        private void Start()
        {
            foreach (Transform child in transform) Destroy(child.gameObject);

            foreach (var status in _questList.QuestStatuses)
            {
                var instance = _questListButtonFactory.Create(transform);
                instance.Setup(status);
                instance.OnClick = () => OnQuestButtonClicked(instance);
                _questListButtons.Add(instance);
            }

            // Автоматически выбираем первый квест, если есть хотя бы один
            if (_questListButtons.Count > 0) _questListButtons[0].Select();
        }

        /// <summary> Обрабатывает нажатие на кнопку квеста, делая её неактивной, а остальные — активными. </summary>
        /// <param name="clickedButton"> Кнопка квеста, на которую был сделан клик. </param>
        private void OnQuestButtonClicked(QuestListButton clickedButton)
        {
            foreach (var button in _questListButtons) button.Interactable = button != clickedButton;
        }
    }
}