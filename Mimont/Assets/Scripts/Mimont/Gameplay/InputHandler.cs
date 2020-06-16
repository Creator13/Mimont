using UnityEngine;
using UnityEngine.InputSystem;

namespace Mimont.Gameplay {
public class InputHandler : MonoBehaviour {
    [SerializeField] private new Camera camera;
    private DefaultInput input;

    public event System.Action<Vector3> WorldClickPerformed;
    public event System.Action HoldStarted;
    public event System.Action HoldReleased;

    private bool ClickHeld {
        set {
            if (value) {
                HoldStarted?.Invoke();
            }
            else {
                HoldReleased?.Invoke();
            }
        }
    }

    private void Awake() {
        // Load camera if unset
        if (!camera) {
            Debug.LogWarning("No camera set for input, using Camera.main. Please set a camera!");
            camera = Camera.main;
        }

        input = new DefaultInput();

        input.Default.Click.performed += BroadcastClick;
        input.Default.Click.performed += ctx => ClickHeld = true;
        input.Default.Click.canceled += ctx => ClickHeld = false;
        input.Default.Click.Enable();
    }

    private void OnEnable() {
        input.Enable();
    }

    private void OnDisable() {
        input.Disable();
    }

    private void BroadcastClick(InputAction.CallbackContext ctx) {
        var pos = input.Default.Position.ReadValue<Vector2>();
        WorldClickPerformed?.Invoke(camera.ScreenToWorldPoint(pos));
    }
}
}
