using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(FieldOfView))]
public abstract class InteractableObject : MonoBehaviour {
    public static float INTERACTION_RANGE = 4f;
    public static float INTERACTION_ANGLE = 360f;

    protected FieldOfView fov;
    protected Canvas promptCanvas;
    protected TMP_Text interactionText;
    protected Image interactionPrompt;

    [Header("Interactable Lookup")]
    [SerializeField] protected ActiveInteractables ActiveInteractables;
    [SerializeField] protected InteractionManager currentInteractor;

    [Header("Configurable Fields")]

    [SerializeField]
    protected string interactionName = "Interact";
    
    [SerializeField]
    protected bool directional = false;
    public bool Directional { get { return directional; } }

    // [SerializeField]
    // protected bool active = false;
    public virtual bool ValidInteractionState { get { return CheckActiveStatus(); } }
    
    // Start is called before the first frame update
    public virtual void Start() {
        if(ActiveInteractables == null) {
            Debug.LogWarning(this.name + " was not given a reference to the Interactable Runtime Set");
            this.enabled = false;
        }

        this.gameObject.layer = LayerMask.NameToLayer("Interactable");
        
        fov = this.GetComponent<FieldOfView>();
        fov.ViewAngle = 360;

        interactionText = this.GetComponentInChildren<TMP_Text>();
        interactionText.text = interactionName;

        interactionPrompt = this.GetComponentInChildren<Image>();

        promptCanvas = this.GetComponentInChildren<Canvas>();
        promptCanvas.gameObject.SetActive(false);
        
    }

    public virtual void Update() {
        bool isActive = CheckActiveStatus();
        UpdateInteraction(isActive);
    }

    void OnDisable() {
        RemoveSelfFromLookup();
    }

    public abstract void OnInteraction(InteractionManager interactor);

    public virtual bool CheckActiveStatus() {
        return ValidTargetsNearby();
    }

    // If an actor enters the interaction range, add this interactable to their interaction list.
    // If isActive is true, activate interaction; Otherwise deactivate.
    public virtual void UpdateInteraction(bool isActive) {
        // active = isActive;
        promptCanvas.gameObject.SetActive(isActive);

        if (isActive) {
            AddSelfToLookup();
        } else {
            RemoveSelfFromLookup();   
        }
    }

    protected virtual bool ValidTargetsNearby() {
        List<Transform> nearbyTargets = fov.FindNearbyTargets(INTERACTION_RANGE, fov.TargetMask);

        if (nearbyTargets.Count == 0) {
            return false;
        }

        bool validTargetNearby = true;

        if (directional) {
            validTargetNearby = CheckTargetExistsWithinAngle(nearbyTargets);
        }

        return validTargetNearby;
    }

    protected bool CheckTargetExistsWithinAngle(List<Transform> targets) {
        if (targets.Count == 0) {
            return false;
        }

        bool targetExists = false;

        foreach (Transform target in targets) {
            if (fov.SourceWithinAngle(target.position, this.transform.position, target.forward, INTERACTION_ANGLE)) {
                targetExists = true;
                break;
            }
        }

        return targetExists;
    }

    protected void AddSelfToLookup() {
        ActiveInteractables.AddItem(this);
    }

    protected void RemoveSelfFromLookup() {
        ActiveInteractables.RemoveItem(this);
    }

    protected void UpdateInteractionName(string newName) {
        interactionName = newName;
        interactionText.text = interactionName;
    }

    protected IEnumerator PromptColorPulse() {

        yield return null;
    }

    protected virtual void EndInteraction() {
        // Make sure we're not being interacted with anymore
        if (currentInteractor != null) {
            
            if (currentInteractor.currentInteraction != null && currentInteractor.currentInteraction == this) {
                currentInteractor.currentInteraction = null;
                currentInteractor = null;
            }
        }

        RemoveSelfFromLookup();
    }
}
