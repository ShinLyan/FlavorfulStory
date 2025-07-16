using UnityEngine;
using Zenject;

namespace FlavorfulStory.InventorySystem.PickupSystem
{
    /// <summary> Фабрика для создания экземпляров предметов, доступных для подбора. </summary>
    public class PickupFactory
    {
        /// <summary> Контейнер Zenject для внедрения зависимостей и создания объектов. </summary>
        private readonly DiContainer _container;

        /// <summary> Конструктор фабрики. </summary>
        /// <param name="container"> Контейнер зависимостей Zenject. </param>
        public PickupFactory(DiContainer container) => _container = container;

        /// <summary> Создаёт объект подбираемого предмета на сцене. </summary>
        /// <param name="itemStack"> Предмет и его количество, которые должны быть созданы. </param>
        /// <param name="position"> Позиция появления предмета. </param>
        /// <param name="pickupDelay"> Задержка (в секундах) активации  возможности подобрать предмет. </param>
        /// <param name="parent"> Родительский объект в иерархии. </param>
        /// <returns> Ссылка на созданный объект <see cref="Pickup"/>, либо <c>null</c> при ошибке. </returns>
        public Pickup Create(ItemStack itemStack, Vector3 position, float pickupDelay = 1f, Transform parent = null)
        {
            var item = itemStack.Item;
            if (!item || !item.PickupPrefab)
            {
                Debug.LogError($"PickupPrefab не назначен для предмета {item.name}");
                return null;
            }

            var pickup = _container.InstantiatePrefabForComponent<Pickup>(item.PickupPrefab, position,
                Quaternion.identity, parent);
            pickup.Setup(itemStack, pickupDelay);
            return pickup;
        }
    }
}