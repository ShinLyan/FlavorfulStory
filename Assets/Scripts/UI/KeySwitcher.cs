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

        /// <summary> При старте выключаем вкладку.</summary>
        private void Start() => SwitchTab(false);

        /// <summary> При нажатии на клавишу переключать вкладку.</summary>
        private void Update()
        {
            if (Input.GetKeyDown(_switchKey))
            {
                SwitchTab(!_tab.activeSelf);
            }
        }

        /// <summary> Переключить вкладку.</summary>
        /// <param name="enabled"> Состояние вкладки - Вкл / Выкл.</param>
        private void SwitchTab(bool enabled) => _tab.SetActive(enabled);
    }
}