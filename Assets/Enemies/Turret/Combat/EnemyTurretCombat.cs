using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurretCombat : EnemyCombat {
    // -----
    // Adjustable fields
    // -----

    [Header("Ranged Attack")]

    [SerializeField] protected ProjectileAttackConfig rangedAttack;

    [SerializeField] protected float rangedCooldown = 2f;
    public float RangedCooldown { get { return rangedCooldown; } }
    
    [Header("Melee Attack")]

    [SerializeField] protected MeleeAttackConfig meleeAttack;

    // [SerializeField] protected float meleeCoodown = 2f;
    // public float MeleeCooldown { get { return meleeCoodown; } }

    // -----
    // Turret Combat state
    // -----

    protected IEnumerator attackCoroutine;

    // -----
    // Turret Combat API
    // -----

    public void RangedAttack(StateMachine frame) {
        if (!canAttack) {
            return;
        }

        StopAttackCoroutine();

        attackCoroutine = Ranged(frame);
        StartCoroutine(attackCoroutine);
    }

    public void MeleeAttack(StateMachine frame) {
        if (!canAttack) {
            return;
        }

        StopAttackCoroutine();

        attackCoroutine = Melee(frame);
        StartCoroutine(attackCoroutine);
    }

    // -----
    // Protected Methods
    // -----

    protected IEnumerator Ranged(StateMachine frame) {
        canAttack = false;

        EnemyStateMachine enemyFrame = (EnemyStateMachine) frame;
        Vector3 aimVector = (enemyFrame.view.Target.position - transform.position).normalized;

        rangedAttack.Attack(transform.position + new Vector3(0,0.5f,0) + (aimVector * 1), aimVector, Quaternion.identity);
        
        yield return new WaitForSeconds(rangedCooldown);

        canAttack = true;
    }

    protected IEnumerator Melee(StateMachine frame) {
        canAttack = false;

        EnemyStateMachine enemyFrame = (EnemyStateMachine) frame;

        // Windup attack
        float countdown = meleeAttack.WindupTime;
        while (countdown > 0) {
            countdown -= Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        // start animation here

        // Send out attack
        countdown = meleeAttack.SwingTime;
        while (countdown > 0) {
            countdown -= Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        // testing -- always facing right for now.
        meleeAttack.Attack(gameObject, transform.position, new Vector3(1, 0, 0));

        // Recover from attack
        countdown = meleeAttack.RecoveryTime;
        while (countdown > 0) {
            countdown -= Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        // yield return new WaitForSeconds(meleeCoodown);

        attackCoroutine = null;
        canAttack = true;
    }


    // Convenience method
    protected void StopAttackCoroutine() {
        if (attackCoroutine != null) {
            StopCoroutine(attackCoroutine);
        }
    }
}
