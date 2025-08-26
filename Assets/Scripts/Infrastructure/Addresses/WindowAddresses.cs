using FlavorfulStory.UI.Windows;
using UnityEngine;

namespace FlavorfulStory.Infrastructure.Addresses
{
    /// <summary> Адреса префабов окон, используемых в игре. </summary>
    [CreateAssetMenu(menuName = "FlavorfulStory/StaticData/WindowAddresses")]
    public class WindowAddresses : ScriptableObject
    {
        /// <summary> Окно подтверждения действия. </summary>
        [field: SerializeField] public BaseWindow ConfirmationWindow { get; private set; }

        /// <summary> Окно итога (например, сна или события). </summary>
        [field: SerializeField] public BaseWindow SummaryWindow { get; private set; }

        /// <summary> Окно ремонта здания. </summary>
        [field: SerializeField] public BaseWindow RepairableBuildingWindow { get; private set; }

        /// <summary> Окно обмена предметами между инвентарями. </summary>
        [field: SerializeField] public BaseWindow InventoryExchangeWindow { get; private set; }

        /// <summary> Главное меню во время игры. </summary>
        [field: SerializeField] public BaseWindow GameMenuWindow { get; private set; }

        /// <summary> Окно настроек игры. </summary>
        [field: SerializeField] public BaseWindow SettingsWindow { get; private set; }

        /// <summary> Окно создания новой игры. </summary>
        [field: SerializeField] public BaseWindow NewGameWindow { get; private set; }

        /// <summary> Окно подтверждения выхода в меню. </summary>
        [field: SerializeField] public BaseWindow ExitConfirmationWindow { get; private set; }
    }
}