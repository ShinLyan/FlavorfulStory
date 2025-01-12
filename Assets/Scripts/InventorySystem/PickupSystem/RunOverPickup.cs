using UnityEngine;

namespace FlavorfulStory.InventorySystem.PickupSystem
{
    [RequireComponent(typeof(Pickup))]
    public class RunOverPickup : MonoBehaviour
    {
        /// <summary> Метод, вызывающийся при пересечении с коллайдером другого объекта. </summary>
        /// <param name="other"> Коллайдер входящего объекта. </param>
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var pickup = GetComponent<Pickup>();
                if (pickup.CanBePickedUp) pickup.PickupItem();
            }
        }
    }
}
