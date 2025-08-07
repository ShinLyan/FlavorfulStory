using FlavorfulStory.GridSystem;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.PlacementSystem
{
    /// <summary> Отвечает за отображение предпросмотра размещаемого объекта в мире. </summary>
    public class PlacementPreview : MonoBehaviour
    {
        /// <summary> Материал, используемый для предпросмотра объекта. </summary>
        [SerializeField] private Material _previewMaterialPrefab;

        /// <summary> Текущий отображаемый объект предпросмотра. </summary>
        private PlaceableObject _previewObject;

        /// <summary> Экземпляр материала предпросмотра, назначаемый на рендереры. </summary>
        private Material _previewMaterialInstance;

        /// <summary> Сервис отображения индикатора грида. </summary>
        private GridSelectionService _gridSelectionService;

        /// <summary> Смещение по Y для предпросмотра объекта (визуальный отступ от земли). </summary>
        private const float PreviewYOffset = 0.05f;

        /// <summary> Внедрение зависимостей Zenject. </summary>
        /// <param name="gridSelectionService"> Сервис отображения индикатора грида. </param>
        [Inject]
        private void Construct(GridSelectionService gridSelectionService) =>
            _gridSelectionService = gridSelectionService;

        /// <summary> Инициализация компонента. </summary>
        private void Awake() => _previewMaterialInstance = new Material(_previewMaterialPrefab);

        /// <summary> Начать отображение предпросмотра размещения объекта. </summary>
        /// <param name="placeable"> Объект, который планируется разместить. </param>
        public void StartShowingPlacementPreview(PlaceableObject placeable)
        {
            if (_previewObject) ClearPreviewObject();

            _previewObject = Instantiate(placeable, transform);
            _previewObject.SetCollidersEnabled(false);

            ApplyPreviewMaterial(_previewObject.gameObject);
        }

        /// <summary> Начать отображение предпросмотра удаления (без объекта, только индикатор). </summary>
        public void StartShowingRemovePreview()
        {
            if (_previewObject) ClearPreviewObject();

            SetPreviewColor(false);
        }

        /// <summary> Остановить отображение предпросмотра и скрыть индикатор. </summary>
        public void StopShowingPreview()
        {
            _gridSelectionService.HideGridIndicator();
            if (_previewObject) ClearPreviewObject();
        }

        /// <summary> Обновить позицию предпросмотра и цвет в зависимости от валидности размещения. </summary>
        /// <param name="position"> Новая позиция предпросмотра. </param>
        /// <param name="isValid"> Является ли текущая позиция допустимой. </param>
        public void UpdatePosition(Vector3 position, bool isValid)
        {
            if (_previewObject)
            {
                _previewObject.transform.position = position + Vector3.up * PreviewYOffset;
                SetPreviewColor(isValid);
            }

            var size = _previewObject ? _previewObject.Size : Vector2Int.one;
            _gridSelectionService.ShowGridIndicator(position, size, isValid);
        }

        /// <summary> Применить материал предпросмотра ко всем рендерерам объекта. </summary>
        /// <param name="previewObject"> Объект предпросмотра. </param>
        private void ApplyPreviewMaterial(GameObject previewObject)
        {
            foreach (var renderer in previewObject.GetComponentsInChildren<Renderer>())
            {
                var materials = renderer.materials;
                for (int i = 0; i < materials.Length; i++) materials[i] = _previewMaterialInstance;
                renderer.materials = materials;
            }
        }

        /// <summary> Установить цвет предпросмотра в зависимости от валидности размещения. </summary>
        /// <param name="isValid"> true – зелёный, false – красный. </param>
        private void SetPreviewColor(bool isValid) =>
            _previewMaterialInstance.color =
                isValid ? GridSelectionService.ValidColor : GridSelectionService.InvalidColor;

        /// <summary> Удалить объект предпросмотра и вернуть коллайдеры. </summary>
        private void ClearPreviewObject()
        {
            _previewObject.SetCollidersEnabled(true);
            Destroy(_previewObject.gameObject);
            _previewObject = null;
        }
    }
}