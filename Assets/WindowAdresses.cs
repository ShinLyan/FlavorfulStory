using UnityEngine;

namespace FlavorfulStory
{
    [CreateAssetMenu(menuName = "FlavorfulStory/StaticData/WindowAddresses", fileName = "WindowAddresses")]
    public class WindowAdresses: ScriptableObject
    {
        //Все окна добавляются сюда
        [field: SerializeField] public GameObject ConfirmationWindow { get; private set; }
        [field: SerializeField] public GameObject SummaryWindow { get; private set; }
        [field: SerializeField] public GameObject RepairableBuildingWindow { get; private set; }
        [field: SerializeField] public GameObject InventoryExchangeWindow { get; private set; }
        [field: SerializeField] public GameObject GameMenu { get; private set; }
        [field: SerializeField] public GameObject SettingsWindow { get; private set; }
        [field: SerializeField] public GameObject NewGameWindow { get; private set; }
        [field: SerializeField] public GameObject ExitConfirmationWindow { get; private set; }
    }
}