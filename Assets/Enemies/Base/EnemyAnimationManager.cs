using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyAnimations {
    // Parameters
    public static string param_ActionTrigger { get { return "ActionTrigger"; } }
    public static string param_Grounded { get { return "Grounded"; } }
    public static string param_Moving { get { return "Moving"; } }

    // Animation States
    public static string anim_ActionEnter { get { return "_ActionEnter"; } }
    
}

public class EnemyAnimationManager : MonoBehaviour
{
    // ---
    // Component References
    // ---
    protected Animator animator;
    protected EnemyMotor motor;

    [Header("Model Config")]
    [SerializeField] protected GameObject enemyModel;
    [SerializeField] protected GameObject[] randomizedModels;

    [Header("Move Align Config")]
    [SerializeField] protected bool lockAlignment = false;
    [SerializeField] protected Vector2 alignVector;
    public Vector2 FacingVector { get { return alignVector; } }
    protected Vector2 deltaPosition;
    protected Vector3 lastPosition;
    protected float minimumMoveMagnitude = 0.15f;

    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
        motor = this.GetComponent<EnemyMotor>();

        if (randomizedModels.Length > 0) {
            int randomModelIndex = Random.Range(0, randomizedModels.Length);

            foreach (GameObject model in randomizedModels) {
                model.SetActive(false);
            }

            randomizedModels[randomModelIndex].SetActive(true);
            enemyModel = randomizedModels[randomModelIndex];
        }

        if (enemyModel == null) {
            Debug.LogWarning(this.name + " was not provided a model in it's Animation Manager. Animations will be disabled.");
            this.enabled = false;
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateDeltaPosition();
    
        AnimateLocomotion();

    }

    protected void UpdateDeltaPosition() {
        deltaPosition = (this.transform.position - lastPosition);

        lastPosition = this.transform.position;
    }

    protected void AnimateLocomotion() {
        if (animator == null) {
            return;
        }

        // Update facing direction based on new deltaPosition
        Vector2 newFacingDir = -deltaPosition.IsolateX().normalized;

        if (newFacingDir.x != 0 && newFacingDir.x != alignVector.x) {
            AlignModelWithVector(newFacingDir);
        }

        // Update animator parameters
        animator.SetFloat("DeltaPosX", deltaPosition.x);
        animator.SetFloat("DeltaPosY", deltaPosition.y);

        if (deltaPosition.x > minimumMoveMagnitude || deltaPosition.x < -minimumMoveMagnitude) {
            animator.SetBool(EnemyAnimations.param_Moving, true);
        } else {
            animator.SetBool(EnemyAnimations.param_Moving, false);
        }

        float curSpeed = deltaPosition.magnitude / Time.deltaTime;
        animator.SetFloat("Speed", curSpeed / motor.BaseMoveSpeed);

        if (motor.UsingOffMeshLink) {
            animator.SetBool(EnemyAnimations.param_Grounded, false);
        } else {
            animator.SetBool(EnemyAnimations.param_Grounded, true);
        }
    }

    public void LockAlignment() {
        lockAlignment = true;
    }

    public void AlignModelWithVector(Vector2 newAlignVector) {
        alignVector = newAlignVector;
        enemyModel.transform.rotation = Quaternion.LookRotation(-alignVector, Vector3.up);
    }

    public void UnlockAlignment() {
        lockAlignment = false;
    }

    // ---
    // Animation
    // ---
    protected IEnumerator actionTriggerCoroutine;
    public void ActionTrigger() {
        if (actionTriggerCoroutine != null) {
            StopCoroutine(actionTriggerCoroutine);
        }

        actionTriggerCoroutine = TriggerCoroutine(EnemyAnimations.param_ActionTrigger);
        StartCoroutine(actionTriggerCoroutine);
    }

    protected IEnumerator TriggerCoroutine(string boolName) {
        animator.SetBool(boolName, true);

        yield return new WaitForSeconds(0.1f);

        animator.SetBool(boolName, false);
    }

    public void PlayAnimation(string animationName) {
        if (animator == null) {
            return;
        }

        animator.Play(animationName);
    }
}
