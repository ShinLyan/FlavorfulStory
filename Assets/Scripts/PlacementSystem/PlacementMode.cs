using System.Collections.Generic;
using FlavorfulStory.Audio;
using FlavorfulStory.GridSystem;
using UnityEngine;

namespace FlavorfulStory.PlacementSystem
{
    /// <summary> Режим размещения объекта. </summary>
    public class PlacementMode : IPlacementMode
    {
        /// <summary> Провайдер координат и конвертации между миром и гридом. </summary>
        private readonly GridPositionProvider _positionProvider;

        /// <summary> Компонент, отображающий превью размещаемого объекта. </summary>
        private readonly PlacementPreview _placementPreview;

        /// <summary> Словарь слоев размещения и данных по занятым ячейкам. </summary>
        private readonly Dictionary<PlacementLayer, PlacementGridData> _gridLayers;

        /// <summary> Объект, который будет размещен. </summary>
        public PlaceableObject PlaceableObject { get; set; }

        /// <summary> Конструктор с параметрами. </summary>
        /// <param name="positionProvider"> Провайдер координат грида. </param>
        /// <param name="placementPreview"> Превью размещаемого объекта. </param>
        /// <param name="gridLayers"> Словарь данных по слоям размещения. </param>
        public PlacementMode(GridPositionProvider positionProvider, PlacementPreview placementPreview,
            Dictionary<PlacementLayer, PlacementGridData> gridLayers)
        {
            _positionProvider = positionProvider;
            _placementPreview = placementPreview;
            _gridLayers = gridLayers;
        }

        /// <summary> Вход в режим — начать показ превью размещения. </summary>
        public void Enter() => _placementPreview.StartShowingPlacementPreview(PlaceableObject);

        /// <summary> Выход из режима — скрыть превью размещения. </summary>
        public void Exit() => _placementPreview.StopShowingPreview();

        /// <summary> Применить размещение объекта в заданной клетке. </summary>
        /// <param name="gridPosition"> Позиция в координатах грида. </param>
        /// <returns> <c>true</c>, если размещение прошло успешно. </returns>
        public bool TryApply(Vector3Int gridPosition)
        {
            if (!PlaceableObject || !CanPlace(gridPosition, PlaceableObject.Size, PlaceableObject.Layer))
            {
                SfxPlayer.Play(SfxType.PlacementError);
                return false;
            }

            SfxPlayer.Play(SfxType.PlacementSuccess);

            var instance = Object.Instantiate(PlaceableObject.gameObject);
            instance.transform.position = _positionProvider.GridToWorld(gridPosition);

            _gridLayers[PlaceableObject.Layer].AddObjectAt(gridPosition, PlaceableObject.Size, instance);

            _placementPreview.UpdatePosition(_positionProvider.GridToWorld(gridPosition), false);
            return true;
        }

        /// <summary> Обновить состояние превью на новой позиции грида. </summary>
        /// <param name="gridPosition"> Новая позиция клетки в гриде. </param>
        public void Refresh(Vector3Int gridPosition)
        {
            if (!PlaceableObject) return;

            bool isValid = CanPlace(gridPosition, PlaceableObject.Size, PlaceableObject.Layer);
            _placementPreview.UpdatePosition(_positionProvider.GridToWorld(gridPosition), isValid);
        }

        /// <summary> Проверяет, можно ли разместить объект в данной позиции и слое. </summary>
        /// <param name="position"> Позиция в координатах грида. </param>
        /// <param name="size"> Размер объекта в клетках. </param>
        /// <param name="layer"> Слой размещения. </param>
        /// <returns> <c>true</c>, если размещение допустимо. </returns>
        private bool CanPlace(Vector3Int position, Vector2Int size, PlacementLayer layer) =>
            _gridLayers[layer].CanPlaceObjectAt(position, size) &&
            (layer == PlacementLayer.Floor || !HasBlockingColliders(position, size));

        /// <summary> Проверяет наличие коллайдеров, мешающих размещению. </summary>
        /// <param name="position"> Позиция в координатах грида. </param>
        /// <param name="size"> Размер проверяемой области в клетках. </param>
        /// <returns> <c>true</c>, если есть блокирующие коллайдеры. </returns>
        private bool HasBlockingColliders(Vector3Int position, Vector2Int size)
        {
            var worldPosition = _positionProvider.GridToWorld(position);
            var center = worldPosition + new Vector3(size.x / 2f, 1f, size.y / 2f);
            var halfExtents = new Vector3(size.x / 2f, 1.5f, size.y / 2f);

            var colliders = Physics.OverlapBox(center, halfExtents, Quaternion.identity);
            foreach (var collider in colliders)
            {
                if (collider.isTrigger ||
                    collider.GetComponentInParent<PlacementPreview>() ||
                    (collider.TryGetComponent<PlaceableObject>(out var obj) && obj.Layer == PlacementLayer.Floor) ||
                    collider.gameObject.layer == LayerMask.NameToLayer("Terrain"))
                    continue;

                return true;
            }

            return false;
        }
    }
}