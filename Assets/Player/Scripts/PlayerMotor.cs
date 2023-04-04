using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputBuffer))]
public class PlayerMotor : MonoBehaviour
{
    private Animator animator;
    private PhysicsModule physics;
    private PlayerCombat combat;
    private InputBuffer input;
    private Rigidbody rb;
    private PlayerAudio playerAudio;

    [SerializeField] private CameraController cam;
    public CameraController Cam { get { return cam; } }

    // Serialized Parameters
    [Header("Player Movement")]
    protected float timeInAir = 0f;
    protected float groundTimeThreshold = 0.1f;
    public bool PlayerIsOnGround { get { return timeInAir < groundTimeThreshold; } }
    protected Vector3 lastPosition;
    [Header("Motor Vectors")]
    [SerializeField] protected Vector2 deltaPosition;
    public Vector2 DeltaPosition { get { return deltaPosition; } }
    [SerializeField] protected Vector2 facingVector;
    public Vector2 FacingVector { get { return facingVector; } }
    [SerializeField] protected Vector2 aimVector;
    public Vector2 AimVector { get { return aimVector; } }

    // Speed Modifications
    [Header("Motor Config")]
    [SerializeField] protected float playerMoveSpeed = 14f;
    protected float MoveSpeed { get { return playerMoveSpeed * moveSpeedModifier; } }
    protected float moveSpeedModifier {
        get {
            float speedMod = 1f;
            foreach (float mod in speedModifications) {
                speedMod += mod;
            }

            return Mathf.Clamp(speedMod, 0.1f, Mathf.Infinity);
        }
    }
    List<float> speedModifications;
    public void ClearSpeedModifications() {
        speedModifications.Clear();
    }
    public void AddSpeedModifier(float modification) {
        speedModifications.Add(modification);
    }

    public void RemoveSpeedModifier(float modification) {
        speedModifications.Remove(modification);
    }

    [SerializeField] protected float dashForce = 4000;
    [SerializeField] protected float jumpForce = 3000;
    [SerializeField] protected float braceDragModifier = 8f;

    [Header("Roll Config")]
    [SerializeField] protected float rollTime = 0.3f;
    [SerializeField] protected float rollSpeed = 30f;
    [SerializeField] protected float rollRecoveryDampen = 3f;
    protected bool rolling = false;
    protected bool canRoll = true;
    public bool CanRoll {
        get {
            Vector2 move = new Vector2(input.GetMovementVector().x, 0);
            return canRoll && move.magnitude > Mathf.Epsilon;
        }

        set {
            canRoll = value;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        speedModifications = new List<float>();

        animator = this.GetComponent<Animator>();
        physics = this.GetComponent<PhysicsModule>();
        input = this.GetComponent<InputBuffer>();
        combat = this.GetComponent<PlayerCombat>();
        rb = this.GetComponent<Rigidbody>();
        playerAudio = GetComponent<PlayerAudio>();

        animator.SetFloat("FacingDirectionX", 1f);

        // Update Delta movement
        lastPosition = this.transform.position;
        animator.SetBool("FacingRight", true);

        // Set weapon objects
        combat.DrawGun();

        facingVector = transform.right;
    }

    // Update is called once per frame, good for inputs
    void Update()
    {
        AnimateMovement();
    }

    void FixedUpdate() {
        UpdateDeltaPosition();

        UpdateGroundedState();
    }

    public void Move() {
        Vector2 currentMoveVector = input.GetMovementVector();
        
        if (currentMoveVector.x > 0) {
            Vector2 rightMoveVector = physics.GroundFacingVector * currentMoveVector.x;
            if (!physics.WallRight) {
                physics.Translate(rightMoveVector * MoveSpeed);
            }
        } else if (currentMoveVector.x < 0) {
            Vector2 leftMoveVector = -physics.GroundFacingVector * -currentMoveVector.x;
            if (!physics.WallLeft) {
                physics.Translate(leftMoveVector * MoveSpeed);
            }
        }

    }

    protected void UpdateDeltaPosition() {
        
        deltaPosition = (this.transform.position - lastPosition);
        animator.SetFloat("DeltaPosX", deltaPosition.x);
        animator.SetFloat("DeltaPosY", deltaPosition.y);

        lastPosition = this.transform.position;
    }


    // Manage axis movement Input (wasd & mouse)
    protected void AnimateMovement() {
        if (physics.Grounded) {
            AnimateGroundedMovement();
            return;
        }

        float moveSpeedX = (physics.SummedVelocity.x / playerMoveSpeed);

        animator.SetFloat("MoveX", moveSpeedX);
        animator.SetBool("Moving", moveSpeedX > Mathf.Epsilon || moveSpeedX < -Mathf.Epsilon);
    }

    protected void AnimateGroundedMovement() {
        Vector2 currentMoveVector = input.GetMovementVector();
        float moveSpeedX = currentMoveVector.x * moveSpeedModifier;

        animator.SetFloat("MoveX", moveSpeedX);
        animator.SetBool("Moving", moveSpeedX > Mathf.Epsilon || moveSpeedX < -Mathf.Epsilon);
    }

    public void FaceBasedOnVector(Vector2 vector) {
        if (vector.x > Mathf.Epsilon || vector.x < -Mathf.Epsilon) {
            if (vector.x > 0.4f) {
                facingVector = transform.right;
                animator.SetBool("FacingRight", true);
            } else if (vector.x < -0.4f) {
                facingVector = -transform.right;
                animator.SetBool("FacingRight", false);
            }

            // Debug.Log("facing vector " + facingVector);

            animator.SetFloat("FacingDirectionX", facingVector.x);
        }
    }

    public void FaceBasedOnMovement() {
        Vector2 move = input.GetMovementVector();

        FaceBasedOnVector(move);

    }

    public void FaceBasedOnAim() {
        Vector2 aim = input.GetRotationVector().normalized;

        FaceBasedOnVector(aim);

        if (aim.magnitude > 0) {
            aimVector = aim;
        } else {
            aimVector = facingVector;
        }

        animator.SetFloat("AimX", aimVector.x);
        animator.SetFloat("AimY", aimVector.y);

        // Debug.Log("aim vector " + aimVector);

        animator.SetBool("FacingRight", (aimVector.x >= 0));

        Debug.DrawRay(this.transform.position + new Vector3(0, 0.5f, 0), aim * 2f, Color.red);
    }

    protected void UpdateGroundedState() {
        if (!physics.Grounded) {
            timeInAir += Time.fixedDeltaTime;
        } else {
            timeInAir = 0f;
        }

        
        if (PlayerIsOnGround) {
            animator.SetBool("Grounded", true);
        } else {
            animator.SetBool("Grounded", false);
        }
        
    }

    public void Jump() {
        ClearSpeedModifications();

        if (activeEvasionCoroutine != null) {
            StopCoroutine(activeEvasionCoroutine);
        }

        if (PlayerIsOnGround) {
            // Jump, not dash
            physics.AddForce(physics.JumpVector * (jumpForce*0.42f));
            playerAudio.PlayClipOneShot("jump");
            StartCoroutine(DisableGroundCheck());

            combat.DrawGun();

            InterruptJump();
            currentJumpCoroutine = ContinuousJumpForce();
            StartCoroutine(currentJumpCoroutine);
        }
    }

    protected IEnumerator DisableGroundCheck() {
        float time = 0.05f;
        while (time > 0) {
            timeInAir = groundTimeThreshold;
            time -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

    }
    protected IEnumerator currentJumpCoroutine;
    public void InterruptJump() {
        if (currentJumpCoroutine != null) {
            StopCoroutine(currentJumpCoroutine);
        }
        currentJumpCoroutine = null;
    }

    protected IEnumerator ContinuousJumpForce() {
        float maxTime = 0.5f; // 0.5f

        float curTime = maxTime;
        while (input.ActionHeld(InputName.Jump) && curTime > 0 && !physics.CeilingAbove) {
            physics.AddForce(physics.JumpVector * jumpForce * 0.06f * curTime / maxTime); // 0.06
            // physics.Translate(physics.JumpVector * (30 * (curTime / maxTime)));

            curTime -= Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }
        currentJumpCoroutine = null;
    }

    protected IEnumerator activeEvasionCoroutine;
    public void Roll() {
        if (activeEvasionCoroutine != null) {
            StopCoroutine(activeEvasionCoroutine);
        }

        activeEvasionCoroutine = ExecuteDodgeRoll();
        StartCoroutine(activeEvasionCoroutine);
    }

    protected IEnumerator ExecuteDodgeRoll() {

        Vector3 rollVector;

        Vector2 move = input.GetMovementVector();
        
        FaceBasedOnMovement();
        animator.Play("Roll");
        playerAudio.PlayClipOneShot("roll");
        rolling = true;
        
        if (move.x > 0) {
            rollVector = transform.right;
        } else if (move.x < 0) {
            rollVector = -transform.right;
        } else {
            rollVector = -facingVector;
        }

        // Section of the roll in the air
        float rollInAirTimer = rollTime * 0.5f;
        physics.AddForce(this.transform.up * 1200);
        while (rollInAirTimer > 0) {
            physics.Translate(rollVector * rollSpeed);
            rollInAirTimer -= Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        // Section of the roll on the ground
        float rollOnGroundTimer = rollTime * rollRecoveryDampen;
        while (rollOnGroundTimer > 0) {
            physics.Translate(rollVector * (rollSpeed * rollOnGroundTimer));
            rollOnGroundTimer -= Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        rolling = false;
        activeEvasionCoroutine = null;
    }

    // Make animator triggers not crap
    public IEnumerator AnimationTrigger(string name) {
        animator.SetBool(name, true);

        yield return new WaitForSeconds(0.1f);

        animator.SetBool(name, false);
    }
    
}
