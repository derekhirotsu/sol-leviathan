using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    [SerializeField]
    protected float raycastCheckDistance = 15f;

    [Header("Interactable Lookup")]

    [SerializeField]
    protected ActiveInteractables ActiveInteractables;

    [Header("Interaction States")]

    [SerializeField]
    protected State state_interactionInputListening;

    [SerializeField]
    protected State state_interactionInProgress;
    public State InteractionInProgressState { get { return state_interactionInProgress; } }

    protected StateMachine stateMachine;

    [Header("Tracking Configs")]

    [SerializeField]
    public bool interacting = false;

    [SerializeField]
    public InteractableObject currentInteraction;

    [SerializeField]
    protected LayerMask interactionLayer;

    [Header("Cooldown")]

    [SerializeField]
    protected float interactionCooldown = 0.6f;

    protected bool onCooldown = false;

    // -----
    // Unity Lifecycle Methods
    // -----

    void Start() {
        stateMachine = this.GetComponent<StateMachine>();
    }

    void FixedUpdate() {
        // If an interactable object is nearby, we add a listening substate.
        if (ActiveInteractables.ItemCount > 0 && currentInteraction == null) {
            stateMachine.AddSubstate(state_interactionInputListening);
        }
    }

    // -----
    // InteractionManager Public API
    // -----

    public void OnInteract() {
        Debug.Log("Player pressed interact.", this);

        if (onCooldown) {
            Debug.Log("Interaction on cooldown.", this);
            return;
        }

        StartCoroutine(Cooldown());

        // If engaged in an ongoing interaction, check to see if current interaction can be resolved.
        if (interacting) {
            if (currentInteraction == null) {
                return;
            }

            currentInteraction.OnInteraction(this);
            // currentInteraction = null;
            // interacting = false;

            return;
        }

        // Check if any interactables are near.
        if (ActiveInteractables.ItemCount == 0) {
            Debug.Log("No Nearby Interactions");
            return;
        }
            
        // Remove invalid interactions from the list before resolving the list
        RemoveInvalidItems();

        // If there's only one interactable, choose it.
        if (ActiveInteractables.ItemCount == 1) {
            InteractWithFirstValidItem();
            return;
        }

        // There is more than one interactable; Do a raycast for interactable objects.
        RaycastHit HitInfo;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out HitInfo, raycastCheckDistance)) {
            InteractableObject raycastInteractable = HitInfo.collider.gameObject.GetComponent<InteractableObject>();
            
            // If the collider hit has an interactable object, then use that for interaction
            if (raycastInteractable != null) {
                raycastInteractable.OnInteraction(this);
            }

            return;
        }

        // The raycast didn't hit a collider so get the closest interactable from the Runtime Set instead.
        // At this point, we know there is more than one interactable, and they must be sorted by distance.
        ActiveInteractables.Items.Sort(CompareInteractablesByDistance);
        InteractWithFirstValidItem();
    }

    // -----
    // Protected Methods
    // -----

    protected IEnumerator Cooldown() {
        onCooldown = true;

        yield return new WaitForSeconds(interactionCooldown);

        onCooldown = false;
    }

    // Sort InteractableObjects based on distance from InteractionManger transform.
    protected int CompareInteractablesByDistance(InteractableObject a, InteractableObject b) {
        if (a == null && b == null) {
            return 0;
        }

        if (a == null) {
            return -1;
        }
        
        if (b == null) {
            return 1;
        }

        float aDistance = (a.transform.position - this.transform.position).sqrMagnitude;
        float bDistance = (b.transform.position - this.transform.position).sqrMagnitude;

        return aDistance.CompareTo(bDistance);
    }

    // Remove invalid interactions from the Runtime Set.
    protected void RemoveInvalidItems() {
        for (int i = ActiveInteractables.ItemCount - 1; i >= 0 ; --i) {
            InteractableObject interactable = ActiveInteractables.Items[i];

            if (!interactable.ValidInteractionState) {
                ActiveInteractables.RemoveItem(interactable);
            }
        }
    }

    // Iterates through ActiveInteractables. Invokes OnInteraction on the first usable interactable found in the list.
    protected void InteractWithFirstValidItem() {
        foreach (InteractableObject interactable in ActiveInteractables.Items) {
            if (interactable != null) {
                // Could optionally check if player is within interactable angle here if
                // interactable is set to be directional.
                // interactable.Directional && fov.WithinAngle(interactable.transform.position, this.transform.forward, InteractableObject.INTERACTION_ANGLE)
                interactable.OnInteraction(this);
                Debug.Log("Interacting with " + interactable.name);
                return;

            }
        }
    }
}
