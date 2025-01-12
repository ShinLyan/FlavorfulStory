using UnityEngine;

namespace FlavorfulStory.UI
{
    //TODO: Refactor
    /// <summary> Переключатель вкладки при нажатии на клавишу. </summary>
    public class KeySwitcher : MonoBehaviour
    {
        /// <summary> Клавиша переключения. </summary>
        [SerializeField] private KeyCode _switchKey;

        /// <summary> Вкладка, которую необходимо переключать. </summary>
        [SerializeField] private GameObject _tab;

        /// <summary> Поле дочернего UISwitcher. </summary>
        [SerializeField] private UISwitcher _uiSwitcher;

        /// <summary> Массив имен кнопок для включения определенных вкладок. </summary>
        private string[] _tabButtonNames;

        /// <summary> Текущая выбранная вкладка. </summary>
        private TabType _currentTab;

        /// <summary> Инициализация компонента UISwitcher. Получение имен кнопок для переключения вкладок. </summary>
        private void Awake()
        {
            _tabButtonNames = _uiSwitcher.GetTabNames();
            _uiSwitcher.OnTabSelected += SetCurrentTab;
        }

        /// <summary> При старте выключаем вкладку. </summary>
        private void Start()
        {
            SwitchTab(false);
        }

        /// <summary> При нажатии на клавишу переключать вкладку или открыть выбранную. </summary>
        private void Update()
        {
            if (Input.GetKeyDown(_switchKey))
            {
                SwitchTab(!_tab.activeSelf);
            }
            HandleTabButtonsPressed();
        }

        /// <summary> Метод обработки клавиш включения вкладок меню. </summary>
        private void HandleTabButtonsPressed()
        {
            if (_tab.activeInHierarchy && Input.GetButtonDown(_currentTab.ToString()))
            {
                SwitchTab(!_tab.activeSelf);
                return;
            }
            
            for (int i = 0; i < _tabButtonNames.Length; i++)
            {
                if (Input.GetButtonDown(_tabButtonNames[i]))
                {
                    print("OpenTab");
                    SwitchTab(true);
                    _uiSwitcher.SelectTab(i);
                    break; 
                }
            }
        }
        
        /// <summary> Установить текущую вкладку. </summary>
        /// <param name="index"> Индекс вкладки. Конвертируется в enum(ButtonType). </param>
        private void SetCurrentTab(int index)
        {
            _currentTab = (TabType) index;
        }
        
        /// <summary> Переключить вкладку. </summary>
        /// <param name="enabled"> Состояние вкладки - Вкл / Выкл. </param>
        private void SwitchTab(bool enabled) => _tab.SetActive(enabled);
    }
}