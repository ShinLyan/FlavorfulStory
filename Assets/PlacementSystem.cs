using UnityEngine;

namespace FlavorfulStory
{
    public class PlacementSystem : MonoBehaviour
    {
        [SerializeField] private InputManager _inputManager;
        [SerializeField] private Grid _grid;
        [SerializeField] private ObjectsDatabase _objectsDatabase;
        [SerializeField] private GameObject _gridVisualization;

        private GridData _floorData;
        private GridData _furnitureData;

        [SerializeField] private PreviewSystem _previewSystem;

        private Vector3Int _lastDetectedPosition = Vector3Int.zero;

        [SerializeField] private ObjectPlacer _objectPlacer;

        private IBuildingState _buildingState;

        private void Start()
        {
            StopPlacement();
            _floorData = new GridData();
            _furnitureData = new GridData();
        }

        public void StartPlacement(int id)
        {
            StopPlacement();
            _gridVisualization.SetActive(true);
            _buildingState = new PlacementState(id, _grid, _previewSystem,
                _objectsDatabase, _floorData, _furnitureData, _objectPlacer);

            _inputManager.OnClicked += PlaceStructure;
            _inputManager.OnExit += StopPlacement;
        }

        public void StartRemoving()
        {
            StopPlacement();
            _gridVisualization.SetActive(true);
            _buildingState = new RemovingState(_grid, _previewSystem, _floorData, _furnitureData, _objectPlacer);

            _inputManager.OnClicked += PlaceStructure;
            _inputManager.OnExit += StopPlacement;
        }

        private void PlaceStructure()
        {
            if (_inputManager.IsPointerOverUI()) return;

            var mousePosition = _inputManager.GetSelectedMapPosition();
            var gridPosition = _grid.WorldToCell(mousePosition);

            _buildingState.OnAction(gridPosition);
        }

        private void StopPlacement()
        {
            if (_buildingState == null) return;

            _lastDetectedPosition = Vector3Int.zero;
            _gridVisualization.SetActive(false);

            _buildingState.EndState();
            _buildingState = null;

            _inputManager.OnClicked -= PlaceStructure;
            _inputManager.OnExit -= StopPlacement;
        }

        private void Update()
        {
            if (_buildingState == null) return;

            var mousePosition = _inputManager.GetSelectedMapPosition();
            var gridPosition = _grid.WorldToCell(mousePosition);

            if (_lastDetectedPosition == gridPosition) return;

            _buildingState.UpdateState(gridPosition);
            _lastDetectedPosition = gridPosition;
        }
    }
}