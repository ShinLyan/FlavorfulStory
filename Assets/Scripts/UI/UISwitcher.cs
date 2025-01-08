using System;
using System.Linq;
using UnityEngine;

namespace FlavorfulStory.UI
{
    /// <summary> ������������� UI.</summary>
    public class UISwitcher : MonoBehaviour
    {
        /// <summary> ������, ������� �� ��������� ���������� ��� ������ �����.</summary>
        [SerializeField] private GameObject _entryPoint;

        /// <summary> ������ ������, ����������� ��������������� �������.</summary>
        [SerializeField] private TabSwitcherButton[] _tabButtons;

        /// <summary> �������, ������� ����������� �� ���������� �������.</summary>
        private const KeyCode _previousTabKey = KeyCode.Q;

        /// <summary> �������, ������� ������������� �� ��������� �������.</summary>
        private const KeyCode _nextTabKey = KeyCode.R;

        private int _currentTabIndex = 0;

        /// <summary> ��� ������ �������� ������ ������.</summary>
        private void Start()
        {
            if (_entryPoint != null) SwitchTo(_entryPoint);

            if (ValidateSetup()) _tabButtons[_currentTabIndex].Select();
            
            InitializeTabButtons();
        }

        private void Update()
        {
            if (!ValidateSetup()) return;

            if (Input.GetKeyDown(_previousTabKey) || Input.GetKeyDown(_nextTabKey))
            {
                bool isPreviousTab = Input.GetKeyDown(_previousTabKey);
                int direction = isPreviousTab ? -1 : 1;
                int index = (_currentTabIndex + _tabButtons.Length + direction) % _tabButtons.Length;
                SelectTab(index);
            }
        }

        public void SelectTab(int index)
        {
            _tabButtons[_currentTabIndex].ResetSelection();
            _tabButtons[index].Select();
            _currentTabIndex = index;
        }

        //  ����������, ��� ���������� ������ � ������� ���������
        private bool ValidateSetup()
        {
            return _tabButtons != null && _tabButtons.Length == transform.childCount;
        }

        /// <summary> Метод инициализации кнопок открытия вкладки.
        /// Добавляет хэндлер события, что вызывается при нажатии кнопки. </summary>
        private void InitializeTabButtons()
        {
            for (int i = 0; i < _tabButtons.Length; i++)
            {
                int tabIndex = i;
                _tabButtons[i].AddOnClickHandler(() => SelectTab(tabIndex));
            }
        }
        
        /// <summary> ����������� �� ������ �������.</summary>
        /// <param name="toDisplay"> �������, ������� ����� �������.</param>
        public void SwitchTo(GameObject toDisplay)
        {
            if (toDisplay.transform.parent != transform) return;

            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
            toDisplay.SetActive(true);
        }
        
        /// <summary> Метод нахождения количества вкладок меню. </summary>
        /// <returns> Количество вкладок. </returns>
        public int GetTabCount() => _tabButtons.Length;
        
        /// <summary> Метод получения имен вкладок меню, используемых в InputManager. </summary>
        /// <returns> Имена вкладок меню в виде массива </returns>
        public string[] GetTabNames() => _tabButtons.Select(button => button.ButtonType.ToString()).ToArray();
    }
}