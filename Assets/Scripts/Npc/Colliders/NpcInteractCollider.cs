using UnityEngine;

namespace NPC
{
    /// <summary> Класс описывающий коллайдер-триггер для взаимодействия с интерактивным NPC.</summary>
    public class NpcInteractCollider : MonoBehaviour //все коллайдеры оптимизировать. Много лишних действий в OnTriggerEnter / Stay
    {
        /// <summary>Метод вызывающийся при нахождении в коллайдере объекта на котором этот скрипт висит .</summary>
        /// <param name="other"> Коллайдер входящего объекта.</param>
        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                Npc npc = this.GetComponent<Npc>();
                if (npc != null)
                {
                    if (npc is INpcIteractive)
                    {
                        (npc as INpcIteractive).Interact();
                    }
                }
            }
        }
    }
}