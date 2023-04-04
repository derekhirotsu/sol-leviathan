using UnityEngine;

public class PlayerStateMachine : StateMachine {
    // Player Components
    [HideInInspector] public PlayerMotor controller;
    [HideInInspector] public PlayerRangedCombat rangedCombat;
    [HideInInspector] public PlayerMeleeCombat meleeCombat;
    [HideInInspector] public PlayerCombat combat;
    [HideInInspector] public InputBuffer inputBuffer;
    [HideInInspector] public HitDetection.HealthController health;
    [HideInInspector] public HitDetection.HitboxController hitboxController;
    [HideInInspector] public PhysicsModule physics;
    [HideInInspector] public Animator animator;
    [HideInInspector] public InteractionManager interactions;
    [HideInInspector] public PlayerAudio playerAudio;

    [Header("Player State Collection")]
    [SerializeField] protected PlayerStateCollection stateCollection;
    public PlayerStateCollection StateCollection { get { return stateCollection; } }

    void Awake() {
        combat = this.GetComponent<PlayerCombat>();
        interactions = this.GetComponent<InteractionManager>();
        inputBuffer = this.GetComponent<InputBuffer>();
        controller = this.GetComponent<PlayerMotor>();
        rangedCombat = this.GetComponent<PlayerRangedCombat>();
        meleeCombat = this.GetComponent<PlayerMeleeCombat>();
        health = GetComponent<HitDetection.HealthController>();
        hitboxController = GetComponent<HitDetection.HitboxController>();
        physics = this.GetComponent<PhysicsModule>();
        animator = this.GetComponent<Animator>();
        playerAudio = GetComponent<PlayerAudio>();
    }

    public override void Activate() {
        ReturnToIdle();
        active = true;
        inputBuffer.active = true;
        
        // commands.active = true;
    }

    public override void Deactivate() {
        ReturnToIdle();
        active = false;
        inputBuffer.active = false;
        
    }
}
