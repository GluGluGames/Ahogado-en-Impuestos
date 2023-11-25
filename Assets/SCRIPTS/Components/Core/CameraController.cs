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
        [Tooltip("Amount of zoom in the XYZ axis")]
        [SerializeField] private Vector3 ZoomAmount;
        [Tooltip("Limit of zoom in the Y axis")]
        [SerializeField] private Vector2 ZoomLimitsY;
        [Tooltip("Limit of zoom in the Z axis")]
        [SerializeField] private Vector2 ZoomLimitsZ;
        #if UNITY_ANDROID
        [Tooltip("Limit of zoom in the Y axis when using touch screen")]
        [SerializeField] private Vector2 ZoomLimitsTouch;
        #endif
        [Space(5)]
        [Header("Other")]
        [Tooltip("Amount of movement over time")]
        [SerializeField] private float MovementTime;
        
        #if UNITY_ANDROID
        private LeanDragCamera _dragCamera;
        private LeanTwistRotateAxis _rotateCamera;
        private LeanPinchScale _zoomCamera;
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
            

            #if UNITY_ANDROID
            _dragCamera = GetComponent<LeanDragCamera>();
            _rotateCamera = GetComponent<LeanTwistRotateAxis>();
            _zoomCamera = GetComponent<LeanPinchScale>();
            
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
            StartCoroutine(HandleLeftMouseInput());
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
            
            //ClampCamera();
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
            _newZoom = _cameraTransform.localPosition;
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
                        _newPosition.x = Mathf.Clamp(_newPosition.x, MinBounds.x, MaxBounds.x);
                        _newPosition.z = Mathf.Clamp(_newPosition.z, MinBounds.y, MaxBounds.y);
                        Holding.IsHolding(true);
                    }
                }

                _results.Clear();
            } else Holding.IsHolding(false);
        }
        
        private IEnumerator HandleLeftMouseInput() {
            if (_input.IsSecondaryTouching()) {
                Plane plane = new Plane(Vector3.up, Vector3.zero);
                Ray ray = _mainCamera.ScreenPointToRay(_input.TouchPosition());

                if (plane.Raycast(ray, out float distance)) {
                    _dragStartPosition = ray.GetPoint(distance);
                }
            }

            yield return new WaitForSeconds(0.05f);

            if (_input.IsSecondaryTouching()) {
                _pointerEventData.position = _input.TouchPosition();
                _graphicRaycaster.Raycast(_pointerEventData, _results);

                if (_results.Count == 0) {
                    Plane plane = new Plane(Vector3.up, Vector3.zero);
                    Ray ray = _mainCamera.ScreenPointToRay(_input.TouchPosition());

                    if (plane.Raycast(ray, out float distance))
                    {
                        _dragCurrentPosition = ray.GetPoint(distance);
                        // _newRotation = Quaternion.Euler(_transform.localEulerAngles + new Vector3(_dragStartPosition.x - _dragCurrentPosition.x, 0, 0));
                    }
                }

                _results.Clear();
            }
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
