using FlavorfulStory.InventorySystem.DropSystem;
using FlavorfulStory.Saving;
using UnityEngine;
using Zenject;

namespace FlavorfulStory
{
    /// <summary> Компонент, сохраняющий и восстанавливающий состояние выброшенных предметов. </summary>
    /// <remarks> Устанавливает контейнер для дропа и делегирует сохранение `ItemDropService`. </remarks>
    public class ItemDropServiceSaver : MonoBehaviour, ISaveable
    {
        /// <summary> Ссылка на сервис, реализующий сохранение выброшенных предметов. </summary>
        private ISaveable _dropService;

        /// <summary> Внедряет сервис выброса предметов и задаёт контейнер для дропа. </summary>
        /// <param name="dropService"> Сервис выброса предметов. </param>
        [Inject]
        private void Construct(IItemDropService dropService)
        {
            _dropService = dropService as ISaveable;
            dropService.SetDroppedItemsContainer(transform);
        }

        /// <summary> Сохраняет состояние всех выброшенных предметов. </summary>
        public object CaptureState() => _dropService?.CaptureState();

        /// <summary> Восстанавливает выброшенные предметы из сохранённого состояния. </summary>
        /// <param name="state"> Сериализованные данные. </param>
        public void RestoreState(object state) => _dropService?.RestoreState(state);
    }
}