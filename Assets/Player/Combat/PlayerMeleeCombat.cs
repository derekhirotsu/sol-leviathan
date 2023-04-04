using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeleeCombat : MonoBehaviour {
    Animator animator;
    PlayerCombat combat;
    InputBuffer input;
    PlayerMotor motor;
    PhysicsModule physics;
    PlayerAudio playerAudio;

    [Header("Ground Blade System")]

    [SerializeField]
    protected MeleeAttackConfig groundMelee;

    [SerializeField]
    protected int maxMeleeAttacks = 3;
    
    protected int currentMeleeAttack = 0;
    
    public bool CanSlash { get { return currentMeleeAttack < maxMeleeAttacks; } }

    [Header("Aerial Blade System")]

    [SerializeField]
    protected MeleeAttackConfig airMelee;

    [SerializeField]
    protected ScriptableVariables.ScriptableVariableReference<int> maxAirAttacks;

    [SerializeField] 
    protected ScriptableVariables.IntVariable remainingAirAttacks;

    [SerializeField]
    protected ScriptableVariables.ScriptableVariableReference<bool> airMeleeUnlocked;

    public bool AirAttackAvailable { get { return remainingAirAttacks.Value > 0 && airMeleeUnlocked.Value; } }

    [Header("Roll Attack")]

    [SerializeField]
    protected MeleeAttackConfig rollAttack;

    [SerializeField]
    protected ScriptableVariables.ScriptableVariableReference<bool> rollAttackUnlocked;

    public bool RollAttackAvailable { get { return rollAttackUnlocked.Value; } }

    [Header("Slam Attack")]

    [SerializeField]
    protected MeleeAttackConfig groundSlam;

    [SerializeField]
    protected ScriptableVariables.ScriptableVariableReference<bool> groundSlamUnlocked;
    
    public bool GroundSlamAvailable { get { return groundSlamUnlocked.Value; } }

    // -----
    // Unity Lifecycle Methods
    // -----

    void Start() {
        animator = this.GetComponent<Animator>();
        combat = this.GetComponent<PlayerCombat>();
        input = this.GetComponent<InputBuffer>();
        motor = this.GetComponent<PlayerMotor>();
        physics = this.GetComponent<PhysicsModule>();
        playerAudio = GetComponent<PlayerAudio>();
    }

    // void Update() {
    //     // Refresh aerial charges once we land
    //     if (motor.PlayerIsOnGround) {
    //         SetRemainingAirAttacksValue(maxAirAttacks);
    //     }
    // }

    void OnEnable() {
        maxAirAttacks.Subscribe(OnMaxAirAttacksChange);
    }

    void OnDisable() {
        maxAirAttacks.Unsubscribe(OnMaxAirAttacksChange);
    }

    public bool MeleeAttackInProgress { get { return activeMeleeCoroutine != null; } }
    protected IEnumerator activeMeleeCoroutine;

    // -----
    // Player Melee API
    // -----

    public void InterruptMelee() {
        if (activeMeleeCoroutine != null) {
            StopCoroutine(activeMeleeCoroutine);
        }
    }

    public void RecoverMelee() {
        currentMeleeAttack = 0;
    }

    public void GroundMelee() {
        InterruptMelee();

        combat.DrawBlade();
        activeMeleeCoroutine = MeleeAttack(groundMelee, "GroundedMelee");
        animator.SetInteger("CurrentMelee", ++currentMeleeAttack);
        StartCoroutine(activeMeleeCoroutine);
    }

    public void AirMelee() {
        InterruptMelee();

        physics.NeutralizeVerticalForce();
        physics.AddForce((this.transform.up).normalized * 3500);
        physics.AddForce((motor.FacingVector).normalized * 1000);

        combat.DrawBlade();

        string animationName = motor.FacingVector.x > 0 ? "AerialSlashRight_Enter" : "AerialSlashLeft_Enter";
        activeMeleeCoroutine = MeleeAttack(airMelee, animationName);
        
        IncrementRemainingAirAttacksValue(-1);
        animator.SetInteger("CurrentMelee", remainingAirAttacks.Value);

        StartCoroutine(activeMeleeCoroutine);
    }

    public void GroundSlam() {
        InterruptMelee();

        combat.DrawSpear();
        activeMeleeCoroutine = PlungingSlash(groundSlam);
        StartCoroutine(activeMeleeCoroutine);
    }

    public void RollAttack() {
        InterruptMelee();

        combat.DrawSpear();
        activeMeleeCoroutine = SpearThrust(rollAttack, "GroundedSpearThrust");
        StartCoroutine(activeMeleeCoroutine);
    }

    public void RecoverAirAttacks() {
        SetRemainingAirAttacksValue(maxAirAttacks);
    }

    // -----
    // Protected Methods
    // -----

    protected void OnMaxAirAttacksChange(int newValue) {
        SetRemainingAirAttacksValue(newValue);
    }

    public void SetRemainingAirAttacksValue(int newValue) {
        remainingAirAttacks.SetValue(newValue);
    }

    protected void IncrementRemainingAirAttacksValue(int amount) {
        remainingAirAttacks.ApplyChange(amount);
    }

    protected IEnumerator MeleeAttack(MeleeAttackConfig stats, string animationName) {
        Vector2 strikeVector = motor.FacingVector;

        animator.Play(animationName);
        
        // Windup
        float windupTime = stats.WindupTime;
        while (windupTime > 0) {
            
            windupTime -= Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        StartCoroutine(motor.AnimationTrigger("MeleeStrike"));
        
        // Execution
        float swingTime = stats.SwingTime;
        while (swingTime > 0) {
            physics.Translate(strikeVector * stats.MoveSpeedWhileStriking);
            swingTime -= Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        Vector3 forwardOffset = motor.FacingVector * 2;

        stats.Attack(this.gameObject, this.transform.position + forwardOffset, motor.FacingVector);
        // Instantiate(meleePrefab, this.transform.position + forwardOffset, Quaternion.identity);
        motor.Cam.Hitstop(0.1f);
        motor.Cam.CameraShake(0.2f, 0.2f);
        playerAudio.PlayClipOneShot("melee_attack");

        // Section of the recovery
        float recoveryTimer = stats.RecoveryTime;
        while (recoveryTimer > 0) {
            physics.Translate(strikeVector * (stats.MoveSpeedWhileStriking * recoveryTimer));
            recoveryTimer -= Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        // Recovery
        activeMeleeCoroutine = null;
    }

    protected IEnumerator PlungingSlash(MeleeAttackConfig stats) {
        Vector2 strikeVector = motor.FacingVector;

        animator.SetInteger("CurrentMelee", 3);
        animator.Play("AerialSlashLand_Enter");
        playerAudio.PlayClipOneShot("ground_slam");
        // HitStop1

        motor.Cam.Hitstop(0.1f);
        motor.Cam.CameraShake(0.1f, 0.2f);

        float swingTime = 0.1f;
        while (swingTime > 0) {
            swingTime -= Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }
        
        stats.Attack(this.gameObject, this.transform.position, motor.FacingVector);
        StartCoroutine(motor.AnimationTrigger("MeleeStrike"));
        motor.Cam.Hitstop(0.1f);
        motor.Cam.CameraShake(0.4f, 0.4f);

        float holdTime = 0.5f;
        while (holdTime > 0) {
            holdTime -= Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        // Recovery
        RecoverMelee();
        activeMeleeCoroutine = null;
    }

    protected IEnumerator SpearThrust(MeleeAttackConfig stats, string animationName) {
        motor.FaceBasedOnMovement();
        Vector2 strikeVector = ClampedMeleeAimVector().normalized;

        animator.Play(animationName);
        playerAudio.PlayClipOneShot("roll_attack_windup");
        
        // Windup
        float windupTime = stats.WindupTime;
        while (windupTime > 0) {
            
            windupTime -= Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        StartCoroutine(motor.AnimationTrigger("MeleeStrike"));
        playerAudio.PlayClipOneShot("roll_attack_swing");
        // Execution
        float swingTime = stats.SwingTime;
        while (swingTime > 0) {
            physics.Translate(motor.FacingVector * stats.MoveSpeedWhileStriking);
            swingTime -= Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        Vector3 forwardOffset = strikeVector * -1.3f;

        stats.Attack(this.gameObject, forwardOffset, strikeVector, parentedToSource:true);
        // Instantiate(meleePrefab, this.transform.position + forwardOffset, Quaternion.identity);
        motor.Cam.Hitstop(0.1f);
        motor.Cam.CameraShake(0.2f, 0.2f);

        // Section of the roll on the ground
        float recoveryTimer = stats.RecoveryTime;
        while (recoveryTimer > 0) {
            physics.Translate(motor.FacingVector * (stats.MoveSpeedWhileStriking * recoveryTimer));
            recoveryTimer -= Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        // Recovery
        activeMeleeCoroutine = null;
    }

    protected Vector2 ClampedMeleeAimVector() {
        Vector2 aimVec = input.GetMovementVector();
        

        if (aimVec.y < 0) {
            aimVec.y = 0;
        }
        
        // If no x was provided, default to facing vector
        if (aimVec.x == 0) {
            aimVec.x = motor.FacingVector.x;
        }

        if (motor.FacingVector.x > 0 && aimVec.x < 0) {
            aimVec.x = 0;
        }

        if (motor.FacingVector.x < 0 && aimVec.x > 0) {
            aimVec.x = 0;
        }

        animator.SetFloat("AimX", aimVec.x);
        animator.SetFloat("AimY", aimVec.y);

        Debug.Log(aimVec);

        return aimVec;
    }
}
