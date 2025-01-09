using UnityEngine;

namespace FlavorfulStory.UI
{
    /// <summary> Переключатель вкладки при нажатии на клавишу.</summary>
    public class KeySwitcher : MonoBehaviour
    {
        /// <summary> Клавиша переключения.</summary>
        [SerializeField] private KeyCode _switchKey;

        /// <summary> Вкладка, которую необходимо переключать.</summary>
        [SerializeField] private GameObject _tab;

        /// <summary> Поле дочернего UISwitcher. </summary>
        private UISwitcher _uiSwitcher;

        /// <summary> Массив имен кнопок для включения определенных вкладок. </summary>
        private string[] _tabButtonNames;

        /// <summary> Инициализация компонента UISwitcher. Получение имен кнопок для переключения вкладок. </summary>
        private void Awake()
        {
            _uiSwitcher = GetComponentInChildren<UISwitcher>();
            _tabButtonNames = _uiSwitcher.GetTabNames();
        }

        /// <summary> При старте выключаем вкладку.</summary>
        private void Start()
        {
            SwitchTab(false);
        }

        /// <summary> При нажатии на клавишу переключать вкладку или открыть выбранную.</summary>
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
            for (int i = 0; i < _tabButtonNames.Length; i++)
            {
                if (Input.GetButtonDown(_tabButtonNames[i]))
                {
                    SwitchTab(true);
                    _uiSwitcher.SelectTab(i);
                    return;
                }
            }
        }

        /// <summary> Переключить вкладку.</summary>
        /// <param name="enabled"> Состояние вкладки - Вкл / Выкл.</param>
        private void SwitchTab(bool enabled) => _tab.SetActive(enabled);
    }
}