using FlavorfulStory.InteractionSystem;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.InventorySystem.UI;
using FlavorfulStory.Player;
using FlavorfulStory.TooltipSystem.ActionTooltips;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.Actions.Interactables
{
    /// <summary> Объект-сундук, с которым можно взаимодействовать для обмена предметами. </summary>
    [RequireComponent(typeof(Inventory))]
    public class Chest : MonoBehaviour, IInteractable
    {
        /// <summary> Данные для отображения тултипа действия (открыть сундук). </summary>
        [Inject] private InventoryExchangeWindow _exchangeWindow;
        
        /// <summary> Разрешено ли взаимодействие с объектом. </summary>
        public ActionTooltipData ActionTooltip => new("E", ActionType.Open, "Chest");
        public bool IsInteractionAllowed => true;

        /// <summary> Расстояние до игрока. </summary>
        public float GetDistanceTo(Transform otherTransform) =>
            Vector3.Distance(transform.position, otherTransform.position);

        /// <summary> Начать взаимодействие — открыть окно обмена между игроком и сундуком. </summary>
        public void BeginInteraction(PlayerController player)
        {
            player.SetBusyState(true);

            _exchangeWindow.Show(
                player.GetComponent<Inventory>(), 
                GetComponent<Inventory>(),
                () => player.SetBusyState(false)
            );
        }

        /// <summary> Завершить взаимодействие. </summary>
        public void EndInteraction(PlayerController player) { }
    }
}