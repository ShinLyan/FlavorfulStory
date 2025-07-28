using Cysharp.Threading.Tasks;
using FlavorfulStory.InputSystem;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.TimeManagement;
using UnityEngine;
using UnityEngine.UI;

namespace FlavorfulStory.UI
{
    /// <summary> Меню игры, управляющее вкладками, их выбором и скрытием контента.
    /// Обрабатывает ввод для переключения вкладок и скрытия/показа меню. </summary>
    public class GameMenu : MonoBehaviour
    {
        /// <summary> Контейнер с контентом меню. </summary>
        [SerializeField] private GameObject _content;

        /// <summary> Кнопка продолжения игры. </summary>
        [SerializeField] private Button _continueButton;

        /// <summary> Кнопка выхода в главное меню. </summary>
        [SerializeField] private Button _exitButton;

        /// <summary> Массив вкладок в меню. </summary>
        private Tab[] _tabs;

        /// <summary> Индекс текущей активной вкладки. </summary>
        private int _currentTabIndex;

        /// <summary> Инициализация компонента.
        /// Находит все вкладки и назначает обработчики для их выбора. </summary>
        private void Awake()
        {
            _tabs = GetComponentsInChildren<Tab>(true);
            for (int i = 0; i < _tabs.Length; i++) _tabs[i].Initialize(i, OnTabSelected);

            _continueButton.onClick.AddListener(OnClickContinue);
            _exitButton.onClick.AddListener(OnClickReturnToMainMenu);
        }

        /// <summary> Отписывается от событий при уничтожении компонента. </summary>
        private void OnDestroy()
        {
            _continueButton.onClick.RemoveListener(OnClickContinue);
            _exitButton.onClick.RemoveListener(OnClickReturnToMainMenu);
        }

        /// <summary> Обрабатывает выбор вкладки по её индексу. 
        /// Активирует меню (если скрыто) и отображает выбранную вкладку. </summary>
        /// <param name="index"> Индекс выбранной вкладки. </param>
        private void OnTabSelected(int index)
        {
            SwitchMenu(true);
            SelectTab(index);
        }

        /// <summary> Устанавливает начальную вкладку - MainTab. </summary>
        private void Start()
        {
            _currentTabIndex = 0;
            SelectTab(_currentTabIndex);
        }

        /// <summary> Обрабатывает ввод пользователя для переключения состояния меню,
        /// смены вкладки и нажатия на кнопки вкладок. </summary>
        private void Update()
        {
            HandleInputToSwitchMenu();
            HandleTabInput();
        }

        /// <summary> Обрабатывает ввод для переключения состояния меню. </summary>
        private void HandleInputToSwitchMenu()
        {
            if (InputWrapper.GetButtonDown(InputButton.SwitchGameMenu)) SwitchMenu(!_content.activeSelf);
        }

        /// <summary> Переключает состояние видимости меню. </summary>
        /// <param name="isEnabled"> Новое состояние видимости меню. </param>
        public void SwitchMenu(bool isEnabled)
        {
            if (isEnabled)
            {
                InputWrapper.BlockPlayerInput();
                WorldTime.Pause();
            }
            else
            {
                InputWrapper.UnblockPlayerInput();
                WorldTime.Unpause();
            }

            _content.SetActive(isEnabled);
        }

        /// <summary> Обрабатывает ввод для переключения между соседними вкладками. </summary>
        private void HandleTabInput()
        {
            if (!_content.activeSelf) return;

            if (InputWrapper.GetButtonDown(InputButton.SwitchToPreviousTab) ||
                InputWrapper.GetButtonDown(InputButton.SwitchToNextTab))
            {
                int direction = InputWrapper.GetButtonDown(InputButton.SwitchToPreviousTab) ? -1 : 1;
                int newIndex = (_currentTabIndex + _tabs.Length + direction) % _tabs.Length;
                SelectTab(newIndex);
            }

            for (int i = 0; i < _tabs.Length; i++)
            {
                if (!InputWrapper.GetButtonDown(_tabs[i].InputButton)) continue;

                SwitchMenu(true);
                SelectTab(i);
                return;
            }
        }

        /// <summary> Выбирает вкладку и скрывает текущую. </summary>
        /// <param name="index"> Индекс вкладки для выбора. </param>
        private void SelectTab(int index)
        {
            _tabs[_currentTabIndex].Deactivate();
            _currentTabIndex = index;
            _tabs[_currentTabIndex].Activate();
        }

        /// <summary> Обрабатывает нажатие кнопки "Продолжить", скрывая игровое меню. </summary>
        private void OnClickContinue() => SwitchMenu(false);

        /// <summary> Обрабатывает нажатие кнопки "Вернуться в главное меню". Загружает сцену главного меню. </summary>
        private void OnClickReturnToMainMenu() =>
            SavingWrapper.LoadSceneAsyncByName(SceneName.MainMenu.ToString()).Forget();
    }
}