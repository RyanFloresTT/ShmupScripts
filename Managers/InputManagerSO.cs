using _Project.Scripts.EventBus;
using UnityEngine;
using UnityEngine.InputSystem;

public enum ActionMap {
    Player,
    Menu
}

public struct OnPlayerMoveEvent : IEvent {
    public Vector2 MovementDirection;
}

public struct OnPlayerLookEvent : IEvent {
    public Vector2 LookDirection;
}

public struct OnPlayerZoomEvent : IEvent {
    public Vector2 Scroll;
}

public struct OnMenuUpEvent : IEvent { }

public struct OnMenuDownEvent : IEvent { }

public struct OnMenuLeftEvent : IEvent { }

public struct OnMenuRightEvent : IEvent { }

public struct OnMenuConfirmEvent : IEvent { }

[CreateAssetMenu(fileName = "InputManager", menuName = "ScriptableObjects/Managers/InputManager")]
public class InputManagerSO : ScriptableObject {
    // Components/classes
    PlayerInputActions _input;

    // State
    ActionMap activeActionMap = ActionMap.Player;

    // Lifecycle
    void OnEnable() {
        // Create input action instance
        this._input ??= new PlayerInputActions();

        // Subscribe to events
        this.SubscribeToEvents();

        // TODO: this should probably happen somewhere scene specific
        this.ChangeInputMap(ActionMap.Player);
    }

    void OnDisable() {
        // Subscribe to events
        this.DesubscribeToEvents();
    }

    // Methods
    void SubscribeToEvents() {
        // Subscribe to world events
        this._input.Player.Move.performed += this.HandlePlayerMovement;
        this._input.Player.Move.canceled += this.HandlePlayerMovement;
        this._input.Player.Look.performed += this.HandlePlayerLook;
        this._input.Player.CameraZoom.performed += this.HandlePlayerCameraZoom;

        // Subscribe to world events
        this._input.Menu.Up.performed += this.HandleMenuUp;
        this._input.Menu.Down.performed += this.HandleMenuDown;
        this._input.Menu.Left.performed += this.HandleMenuLeft;
        this._input.Menu.Right.performed += this.HandleMenuRight;
        this._input.Menu.Confirm.performed += this.HandleMenuConfirm;
    }

    void DesubscribeToEvents() {
        // Subscribe to world events
        this._input.Player.Move.performed -= this.HandlePlayerMovement;
        this._input.Player.Move.canceled -= this.HandlePlayerMovement;
        this._input.Player.CameraZoom.performed -= this.HandlePlayerCameraZoom;

        this._input.Menu.Up.performed -= this.HandleMenuUp;
        this._input.Menu.Down.performed -= this.HandleMenuDown;
        this._input.Menu.Left.performed -= this.HandleMenuLeft;
        this._input.Menu.Right.performed -= this.HandleMenuRight;
        this._input.Menu.Confirm.performed -= this.HandleMenuConfirm;

        // Disable action maps
        this.DisableInput();
    }

    public void DisableInput() {
        this._input.Player.Disable();
        this._input.Menu.Disable();
    }

    public void ChangeInputMap(ActionMap map) {
        this.activeActionMap = map;

        switch (map) {
            case ActionMap.Player:
                this._input.Player.Enable();
                break;
            case ActionMap.Menu:
                this._input.Menu.Enable();
                break;
        }
    }

    // Event handlers
    void HandlePlayerMovement(InputAction.CallbackContext context) {
        EventBus<OnPlayerMoveEvent>.Raise(new OnPlayerMoveEvent {
            MovementDirection = context.ReadValue<Vector2>().normalized
        });
    }

    void HandlePlayerLook(InputAction.CallbackContext context) {
        EventBus<OnPlayerLookEvent>.Raise(new OnPlayerLookEvent {
            LookDirection = context.ReadValue<Vector2>()
        });
    }

    void HandlePlayerCameraZoom(InputAction.CallbackContext context) {
        EventBus<OnPlayerZoomEvent>.Raise(new OnPlayerZoomEvent {
            Scroll = context.ReadValue<Vector2>()
        });
    }

    void HandleMenuUp(InputAction.CallbackContext obj) {
        EventBus<OnMenuUpEvent>.Raise(new OnMenuUpEvent());
    }

    void HandleMenuDown(InputAction.CallbackContext obj) {
        EventBus<OnMenuDownEvent>.Raise(new OnMenuDownEvent());
    }

    void HandleMenuLeft(InputAction.CallbackContext obj) {
        EventBus<OnMenuLeftEvent>.Raise(new OnMenuLeftEvent());
    }

    void HandleMenuRight(InputAction.CallbackContext obj) {
        EventBus<OnMenuRightEvent>.Raise(new OnMenuRightEvent());
    }

    void HandleMenuConfirm(InputAction.CallbackContext obj) {
        EventBus<OnMenuConfirmEvent>.Raise(new OnMenuConfirmEvent());
    }
}