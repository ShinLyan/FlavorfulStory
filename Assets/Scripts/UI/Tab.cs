using System;
using FlavorfulStory.Input;
using UnityEngine;

namespace FlavorfulStory.UI
{
    /// <summary> Реализует вкладку в интерфейсе. Включает кнопку вкладки и отображаемый контент. </summary>
    public class Tab : MonoBehaviour
    {
        /// <summary> Кнопка вкладки. </summary>
        [SerializeField] private TabButton _tabButton;

        ///<summary> Контент, который отображается при активной вкладке. </summary>
        [SerializeField] private GameObject _tabContent;
        
        ///<summary> Индекс вкладки в наборе вкладок. </summary>
        private int _index;

        ///<summary> Кнопка ввода, связанная с данной вкладкой. </summary>
        [field: SerializeField] public InputButton InputButton { get; private set; }

        ///<summary> Событие, вызываемое при выборе вкладки. </summary>
        ///<remarks> Передает индекс выбранной вкладки. </remarks>
        public event Action<int> OnTabSelected;
        
        ///<summary> Инициализирует обработчик события клика по кнопке вкладки. </summary>
        private void Awake()
        {
            _tabButton.OnClick += SwitchTo;
        }

        ///<summary> Переключает интерфейс на данную вкладку. </summary>
        ///<remarks> Вызывает событие выбора вкладки и активирует её контент. </remarks>
        private void SwitchTo()
        {
            OnTabSelected?.Invoke(_index);
            Select();
        }

        ///<summary> Активирует вкладку и отображает её контент. </summary>
        public void Select()
        {
            _tabButton.IsActive = true;
            _tabButton.SetNameState(true);
            _tabContent.SetActive(true);
        }

        ///<summary> Деактивирует вкладку и скрывает её контент. </summary>
        public void ResetSelection()
        {
            _tabButton.IsActive = false;
            _tabButton.SetNameState(false);
            _tabContent.SetActive(false);
        }

        ///<summary> Устанавливает индекс вкладки. </summary>
        ///<param name="index"> Числовой индекс вкладки. </param>
        public void SetIndex(int index) => _index = index;
    }
}