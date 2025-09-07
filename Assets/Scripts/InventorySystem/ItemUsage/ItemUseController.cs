using System;
using FlavorfulStory.TimeManagement;
using FlavorfulStory.Toolbar;
using Zenject;

namespace FlavorfulStory.InventorySystem.ItemUsage
{
    /// <summary> Базовый обработчик ввода для предметов. </summary>
    /// <remarks> Подписывается на выбор предмета в тулбаре,
    /// хранит активный предмет нужного типа и тикает только когда игра не на паузе. </remarks>
    public abstract class ItemUseController<TItem> : IInitializable, IDisposable, ITickable
        where TItem : InventoryItem
    {
        /// <summary> Сигнальная шина Zenject. </summary>
        private readonly SignalBus _signalBus;

        /// <summary> Активный предмет. </summary>
        private TItem _activeItem;

        /// <summary> Создаёт контроллер использования предметов указанного типа. </summary>
        /// <param name="signalBus"> Шина сигналов для подписки и отправки событий. </param>
        protected ItemUseController(SignalBus signalBus) => _signalBus = signalBus;

        /// <summary> Подписаться на события. </summary>
        public void Initialize() => _signalBus.Subscribe<ToolbarSlotSelectedSignal>(OnToolbarItemChanged);

        /// <summary> Отписаться от событий. </summary>
        public void Dispose() => _signalBus.Unsubscribe<ToolbarSlotSelectedSignal>(OnToolbarItemChanged);

        /// <summary> Обновляет активный предмет при смене выбранного слота тулбара. </summary>
        /// <param name="signal"> Сигнал о выборе нового предмета. </param>
        private void OnToolbarItemChanged(ToolbarSlotSelectedSignal signal)
        {
            var previous = _activeItem;
            _activeItem = signal.SelectedItem as TItem;
            OnSelectedChanged(previous, _activeItem);
        }

        /// <summary> Покадровая логика – вызывается только если игра не на паузе и есть активный предмет. </summary>
        public void Tick()
        {
            if (WorldTime.IsPaused || !_activeItem) return;

            TickItem(_activeItem);
        }

        /// <summary> Реакция на смену выбранного предмета. </summary>
        /// <param name="previous"> Предыдущий выбранный предмет. </param>
        /// <param name="current"> Новый выбранный предмет. </param>
        protected virtual void OnSelectedChanged(TItem previous, TItem current) { }

        /// <summary> Покадровая логика для активного предмета (когда не пауза и предмет не null). </summary>
        /// <param name="item"> Активный предмет. </param>
        protected virtual void TickItem(TItem item) { }

        /// <summary> Отправить сигнал через шину сигналов. </summary>
        /// <typeparam name="TSignal"> Тип отправляемого сигнала. </typeparam>
        /// <param name="signal"> Экземпляр сигнала. </param>
        protected void Fire<TSignal>(TSignal signal) => _signalBus.Fire(signal);
    }
}