using System;
using System.Collections.Generic;
using FlavorfulStory.PlacementSystem;
using UnityEngine;

namespace FlavorfulStory
{
    public class PlacementController : MonoBehaviour
    {
        [SerializeField] private InputManager _inputManager;
        [SerializeField] private Grid _grid;
        [SerializeField] private PlaceableObjectsCatalog _catalog;
        [SerializeField] private GameObject _gridView;
        [SerializeField] private ObjectPlacer _objectPlacer;
        [SerializeField] private PreviewSystem _preview;

        private readonly Dictionary<PlacementLayer, GridData> _gridLayers = new();
        private Vector3Int _lastGridPosition = Vector3Int.zero;
        private IBuildingState _currentState;

        private void Start() => InitializeGridLayers();

        private void InitializeGridLayers()
        {
            foreach (PlacementLayer layer in Enum.GetValues(typeof(PlacementLayer)))
                _gridLayers[layer] = new GridData();
        }

        public void StartPlacement(int objectId) =>
            EnterPlacementMode(new PlacementState(objectId, _grid, _preview, _catalog, _gridLayers, _objectPlacer));

        public void StartRemoving() =>
            EnterPlacementMode(new RemovingState(_grid, _preview, _gridLayers, _objectPlacer));

        private void EnterPlacementMode(IBuildingState state)
        {
            ExitPlacementMode();

            _currentState = state;
            _currentState.BeginState();
            _gridView.SetActive(true);

            SubscribeInput();
        }

        private void PlaceStructure()
        {
            if (_inputManager.IsPointerOverUI()) return;

            var mousePosition = _inputManager.GetSelectedMapPosition();
            var gridPosition = _grid.WorldToCell(mousePosition);

            _currentState.OnAction(gridPosition);
        }

        private void ExitPlacementMode()
        {
            if (_currentState == null) return;

            _lastGridPosition = Vector3Int.zero;
            _gridView.SetActive(false);

            _currentState.EndState();
            _currentState = null;

            UnsubscribeInput();
        }

        private void SubscribeInput()
        {
            _inputManager.OnClicked += PlaceStructure;
            _inputManager.OnExit += ExitPlacementMode;
        }

        private void UnsubscribeInput()
        {
            _inputManager.OnClicked -= PlaceStructure;
            _inputManager.OnExit -= ExitPlacementMode;
        }

        private void Update()
        {
            if (_currentState == null) return;

            var mousePosition = _inputManager.GetSelectedMapPosition();
            var gridPosition = _grid.WorldToCell(mousePosition);

            if (_lastGridPosition == gridPosition) return;

            _currentState.UpdateState(gridPosition);
            _lastGridPosition = gridPosition;
        }
    }
}