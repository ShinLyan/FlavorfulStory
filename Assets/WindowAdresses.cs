using UnityEngine;

namespace FlavorfulStory
{
    [CreateAssetMenu(menuName = "FlavorfulStory/StaticData/WindowAddresses", fileName = "WindowAddresses")]
    public class WindowAdresses: ScriptableObject
    {
        //Все окна добавляются сюда
        [field: SerializeField] public GameObject ConfirmationWindow { get; private set; }
    }
}