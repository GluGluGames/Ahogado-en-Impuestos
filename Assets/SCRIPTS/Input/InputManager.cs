using UnityEngine;
using UnityEngine.InputSystem;

namespace GGG.Input {
    public class InputManager : MonoBehaviour {
        public static InputManager Instance;

        private Controls _input;
        private Vector2 _cameraMovement;

        #region Unity Events

        private void Awake() {
            if(Instance == null) {
                Instance = this;
            }

            _input = new Controls();
        }

        private void Update() {
            _cameraMovement = _input.a_Camera.CameraMovement.ReadValue<Vector2>();
        }

        private void OnEnable() {
            _input.Enable();
        }

        private void OnDisable() {
            _input.Disable();
        }

        #endregion

        #region Getters & Setters

        public Vector2 CameraMovement() { return _cameraMovement; }
        public float CameraRotation() { return _input.a_Camera.CameraRotation.ReadValue<float>(); }
        public float CameraZoom() { return _input.a_Camera.CameraZoom.ReadValue<float>(); }

        public bool IsTouching() { return Mouse.current.rightButton.isPressed; }
        public bool IsHolding() { return Mouse.current.rightButton.wasPressedThisFrame; }
        public Vector2 TouchPosition() { return Mouse.current.position.ReadValue(); }

        #endregion
    }
}
