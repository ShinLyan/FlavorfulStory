using FlavorfulStory.Actions;
using FlavorfulStory.InteractionSystem;
using FlavorfulStory.Player;
using UnityEngine;

namespace FlavorfulStory
{
    public class CraftingStation : MonoBehaviour, IInteractable
    {
        public ActionDescription ActionDescription { get; }
        public bool IsInteractionAllowed { get; }
        public float GetDistanceTo(Transform otherTransform) 
            => Vector3.Distance(otherTransform.position, transform.position);

        public void BeginInteraction(PlayerController player)
        {
            throw new System.NotImplementedException();
        }

        public void EndInteraction(PlayerController player) => player.SetBusyState(false);
    }
}