using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySniperCombat : EnemyCombat
{
    protected SniperStateCollection sniperStateCollection;
    public SniperStateCollection SniperStateCollection { get { return sniperStateCollection; } }

    protected Vector3 targetPosition;
    public Vector3 TargetPosition { get { return targetPosition; } }

    [Header("Sniper Config")]
    [SerializeField] protected GameObject telegraphObject;
    // [SerializeField] protected config_MeleeStats sniperShot;
    [SerializeField]
    protected MeleeAttackConfig sniperShot;

    [SerializeField] protected float channelDuration = 1f;
    protected float chargeTime = 0;
    public bool FullyCharged { get { return chargeTime >= channelDuration; } }
    
    public override void Awake() {
        base.Awake();
        sniperStateCollection = (SniperStateCollection) baseStateCollection;

        // telegraphObject.SetActive(false);
    }

    public void DisplayTelegraph(bool display = true) {
        telegraphObject.SetActive(display);
    }

    public void AimAtPlayer() {
        Vector3 playerPos = ActivePlayer.Items[0].transform.position;
        
        Vector3 distanceVector = (targetPosition - this.transform.position);
        chargeTime += Time.fixedDeltaTime;

        if (chargeTime < channelDuration * 0.95f) {
            targetPosition = playerPos;
            Quaternion alignedRotation = Quaternion.FromToRotation(Vector3.up, distanceVector);
            telegraphObject.transform.rotation = alignedRotation;
        }

        telegraphObject.transform.localScale = new Vector3(telegraphObject.transform.localScale.x, distanceVector.sqrMagnitude, telegraphObject.transform.localScale.z);
        Debug.DrawLine(this.transform.position, playerPos, Color.green);
    }

    public override void InterruptAttack()
    {
        base.InterruptAttack();
        chargeTime = 0;
    }

    public void FireSniper() {
        InterruptAttack();

        activeAttackCoroutine = SniperAttack(sniperShot);
        StartCoroutine(activeAttackCoroutine);
        
    }

    public void MeleeAttack() {
        InterruptAttack();
    }

    protected IEnumerator SniperAttack(MeleeAttackConfig stats) {

        // Continue targetig for half the windup:
        float targetWindup = stats.WindupTime;
        while (targetWindup > 0) {
            // Vector3 playerPos = ActivePlayer.Items[0].transform.position;
            // targetPosition = playerPos;
            
            targetWindup -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        Vector2 strikeVector = (targetPosition - this.transform.position).normalized;

        Vector3 forwardOffset = strikeVector * 2;
        stats.Attack(this.gameObject, this.transform.position + forwardOffset, strikeVector);

        // Execution
        float recoveryTime = stats.SwingTime + stats.RecoveryTime;
        while (recoveryTime > 0) {
            recoveryTime -= Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        // Recovery
        activeAttackCoroutine = null;
    }
}
