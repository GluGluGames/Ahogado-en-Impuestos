using GGG.Input;

using UnityEngine;

namespace GGG.Components.Core
{
    public class CameraController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float MovementSpeed;
        [Space(5)]
        [Header("Rotation")]
        [SerializeField] private float RotationSpeed;
        [Space(5)]
        [Header("Zoom")]
        [SerializeField] private Vector3 ZoomAmount;
        [Space(5)]
        [Header("Other")]
        [SerializeField] private Transform CameraTransform;
        [SerializeField] private float MovementTime;

        private InputManager _input;
        private Transform _transform;
        private Camera _mainCamera;

        private Vector3 _newPosition;
        private Quaternion _newRotation;
        private Vector3 _newZoom;

        // Mouse
        private Vector3 _dragStartPosition;
        private Vector3 _dragCurrentPosition;

        private void Start() {
            _input = InputManager.Instance;
            _transform = transform;
            _mainCamera = Camera.main;

            _newPosition = _transform.position;
            _newRotation = _transform.rotation;
            _newZoom = CameraTransform.localPosition;
        }

        private void Update() {
            HandleMouseInput();
        }

        private void LateUpdate() {
            HandleCameraMovement();
            HandleCameraRotation();
            HandleZoom();
        }

        private void HandleCameraMovement() {
            Vector2 aux = _input.CameraMovement();

            if(aux.x == 1) { // A
                _newPosition += _transform.right * MovementSpeed;
            }
            if(aux.x == -1) { // D
                _newPosition += _transform.right * -MovementSpeed;
            }
            if(aux.y == 1) { // W
                _newPosition += _transform.up * MovementSpeed;
            }
            if(aux.y == -1) { // S
                _newPosition += _transform.up * -MovementSpeed;
            }
            
            _transform.position = Vector3.Lerp(_transform.position, _newPosition, Time.deltaTime * MovementTime);
        }

        private void HandleCameraRotation() {
            if(_input.CameraRotation() == 1) { // E
                _newRotation *= Quaternion.Euler(Vector3.up * -RotationSpeed);
            }
            if(_input.CameraRotation() == -1) { // Q
                _newRotation *= Quaternion.Euler(Vector3.up * RotationSpeed);
            }

            _transform.rotation = Quaternion.Lerp(_transform.rotation, _newRotation, Time.deltaTime * MovementTime);
        }

        private void HandleZoom() {
            if(_input.CameraZoom() > 0f) {
                _newZoom += ZoomAmount;
            }
            if(_input.CameraZoom() < 0f) {
                _newZoom -= ZoomAmount;
            }
            
            CameraTransform.localPosition = Vector3.Lerp(CameraTransform.localPosition, _newZoom, Time.deltaTime * MovementTime);
        }

        private void HandleMouseInput() {
            if (_input.IsTouching()) {

                Plane plane = new Plane(Vector3.up, Vector3.zero);
                Ray ray = _mainCamera.ScreenPointToRay(_input.TouchPosition());

                if (plane.Raycast(ray, out float distance)) {
                    _dragStartPosition = ray.GetPoint(distance);
                }
            }

            if (_input.IsHolding()) {
                Plane plane = new Plane(Vector3.up, Vector3.zero);
                Ray ray = _mainCamera.ScreenPointToRay(_input.TouchPosition());

                if (plane.Raycast(ray, out float distance)) {
                    _dragCurrentPosition = ray.GetPoint(distance);
                    _newPosition = _transform.position + (_dragStartPosition - _dragCurrentPosition);
                }
            }
        }
    }
}
