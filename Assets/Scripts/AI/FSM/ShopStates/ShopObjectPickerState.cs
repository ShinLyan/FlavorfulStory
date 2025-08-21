using System;
using FlavorfulStory.Shop;
using UnityEngine;

namespace FlavorfulStory.AI.FSM.ShopStates
{
    /// <summary> Универсальное состояние для выбора объекта магазина. </summary>
    public class ShopObjectPickerState<T> : CharacterState where T : ShopObject
    {
        /// <summary> Делегат для выбора объекта. </summary>
        private readonly Func<T> _objectSelector;

        /// <summary> Инициализирует состояние выбора объекта. </summary>
        /// <param name="objectSelector"> Функция выбора объекта. </param>
        public ShopObjectPickerState(Func<T> objectSelector) => _objectSelector = objectSelector;

        /// <summary> Выполняется при входе в состояние. </summary>
        public override void Enter()
        {
            base.Enter();

            var shopObject = _objectSelector();
            if (!shopObject)
            {
                Debug.LogWarning($"No available {typeof(T)} found!");
                return;
            }

            shopObject.IsOccupied = true;

            Context?.Set(FsmContextType.SelectedObject, shopObject);
            Context?.Set(FsmContextType.AnimationType, shopObject.InteractableObjectAnimation);
            Context?.Set(FsmContextType.AnimationTime, 3f);

            var point = shopObject.GetAccessiblePoint();
            if (point.HasValue)
                Context?.Set(FsmContextType.DestinationPoint, point.Value);
            else
                Debug.LogWarning($"No accessible point found for {shopObject.name}!");
        }

        /// <summary> Проверяет завершение состояния. </summary>
        /// <returns> Всегда true, так как состояние выполняется мгновенно. </returns>
        public override bool IsComplete() => true;
    }
}