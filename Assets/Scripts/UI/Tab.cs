using System;
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

        /// <summary> Тип вкладки. </summary>
        [SerializeField] private TabType _tabType;

        /// <summary> Событие выбора вкладки. </summary>
        public event Action<TabType> OnTabSelected;

        /// <summary> Подписка на событие клика по кнопке вкладки при инициализации. </summary>
        private void Awake()
        {
            _tabButton.OnClick += SwitchTo;
        }

        /// <summary> Выбор этой вкладки, вызов события выбора и активация контента. </summary>
        private void SwitchTo()
        {
            OnTabSelected?.Invoke(_tabType);
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
    }
}