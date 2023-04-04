using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* The goal of the StateFrame is to enable complex state-based behaviour through
    interactions between Finite Superstates and Pushdown Substates that modify them.

        1. Listen for Input Updates (based on active states).
        2. Call Relevant functions of those states.
        3. Add, Remove, or Transition to a new state.

    The states listen for updates and CALL functions from other relevant Monobehaviour Objects,
        They do NOT contain the funtions themselves. This is so Engine-Specific Operations such as
        Coroutines, Hitboxes & Registration, UI Elements, etc. are abstracted away from the States,
        and handled by Scripts & Objects that are specifically designed to handle these things.

        The States need to handle WHEN functions should be called.
    
    Each frame,
        1. Check Substate Updates in Stack. 
        2. Check Finite State Updates
        3. Check Component States
        4. Lock / Unlock Transitions if necessary
        5. Check to see if a State has expired.
*/ 
public abstract class StateMachine : MonoBehaviour {
    [Header("State Visualization")]

    public float gizmoRadius = 2f;

    [Header("Logic Fields")]

    // To check if states should currently be listened to.
    public bool active;

    [SerializeField]
    protected bool canTransition;

    // Superstate starts as idle, allowing free movement. Can transition to specific attacking states.
    // Superstates are finite: Only one can be active at a time, and they must transition to some other state.
    [SerializeField]
    protected State restingState;
    
    [SerializeField]
    protected State finiteState;

    protected float transitionTime;

    // Pushdown States are stored in a list. Many can be active at any point.
    // Pushdown States modify the current finiteState rather than outright replacing it.
    //      A good use of pushdown states are Aiming States, Combo Recoveries, etc.
    [SerializeField]
    protected List<State> pushdownStates = null;

    [SerializeField]
    protected List<float> pushdownExpirationTimes = null;
    
    // Component States are listeners for functions that may only be performed when the StateFrame is "Idle", but are
    //      universally used regardless of what that Idle State is. For instance, the Command Deck is a combat-component
    //      that may be tracked / used regardless of what weapon is currently equipped.
    //
    //      These are set in the inspector and are not removed at Runtime.
    [SerializeField]
    protected List<State> componentStates = null;

    // -----
    // Unity Lifecycle Methods
    // -----

    public virtual void Start() {
        restingState.OnStateEnter(this);
    }

    void FixedUpdate() {
        if (!active || restingState == null) 
            return;

        // 1. listen for substate updates
        UpdateSubstates();

        // 2. Check to see if a timed transition should occur
        CheckCurrentState();

        // 3. listen for updates.
        finiteState.Listen(this);

        // 4. If Stateframe is in an IdleState, listen to Component States
        CheckComponentStates();
    }

    // -----
    // StateMachine Methods
    // -----

    public virtual void Activate() { }

    public virtual void Deactivate() { }
    
    public virtual void SetRestingState(State newState) {
        // 1. Lock transitions.
        canTransition = false;

        // 2. Cleanup all the active Substates
        ClearSubstates();

        // 3. Set the resting state
        restingState = newState;

        // 4. Set the current finite state
        SetFiniteState(restingState);

        // 5. Unlock transitions
        canTransition = true;
    }

    public virtual void ReturnToIdle() {
        ClearSubstates();
        StateTransition(restingState);
    }

    // Replaces current Superstate with the newState, and resets the transition timer.
    // Each time a transition takes place, the states Action is called via Act().
    public virtual void StateTransition(State newState) {
        if (!canTransition) {
            return;
        }

        // If attempting to transition to the same state, and it can be refreshed...
        if (newState.Equals(finiteState) && finiteState.CanBeRefreshed) {
            // Refresh the state
            RefreshState(newState);
        } else {
            // Transition to new state
            SetFiniteState(newState);
        }
    }

    // Reset the time to live of the given state. Does NOT call OnStateEnter, as the state is already present.
    protected void RefreshState(State newState) {
        if (newState.IsFinite) {
            transitionTime = SetTimer(newState);
        } else {
            int stateIndex = pushdownStates.FindIndex(newState.Equals);

            pushdownExpirationTimes[stateIndex] = SetTimer(newState);
        }
    }

    // Transition from the current finite state to a new one, calling OnStateEnter
    protected void SetFiniteState(State newState) {
        // 1. Call exit function of current state
        if (finiteState != null) {
            finiteState.OnStateExit(this);
        }

        // 2. Set the new current state.
        finiteState = newState;
        finiteState.OnStateEnter(this);
        transitionTime = SetTimer(newState);
    }

    // Return a time to live of the given state
    protected float SetTimer(State state) {
        return Time.time + state.TimeToLive;
    }

    // Return the required substate from the asset menu
    // REVIEW: Perhaps Substate should implement a list of it's own State references to call when needed. Performance may suffer otherwise...?
    public virtual State GetStateFromResources(string prefix, string stateName, float stateTimeOverride = 0f) {
        string resourcePath = prefix+"_States/"+prefix+"_"+stateName;
        State newState = Resources.Load<State>(resourcePath);

        if (newState == null) {
            Debug.LogWarning("The requested state could not be found at path " + resourcePath);
            return restingState;
        }

        // If no time to live override is provided, return the existing version of the state. Otherwise, make a copy and override.
        if (stateTimeOverride > 0f) {
            State clonedState = Instantiate(newState);
            clonedState.TimeToLive = stateTimeOverride;
            return clonedState;
        } else {
            return newState;
        }
    }

    // -----
    // State Comparison
    // -----

    public bool MatchesFiniteState(State state) {
        return (finiteState.Equals(state));
    }

    // Check if the current resting state is active.
    public virtual bool IsIdle() {
        return (restingState.Equals(finiteState));
    }

    // -----
    // Substate Methods
    // -----

    public virtual void AddSubstate(State newState) {
        // Add the new state if it's not in the list
        if (!pushdownStates.Contains(newState)) {
            pushdownStates.Add(newState);
            pushdownExpirationTimes.Add(SetTimer(newState));
            newState.OnStateEnter(this);
        } else {
        // Refresh the state if it's found in the list.
            RefreshState(newState);
        }
    }

    public virtual void RemoveSubstate(State oldState, bool interruptState = false) {
        // Given that the state exists in the list ...
        if (pushdownStates.Contains(oldState)) {
            // Find the index of the state and it's expiration time.
            int stateIndex = pushdownStates.FindIndex(oldState.Equals);
            // Debug.Log(oldState + " found at index " + stateIndex);

            // Call it's Exit method, given the Substate is not interrupted.
            if (interruptState && oldState.Interruptable) {
                Debug.Log(oldState.name + " was interrupted. ");
            } else {
                oldState.OnStateExit(this);
            }

            // Remove both the state and it's expiration time from respective collections.
            pushdownExpirationTimes.RemoveAt(stateIndex);
            pushdownStates.RemoveAt(stateIndex);
        }
    }

    private void ClearSubstates() {
        for (int i = pushdownStates.Count - 1; i >= 0; i--) {
            // Make sure exit state is called 
            if (!pushdownStates[i].Interruptable) {
                pushdownStates[i].OnStateExit(this);
            }
            pushdownStates.RemoveAt(i);
            pushdownExpirationTimes.RemoveAt(i);
        }
    }

    public bool Contains(State pushdownState) {
        return pushdownStates.Contains(pushdownState);
    }

    public bool MatchesSubstateList(List<State> targetStates) {
        
        foreach (State state in targetStates) {
            // Return false on the first mismatch
            if (!pushdownStates.Contains(state)) {
                return false;
            }
        }
        
        return true;
    }

    // -----
    // Private Methods
    // -----
    
    private void UpdateSubstates() {
        for (int i = pushdownStates.Count - 1; i >= 0; i--) {
            // Check for expiration BEFORE listening. A trigger may remove the state, rendering expiration pointless.
            if (Time.time >= pushdownExpirationTimes[i]) {
                // Debug.Log(pushdownStates[i] + " has expired.");
                RemoveSubstate(pushdownStates[i]);
            } else {
                pushdownStates[i].Listen(this);
            }
        }
    }

    private void CheckCurrentState() {
        if (Time.time < transitionTime) { // State hasn't expired yet; No need to do anything.
            return;
        }

        // Use TransitionState If the state has it's own resting state. Otherwise, use default resting state.
        State nextState = finiteState.TransitionState ?? restingState;

        StateTransition(nextState);
    }

    private void CheckComponentStates() {
        if (!IsIdle()) {
            return;
        }

        for (int i = componentStates.Count - 1; i >= 0; i--) {
            componentStates[i].Listen(this);
        }
    }

    // -----
    // Debug
    // -----

    void OnDrawGizmos() {
        if (finiteState != null) {
            Gizmos.color = finiteState.sceneGizmoColor;
            Gizmos.DrawWireSphere(this.transform.position, gizmoRadius);
        }
    }
}
