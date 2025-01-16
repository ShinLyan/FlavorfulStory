using System;
using FlavorfulStory.SceneManagement;
using TMPro;
using UnityEngine;

namespace FlavorfulStory.UI
{
    /// <summary> Меню игры, управляющее вкладками, их выбором и скрытием контента.
    /// Обрабатывает ввод для переключения вкладок и скрытия/показа меню. </summary>
    public class GameMenu : MonoBehaviour
    {
        /// <summary> Клавиша для переключения состояния меню. </summary>
        [SerializeField] private KeyCode _switchKey;

        /// <summary> Контейнер с контентом меню. </summary>
        [SerializeField] private GameObject _content;

        /// <summary> Текст обозначения кнопки для переключения на предыдущую вкладку. </summary>
        [SerializeField] private TMP_Text _previousTabLabel;
        
        /// <summary> Текст обозначения кнопки для переключения на следующую вкладку. </summary>
        [SerializeField] private TMP_Text _nextTabLabel;
        
        /// <summary> Массив вкладок в меню. </summary>
        private Tab[] _tabs;

        /// <summary> Текущая выбранная вкладка. </summary>
        private TabType _currentTabType;

        /// <summary> Клавиша для перехода к предыдущей вкладке. </summary>
        private const KeyCode PreviousTabKey = KeyCode.Q;

        /// <summary> Клавиша для перехода к следующей вкладке. </summary>
        private const KeyCode NextTabKey = KeyCode.R;

        /// <summary> Инициализация компонента.
        /// Находит все вкладки и назначает обработчики для их выбора. </summary>
        private void Awake()
        {
            _tabs = GetComponentsInChildren<Tab>(true);
            foreach (var tab in _tabs)
            {
                tab.OnTabSelected += SelectTab;
            }
        }

        /// <summary> Устанавливает начальную вкладку (главную). </summary>
        private void Start()
        {
            _currentTabType = TabType.MainTab;
            ShowCurrentTab();

            _previousTabLabel.text = PreviousTabKey.ToString();
            _nextTabLabel.text = NextTabKey.ToString();
        }

        /// <summary> Обрабатывает ввод пользователя для переключения состояния меню,
        /// смены вкладки и нажатия на кнопки вкладок. </summary>
        private void Update()
        {
            HandleSwitchInput();
            HandleAdjacentTabsInput();
            HandleTabButtonsInput();
        }

        /// <summary> Обрабатывает ввод для переключения состояния меню (нажатие клавиши для скрытия/показа меню). </summary>
        private void HandleSwitchInput()
        {
            if (Input.GetKeyDown(_switchKey))
            {
                SwitchContent(!_content.activeSelf);
            }
        }

        /// <summary> Обрабатывает ввод для переключения между соседними вкладками. </summary>
        private void HandleAdjacentTabsInput()
        {
            if (!_content.activeInHierarchy) return;

            if (Input.GetKeyDown(PreviousTabKey) || Input.GetKeyDown(NextTabKey))
            {
                bool isPreviousTab = Input.GetKeyDown(PreviousTabKey);
                int direction = isPreviousTab ? -1 : 1;
                var currentTabIndex = (int)_currentTabType;

                int newTabIndex = (currentTabIndex + _tabs.Length + direction) % _tabs.Length;
                SelectTab((TabType)newTabIndex);
            }
        }

        /// <summary> Обрабатывает ввод для нажатия на кнопки вкладок. </summary>
        private void HandleTabButtonsInput()
        {
            if (_content.activeInHierarchy && Input.GetButtonDown(_currentTabType.ToString()))
            {
                SwitchContent(false);
                return;
            }

            foreach (TabType tabType in Enum.GetValues(typeof(TabType)))
            {
                if (Input.GetButtonDown(tabType.ToString()))
                {
                    SwitchContent(true);
                    SelectTab(tabType);
                    return;
                }
            }
        }
        
        /// <summary> Выбирает вкладку и скрывает текущую. </summary>
        /// <param name="tabType"> Тип вкладки для выбора. </param>
        private void SelectTab(TabType tabType)
        {
            HideCurrentTab();
            _currentTabType = tabType;
            ShowCurrentTab();
        }

        /// <summary> Показывает текущую вкладку. </summary>
        private void ShowCurrentTab()
        {
            _tabs[(int)_currentTabType].Select();
        }

        /// <summary> Скрывает текущую вкладку. </summary>
        private void HideCurrentTab()
        {
            _tabs[(int)_currentTabType].ResetSelection();
        }

        /// <summary> Переключает состояние видимости меню. </summary>
        /// <param name="isEnabled"> Новое состояние видимости меню. </param>
        private void SwitchContent(bool isEnabled)
        {
            _content.SetActive(isEnabled);
        }

        /// <summary> Скрывает меню. </summary>
        public void Hide()
        {
            _content.SetActive(false);
        }

        /// <summary> Обработчик нажатия кнопки возврата в главное меню. Загружает сцену главного меню. </summary>
        public void OnClickReturnToMainMenu()
        {
            SavingWrapper.LoadSceneByName(SceneType.MainMenu.ToString());
        }
    }
}