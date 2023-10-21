using GGG.Input;
using GGG.Shared;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace GGG.Components.Core
{
    public class CameraController : MonoBehaviour
    {
        [Header("Movement")]
        [Tooltip("Movement speed of the camera")]
        [SerializeField] private float MovementSpeed;
        [Space(5)]
        [Header("Rotation")]
        [Tooltip("Rotation speed of the camera")]
        [SerializeField] private float RotationSpeed;
        [Space(5)]
        [Header("Zoom")]
        [Tooltip("Amount of zoom in the XYZ axis")]
        [SerializeField] private Vector3 ZoomAmount;
        [Tooltip("Limit of zoom in the Y axis")]
        [SerializeField] private Vector2 ZoomLimitsY;
        [Tooltip("Limit of zoom in the Z axis")]
        [SerializeField] private Vector2 ZoomLimitsZ;
        [Space(5)]
        [Header("Other")]
        [Tooltip("Amount of movement over time")]
        [SerializeField] private float MovementTime;

        private InputManager _input;
        private GameManager _gameManager;
        private Transform _transform;
        private Camera _mainCamera;
        private Transform _cameraTransform;
        private Canvas _mainCanvas;

        private GraphicRaycaster _graphicRaycaster;
        private PointerEventData _pointerEventData = new PointerEventData(EventSystem.current);
        private List<RaycastResult> _results = new();

        private Vector3 _newPosition;
        private Quaternion _newRotation;
        private Vector3 _newZoom;

        // Mouse
        private Vector3 _dragStartPosition;
        private Vector3 _dragCurrentPosition;

        private void Start() {
            _input = InputManager.Instance;
            _gameManager = GameManager.Instance;
            _transform = transform;
            _mainCamera = Camera.main;
            _mainCanvas = GameObject.FindGameObjectWithTag("HUD").GetComponent<Canvas>();
            _graphicRaycaster = _mainCanvas.GetComponent<GraphicRaycaster>();

            _cameraTransform = _mainCamera.transform;
            _newPosition = _transform.position;
            _newRotation = _transform.rotation;
            _newZoom = _cameraTransform.localPosition;
        }

        private void Update()
        {
            if (!_gameManager.PlayingGame()) return;
            
            StartCoroutine(HandleMouseInput());
        }

        private void LateUpdate() {
            if (!_gameManager.PlayingGame()) return;
            
            HandleCameraMovement();
            HandleCameraRotation();
            HandleZoom();
        }

        /// <summary>
        /// Handles the camera movement
        /// </summary>
        private void HandleCameraMovement() {
            Vector2 inputDirection = _input.CameraMovement();
            Vector3 moveDirection = new Vector3(inputDirection.x, 0f, inputDirection.y).normalized;

            if (moveDirection != Vector3.zero)
                _newPosition += _transform.TransformDirection(moveDirection) * MovementSpeed;
            
            _transform.position = Vector3.Lerp(_transform.position, _newPosition, Time.deltaTime * MovementTime);
        }

        /// <summary>
        /// Handles the camera rotation
        /// </summary>
        private void HandleCameraRotation() {
            if(_input.CameraRotation() == 1) { // E
                _newRotation *= Quaternion.Euler(Vector3.up * -RotationSpeed);
            }
            if(_input.CameraRotation() == -1) { // Q
                _newRotation *= Quaternion.Euler(Vector3.up * RotationSpeed);
            }

            _transform.rotation = Quaternion.Lerp(_transform.rotation, _newRotation, Time.deltaTime * MovementTime);
        }

        /// <summary>
        /// Handles the camera zoom
        /// </summary>
        private void HandleZoom() {
            if(_input.CameraZoom() > 0f) {
                _newZoom += ZoomAmount;
            }
            if(_input.CameraZoom() < 0f) {
                _newZoom -= ZoomAmount;
            }

            ClampZoom();

            _cameraTransform.localPosition = Vector3.Lerp(_cameraTransform.localPosition, _newZoom, Time.deltaTime * MovementTime);
        }

        /// <summary>
        /// Clamps the zoom between two values
        /// </summary>
        private void ClampZoom()
        {
            if(_newZoom.y < ZoomLimitsY.x) _newZoom.y = ZoomLimitsY.x;
            if(_newZoom.y > ZoomLimitsY.y) _newZoom.y = ZoomLimitsY.y;
            if(_newZoom.z < ZoomLimitsZ.x) _newZoom.z = ZoomLimitsZ.x;
            if(_newZoom.z > ZoomLimitsZ.y) _newZoom.z = ZoomLimitsZ.y;
        }

        /// <summary>
        /// Handles the movement with the mouse
        /// </summary>
        private IEnumerator HandleMouseInput() {
            if (_input.IsTouching()) {
                Plane plane = new Plane(Vector3.up, Vector3.zero);
                Ray ray = _mainCamera.ScreenPointToRay(_input.TouchPosition());

                if (plane.Raycast(ray, out float distance)) {
                    _dragStartPosition = ray.GetPoint(distance);
                }
            }

            yield return new WaitForSeconds(0.05f);

            if (_input.IsHolding()) {
                _pointerEventData.position = _input.TouchPosition();
                _graphicRaycaster.Raycast(_pointerEventData, _results);

                if (_results.Count == 0) {
                    Plane plane = new Plane(Vector3.up, Vector3.zero);
                    Ray ray = _mainCamera.ScreenPointToRay(_input.TouchPosition());

                    if (plane.Raycast(ray, out float distance)) {
                        _dragCurrentPosition = ray.GetPoint(distance);
                        _newPosition = _transform.position + (_dragStartPosition - _dragCurrentPosition);
                        Holding.IsHolding(true);
                    }
                }

                _results.Clear();
            } else Holding.IsHolding(false);
        }
    }
}
