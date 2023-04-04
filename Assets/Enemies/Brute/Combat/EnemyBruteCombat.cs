using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBruteCombat : EnemyCombat
{
    protected BruteStateCollection bruteStateCollection;
    public BruteStateCollection BruteStateCollection { get { return bruteStateCollection; } }
    public override void Awake() {
        base.Awake();
        bruteStateCollection = (BruteStateCollection) baseStateCollection;

    }

    [Header("Lightning Storm")]

    [SerializeField]
    protected int projectilesInStorm = 7;

    [SerializeField]
    protected ProjectileAttackConfig lightningAttack;

    [Header("Seeker")]

    [SerializeField]
    protected int projectilesInVolley = 14;

    [SerializeField]
    protected ProjectileAttackConfig seekerAttack;
    
    public void CastLightningStorm() {
        
        InterruptAttack();

        activeAttackCoroutine = LightningStormVolley();
        StartCoroutine(activeAttackCoroutine);

    }

    public void CastSeekerBarrage() {
        InterruptAttack();

        activeAttackCoroutine = SeekerBarrageVolley();
        StartCoroutine(activeAttackCoroutine);
    }

    protected IEnumerator LightningStormVolley() {
        int currentProjectile = 0;

        while (currentProjectile < projectilesInStorm) {

            currentProjectile++;

            // Calculate projectile pos
            Vector3 projectilePos = PlayerPosition;
            projectilePos.x += Random.Range(-5f, 5f);
            projectilePos.y += 25f;
            
            
            lightningAttack.Attack(projectilePos, -Vector3.up, Quaternion.identity);
            yield return new WaitForSeconds(lightningAttack.FireRate);
        }
    }

    protected IEnumerator SeekerBarrageVolley() {
        int currentProjectile = 0;

        while (currentProjectile < projectilesInVolley) {

            currentProjectile++;

            // Calculate projectile pos
            Vector3 projectilePos = this.transform.position + new Vector3(0f, 4f, 0f);
            float randomDeviation = Random.Range(-20f, 20f);

            Vector3 upwardDiagonalVector = new Vector2((projectilePos - PlayerPosition).normalized.x * 0.8f, 1).normalized;
            Vector3 projectileVector = FieldOfView.DirFromAngleXY(randomDeviation, upwardDiagonalVector);
            Vector3 facingDirection = stateMachine.animationManager.FacingVector;
            
            seekerAttack.Attack(projectilePos, projectileVector, Quaternion.identity);
            yield return new WaitForSeconds(seekerAttack.FireRate);
        }
    }
}
