using FlavorfulStory.Saving;
using FlavorfulStory.TimeManagement;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.InventorySystem.DropSystem
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

        private void OnEnable()
        {
            WorldTime.OnDayEnded += HandleDayEnded;
        }

        private void OnDisable()
        {
            WorldTime.OnDayEnded -= HandleDayEnded;
        }
        
        /// <summary> Удаляет все выброшенные предметы в контейнере по окончанию дня. </summary>
        private void HandleDayEnded(DateTime _)
        {
            // C конца, чтобы безопасно изменить иерархию
            //TODO: Проверить!
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                var child = transform.GetChild(i);
                if (child) Destroy(child.gameObject);
            }
        }
        
        /// <summary> Сохраняет состояние выброшенных предметов. </summary>
        /// <returns> Сериализованное состояние выброшенных предметов. </returns>
        public object CaptureState() => _dropService?.CaptureState();

        /// <summary> Восстанавливает состояние выброшенных предметов из сериализованных данных. </summary>
        /// <param name="state"> Сохранённое состояние выброшенных предметов. </param>
        public void RestoreState(object state) => _dropService?.RestoreState(state);
    }
}