using System.Collections.Generic;
using FlavorfulStory.InputSystem;
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

        /// <summary> Индекс текущей активной вкладки. </summary>
        private int _currentTabindex;

        /// <summary> Инициализация компонента.
        /// Находит все вкладки и назначает обработчики для их выбора. </summary>
        private void Awake()
        {
            _tabs = GetComponentsInChildren<Tab>(true);
            for (int i = 0; i < _tabs.Length; i++)
            {
                _tabs[i].OnTabSelected += SelectTab;
                _tabs[i].SetIndex(i);
            }
        }

        /// <summary> Устанавливает начальную вкладку (главную). </summary>
        private void Start()
        {
            _currentTabindex = 0; // MainTab по умолчанию
            ShowCurrentTab();
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
            if (InputWrapper.GetButtonDown(InputButton.SwitchGameMenu))
            {
                SwitchContent(!_content.activeSelf);
            }
        }

        /// <summary> Переключает состояние видимости меню. </summary>
        /// <param name="isEnabled"> Новое состояние видимости меню. </param>
        private void SwitchContent(bool isEnabled)
        {
            if (isEnabled)
            {
                InputWrapper.BlockPlayerMovement();
                InputWrapper.BlockInput(InputButton.MouseScroll);
            }
            else
            {
                InputWrapper.UnblockPlayerMovement();
                InputWrapper.UnblockInput(InputButton.MouseScroll);
            }

            _content.SetActive(isEnabled);
        }

        /// <summary> Обрабатывает ввод для переключения между соседними вкладками. </summary>
        private void HandleAdjacentTabsInput()
        {
            if (!_content.activeInHierarchy) return;

            if (InputWrapper.GetButtonDown(InputButton.SwitchToPreviousTab) ||
                InputWrapper.GetButtonDown(InputButton.SwitchToNextTab))
            {
                bool isPreviousTab = InputWrapper.GetButtonDown(InputButton.SwitchToPreviousTab);
                int direction = isPreviousTab ? -1 : 1;
                int newTabIndex = (_currentTabindex + _tabs.Length + direction) % _tabs.Length;
                SelectTab(newTabIndex);
            }
        }

        /// <summary> Обрабатывает ввод для нажатия на кнопки вкладок. </summary>
        private void HandleTabButtonsInput()
        {
            if (_content.activeInHierarchy &&
                InputWrapper.GetButtonDown(_tabs[_currentTabindex].InputButton))
            {
                SwitchContent(false);
                return;
            }

            for (int i = 0; i < _tabs.Length; i++)
            {
                if (InputWrapper.GetButtonDown(_tabs[i].InputButton))
                {
                    SwitchContent(true);
                    SelectTab(i);
                    return;
                }
            }
        }

        /// <summary> Выбирает вкладку и скрывает текущую. </summary>
        /// <param name="index"> Индекс вкладки для выбора. </param>
        private void SelectTab(int index)
        {
            HideCurrentTab();
            _currentTabindex = index;
            ShowCurrentTab();
        }

        /// <summary> Показывает текущую вкладку. </summary>
        private void ShowCurrentTab() => _tabs[_currentTabindex].Select();

        /// <summary> Скрывает текущую вкладку. </summary>
        private void HideCurrentTab() => _tabs[_currentTabindex].ResetSelection();

        /// <summary> Скрывает меню. </summary>
        public void OnClickContinue() => SwitchContent(false);

        /// <summary> Обработчик нажатия кнопки возврата в главное меню. Загружает сцену главного меню. </summary>
        public void OnClickReturnToMainMenu() => SavingWrapper.LoadSceneByName(SceneType.MainMenu.ToString());
    }
}