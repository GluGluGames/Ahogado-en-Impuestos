using UnityEngine;
using UnityEngine.InputSystem;

namespace GGG.Input {
    public class InputManager : MonoBehaviour {
        public static InputManager Instance;

        private Controls _input;

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

        #endregion

        #region Getters & Setters

        public Vector2 CameraMovement() { return _input.a_Camera.CameraMovement.ReadValue<Vector2>(); }
        public float CameraRotation() { return _input.a_Camera.CameraRotation.ReadValue<float>(); }
        public float CameraZoom() { return _input.a_Camera.CameraZoom.ReadValue<float>(); }
        public bool IsTouching() { return _input.a_Camera.PrimaryTouchContact.WasPressedThisFrame(); }
        public Vector2 TouchPosition() { return _input.a_Camera.PrimaryTouch.ReadValue<Vector2>(); }
        public bool IsHolding() { return _input.a_Camera.PrimaryTouchContact.IsPressed(); }
        
        public bool MouseClick() { return _input.a_Dialogue.Continue.WasPerformedThisFrame(); }

        #endregion
    }
}
