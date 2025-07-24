using UnityEngine;

namespace FlavorfulStory
{
    public class PreviewSystem : MonoBehaviour
    {
        [SerializeField] private float _previewYOffset = 0.06f;

        [SerializeField] private GameObject _cellIndicator;

        private GameObject _previewObject;

        [SerializeField] private Material _previewMaterialPrefab;

        private Material _previewMaterialInstance;

        private Renderer _cellIndicatorRenderer;

        private void Start()
        {
            _previewMaterialInstance = new Material(_previewMaterialPrefab);
            _cellIndicator.SetActive(false);
            _cellIndicatorRenderer = _cellIndicator.GetComponentInChildren<Renderer>();
        }

        public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size)
        {
            _previewObject = Instantiate(prefab);
            PreparePreview(_previewObject);
            PrepareCursor(size);
            _cellIndicator.SetActive(true);
        }

        private void PreparePreview(GameObject previewObject)
        {
            var renderers = previewObject.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                var materials = renderer.materials;
                for (int i = 0; i < materials.Length; i++) materials[i] = _previewMaterialInstance;
                renderer.materials = materials;
            }
        }

        private void PrepareCursor(Vector2Int size)
        {
            if (size is { x: <= 0, y: <= 0 }) return;

            _cellIndicator.transform.localScale = new Vector3(size.x, 1f, size.y);
            _cellIndicatorRenderer.material.mainTextureScale = size;
        }

        public void StopShowingPreview()
        {
            _cellIndicator.SetActive(false);
            if (_previewObject) Destroy(_previewObject);
        }

        public void UpdatePosition(Vector3 position, bool validity)
        {
            if (_previewObject)
            {
                MovePreview(position);
                ApplyFeedbackToPreview(validity);
            }

            MoveCursor(position);
            ApplyFeedbackToCursor(validity);
        }

        private void MovePreview(Vector3 position) => _previewObject.transform.position =
            new Vector3(position.x, position.y + _previewYOffset, position.z);

        private void MoveCursor(Vector3 position) => _cellIndicator.transform.position = position;

        private void ApplyFeedbackToPreview(bool validity)
        {
            var color = validity ? Color.white : Color.red;
            color.a = 0.5f;
            _previewMaterialInstance.color = color;
        }

        private void ApplyFeedbackToCursor(bool validity)
        {
            var color = validity ? Color.white : Color.red;
            color.a = 0.5f;
            _cellIndicatorRenderer.material.color = color;
        }

        public void StartShowingRemovePreview()
        {
            _cellIndicator.SetActive(true);
            PrepareCursor(Vector2Int.one);
            ApplyFeedbackToPreview(false);
        }
    }
}