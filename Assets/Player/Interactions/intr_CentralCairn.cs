using UnityEngine;

public class intr_CentralCairn : InteractableObject
{
    [Header("Interaction State Config")]

    [SerializeField]
    protected ScriptableVariables.ScriptableVariableReference<bool> waveInProgress;

    [SerializeField]
    protected ScriptableVariables.BoolVariable upgradeAvailable;

    [Header("Cairn Menu Config")]

    [SerializeField]
    protected MenuController menuController;

    public override bool ValidInteractionState {
        get {
            return CheckActiveStatus();
        }
    }

    public override void Update()
    {
        base.Update();

        if (currentInteractor != null) {
            promptCanvas.gameObject.SetActive(false);
        }
    }

    public override void OnInteraction(InteractionManager interactor)
    {
        // Cairn was toggled on.
        if (interactor.currentInteraction == null) {
            
            interactor.currentInteraction = this;
            currentInteractor = interactor;

            InputBuffer.ToggleActionMap(InputBuffer.PlayerInputActions.UI);

            if (upgradeAvailable.Value) {
                menuController.ShowMenu("shop");
            } else {
                menuController.ShowMenu("next_wave");
            }

        // Cairn was toggled off.
        } else {
            promptCanvas.gameObject.SetActive(true);
            
            interactor.currentInteraction = null;
            currentInteractor = null;
            menuController.ShowMenu("player_hud");
        }        
    }

    public override bool CheckActiveStatus() {
        // True if there are valid targets nearby and a wave is not in progress.
        return base.CheckActiveStatus() && !waveInProgress.Value;
    }

    public void NewWaveBeginning() {

        EndInteraction();
    }

    public void PurchaseUpgrade(int index) {
        Debug.Log("Upgrade " + (index+1) + " purchased.");
        upgradeAvailable.SetValue(false);
        menuController.ShowMenu("next_wave");
    }

    protected override void EndInteraction() {
        base.EndInteraction();

        if (!waveInProgress.Value) {
            menuController.ShowMenu("player_hud");
        }

        InputBuffer.ToggleActionMap(InputBuffer.PlayerInputActions.Player);
        
    }

    
}
