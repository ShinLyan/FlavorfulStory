using FlavorfulStory.AI.NonInteractableNpc;
using FlavorfulStory.Shop;
using UnityEngine;

namespace FlavorfulStory.AI.FSM.ShopStates
{
    /// <summary> Состояние для выбора мебели и перемещения к ней персонажа. </summary>
    public class FurniturePickerState : CharacterState
    {
        /// <summary> Контроллер движения неинтерактивного NPC. </summary>
        private readonly NonInteractableNpcMovementController _movementController;

        /// <summary> Локация магазина для получения доступной мебели. </summary>
        private readonly ShopLocation _shopLocation;

        /// <summary> Инициализирует новый экземпляр состояния выбора мебели. </summary>
        /// <param name="movementController"> Контроллер движения для управления перемещением NPC. </param>
        /// <param name="shopLocation"> Локация магазина для поиска доступной мебели. </param>
        public FurniturePickerState(NonInteractableNpcMovementController movementController, ShopLocation shopLocation)
        {
            _movementController = movementController;
            _shopLocation = shopLocation;
        }

        /// <summary> Выполняет вход в состояние, выбирает доступную мебель и устанавливает цель движения. </summary>
        public override void Enter()
        {
            var furniture = _shopLocation.GetAvailableFurniture();

            if (!furniture) return;

            furniture.IsOccupied = true;
            Context?.Set<ShopObject>(FsmContextType.SelectedObject, furniture);
            Context?.Set(FsmContextType.AnimationType, furniture.InteractableObjectAnimation);
            Context?.Set(FsmContextType.AnimationTime, 3f);

            var accessiblePoint = furniture.GetAccessiblePoint();

            if (accessiblePoint.HasValue)
                _movementController.SetPoint(accessiblePoint.Value);
            else
                Debug.LogWarning("No accessible point found for the furniture!");
        }

        /// <summary> Возвращает статус завершения состояния. </summary>
        /// <returns> Всегда возвращает true, так как состояние завершается сразу после входа. </returns>
        public override bool IsComplete() => true;
    }
}