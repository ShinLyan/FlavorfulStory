using System.Linq;
using UnityEngine;

namespace FlavorfulStory.UI
{
    /// <summary> Управление переключением UI. </summary>
    public class UISwitcher : MonoBehaviour
    {
        /// <summary> Объект, на который переключится UI при запуске. </summary>
        [SerializeField] private GameObject _entryPoint;

        /// <summary> Массив кнопок, управляющих переключением вкладок. </summary>
        [SerializeField] private TabSwitcherButton[] _tabButtons;

        /// <summary> Клавиша для переключения на предыдущую вкладку. </summary>
        private const KeyCode _previousTabKey = KeyCode.Q;

        /// <summary> Клавиша для переключения на следующую вкладку. </summary>
        private const KeyCode _nextTabKey = KeyCode.R;

        /// <summary> Индекс текущей активной вкладки. </summary>
        private int _currentTabIndex = 0;

        /// <summary> Инициализация UI при старте. Устанавливает начальную вкладку и подготавливает кнопки. </summary>
        private void Start()
        {
            if (_entryPoint != null) SwitchTo(_entryPoint);

            if (IsSetupValid()) _tabButtons[_currentTabIndex].Select();

            InitializeTabButtons();
        }

        /// <summary> Проверка корректности настройки вкладок. </summary>
        /// <returns> Возвращает true, если количество кнопок соответствует числу дочерних объектов. </returns>
        private bool IsSetupValid() => _tabButtons != null && _tabButtons.Length == transform.childCount;

        /// <summary> Инициализация кнопок вкладок. Добавляет обработчики событий для каждой кнопки. </summary>
        private void InitializeTabButtons()
        {
            for (int i = 0; i < _tabButtons.Length; i++)
            {
                int tabIndex = i;
                _tabButtons[i].AddOnClickHandler(() => SelectTab(tabIndex));
            }
        }

        /// <summary> Обновление состояния UI. Обрабатывает переключение вкладок по клавишам. </summary>
        private void Update()
        {
            if (!IsSetupValid()) return;

            if (Input.GetKeyDown(_previousTabKey) || Input.GetKeyDown(_nextTabKey))
            {
                bool isPreviousTab = Input.GetKeyDown(_previousTabKey);
                int direction = isPreviousTab ? -1 : 1;
                int index = (_currentTabIndex + _tabButtons.Length + direction) % _tabButtons.Length;
                SelectTab(index);
            }
        }

        /// <summary> Выбор вкладки по индексу. </summary>
        /// <param name="index"> Индекс вкладки, которую нужно выбрать. </param>
        public void SelectTab(int index)
        {
            _tabButtons[_currentTabIndex].ResetSelection();
            _tabButtons[index].Select();
            _currentTabIndex = index;
        }

        /// <summary> Переключение отображения на указанный объект. </summary>
        /// <param name="toDisplay"> Объект, который нужно отобразить. </param>
        public void SwitchTo(GameObject toDisplay)
        {
            if (toDisplay.transform.parent != transform) return;

            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
            toDisplay.SetActive(true);
        }

        /// <summary> Получение имен вкладок меню, используемых в InputManager. </summary>
        /// <returns> Массив строк с именами вкладок. </returns>
        public string[] GetTabNames() => _tabButtons.Select(button => button.ButtonType.ToString()).ToArray();
    }
}