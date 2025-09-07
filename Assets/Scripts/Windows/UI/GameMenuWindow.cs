using Cysharp.Threading.Tasks;
using FlavorfulStory.InputSystem;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.InventorySystem.UI;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.UI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace FlavorfulStory.Windows.UI
{
    /// <summary> Меню игры, управляющее вкладками, их выбором и скрытием контента.
    /// Обрабатывает ввод для переключения вкладок и скрытия/показа меню. </summary>
    public class GameMenuWindow : BaseWindow
    {
        /// <summary> Контейнер с контентом меню. </summary>
        [SerializeField] private GameObject _content;

        /// <summary> Кнопка продолжения игры. </summary>
        [SerializeField] private Button _continueButton;

        /// <summary> Кнопка выхода в главное меню. </summary>
        [SerializeField] private Button _exitButton;

        /// <summary> View инвентаря игрока во вкладке. </summary>
        [SerializeField] private InventoryView _playerInventoryView;

        /// <summary> Провайдер инвентарей. </summary>
        private IInventoryProvider _inventoryProvider;

        /// <summary> Массив вкладок в меню. </summary>
        private Tab[] _tabs;

        /// <summary> Индекс текущей активной вкладки. </summary>
        private int _currentTabIndex;

        /// <summary> Внедрение зависимостей Zenject. </summary>
        /// <param name="inventoryProvider"> Провайдер инвентарей. </param>
        [Inject]
        private void Construct(IInventoryProvider inventoryProvider) => _inventoryProvider = inventoryProvider;

        /// <summary> Инициализация компонента.
        /// Находит все вкладки и назначает обработчики для их выбора. </summary>
        private void Awake()
        {
            _tabs = GetComponentsInChildren<Tab>(true);
            for (int i = 0; i < _tabs.Length; i++) _tabs[i].Initialize(i, OnTabSelected);

            _continueButton.onClick.AddListener(Close);
            _exitButton.onClick.AddListener(OnClickReturnToMainMenu);
        }

        /// <summary> Устанавливает начальную вкладку - MainTab. </summary>
        private void Start()
        {
            _currentTabIndex = 0;
            SelectTab(_currentTabIndex);
            _playerInventoryView.Initialize(_inventoryProvider.GetPlayerInventory());
        }

        /// <summary> Отписывается от событий при уничтожении компонента. </summary>
        private void OnDestroy()
        {
            _continueButton.onClick.RemoveListener(Close);
            _exitButton.onClick.RemoveListener(OnClickReturnToMainMenu);
        }

        /// <summary> Обрабатывает ввод пользователя для переключения состояния меню,
        /// смены вкладки и нажатия на кнопки вкладок. </summary>
        private void Update() => HandleTabInput();

        /// <summary> Обрабатывает ввод для переключения между соседними вкладками. </summary>
        private void HandleTabInput()
        {
            if (InputWrapper.GetButtonDown(InputButton.SwitchToPreviousTab) ||
                InputWrapper.GetButtonDown(InputButton.SwitchToNextTab))
            {
                int direction = InputWrapper.GetButtonDown(InputButton.SwitchToPreviousTab) ? -1 : 1;
                int newIndex = (_currentTabIndex + _tabs.Length + direction) % _tabs.Length;
                SelectTab(newIndex);
                return;
            }

            for (int i = 0; i < _tabs.Length; i++)
            {
                if (!InputWrapper.GetButtonDown(_tabs[i].InputButton)) continue;

                SwitchMenu(true);
                SelectTab(i);
                return;
            }
        }

        /// <summary> Вызывается при открытии окна. </summary>
        protected override void OnOpened()
        {
            InputWrapper.UnblockInput(InputButton.SwitchToPreviousTab);
            InputWrapper.UnblockInput(InputButton.SwitchToNextTab);

            foreach (var tab in _tabs) InputWrapper.UnblockInput(tab.InputButton);
        }

        /// <summary> Обрабатывает выбор вкладки по её индексу. 
        /// Активирует меню (если скрыто) и отображает выбранную вкладку. </summary>
        /// <param name="index"> Индекс выбранной вкладки. </param>
        private void OnTabSelected(int index)
        {
            SwitchMenu(true);
            SelectTab(index);
        }

        /// <summary> Переключает состояние видимости меню. </summary>
        /// <param name="isEnabled"> Новое состояние видимости меню. </param>
        public void SwitchMenu(bool isEnabled) => _content.SetActive(isEnabled);

        /// <summary> Выбирает вкладку и скрывает текущую. </summary>
        /// <param name="index"> Индекс вкладки для выбора. </param>
        private void SelectTab(int index)
        {
            _tabs[_currentTabIndex].Deactivate();
            _currentTabIndex = index;
            _tabs[_currentTabIndex].Activate();
        }

        /// <summary> Обрабатывает нажатие кнопки "Вернуться в главное меню". </summary>
        private void OnClickReturnToMainMenu()
        {
            Close();
            SavingWrapper.LoadSceneAsyncByName(nameof(SceneName.MainMenu)).Forget();
        }
    }
}