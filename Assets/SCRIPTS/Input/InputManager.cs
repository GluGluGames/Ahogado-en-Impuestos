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

        // CAMERA MOVEMENT
        public Vector2 CameraMovement() => _input.a_Camera.CameraMovement.ReadValue<Vector2>();
        public float CameraRotation() => _input.a_Camera.CameraRotation.ReadValue<float>();
        public float CameraZoom() => _input.a_Camera.CameraZoom.ReadValue<float>();
        public bool IsTouching() => _input.a_Camera.PrimaryTouchContact.WasPerformedThisFrame();
        public Vector2 TouchPosition() => _input.a_Camera.PrimaryTouch.ReadValue<Vector2>();
        public bool IsHolding() => _input.a_Camera.PrimaryTouchContact.inProgress; 
        
        // DIALOGUE BUTTONS
        public bool MouseClick() => _input.a_Dialogue.Continue.WasPerformedThisFrame();
        
        // SHORTCUTS BUTTONS
        public bool Escape() => _input.a_Shortcuts.EscapeKey.WasPerformedThisFrame();
        public bool DebuConsole() => _input.a_Shortcuts.DebugConsole.WasPerformedThisFrame();
        public bool EnterKey() => _input.a_Shortcuts.EnterKey.WasPerformedThisFrame();

        #endregion
    }
}
