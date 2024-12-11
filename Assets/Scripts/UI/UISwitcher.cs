using UnityEngine;

namespace FlavorfulStory.UI
{
    /// <summary> Переключатель UI.</summary>
    public class UISwitcher : MonoBehaviour
    {
        /// <summary> Объект, который по умолчанию включается при старте сцены.</summary>
        [SerializeField] private GameObject _entryPoint;

        /// <summary> Массив кнопок, открывающих соответствующие вкладки.</summary>
        [SerializeField] private TabSwitherButton[] _tabButtons;

        /// <summary> Клавиша, которая переключает на предыдущую вкладку.</summary>
        private const KeyCode _previousTabKey = KeyCode.Q;

        /// <summary> Клавиша, которая переключается на следующую вкладку.</summary>
        private const KeyCode _nextTabKey = KeyCode.R;

        private int _currentTabIndex = 0;

        /// <summary> При старте включаем нужный объект.</summary>
        private void Start()
        {
            if (_entryPoint != null) SwitchTo(_entryPoint);

            if (ValidateSetup()) _tabButtons[_currentTabIndex].Select();
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

        //  Убеждаемся, что количество кнопок и вкладок совпадает
        private bool ValidateSetup()
        {
            return _tabButtons != null && _tabButtons.Length == transform.childCount;
        }

        /// <summary> Переключить на нужную вкладку.</summary>
        /// <param name="toDisplay"> Вкладка, которую нужно открыть.</param>
        public void SwitchTo(GameObject toDisplay)
        {
            if (toDisplay.transform.parent != transform) return;

            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
            toDisplay.SetActive(true);
        }
    }
}