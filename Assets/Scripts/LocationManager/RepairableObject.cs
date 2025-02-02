using UnityEngine;

namespace FlavorfulStory.LocationManager
{
    /// <summary> Объект, который можно восстановить через взаимодействие. </summary>
    public class RepairableObject : InteractableObject2
    {
        /// <summary> Выполняет действие взаимодействия с восстанавливаемым объектом. </summary>
        public override void Interact()
        {
            base.Interact();
            Debug.Log("Interacting with repairable object " + gameObject.name);
        }
    }
}