using FlavorfulStory.Actions;
using FlavorfulStory.InteractionSystem;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.Player;
using FlavorfulStory.TooltipSystem.ActionTooltips;
using UnityEngine;
using Zenject;

namespace FlavorfulStory
{
    //TODO: Добавить сохраняемость, в том числе для тех, кто спавнится в рантайме
    [RequireComponent(typeof(Inventory))]
    public class Chest : MonoBehaviour, IInteractable
    {
        [Inject] private InventoryExchangeWindow _exchangeWindow;
        
        public ActionTooltipData ActionTooltip => new("E", ActionType.Open, "Chest");
        public bool IsInteractionAllowed => true;

        public float GetDistanceTo(Transform otherTransform) =>
            Vector3.Distance(transform.position, otherTransform.position);

        public void BeginInteraction(PlayerController player)
        {
            player.SetBusyState(true);

            _exchangeWindow.Show(
                player.GetComponent<Inventory>(), 
                GetComponent<Inventory>(),
                () => player.SetBusyState(false)
            );
        }

        public void EndInteraction(PlayerController player) { }
    }
}