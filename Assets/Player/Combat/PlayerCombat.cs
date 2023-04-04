using System.Collections;
using UnityEngine;
using HitDetection;

public class PlayerCombat : MonoBehaviour {

    [SerializeField]
    protected ScriptableVariables.BoolVariable playerActive;

    [SerializeField]
    protected State state_TakeDamage;

    // ---
    // Component References
    // ---
    protected PlayerStateMachine stateMachine;
    protected HealthController healthController;
    protected HitboxController hitboxController;

    void Awake() {
        playerActive.SetValue(true);

        stateMachine = this.GetComponent<PlayerStateMachine>();
        healthController = GetComponent<HealthController>();
        hitboxController = GetComponent<HitboxController>();

    }

    // ---
    // Weapon Drawing
    // ---
    [Header("Player Weapons")]
    [SerializeField] protected GameObject gunObject;
    public void DrawGun() {
        if (!gunObject.activeSelf) {
            gunObject.SetActive(true);
        }

        if (!sheathedBladeObject.activeSelf) {
            sheathedBladeObject.SetActive(true);
        }

        if (spearObject.activeSelf) {
            spearObject.SetActive(false);
        }

        if (bladeObject.activeSelf) {
            bladeObject.SetActive(false);
        }

        stateMachine.animator.SetLayerWeight(1, 0.0f);
    }

    [SerializeField] protected GameObject spearObject;
    public void DrawSpear() {
        if (!spearObject.activeSelf) {
            spearObject.SetActive(true);
        }

        if (gunObject.activeSelf) {
            gunObject.SetActive(false);
        }

        if (bladeObject.activeSelf) {
            bladeObject.SetActive(false);
        }

        if (sheathedBladeObject.activeSelf) {
            sheathedBladeObject.SetActive(false);
        }

        stateMachine.animator.SetLayerWeight(1, 1.0f);
    }
    
    [SerializeField] protected GameObject bladeObject;
    [SerializeField] protected GameObject sheathedBladeObject;
    public void DrawBlade() {
        if (!bladeObject.activeSelf) {
            bladeObject.SetActive(true);
        }

        if (sheathedBladeObject.activeSelf) {
            sheathedBladeObject.SetActive(false);
        }

        if (spearObject.activeSelf) {
            spearObject.SetActive(false);
        }

        if (gunObject.activeSelf) {
            gunObject.SetActive(false);
        }
        
        stateMachine.animator.SetLayerWeight(1, 1.0f);
    }
    
    protected IEnumerator rollingCoroutine;
    public void DodgeRollForDuration(float duration) {
        if (rollingCoroutine != null) {
            StopCoroutine(rollingCoroutine);
        }

        rollingCoroutine = HitboxToggle(duration);
        StartCoroutine(rollingCoroutine);
    }

    protected IEnumerator HitboxToggle(float duration) {
        stateMachine.hitboxController.SetAllHitboxesActive(false);

        yield return new WaitForSeconds(duration);

        stateMachine.hitboxController.SetAllHitboxesActive(true);
    }

    // -----
    // Event Callbacks
    // -----

    // public virtual void HandleTakeDamage(DamageSource source) {
    public virtual void HandleTakeDamage(HitDetection.HitInfo info) {

        // stateMachine.StateTransition(state_TakeDamage);

        // stateMachine.melee.InterruptMelee();
        stateMachine.meleeCombat.InterruptMelee();

        // Receive force impulse from attack   
        Vector2 enemyToPlayerX = new Vector2((this.transform.position - info.DamageSource.SourceEntity.transform.position).x, 0).normalized;
        Vector3 popVector = Vector2.up + enemyToPlayerX;

        stateMachine.controller.FaceBasedOnVector(-enemyToPlayerX);
        stateMachine.physics.NeutralizeAllForce();
        stateMachine.physics.AddForce(popVector * 4000f);

        stateMachine.animator.Play("TakeDamage");

        // Apply hitstop
        stateMachine.controller.Cam.Hitstop(0.3f);
        stateMachine.controller.Cam.CameraShake(0.4f, 0.5f);
    }

    public virtual void HandleHealthDepleted() {
        playerActive.SetValue(false);
    }

    public virtual void OnHitboxHit(HitDetection.HitInfo info) {
        Debug.Log(info);
        // stateMachine.health.TakeDamage(info);
        healthController.TakeDamage(info);
    }

    // -----
    // Event Subscriptions
    // -----
    public virtual void OnEnable() {
        // stateMachine.health.OnTakeDamage += HandleTakeDamage;
        // stateMachine.health.OnHealthDepleted += HandleHealthDepleted;
        // stateMachine.hitboxController.OnHitboxHit += OnHitboxHit;
        healthController.OnTakeDamage += HandleTakeDamage;
        healthController.OnHealthDepleted += HandleHealthDepleted;
        hitboxController.OnHitboxHit += OnHitboxHit;
    }

    public virtual void OnDisable() {
        // stateMachine.health.OnTakeDamage -= HandleTakeDamage;
        // stateMachine.health.OnHealthDepleted -= HandleHealthDepleted;
        // stateMachine.hitboxController.OnHitboxHit -= OnHitboxHit;
        healthController.OnTakeDamage -= HandleTakeDamage;
        healthController.OnHealthDepleted -= HandleHealthDepleted;
        hitboxController.OnHitboxHit -= OnHitboxHit;
    }
}
