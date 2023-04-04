using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

// For consistent references of input names from outside components.
public static class InputName {

    // Axes
	public static string Move { get { return "Move"; } }
	public static string Aim { get { return "Aim"; } }

    // Shoulder buttons
	public static string Fire { get { return "Fire"; } }
    public static string HeavyFire { get { return "HeavyFire"; } }
	public static string Brace { get { return "Brace"; } }

    // Face buttons
    public static string Jump { get { return "Jump"; } }
    public static string Melee { get { return "Melee"; } }
    public static string Roll { get { return "Roll"; } }

}

// Input Buffer is made to be a resource that is polled by other components.
//      Specifically, it is designed to handle player combat-related inputs, and attempts
//      to simplify access to specific Action Inputs, such as Movement Vectors and Attack Triggers.

// REQUIREMENTS:
[RequireComponent(typeof(PlayerInput))]
public class InputBuffer : MonoBehaviour {
    // OnControlsChangedEvent
    // public static event Action OnControlsChangedEvent;

    // Active State
    public bool active = true;

    // Component References
    protected static PlayerInputActions inputActions;
    public static PlayerInputActions PlayerInputActions { get { return inputActions; } }
    private static PlayerInput inputComponent;
    private Camera playerCamera;
    public Camera PlayerCamera { get { return playerCamera; } }

    [Header("Logistic Fields")]
    [SerializeField] private float bufferTime = 0.1f; // How long a trigger is stored and considered "active"

    [SerializeField]
    protected ScriptableVariables.BoolVariable gamepadInputActive;
    public bool GamepadInputActive { get { return gamepadInputActive.Value; } }

    // Subscribed Vectors
    [Header("Vectors")]
    [SerializeField] private Vector2 movementVector;
    public Vector2 GetMovementVector() { return movementVector; }

	[SerializeField] private Vector2 rotationVector;
    [SerializeField] private Vector2 mouseRotationVector;
    public Vector2 GetRotationVector() {
        if (gamepadInputActive.Value) {
            return rotationVector;
        } else {
            return mouseRotationVector;
        }
    }

	[SerializeField] private Vector2 mousePosition;
    public Vector2 GetMousePosition() { return mousePosition; }

    [Header("Hold Actions")]
    // [SerializeField] protected InputAction action2;

    // Both Combat Action Triggers and Holds are stored in a Lookup Table.
    //      Access to the values in the table are controlled within the InputBuffer class.
    //      Actions are NOT added to the table until their first invocation.
    //      ONLY explicitly subscribed actions are added to the table.
    private Dictionary<string, bool> triggeredActions;
    private Dictionary<string, IEnumerator> bufferCoroutines;
    private Dictionary<string, bool> heldActions;

    private Queue<string> inputQueueActions;
    private Queue<float> inputQueueExpirations;

    private void PushInput(string actionName) {
        inputQueueActions.Enqueue(actionName);
        inputQueueExpirations.Enqueue(Time.time + bufferTime);
    }

    private string PopInput() {
        if (inputQueueActions.Count > 0 && inputQueueExpirations.Count > 0) {

            // Remove the expiration, and return the top input.
            inputQueueExpirations.Dequeue();
            return inputQueueActions.Dequeue();

        }
        
        return "";
    }

    // Discards all expired inputs, and returns the first valid input.
    public string PopQueuedInput() {
        // bool validInputFound = false;
        string topInput = PopInput();

        return topInput;
    }

    // Discards all expired inputs, and returns the first valid input. Does not remove it from the queue.
    public string PeekQueuedInput() {
        if (inputComponent.currentActionMap.name == "UI") {
            return "";
        }

        bool validInputFound = false;
        string topInput = "";

        // This loop will continue until either a valid input is found or the queues are completely emptied.
        while (!validInputFound && inputQueueExpirations.Count > 0) {

            // This input has expired if the current time has exceeded the expiration.
            if (inputQueueExpirations.Peek() < Time.time) {
                PopInput();
            } else {
                topInput = inputQueueActions.Peek();
                validInputFound = true;
            }
        }

        // Debug.Log(topInput + " is being polled");

        return topInput;
    }

    // public 

    // Returns the value of corresponding action.
    //      Returns FALSE if the action does not exist in the table.
    //      Optionally, may consume the trigger to reset it's value
    public bool ActionTriggered(string actionName, bool consumeTrigger = false) {
        bool value;
        if (!triggeredActions.TryGetValue(actionName, out value)) {
            return false;
        }

        if (consumeTrigger) {
            ConsumeTrigger(actionName);
        }

        return value;
    }

    public void ConsumeTrigger(string actionName) {
        bool value;
        if (!triggeredActions.TryGetValue(actionName, out value)) {
            return;
        }

        if (value) {
            triggeredActions[actionName] = false;
        }
    }

    // Returns the value of corresponding action.
    //      Returns FALSE if the action does not exist in the table.
    public bool ActionHeld(string actionName) {
        bool value;
        if (!heldActions.TryGetValue(actionName, out value)) {
            return false;
        }
        return value;
    }

    // -----
    // Unity Lifecycle Methods
    // -----

    void Awake() {
        // Component setup
        inputActions = new PlayerInputActions();
        inputComponent = this.GetComponent<PlayerInput>();
        triggeredActions = new Dictionary<string, bool>();
        bufferCoroutines = new Dictionary<string, IEnumerator>();
        heldActions = new Dictionary<string, bool>();

        // Queue setup
        inputQueueActions = new Queue<string>();
        inputQueueExpirations = new Queue<float>();

        // Camera setup
        playerCamera = inputComponent.camera;

        if (playerCamera == null) {
            playerCamera = Camera.main;
            Debug.LogWarning(this.name + "'s PlayerInput was not given a camera. ");
        }   
    }

    // Input updates called each frame
    // void Update() {
    //     CheckInputSource();
    // }

    // New Input System requires these function defintions.
    private void OnEnable() {
		inputActions.Enable();
        OnControlsChanged();
	}

	private void OnDisable() {
		inputActions.Disable();
	}

    // -----
    // Player Input Event Callbacks
    // -----

    public void OnMovement(InputAction.CallbackContext value) {
        movementVector = value.ReadValue<Vector2>();
    }

    public void OnRotation(InputAction.CallbackContext value) {
        rotationVector = value.ReadValue<Vector2>();
    }

    public void OnMousePosition(InputAction.CallbackContext value) {
        if (value.canceled) {
            return;
        }
        
        mousePosition = value.ReadValue<Vector2>();

        Vector3 screenPos = playerCamera.WorldToScreenPoint(this.transform.position);
        Vector3 playerToMouseVector = (new Vector3 (mousePosition.x, mousePosition.y, 0) - screenPos).normalized;
        mouseRotationVector =  new Vector2 (playerToMouseVector.x, playerToMouseVector.y);

    }

    public Vector2 WorldVectorToScreenPos(Vector3 worldPos) {
        Vector2 screenPos = playerCamera.WorldToScreenPoint(worldPos);

        return screenPos;
    }

    public void OnControlsChanged() {
        if (inputComponent == null) {
            return;
        }
        // CheckInputSource();

        bool isGamepadCurrentScheme = inputComponent.currentControlScheme == "Gamepad";

        if (isGamepadCurrentScheme != gamepadInputActive.Value) {
            gamepadInputActive.SetValue(isGamepadCurrentScheme);
        }

        // OnControlsChangedEvent?.Invoke();
    }

    public void CheckHold(InputAction.CallbackContext context) {
        if (!active) {
            return;
        }
        
        UpdateHeldAction(context.action.name, context.ReadValue<float>() != 0);
    }

    // Take in the current action's context, and buffer it's trigger for some time.
    // This function increases how responsive combat inputs will feel.
    public void BufferInput(InputAction.CallbackContext context) {
        // Debug.Log(triggeredActions);
        if (!active) {
            return;
        }

        if (context.action.triggered) {
            // Make sure the Input is available in the lookup table
            if (!triggeredActions.ContainsKey(context.action.name)) {
                triggeredActions.Add(context.action.name, false);
                bufferCoroutines.Add(context.action.name, null);
            }

            // TEST new Queue system
            if (bufferCoroutines[context.action.name] == null) {
                PushInput(context.action.name);
            }

            // Check if the trigger is already active before starting a coroutine
            if (bufferCoroutines[context.action.name] != null) {
                StopCoroutine(bufferCoroutines[context.action.name]);
            }
            
            IEnumerator newBuffer = Buffer(context.action.name, 0.1f);
            bufferCoroutines[context.action.name] = newBuffer;
            StartCoroutine(newBuffer);
        }
        
    }

    // -----
    // Public API
    // -----

    // public static event Action<InputActionMap> actionMapChange;
    public static void ToggleActionMap(InputActionMap actionMap) {
        // if (actionMap.enabled) {
        //     return;
        // }

        Debug.Log("Switching to Action Map : " + actionMap.name);

        inputComponent.SwitchCurrentActionMap(actionMap.name);
        // Debug.Log("Swapping to scheme " + actionMap.name);
        // inputActions.Disable();
        // // actionMapChange?.Invoke(actionMap);
        // actionMap.Enable();
    }

    // -----
    // Private Methods
    // -----

    // Check to see if the given Action's input is being held down.
    private void UpdateHeldAction(string actionName, bool value) {
        // Make sure the Action is available in the lookup table
        if (!heldActions.ContainsKey(actionName)) {
            heldActions.Add(actionName, false);
        }

        heldActions[actionName] = value;
    }

    // Activates the designated Action Trigger for some amount of time.
    private IEnumerator Buffer(string actionName, float bufferTime) {
        triggeredActions[actionName] = true;

        while (bufferTime >= 0.0f) {
            // Debug.Log(actionName + " is being buffered");
            bufferTime -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        triggeredActions[actionName] = false;
        bufferCoroutines[actionName] = null;
    }

    // Determines whether gamepad currently being used or not.
    // private void CheckInputSource() {
    //     if (inputComponent.currentControlScheme == "Gamepad") {
    //         gamepadInputActive.Value = true;
    //     } else {
    //         gamepadInputActive.Value = false;
    //     }
    // }
}
