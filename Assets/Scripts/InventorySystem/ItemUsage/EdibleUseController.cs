using FlavorfulStory.Actions;
using FlavorfulStory.Audio;
using FlavorfulStory.InputSystem;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.InventorySystem.ItemUsage
{
    /// <summary> Контроллер использования съедобных предметов. </summary>
    public class EdibleUseController : ItemUseController<EdibleInventoryItem>
    {
        /// <summary> Создаёт контроллер для обработки использования съедобных предметов. </summary>
        /// <param name="signalBus"> Шина сигналов для отправки событий. </param>
        public EdibleUseController(SignalBus signalBus) : base(signalBus) { }

        /// <summary> Проверяет ввод и выполняет действие употребления предмета. </summary>
        /// <param name="item"> Съедобный предмет, который можно использовать. </param>
        protected override void TickItem(EdibleInventoryItem item)
        {
            bool leftClick = InputWrapper.GetLeftMouseButton() && item.UseActionType == UseActionType.LeftClick;
            bool rightClick = InputWrapper.GetRightMouseButton() && item.UseActionType == UseActionType.RightClick;
            if (!leftClick && !rightClick) return;

            Fire(new ConsumeSelectedItemSignal(1));
            SfxPlayer.Play(item.SfxType);
            Debug.Log("🍎 Ем вкусную еду. Восстановил HP и энергию.");
        }
    }
}