using GGG.Input;
using GGG.Shared;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Lean.Touch;

namespace GGG.Components.Core
{
    public class CameraController : MonoBehaviour
    {
        [Header("Movement")]
        [Tooltip("Movement speed of the camera")]
        [SerializeField] private float MovementSpeed;
        [SerializeField] private Vector2 MinBounds;
        [SerializeField] private Vector2 MaxBounds;
        [Space(5)]
        [Header("Rotation")]
        [Tooltip("Rotation speed of the camera")]
        [SerializeField] private float RotationSpeed;
        [Space(5)]
        [Header("Zoom")]
        [Tooltip("Limit of zoom in the Y axis")]
        [SerializeField] private Vector2 ZoomLimits;
        #if UNITY_ANDROID
        [Tooltip("Limit of zoom in the Y axis when using touch screen")]
        [SerializeField] private Vector2 ZoomLimitsTouch;
        #endif
        [Space(5)]
        [Header("Other")]
        [Tooltip("Amount of movement over time")]
        [SerializeField] private float MovementTime;
        
        
        private LeanDragCamera _dragCamera;
        private LeanTwistRotateAxis _rotateCamera;
        private LeanPinchScale _zoomCamera;
        #if UNITY_ANDROID
        private bool _cameraToggle;
        #endif

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
            
            _dragCamera = GetComponent<LeanDragCamera>();
            _rotateCamera = GetComponent<LeanTwistRotateAxis>();
            _zoomCamera = GetComponent<LeanPinchScale>();
            
            #if UNITY_ANDROID
            _dragCamera.enabled = true;
            _rotateCamera.enabled = true;
            _zoomCamera.enabled = true;
            
            LeanTouch.OnFingerDown += (x) => Holding.IsHolding(true);
            LeanTouch.OnFingerUp += (x) => Holding.IsHolding(false);
            #endif

            Initialize();
        }
#if !UNITY_ANDROID
        private void Update()
        {
            if(!_cameraTransform) Initialize();
            if (_gameManager.IsOnUI() || _gameManager.TutorialOpen()) return;
            
            StartCoroutine(HandleMouseInput());
        }
#endif

        private void LateUpdate() {
            if (_gameManager.IsOnUI() || _gameManager.TutorialOpen())
            {
                #if UNITY_ANDROID
                ToggleCamera(false);
                #endif
                
                return;
            }
            
#if UNITY_ANDROID
            if(!_cameraToggle) ToggleCamera(true);
            
            ClampCamera();
#else
            HandleCameraMovement();
            HandleCameraRotation();
            HandleZoom();
#endif

        }

        private void Initialize()
        {
            _transform = transform;
            _mainCamera = Camera.main;
            _mainCanvas = GameObject.FindGameObjectWithTag("HUD").GetComponent<Canvas>();
            _graphicRaycaster = _mainCanvas.GetComponent<GraphicRaycaster>();
            _cameraTransform = _mainCamera.transform;
            _newPosition = _transform.position;
            _newRotation = _transform.rotation;
            _newZoom = _transform.localScale;
        }

        /// <summary>
        /// Handles the camera movement
        /// </summary>
        private void HandleCameraMovement() {
            Vector2 inputDirection = _input.CameraMovement();
            Vector3 moveDirection = new Vector3(inputDirection.x, 0f, inputDirection.y).normalized;

            if (moveDirection != Vector3.zero)
            {
                _newPosition += _transform.TransformDirection(moveDirection) * (MovementSpeed * Time.deltaTime);

                _newPosition.x = Mathf.Clamp(_newPosition.x, MinBounds.x, MaxBounds.x);
                _newPosition.z = Mathf.Clamp(_newPosition.z, MinBounds.y, MaxBounds.y);

                _newPosition.y = 0;
            }
            
            _transform.position = Vector3.Lerp(_transform.position, _newPosition, Time.deltaTime * MovementTime);
        }

        /// <summary>
        /// Handles the camera rotation
        /// </summary>
        private void HandleCameraRotation()
        {
            if(_input.CameraRotation() == 1) { // E
                _newRotation *= Quaternion.Euler(Vector3.up * (-RotationSpeed * Time.deltaTime));
            }
            if(_input.CameraRotation() == -1) { // Q
                _newRotation *= Quaternion.Euler(Vector3.up * (RotationSpeed * Time.deltaTime));
            }

            _transform.rotation = Quaternion.Lerp(_transform.rotation, _newRotation, Time.deltaTime * MovementTime);
        }

        /// <summary>
        /// Handles the camera zoom
        /// </summary>
        private void HandleZoom()
        {
            if(_input.CameraZoom() > 0f) {
                _newZoom -= new Vector3(0, Time.deltaTime * MovementTime, Time.deltaTime * MovementTime);
            }
            if(_input.CameraZoom() < 0f) {
                _newZoom += new Vector3(0, Time.deltaTime * MovementTime, Time.deltaTime * MovementTime);;
            }
            
            ClampZoom();
            _transform.localScale = _newZoom;
        }

        /// <summary>
        /// Clamps the zoom between two values
        /// </summary>
        private void ClampZoom()
        {
            _newZoom.x = 1;
            if (_newZoom.y < ZoomLimits.x) _newZoom.y = ZoomLimits.x;
            if (_newZoom.y > ZoomLimits.y) _newZoom.y = ZoomLimits.y;
            
            if (_newZoom.z < ZoomLimits.x) _newZoom.z = ZoomLimits.x;
            if (_newZoom.z > ZoomLimits.y) _newZoom.z = ZoomLimits.y;
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

            yield return null;

            if (_input.IsHolding()) {
                _pointerEventData.position = _input.TouchPosition();
                _graphicRaycaster.Raycast(_pointerEventData, _results);

                if (_results.Count == 0) {
                    Plane plane = new Plane(Vector3.up, Vector3.zero);
                    Ray ray = _mainCamera.ScreenPointToRay(_input.TouchPosition());

                    if (plane.Raycast(ray, out float distance)) {
                        _dragCurrentPosition = ray.GetPoint(distance);
                        _newPosition = _transform.position + (_dragStartPosition - _dragCurrentPosition);
                        _newPosition.x = Mathf.Clamp(_newPosition.x, MinBounds.x, MaxBounds.x);
                        _newPosition.z = Mathf.Clamp(_newPosition.z, MinBounds.y, MaxBounds.y);
                        Holding.IsHolding(true);
                    }
                }

                _results.Clear();
            } else Holding.IsHolding(false);
        }

        #if UNITY_ANDROID
        private void ToggleCamera(bool state)
        {
            _dragCamera.enabled = state;
            _rotateCamera.enabled = state;
            _zoomCamera.enabled = state;

            _cameraToggle = state;
        }
        
        private void ClampCamera()
        {
            Vector3 newPosition = _transform.position;
            
            newPosition.x = Mathf.Clamp(newPosition.x, MinBounds.x, MaxBounds.x);
            newPosition.y = 0;
            newPosition.z = Mathf.Clamp(newPosition.z, MinBounds.x, MaxBounds.y);

            _transform.position = newPosition;

            Vector3 newScale = _transform.localScale;

            newScale.x = 1;
            newScale.y = Mathf.Clamp(newScale.y, ZoomLimitsTouch.x, ZoomLimitsTouch.y);
            newScale.z = Mathf.Clamp(newScale.z, ZoomLimitsTouch.x, ZoomLimitsTouch.y);

            _transform.localScale = newScale;
        }
        #endif
    }
}
