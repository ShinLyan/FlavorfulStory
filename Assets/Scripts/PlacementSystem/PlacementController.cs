using System;
using System.Collections.Generic;
using FlavorfulStory.GridSystem;
using UnityEngine;

namespace FlavorfulStory.PlacementSystem
{
    public class PlacementController : MonoBehaviour
    {
        [SerializeField] private GameObject _gridView;
        [SerializeField] private PlacementPreview _placementPreview;

        private readonly Dictionary<PlacementLayer, PlacementGridData> _gridLayers = new();
        private readonly Dictionary<PlacementModeType, IPlacementMode> _modes = new();

        private IPlacementMode _currentMode;
        private Vector3Int _lastGridPosition;

        // TODO: Заменить на Zenject
        [SerializeField] private Camera _sceneCamera;
        [SerializeField] private Grid _grid;
        private IGridPositionProvider _positionProvider;

        private void Awake()
        {
            // TODO: Заменить на Zenject
            _positionProvider = new GridPositionProvider(_sceneCamera, _grid);

            InitializeGridLayers();
            InitializeModes();
        }

        private void InitializeGridLayers()
        {
            foreach (PlacementLayer layer in Enum.GetValues(typeof(PlacementLayer)))
                _gridLayers[layer] = new PlacementGridData();
        }

        private void InitializeModes()
        {
            _modes[PlacementModeType.Place] = new PlacementMode(_positionProvider, _placementPreview, _gridLayers);
            _modes[PlacementModeType.Remove] = new RemovingMode(_positionProvider, _placementPreview, _gridLayers);
        }

        public void EnterPlacementMode(PlacementModeType mode, PlaceableObject placeableObject)
        {
            if (!_modes.TryGetValue(mode, out var modeInstance)) return;

            if (placeableObject && modeInstance is PlacementMode placementMode)
                placementMode.PlaceableObject = placeableObject;

            ActivateMode(modeInstance);
        }

        private void ActivateMode(IPlacementMode mode)
        {
            ExitCurrentMode();

            _currentMode = mode;
            _currentMode.Enter();
            _gridView.SetActive(true);
        }

        public void ExitCurrentMode()
        {
            if (_currentMode == null) return;

            _currentMode.Exit();
            _currentMode = null;
            _gridView.SetActive(false);
            _lastGridPosition = Vector3Int.zero;
        }

        private void Update()
        {
            if (_currentMode == null || !_positionProvider.TryGetCursorGridPosition(out var gridPosition)) return;

            RefreshCursor(gridPosition);

            if (Input.GetMouseButtonDown(0)) _currentMode.Apply(gridPosition);
        }

        private void RefreshCursor(Vector3Int gridPosition)
        {
            if (_lastGridPosition == gridPosition) return;

            _currentMode.Refresh(gridPosition);
            _lastGridPosition = gridPosition;
        }

        // DEBUG
        private void LateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.M) && _currentMode is not RemovingMode)
                EnterPlacementMode(PlacementModeType.Remove, null);
        }
    }
}