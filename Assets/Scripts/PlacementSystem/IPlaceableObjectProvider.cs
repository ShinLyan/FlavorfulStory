using System.Collections.Generic;
using UnityEngine;

namespace FlavorfulStory.PlacementSystem
{
    /// <summary> Интерфейс провайдера размещаемых объектов. </summary>
    public interface IPlaceableObjectProvider
    {
        /// <summary> Зарегистрировать объект. </summary>
        /// <param name="placeableObject"> Объект для регистрации. </param>
        void Register(PlaceableObject placeableObject);

        /// <summary> Отменить регистрацию объекта. </summary>
        /// <param name="placeableObject"> Объект для отмены регистрации. </param>
        void Unregister(PlaceableObject placeableObject);

        /// <summary> Получить все объекты заданного типа компонента. </summary>
        /// <typeparam name="T"> Тип компонента для поиска. </typeparam>
        /// <returns> Перечисление найденных объектов. </returns>
        IEnumerable<T> GetObjectsOfType<T>() where T : Component;
    }
}