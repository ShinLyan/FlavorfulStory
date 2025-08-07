using UnityEngine;

namespace FlavorfulStory.PlacementSystem
{
    public class PlacementPreview : MonoBehaviour
    {
        [SerializeField] private GameObject _gridIndicator;
        [SerializeField] private Material _previewMaterialPrefab;

        private PlaceableObject _previewObject;
        private Material _previewMaterialInstance;
        private Renderer _gridIndicatorRenderer;

        private const float PreviewYOffset = 0.05f;
        private static readonly Color ValidColor = new(1f, 1f, 1f, 0.5f);
        private static readonly Color InvalidColor = new(1f, 0f, 0f, 0.5f);

        private void Awake()
        {
            _previewMaterialInstance = new Material(_previewMaterialPrefab);
            _gridIndicatorRenderer = _gridIndicator.GetComponentInChildren<Renderer>();
            _gridIndicator.SetActive(false);
        }

        public void StartShowingPlacementPreview(PlaceableObject placeable)
        {
            if (_previewObject) ClearPreviewObject();

            _previewObject = Instantiate(placeable, transform);
            _previewObject.SetCollidersEnabled(false);

            ApplyPreviewMaterial(_previewObject.gameObject);
            ConfigureCellIndicator(_previewObject.Size);
        }

        public void StartShowingRemovePreview()
        {
            if (_previewObject) ClearPreviewObject();

            SetPreviewColor(false);
            ConfigureCellIndicator(Vector2Int.one);
        }

        public void StopShowingPreview()
        {
            _gridIndicator.SetActive(false);
            ClearPreviewObject();
        }

        public void UpdatePosition(Vector3 position, bool isValid)
        {
            if (_previewObject)
            {
                _previewObject.transform.position = position + Vector3.up * PreviewYOffset;
                SetPreviewColor(isValid);
            }

            _gridIndicator.transform.position = position;
            SetCursorColor(isValid);
        }

        private void ApplyPreviewMaterial(GameObject previewObject)
        {
            foreach (var renderer in previewObject.GetComponentsInChildren<Renderer>())
            {
                var materials = renderer.materials;
                for (int i = 0; i < materials.Length; i++) materials[i] = _previewMaterialInstance;
                renderer.materials = materials;
            }
        }

        private void ConfigureCellIndicator(Vector2Int size)
        {
            if (size is { x: <= 0, y: <= 0 }) return;

            _gridIndicator.transform.localScale = new Vector3(size.x, 1f, size.y);
            _gridIndicatorRenderer.material.mainTextureScale = size;

            _gridIndicator.SetActive(true);
        }

        private void SetPreviewColor(bool isValid) =>
            _previewMaterialInstance.color = isValid ? ValidColor : InvalidColor;

        private void SetCursorColor(bool isValid) =>
            _gridIndicatorRenderer.material.color = isValid ? ValidColor : InvalidColor;

        private void ClearPreviewObject()
        {
            _previewObject.SetCollidersEnabled(true);
            Destroy(_previewObject.gameObject);
            _previewObject = null;
        }
    }
}