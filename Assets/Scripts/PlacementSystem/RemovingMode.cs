using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.Audio;
using FlavorfulStory.GridSystem;
using UnityEngine;

namespace FlavorfulStory.PlacementSystem
{
    /// <summary> Режим удаления размещённых объектов. </summary>
    public class RemovingMode : IPlacementMode
    {
        /// <summary> Провайдер позиции на гриде. </summary>
        private readonly GridPositionProvider _positionProvider;

        /// <summary> Компонент предпросмотра действий размещения/удаления. </summary>
        private readonly PlacementPreview _placementPreview;

        /// <summary> Данные грид-слоёв с занятостью ячеек и объектами. </summary>
        private readonly Dictionary<PlacementLayer, PlacementGridData> _gridLayers;

        /// <summary> Приоритет слоёв для удаления (сверху вниз). </summary>
        private static readonly PlacementLayer[] RemovalPriority =
        {
            PlacementLayer.Decoration,
            PlacementLayer.Furniture,
            PlacementLayer.Wall,
            PlacementLayer.Floor
        };

        /// <summary> Провайдер размещаемых объектов. </summary>
        private readonly PlaceableObjectProvider _placeableObjectProvider;

        /// <summary> Конструктор режима удаления. </summary>
        /// <param name="positionProvider"> Провайдер позиционирования на гриде. </param>
        /// <param name="placementPreview"> Компонент предпросмотра. </param>
        /// <param name="gridLayers"> Карта слоёв размещения с их данными. </param>
        /// <param name="placeableObjectProvider"> Провайдер размещаемых объектов. </param>
        public RemovingMode(GridPositionProvider positionProvider, PlacementPreview placementPreview,
            Dictionary<PlacementLayer, PlacementGridData> gridLayers, PlaceableObjectProvider placeableObjectProvider)
        {
            _positionProvider = positionProvider;
            _placementPreview = placementPreview;
            _gridLayers = gridLayers;
            _placeableObjectProvider = placeableObjectProvider;
        }

        /// <summary> Вход в режим — показать превью удаления. </summary>
        public void Enter() => _placementPreview.StartShowingRemovePreview();

        /// <summary> Выход из режима — скрыть превью. </summary>
        public void Exit() => _placementPreview.StopShowingPreview();

        /// <summary> Пытается удалить объект в указанной ячейке грида. </summary>
        /// <param name="gridPosition"> Позиция ячейки в координатах грида. </param>
        /// <returns> True — если объект найден и удалён; False — если удалить нечего. </returns>
        public bool TryApply(Vector3Int gridPosition)
        {
            if (TryRemoveAtSilent(gridPosition, out var removed))
            {
                _placeableObjectProvider.Unregister(removed);
                Object.Destroy(removed);
                SfxPlayer.Play(SfxType.RemoveObject);

                _placementPreview.UpdatePosition(_positionProvider.GridToWorld(gridPosition),
                    IsSelectionValid(gridPosition));
                return true;
            }

            SfxPlayer.Play(SfxType.PlacementError);
            _placementPreview.UpdatePosition(_positionProvider.GridToWorld(gridPosition), false);
            return false;
        }

        /// <summary> Попробовать выполнить удаление без звуков и обновления превью. </summary>
        /// <param name="gridPosition"> Позиция ячейки в координатах грида. </param>
        /// <param name="removedPlaceable"> Найденный и удалённый объект, либо null. </param>
        /// <returns> <c>true</c> — если объект был найден и удалён; <c>false</c> — если удалить нечего. </returns>
        public bool TryRemoveAtSilent(Vector3Int gridPosition, out PlaceableObject removedPlaceable)
        {
            removedPlaceable = null;

            foreach (var layer in RemovalPriority)
            {
                if (!_gridLayers.TryGetValue(layer, out var gridData)) continue;
                if (gridData.CanPlaceObjectAt(gridPosition, Vector2Int.one)) continue;

                var target = gridData.GetPlacedObject(gridPosition);
                if (!target) return false;

                gridData.RemoveObjectAt(gridPosition);
                removedPlaceable = target;
                _placeableObjectProvider.Unregister(removedPlaceable);
                return true;
            }

            return false;
        }

        /// <summary> Обновляет превью для текущей ячейки. </summary>
        /// <param name="gridPosition"> Позиция ячейки в координатах грида. </param>
        public void Refresh(Vector3Int gridPosition) =>
            _placementPreview.UpdatePosition(_positionProvider.GridToWorld(gridPosition),
                IsSelectionValid(gridPosition));

        /// <summary> Проверяет, есть ли что удалять в указанной ячейке хотя бы в одном слое по приоритету. </summary>
        /// <param name="gridPosition"> Позиция ячейки в координатах грида. </param>
        /// <returns> True — если ячейка занята (удаление возможно); False — если свободна. </returns>
        private bool IsSelectionValid(Vector3Int gridPosition) =>
            RemovalPriority.Any(layer => !_gridLayers[layer].CanPlaceObjectAt(gridPosition, Vector2Int.one));
    }
}