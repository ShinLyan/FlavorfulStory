using System;
using System.Collections.Generic;
using FlavorfulStory.GridSystem;
using FlavorfulStory.TimeManagement;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.PlacementSystem
{
    public class PlacementController : MonoBehaviour
    {
        /// <summary> Провайдер позиции на гриде. </summary>
        private GridPositionProvider _gridPositionProvider;

        private PlacementPreview _placementPreview;

        private IPlacementMode _currentMode;
        private Vector3Int _lastGridPosition;

        private readonly Dictionary<PlacementLayer, PlacementGridData> _gridLayers = new();
        private readonly Dictionary<PlacementModeType, IPlacementMode> _modes = new();

        [Inject]
        private void Construct(GridPositionProvider gridPositionProvider, PlacementPreview placementPreview)
        {
            _gridPositionProvider = gridPositionProvider;
            _placementPreview = placementPreview;
        }

        private void Awake()
        {
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
            _modes[PlacementModeType.Place] = new PlacementMode(_gridPositionProvider, _placementPreview, _gridLayers);
            _modes[PlacementModeType.Remove] = new RemovingMode(_gridPositionProvider, _placementPreview, _gridLayers);
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
        }

        public void ExitCurrentMode()
        {
            if (_currentMode == null) return;

            _currentMode.Exit();
            _currentMode = null;
            _lastGridPosition = Vector3Int.zero;
        }

        private void Update()
        {
            if (WorldTime.IsPaused || _currentMode == null ||
                !_gridPositionProvider.TryGetCursorGridPosition(out var gridPosition))
                return;

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