using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GGG.Input {
    public class InputManager : MonoBehaviour {
        public static InputManager Instance;

        private Controls _input;
        
        private Vector3 _cameraMovement;
        private float _cameraRotation;
        private float _cameraZoom;
        
        private Vector2 _touchPosition;

        private bool _mouseClick;

        private bool _escape;
        private bool _debugKey;
        private bool _enter;

        #region Unity Events

        private void Awake() {
            if(Instance == null) {
                Instance = this;
            }

            _input = new Controls();
        }

        private void OnEnable() {
            _input.Enable();
        }

        private void OnDisable() {
            _input.Disable();
        }

        private void Update() {
            _cameraMovement = _input.a_Camera.CameraMovement.ReadValue<Vector2>();
            _cameraRotation = _input.a_Camera.CameraRotation.ReadValue<float>();
            _cameraZoom = _input.a_Camera.CameraZoom.ReadValue<float>();

            _touchPosition = _input.a_Camera.PrimaryTouch.ReadValue<Vector2>();

            _mouseClick = _input.a_Dialogue.Continue.WasPerformedThisFrame();

            _escape = _input.a_Shortcuts.EscapeKey.WasPerformedThisFrame();
            _debugKey = _input.a_Shortcuts.DebugConsole.WasPerformedThisFrame();
            _enter = _input.a_Shortcuts.EnterKey.WasPerformedThisFrame();
        }

        #endregion

        #region Getters & Setters

        // CAMERA MOVEMENT
        public Vector2 CameraMovement() => _cameraMovement;
        public float CameraRotation() => _cameraRotation;
        public float CameraZoom() => _cameraZoom;
        public bool IsTouching() => _input.a_Camera.PrimaryTouchContact.WasPerformedThisFrame();
        public bool IsHolding() => _input.a_Camera.PrimaryTouchContact.inProgress;
        public Vector2 TouchPosition() => _touchPosition;
        public bool IsSecondaryTouching() => _input.a_Camera.SecondaryTouch.WasPerformedThisFrame();
        
        // DIALOGUE BUTTONS
        public bool MouseClick() => _mouseClick;
        
        // SHORTCUTS BUTTONS
        public bool Escape() => _escape;
        public bool DebuConsole() => _debugKey;
        public bool EnterKey() => _enter;

        #endregion
    }
}
