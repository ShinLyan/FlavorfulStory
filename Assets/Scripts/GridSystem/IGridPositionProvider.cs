using UnityEngine;

namespace FlavorfulStory.GridSystem
{
    /// <summary> Интерфейс для предоставления информации о позиции на гриде и преобразования координат. </summary>
    public interface IGridPositionProvider
    {
        /// <summary> Пытается получить текущую позицию курсора в координатах грида. </summary>
        /// <param name="position"> Возвращаемая позиция в гриде. </param>
        /// <returns> True – если позиция успешно получена, иначе False. </returns>
        bool TryGetCursorGridPosition(out Vector3Int position);

        /// <summary> Преобразует координаты грида в мировые координаты. </summary>
        /// <param name="gridPosition"> Позиция в гриде. </param>
        /// <returns> Мировые координаты. </returns>
        Vector3 GridToWorld(Vector3Int gridPosition);

        /// <summary> Преобразует мировые координаты в координаты грида. </summary>
        /// <param name="worldPosition"> Мировая позиция. </param>
        /// <returns> Позиция в гриде. </returns>
        Vector3Int WorldToGrid(Vector3 worldPosition);
    }
}