using FlavorfulStory.Actions;
using FlavorfulStory.Shop;
using UnityEngine;

namespace FlavorfulStory.AI.FiniteStateMachine.ShopStates
{
    /// <summary> Состояние для обработки оплаты товаров у кассы. </summary>
    public class PaymentState : CharacterState
    {
        /// <summary> Обработчик предметов для управления экипировкой товаров. </summary>
        private readonly ItemHandler _itemHandler;

        /// <summary> Локация магазина для взаимодействия с кассой. </summary>
        private readonly ShopLocation _shopLocation;

        /// <summary> Инициализирует новый экземпляр состояния оплаты. </summary>
        /// <param name="shopLocation"> Локация магазина для доступа к кассе. </param>
        /// <param name="itemHandler"> Обработчик предметов для управления товарами. </param>
        public PaymentState(ShopLocation shopLocation, ItemHandler itemHandler)
        {
            _shopLocation = shopLocation;
            _itemHandler = itemHandler;
        }

        /// <summary> Выполняет вход в состояние, освобождает точку у кассы и обрабатывает оплату. </summary>
        public override void Enter()
        {
            if (Context != null && Context.TryGet<Transform>("CashDeskPoint", out var point))
                _shopLocation.CashDesk.ReleasePoint(point);
            // _itemHandler.EquipItem(item); // TODO
        }

        /// <summary> Возвращает статус завершения состояния. </summary>
        /// <returns> Всегда возвращает true, так как состояние завершается сразу после входа. </returns>
        public override bool IsComplete() => true;
    }
}