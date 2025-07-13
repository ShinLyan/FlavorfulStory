using UnityEngine;
using Zenject;
using FlavorfulStory.Actions;
using FlavorfulStory.InputSystem;
using FlavorfulStory.InteractionSystem;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.Player;

namespace FlavorfulStory.Crafting
{
    /// <summary> Станция крафта, с которой может взаимодействовать игрок. </summary>
    public class CraftingStation : MonoBehaviour, IInteractable
    {
        /// <summary> Окно крафта, отображаемое при взаимодействии. </summary>
        private CraftingWindow _craftingWindow;

        /// <summary> Инвентарь игрока, участвующий в крафте. </summary>
        private Inventory _playerInventory;

        /// <summary> Описание действия, отображаемого при наведении на объект. </summary>
        [field: SerializeField] public ActionDescription ActionDescription { get; private set; }

        /// <summary> Разрешено ли сейчас взаимодействие с крафт-станцией. </summary>
        public bool IsInteractionAllowed { get; private set; } = true;

        /// <summary> Внедряет зависимости окна крафта и инвентаря игрока. </summary>
        /// <param name="craftingWindow"> Окно крафта. </param>
        /// <param name="playerInventory"> Инвентарь игрока. </param>
        [Inject]
        private void Construct(CraftingWindow craftingWindow, Inventory playerInventory)
        {
            _craftingWindow = craftingWindow;
            _playerInventory = playerInventory;
        }

        /// <summary> Возвращает расстояние до игрока. </summary>
        /// <param name="otherTransform"> Трансформ игрока. </param>
        /// <returns> Расстояние до игрока в метрах. </returns>
        public float GetDistanceTo(Transform otherTransform)
            => Vector3.Distance(otherTransform.position, transform.position);

        /// <summary> Запускает взаимодействие игрока с крафт-станцией. </summary>
        /// <param name="player"> Контроллер игрока. </param>
        public void BeginInteraction(PlayerController player)
        {
            _craftingWindow.Setup(
                CraftingRecipeProvider.GetAllRecipes(),
                OnCraftRequested,
                () => EndInteraction(player)
            );

            _craftingWindow.Open();
            InputWrapper.UnblockInput(InputButton.SwitchGameMenu);
        }

        /// <summary> Завершает взаимодействие с крафт-станцией. </summary>
        /// <param name="player"> Контроллер игрока. </param>
        public void EndInteraction(PlayerController player) { player.SetBusyState(false); }

        /// <summary> Обрабатывает событие запроса крафта. </summary>
        /// <param name="recipe"> Выбранный рецепт. </param>
        /// <param name="count"> Количество создаваемых наборов. </param>
        private async void OnCraftRequested(CraftingRecipe recipe, int count)
        {
            if (!IsInteractionAllowed)
            {
                Debug.LogWarning("Crafting is already in progress.");
                return;
            }

            var result = CraftingProcessor.CanCraft(recipe, count, _playerInventory);
            if (result != CraftingResult.Success)
            {
                Debug.LogWarning($"Crafting aborted: {result}");
                return;
            }

            IsInteractionAllowed = false;

            await CraftingProcessor.ExecuteRecipe(recipe, count, _playerInventory, () =>
            {
                IsInteractionAllowed = true;
                _craftingWindow.OnCraftCompleted();
            });
        }
    }
}