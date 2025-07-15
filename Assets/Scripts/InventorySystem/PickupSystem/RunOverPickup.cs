using UnityEngine;

namespace FlavorfulStory.InventorySystem.PickupSystem
{
    /// <summary> Автоматический подбор предметов при пересечении триггера. </summary>
    [RequireComponent(typeof(Pickup))]
    public class RunOverPickup : MonoBehaviour
    {
        /// <summary> Ссылка на компонент подбираемого предмета. </summary>
        private Pickup _pickup;

        /// <summary> Получает компонент Pickup при инициализации. </summary>
        private void Awake() => _pickup = GetComponent<Pickup>();

        /// <summary> При нахождении игрока в триггере — попытка подбора предмета. </summary>
        /// <param name="other"> Коллайдер игрока. </param>
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player")) _pickup.TryPickup();
        }
    }
}