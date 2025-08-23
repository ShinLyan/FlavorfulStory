using System.Collections.Generic;
using FlavorfulStory.Audio;
using FlavorfulStory.GridSystem;
using FlavorfulStory.Infrastructure.Factories;
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

        /// <summary> Контейнер, в который будет помещён созданный объект. </summary>
        private readonly Transform _container;

        /// <summary> Фабрика для создания экземпляров размещаемых объектов. </summary>
        private readonly IPrefabFactory<PlaceableObject> _placeableFactory;

        /// <summary> Буфер коллайдеров для поиска объектов, которые можно ударить. </summary>
        private readonly Collider[] _hitsBuffer = new Collider[10];

        /// <summary> Провайдер размещаемых объектов. </summary>
        private readonly PlaceableObjectProvider _placeableObjectProvider;

        /// <summary> Объект, который будет размещен. </summary>
        public PlaceableObject PlaceableObject { get; set; }

        /// <summary> Конструктор с параметрами. </summary>
        /// <param name="positionProvider"> Провайдер координат грида. </param>
        /// <param name="placementPreview"> Превью размещаемого объекта. </param>
        /// <param name="gridLayers"> Словарь данных по слоям размещения. </param>
        /// <param name="container"> Родитель для создаваемых объектов. </param>
        /// <param name="placeableFactory"> Фабрика размещаемых объектов. </param>
        public PlacementMode(GridPositionProvider positionProvider, PlacementPreview placementPreview,
            Dictionary<PlacementLayer, PlacementGridData> gridLayers, Transform container,
            IPrefabFactory<PlaceableObject> placeableFactory, PlaceableObjectProvider placeableObjectProvider)
        {
            _positionProvider = positionProvider;
            _placementPreview = placementPreview;
            _gridLayers = gridLayers;
            _container = container;
            _placeableFactory = placeableFactory;
            _placeableObjectProvider = placeableObjectProvider;
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

            var instance = _placeableFactory.Create(PlaceableObject, parentTransform: _container);
            instance.transform.position = _positionProvider.GridToWorld(gridPosition);
            _placeableObjectProvider.Register(instance);

            _gridLayers[PlaceableObject.Layer].AddObjectAt(gridPosition, instance);
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
        /// <param name="placingLayer"> Слой размещения. </param>
        /// <returns> <c>true</c>, если размещение допустимо. </returns>
        private bool CanPlace(Vector3Int position, Vector2Int size, PlacementLayer placingLayer) =>
            _gridLayers[placingLayer].CanPlaceObjectAt(position, size) &&
            !HasBlockingColliders(position, size, placingLayer);

        /// <summary> Проверяет наличие коллайдеров, мешающих размещению. </summary>
        /// <param name="position"> Позиция в координатах грида. </param>
        /// <param name="size"> Размер проверяемой области в клетках. </param>
        /// <param name="placingLayer"> Слой размещения. </param>
        /// <returns> <c>true</c>, если есть блокирующие коллайдеры. </returns>
        /// <remarks> Floor можно класть под все PlaceableObject, кроме другого Floor.
        /// Не Floor можно класть на Floor, но нельзя пересекать другие PlaceableObject
        /// и любые посторонние коллайдеры.</remarks>
        private bool HasBlockingColliders(Vector3Int position, Vector2Int size, PlacementLayer placingLayer)
        {
            var worldPosition = _positionProvider.GridToWorld(position);
            var center = worldPosition + new Vector3(size.x / 2f, 1f, size.y / 2f);
            var halfExtents = new Vector3(size.x / 2f, 1.5f, size.y / 2f);

            int hitCount = Physics.OverlapBoxNonAlloc(center, halfExtents, _hitsBuffer, Quaternion.identity);
            for (int i = 0; i < hitCount; i++)
            {
                var collider = _hitsBuffer[i];
                if (!collider || collider.isTrigger ||
                    collider.GetComponentInParent<PlacementPreview>() ||
                    collider.gameObject.layer == LayerMask.NameToLayer("Terrain"))
                    continue;

                var placeable = collider.GetComponentInParent<PlaceableObject>();
                if (!placeable) return true;

                if (placingLayer == PlacementLayer.Floor)
                {
                    if (placeable.Layer == PlacementLayer.Floor) return true;
                    continue;
                }

                if (placeable.Layer == PlacementLayer.Floor) continue;
                return true;
            }

            return false;
        }
    }
}