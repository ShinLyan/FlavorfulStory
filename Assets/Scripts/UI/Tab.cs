using System;
using FlavorfulStory.InputSystem;
using UnityEngine;

namespace FlavorfulStory.UI
{
    /// <summary> Вкладка в интерфейсе, содержащая кнопку и отображаемый контент. </summary>
    public class Tab : MonoBehaviour
    {
        /// <summary> Кнопка вкладки. </summary>
        [SerializeField] private TabButton _tabButton;

        /// <summary> Контент, отображаемый при активной вкладке. </summary>
        [SerializeField] private GameObject _tabContent;

        /// <summary> Индекс вкладки в наборе вкладок. </summary>
        private int _index;

        /// <summary> Кнопка ввода, связанная с данной вкладкой. </summary>
        [field: SerializeField] public InputButton InputButton { get; private set; }

        /// <summary> Подписка на событие клика по кнопке вкладки при инициализации. </summary>
        /// <summary> Событие, вызываемое при выборе вкладки. </summary>
        ///<remarks> Передает индекс выбранной вкладки. </remarks>
        public event Action<int> OnTabSelected;

        /// <summary> Инициализирует обработчик события клика по кнопке вкладки. </summary>
        private void Awake()
        {
            _tabButton.OnClick += SwitchTo;
        }

        /// <summary> Выбор этой вкладки, вызов события выбора и активация контента. </summary>
        private void SwitchTo()
        {
            OnTabSelected?.Invoke(_index);
            Select();
        }

        /// <summary> Активация вкладки и ее контента. </summary>
        public void Select()
        {
            _tabButton.IsActive = true;
            _tabButton.SetNameState(true);
            _tabContent.SetActive(true);
        }

        /// <summary> Сброс состояния вкладки и скрытие контента. </summary>
        public void ResetSelection()
        {
            _tabButton.IsActive = false;
            _tabButton.SetNameState(false);
            _tabContent.SetActive(false);
        }

        /// <summary> Устанавливает индекс вкладки. </summary>
        ///<param name="index"> Числовой индекс вкладки. </param>
        public void SetIndex(int index) => _index = index;
    }
}