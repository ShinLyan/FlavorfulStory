using UnityEngine;

namespace FlavorfulStory.InventorySystem.PickupSystem
{
    /// <summary> Автоматический подбор предметов при пересечении триггера. </summary>
    [RequireComponent(typeof(Pickup))]
    public class RunOverPickup : MonoBehaviour
    {
        /// <summary> Обрабатывает событие пересечения триггера. </summary>
        /// <param name="other"> Коллайдер объекта, пересекшего триггер. </param>
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            
            var pickup = GetComponent<Pickup>();
            if (pickup.CanBePickedUp) pickup.PickupItem();
        }
    }
}