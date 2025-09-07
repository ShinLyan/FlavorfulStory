using System.Collections.Generic;
using FlavorfulStory.Windows.UI;
using UnityEngine;

namespace FlavorfulStory.Windows
{
    /// <summary> Адреса префабов окон, используемых в игре. </summary>
    [CreateAssetMenu(menuName = "FlavorfulStory/StaticData/WindowAddresses")]
    public class WindowAddresses : ScriptableObject
    {
        /// <summary> Окно подтверждения действия. </summary>
        [SerializeField] private BaseWindow _confirmationWindow;

        /// <summary> Окно итога (например, сна или события). </summary>
        [SerializeField] private BaseWindow _summaryWindow;

        /// <summary> Окно ремонта здания. </summary>
        [SerializeField] private BaseWindow _repairableBuildingWindow;

        /// <summary> Окно обмена предметами между инвентарями. </summary>
        [SerializeField] private BaseWindow _inventoryExchangeWindow;

        /// <summary> Главное меню во время игры. </summary>
        [SerializeField] private BaseWindow _gameMenuWindow;

        /// <summary> Окно настроек игры. </summary>
        [SerializeField] private BaseWindow _settingsWindow;

        /// <summary> Окно создания новой игры. </summary>
        [SerializeField] private BaseWindow _newGameWindow;

        /// <summary> Окно подтверждения выхода в меню. </summary>
        [SerializeField] private BaseWindow _exitConfirmationWindow;

        /// <summary> Возвращает все окна с адресами. </summary>
        /// <returns> Все окна с адресами. </returns>
        public IEnumerable<BaseWindow> AllWindows()
        {
            yield return _confirmationWindow;
            yield return _summaryWindow;
            yield return _repairableBuildingWindow;
            yield return _inventoryExchangeWindow;
            yield return _gameMenuWindow;
            yield return _settingsWindow;
            yield return _newGameWindow;
            yield return _exitConfirmationWindow;
        }
    }
}