using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingProjectile : Projectile {
    [SerializeField] protected EntityLookup ActivePlayer;
    [SerializeField] protected float delayBeforeSeeking = 0.8f;
    [SerializeField] protected float totalSeekingTime = 2f;
    [Range(0f, 1f)] [SerializeField] protected float seekingSpeedScalar = 0.4f;

    // ---
    // Component References
    // ---
    
    protected TrailRenderer trail;
    protected HitDetection.HealthController health;
    protected HitDetection.HitboxController hitboxController;

    public void Awake() {
        trail = this.GetComponentInChildren<TrailRenderer>();
        health = this.GetComponent<HitDetection.HealthController>();
        hitboxController = GetComponent<HitDetection.HitboxController>();

        trail.enabled = false;
    }

    void Start() {
        StartCoroutine(SpawnInvulnerablilty());
    }

    protected IEnumerator SpawnInvulnerablilty() {
        hitboxController.SetHitboxActive("main", false);
        yield return new WaitForSeconds(delayBeforeSeeking);
        hitboxController.SetHitboxActive("main", true);
    }

    protected float seekingTime = 0f;
    public override void FixedUpdate() {
        if (!configured) {
            return;
        }

        seekingTime += Time.fixedDeltaTime;

        if (delayBeforeSeeking > 0) {
            this.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            delayBeforeSeeking -= Time.fixedDeltaTime;
        } else {
            this.transform.localScale = new Vector3(1f, 1f, 1f);
            trail.enabled = true;
        }

        float projectileSpeed = config.ProjectileSpeed;

        if (delayBeforeSeeking <= 0 && seekingTime < totalSeekingTime) {
            Vector3 projectileToPlayer = ActivePlayer.Items[0].transform.position - this.transform.position;

            Vector3 newDirVector = Vector3.Slerp(direction, projectileToPlayer, 0.12f);

            direction = newDirVector;
            projectileSpeed *= seekingSpeedScalar;
        }

        this.transform.position += (direction.normalized * projectileSpeed * Time.fixedDeltaTime);
    }

    protected override void HandleTargetCollision(Collider entity) {
        if (delayBeforeSeeking > 0) {
            return;
        }

        base.HandleTargetCollision(entity);
    }

    // -----
    // Event Callbacks
    // -----

    protected void OnHitboxHit(HitDetection.HitInfo info) {
        health.TakeDamage(info);
    }

    public virtual void HandleTakeDamage(HitDetection.HitInfo info) { }

    public virtual void HandleHealthDepleted() {
        Destroy(this.gameObject);
    }

    // -----
    // Event Subscriptions
    // -----
    public virtual void OnEnable() {
        health.OnTakeDamage += HandleTakeDamage;
        health.OnHealthDepleted += HandleHealthDepleted;
        hitboxController.OnHitboxHit += OnHitboxHit;
    }

    public virtual void OnDisable() {
        health.OnTakeDamage -= HandleTakeDamage;
        health.OnHealthDepleted -= HandleHealthDepleted;
        hitboxController.OnHitboxHit -= OnHitboxHit;
    }
}
