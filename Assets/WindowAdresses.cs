using UnityEngine;

namespace FlavorfulStory
{
    [CreateAssetMenu(menuName = "FlavorfulStory/StaticData/WindowAddresses", fileName = "WindowAddresses")]
    public class WindowAdresses: ScriptableObject
    {
        //Все окна добавляются сюда
        [field: SerializeField] public BaseWindow ConfirmationWindow { get; private set; }
        [field: SerializeField] public BaseWindow SummaryWindow { get; private set; }
        [field: SerializeField] public BaseWindow RepairableBuildingWindow { get; private set; }
        [field: SerializeField] public BaseWindow InventoryExchangeWindow { get; private set; }
        [field: SerializeField] public BaseWindow GameMenu { get; private set; }
        [field: SerializeField] public BaseWindow SettingsWindow { get; private set; }
        [field: SerializeField] public BaseWindow NewGameWindow { get; private set; }
        [field: SerializeField] public BaseWindow ExitConfirmationWindow { get; private set; }
    }
}