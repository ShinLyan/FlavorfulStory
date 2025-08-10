using UnityEngine;

namespace FlavorfulStory.PlacementSystem
{
    /// <summary> Интерфейс режима размещения объектов на сетке. </summary>
    public interface IPlacementMode
    {
        /// <summary> Вход в режим размещения (подготовка UI, инициализация и т.д.). </summary>
        void Enter();

        /// <summary> Выход из режима размещения (очистка состояния, скрытие UI). </summary>
        void Exit();

        /// <summary> Попробовать применить действие размещения в заданной позиции на сетке. </summary>
        /// <param name="gridPosition"> Позиция на сетке, в которую нужно разместить объект. </param>
        /// <returns> <c>true</c> - действие применено, <c>false</c> - не применено. </returns>
        bool TryApply(Vector3Int gridPosition);

        /// <summary> Обновить визуальное представление размещения без применения. </summary>
        /// <param name="gridPosition"> Текущая позиция курсора на сетке. </param>
        void Refresh(Vector3Int gridPosition);
    }
}