using UnityEngine;

public class CameraTracker : MonoBehaviour
{
    private Camera _mainCamera;

    private void Start() => _mainCamera = Camera.main;

    private void LateUpdate()
    {
        if (_mainCamera) transform.LookAt(transform.position + _mainCamera.transform.forward);
    }
}