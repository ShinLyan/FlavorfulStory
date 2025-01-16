using System;
using UnityEngine;

namespace FlavorfulStory.UI
{
    /// <summary> Реализует вкладку в интерфейсе. Включает кнопку вкладки и отображаемый контент. </summary>
    public class Tab : MonoBehaviour
    {
        /// <summary> Кнопка вкладки. </summary>
        [SerializeField] private TabButton _tabButton;

        /// <summary> Контент, который отображается при активной вкладке. </summary>
        [SerializeField] private GameObject _tabContent;

        /// <summary> Тип вкладки. </summary>
        [SerializeField] private TabType _tabType;

        /// <summary> Событие выбора вкладки. </summary>
        public event Action<TabType> OnTabSelected;

        /// <summary> При инициализации подписываемся на событие клика по кнопке вкладки. </summary>
        private void Awake()
        {
            _tabButton.OnClick += SwitchTo;
        }

        /// <summary> Переключает на эту вкладку, вызывая событие выбора и активируя контент. </summary>
        private void SwitchTo()
        {
            OnTabSelected?.Invoke(_tabType);
            Select();
        }

        /// <summary> Активирует вкладку и ее контент. </summary>
        public void Select()
        {
            _tabButton.IsActive = true;
            _tabButton.SetNameState(true);
            _tabContent.SetActive(true);
        }

        /// <summary> Сбрасывает выбор вкладки, скрывает контент. </summary>
        public void ResetSelection()
        {
            _tabButton.IsActive = false;
            _tabButton.SetNameState(false);
            _tabContent.SetActive(false);
        }
    }
}