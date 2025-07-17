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

        /// <summary> Кнопка ввода, связанная с данной вкладкой. </summary>
        [field: SerializeField] public InputButton InputButton { get; private set; }

        /// <summary> Индекс вкладки в наборе вкладок. </summary>
        private int _index;

        /// <summary> Подписка на событие клика по кнопке вкладки при инициализации. </summary>
        /// <summary> Событие, вызываемое при выборе вкладки. </summary>
        /// <remarks> Передает индекс выбранной вкладки. </remarks>
        private Action<int> _onTabSelected;

        public void Initialize(int index, Action<int> onTabSelected)
        {
            _index = index;
            _onTabSelected = onTabSelected;
            _tabButton.OnClick = () => _onTabSelected?.Invoke(_index);
            _tabButton.Interactable = true;
        }

        public void Activate()
        {
            gameObject.SetActive(true);
            _tabButton.IsActive = true;
            _tabButton.SetNameState(true);
        }

        public void Deactivate()
        {
            _tabButton.IsActive = false;
            gameObject.SetActive(false);
            _tabButton.SetNameState(false);
        }
    }
}