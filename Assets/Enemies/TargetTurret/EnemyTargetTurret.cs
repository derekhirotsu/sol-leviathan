using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTargetTurret : MonoBehaviour
{
    // [SerializeField] Enemy
    [Header("Turret Config")]
    [SerializeField] EntityLookup ActivePlayer;
    [SerializeField] GameObject turretObject;
    [SerializeField] Transform targetReticle;
    [SerializeField] Transform endPoint;

    [Header("Attack Config")]
    [SerializeField] protected float trackingSpeed = 0.5f;
    [SerializeField] protected ProjectileAttackConfig turretAttack;

    protected FieldOfView fov;
    protected LineRenderer lr;
    protected HitDetection.HealthController health;
    protected HitDetection.HitboxController hitboxController;

    protected bool canAttack = false;

    Vector3 currentTarget;

    protected IEnumerator firingCoroutine;

    void Awake() {
        health = this.GetComponentInParent<HitDetection.HealthController>();
        hitboxController = this.GetComponentInParent<HitDetection.HitboxController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        fov = this.GetComponent<FieldOfView>();
        lr = targetReticle.GetComponent<LineRenderer>();

        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 0); // Set z-position to zero.
        currentTarget = ActivePlayer.Items[0].transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateTargetTracking();
    }

    void UpdateTargetTracking() {
        // Check validity of nearby targets.
        if (currentTarget != null) {

            if (canAttack == false && fov.VisibleTargets.Contains(ActivePlayer.Items[0].transform)) {
                canAttack = true;

                currentTarget = ActivePlayer.Items[0].transform.position;
            }

            else if (!fov.VisibleTargets.Contains(ActivePlayer.Items[0].transform)) {
                canAttack = false;
            }
            
        }


        // Turn towards target and fire laser pointer.
        if (canAttack) {
            // lr.enabled = true;
            endPoint.gameObject.SetActive(true);

            // Interpolate towards player direction.
            currentTarget = Vector3.Slerp(currentTarget, ActivePlayer.Items[0].transform.position, trackingSpeed);

            // Update the tracking reticle
            endPoint.position = currentTarget + new Vector3(0, 1, 0);

            turretObject.transform.LookAt(endPoint.position, Vector3.up);

            lr.positionCount = 2;

            lr.SetPosition(0, targetReticle.position);
            lr.SetPosition(1, endPoint.position + ( (endPoint.position - targetReticle.position).normalized * 30 ));


            // Begin Firing Coroutine if none exists.
            if (firingCoroutine == null) {
                firingCoroutine = FireProjectileInterval();

                StartCoroutine(firingCoroutine);
            }
            
        } else {
            lr.positionCount = 0;
            lr.enabled = false;
            endPoint.gameObject.SetActive(false);

            // Stop Firing coroutine.
            if (firingCoroutine != null) {
                StopCoroutine(firingCoroutine);
                firingCoroutine = null;
            }
        }
    }

    protected IEnumerator FireProjectileInterval() {

        yield return new WaitForSeconds(turretAttack.FireRate);
        turretAttack.Attack(targetReticle.position, (endPoint.position - targetReticle.position).normalized, Quaternion.identity);

        firingCoroutine = null;
    }

    // -----
    // Event Callbacks
    // -----

    public virtual void OnHealthDepleted() {
        Destroy(this.gameObject);
    }

    public virtual void OnTakeDamage(HitDetection.HitInfo info) {      

    }

    public virtual void OnHitboxHit(HitDetection.HitInfo info) {
        Debug.Log(info);
        health.TakeDamage(info);
    }

    // -----
    // Event Subscriptions
    // -----
    public virtual void OnEnable() {
        health.OnTakeDamage += OnTakeDamage;
        health.OnHealthDepleted += OnHealthDepleted;
        hitboxController.OnHitboxHit += OnHitboxHit;
    }

    public virtual void OnDisable() {
        health.OnTakeDamage -= OnTakeDamage;
        health.OnHealthDepleted -= OnHealthDepleted;
        hitboxController.OnHitboxHit -= OnHitboxHit;
    }
}
