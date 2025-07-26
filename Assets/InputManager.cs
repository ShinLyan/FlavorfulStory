using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FlavorfulStory
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private Camera _sceneCamera;
        [SerializeField] private LayerMask _placementLayerMask;

        private Vector3 _lastMousePosition;

        public event Action OnClicked;
        public event Action OnExit;


        private void Update()
        {
            if (Input.GetMouseButtonDown(0)) OnClicked?.Invoke();

            if (Input.GetKeyDown(KeyCode.M)) OnExit?.Invoke();
        }

        public static bool IsPointerOverUI() => EventSystem.current.IsPointerOverGameObject();

        public Vector3 GetSelectedMapPosition()
        {
            var mousePosition = Input.mousePosition;
            mousePosition.z = _sceneCamera.nearClipPlane;
            var ray = _sceneCamera.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out var hit, 100, _placementLayerMask)) _lastMousePosition = hit.point;
            return _lastMousePosition;
        }
    }
}