using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class PlayerRangedCombat : MonoBehaviour {
    [SerializeField]
    protected Transform aimTarget; // The "look at" target for aim IK

    [SerializeField]
    protected Transform camTarget;

    [SerializeField]
    protected Transform aimTransform; // The position of the Barrel
    protected AimIK aimIK;

    [Header("Blaster")]

    [SerializeField]
    protected HitscanAttackConfig blaster;

    [SerializeField]
    protected GameObject vfx_blasterMuzzleFlare;

    [Header("Missiles")]

    [SerializeField]
    protected ProjectileAttackConfig missiles;

    [SerializeField]
    protected GameObject vfx_missileMuzzleFlare;

    [SerializeField]
    protected ScriptableVariables.ScriptableVariableReference<int> missileAmmoCapacity;

    [SerializeField]
    protected ScriptableVariables.IntVariable currentMissileAmmo;

    [SerializeField]
    protected ScriptableVariables.ScriptableVariableReference<bool> missilesUnlocked;

    public bool MissilesAvailable { get { return currentMissileAmmo.Value > 0 && missilesUnlocked.Value; } }

    [Header("Air Shot")]

    [SerializeField]
    protected ProjectileAttackConfig airShot;

    [SerializeField]
    protected ScriptableVariables.ScriptableVariableReference<int> airShotCapacity;

    [SerializeField]
    protected ScriptableVariables.IntVariable currentAirShots;

    [SerializeField]
    protected float airShotBlastForce;

    [SerializeField]
    protected float airShotRechargeDelayTime = 0.5f;

    [SerializeField]
    protected float airShotAimInterruptTime = 1.5f;

    protected IEnumerator airShotRechargeCoroutine;

    [SerializeField]
    protected ScriptableVariables.ScriptableVariableReference<bool> airShotUnlocked;

    public bool AirShotAvailable { get { return currentAirShots.Value > 0 && airShotUnlocked.Value; } }

    public bool CanRechargeAirShot {
        get { return currentAirShots.Value < airShotCapacity && airShotRechargeCoroutine == null; }
    }
    
    [Header("Hover System")]

    [SerializeField]
    protected ScriptableVariables.ScriptableVariableReference<float> maxHoverTime;

    [SerializeField]
    protected ScriptableVariables.FloatVariable remainingHoverTime;

    [SerializeField]
    protected float hoverDecayRate = 1f;

    [SerializeField]
    protected float hoverRecoveryRate = 1f;

    protected IEnumerator hoverRechargeCoroutine;

    [SerializeField]
    protected ScriptableVariables.ScriptableVariableReference<bool> hoverUnlocked;

    public bool HoverAvailable { get { return remainingHoverTime.Value > 0 && hoverUnlocked.Value; } }

    public bool CanRechargeHover {
        get { return remainingHoverTime.Value < maxHoverTime && hoverRechargeCoroutine == null; }
    }

    [Header("Combat State")]

    [SerializeField]
    protected bool airAimAvailable = true;
    public bool AirAimAvailable { get { return airAimAvailable; } }

    public bool IsAirAimInterrupted {
        get { return !airAimAvailable && airAimInterruptCoroutine != null; }
    }

    // -----
    // Internal state
    // -----

    protected IEnumerator airAimInterruptCoroutine;
    
    protected Vector2 aimVector;

    protected float firingDelay = 0.0f;

    protected IEnumerator activeFiringCoroutine;
    public bool CurrentlyFiring { get { return activeFiringCoroutine != null; } }

    // -----
    // Component References
    // -----

    private PlayerMotor motor;
    private InputBuffer input;
    private PhysicsModule physics;
    private PlayerAudio playerAudio;

    // -----
    // Unity Lifecycle Methods
    // -----

    void Start() {
        motor = this.GetComponent<PlayerMotor>();
        input = this.GetComponent<InputBuffer>();
        physics = this.GetComponent<PhysicsModule>();        
        playerAudio = GetComponent<PlayerAudio>();
        aimIK = this.GetComponent<AimIK>();
        aimTarget.gameObject.SetActive(false);
    }

    void Update() {
        if (firingDelay > 0) {
            firingDelay -= Time.deltaTime;
        }
    }

    void FixedUpdate() {
        // Interpolate the Camera Target towards the AimTarget while it is too far away
        UpdateCamTarget();
    }

    void OnEnable() {
        missileAmmoCapacity.Subscribe(OnMissileAmmoCapacityChange);
        airShotCapacity.Subscribe(OnMaxAirShotsChange);
        maxHoverTime.Subscribe(OnMaxHoverTimeChange);
    }

    void OnDisable() {
        missileAmmoCapacity.Unsubscribe(OnMissileAmmoCapacityChange);
        airShotCapacity.Unsubscribe(OnMaxAirShotsChange);
        maxHoverTime.Unsubscribe(OnMaxHoverTimeChange);
    }

    // -----
    // Blaster
    // -----

    public void FirePrimaryWeapon() {

        if (activeFiringCoroutine != null) {
            StopCoroutine(activeFiringCoroutine);
        }

        activeFiringCoroutine = FireWeaponHitscan(blaster);
        StartCoroutine(activeFiringCoroutine);
    }

    // -----
    // Missiles
    // -----

    public void FireMissile() {

        if (activeFiringCoroutine != null) {
            StopCoroutine(activeFiringCoroutine);
        }

        activeFiringCoroutine = FireWeaponProjectile(missiles);
        StartCoroutine(activeFiringCoroutine);

    }

    protected void OnMissileAmmoCapacityChange(int newValue) {
        SetCurrentMissileValue(newValue);
    }

    protected void SetCurrentMissileValue(int newValue) {
        currentMissileAmmo.SetValue(newValue);
    }

    protected void IncrementCurrentMissileValue(int amount) {
        currentMissileAmmo.ApplyChange(amount);
    }

    // -----
    // Air Shot
    // -----

    public void StartAirShotRecharge() {
        airShotRechargeCoroutine = AirShotRecharge();
        StartCoroutine(airShotRechargeCoroutine);
    }

    protected IEnumerator AirShotRecharge() {
        yield return new WaitForSeconds(airShotRechargeDelayTime);

        SetCurrentAirShotsValue(airShotCapacity);
        airShotRechargeCoroutine = null;
    }

    public void RestoreAirAim() {
        StopCoroutine(airAimInterruptCoroutine);
        airAimInterruptCoroutine = null;

        airAimAvailable = true;
    }

    protected IEnumerator InterruptAirAim() {
        airAimAvailable = false;

        yield return new WaitForSeconds(airShotAimInterruptTime);

        airAimAvailable = true;
        airAimInterruptCoroutine = null;
    }

    public void FireAerialWeapon() {

        if (activeFiringCoroutine != null) {
            StopCoroutine(activeFiringCoroutine);
        }

        firingDelay = 0f;

        // Note - reference to motor could be removed? Call this in aerial aiming state after FireAerialWeapon 
        motor.InterruptJump();

        IncrementCurrentAirShotsValue(-1);

        activeFiringCoroutine = FireWeaponProjectile(airShot);
        StartCoroutine(activeFiringCoroutine);

        physics.AddForce(-(aimVector * airShotBlastForce));

        if (airAimInterruptCoroutine != null) {
            StopCoroutine(airAimInterruptCoroutine);
        }

        airAimInterruptCoroutine = InterruptAirAim();
        StartCoroutine(airAimInterruptCoroutine);

        // Note - reference to motor could be removed? Call this in aerial aiming state after FireAerialWeapon 
        motor.Cam.Hitstop(0.07f);
    }

    protected void OnMaxAirShotsChange(int newValue) {
        SetCurrentAirShotsValue(newValue);
    }

    protected void SetCurrentAirShotsValue(int newValue) {
        currentAirShots.SetValue(newValue);
    }

    protected void IncrementCurrentAirShotsValue(int amount) {
        currentAirShots.ApplyChange(amount);
    }

    // -----
    // Hover
    // -----

    protected IEnumerator HoverRecharge() {
        while (remainingHoverTime.Value < maxHoverTime) {
            float nextIncrement = Time.deltaTime * hoverRecoveryRate;

            if (remainingHoverTime.Value + nextIncrement > maxHoverTime) {
                SetRemainingHoverTimeValue(maxHoverTime);
            } else {
                IncrementRemainingHoverTime(nextIncrement);
            }

            yield return new WaitForFixedUpdate();
        }

        hoverRechargeCoroutine = null;
    }

    public void StartHoverRecharge() {
        hoverRechargeCoroutine = HoverRecharge();
        StartCoroutine(hoverRechargeCoroutine);
    }

    public void UseHover() {
        if (hoverRechargeCoroutine != null) {
            StopCoroutine(hoverRechargeCoroutine);
            hoverRechargeCoroutine = null;
        }

        IncrementRemainingHoverTime(-Time.deltaTime * hoverDecayRate) ;
    }

    protected void OnMaxHoverTimeChange(float newValue) {
        SetRemainingHoverTimeValue(newValue);
    }

    protected void SetRemainingHoverTimeValue(float newValue) {
        remainingHoverTime.SetValue(newValue);
    }

    protected void IncrementRemainingHoverTime(float newValue) {
        remainingHoverTime.ApplyChange(newValue);
    }


    // -----
    // General
    // -----

    public void ToggleAimIK(bool enable) {
        aimIK.enabled = enable;
    }

    public void ToggleAimReticle(bool enable) {
        aimTarget.gameObject.SetActive(enable);
    }

    public void UpdateAimVector() {
        Vector2 aim = input.GetRotationVector().normalized;
        
        
        // Pass direction when firing? Motor has reference to aim and facing vectors from input as well.
        if (aim.magnitude > 0) {
            aimVector = aim;
        } else {
            aimVector = motor.FacingVector;
        }

        // Update the aim target. This will be used for an AimIK pass.
        UpdateAimTarget();
        
    }


    protected void UpdateAimTarget() {

        float minVecDistance = 2f;
        float maxVecDistance = 6f;

        Vector3 startingPoint = new Vector3(aimTransform.position.x, aimTransform.position.y, 0); // This is the same vector defined in FireWeaponProjectile()

        // Get a unit vector from analog or mouse input respectively.
        Vector2 aim = new Vector2();
        float distance = 0;

        if (input.GamepadInputActive) {
            aim = input.GetRotationVector();
            distance = Mathf.Clamp(aim.magnitude * maxVecDistance, minVecDistance, maxVecDistance);
        }

        else {
            Vector3 mousePos = input.GetMousePosition();
            Vector3 startPos = input.WorldVectorToScreenPos(startingPoint);

            Vector2 analogMouseVector = (mousePos - startPos) / 250;

            distance = Mathf.Clamp(analogMouseVector.magnitude * maxVecDistance, minVecDistance, maxVecDistance); 
        }
        
        // Clamp the distance between the lower and upper limit
        aimTarget.position = startingPoint + new Vector3((aimVector.x * distance), (aimVector.y * distance), 0);
    }

    protected void UpdateCamTarget() {
        // Move towards the aimTarget
        if (aimTarget.gameObject.activeInHierarchy) {

            camTarget.position = Vector3.Slerp(camTarget.position, aimTarget.position, 0.04f);
        }
        
        // Move towards the player's position
        else {
            camTarget.position = Vector3.Slerp(camTarget.position, this.transform.position, 0.02f);
        }
    }

    public void InterruptBlasterFire() {
        if (activeFiringCoroutine == null) {
            return;
        }
        
        StopCoroutine(activeFiringCoroutine);
        activeFiringCoroutine = null;
    }

    protected IEnumerator FireWeaponProjectile(ProjectileAttackConfig config) {
        UpdateAimVector();

        if (firingDelay <= 0.0f) {
            
            // 1. Fire
            Vector3 aimVector3 = new Vector3(aimVector.x, aimVector.y, 0);

            config.Attack(new Vector3(aimTransform.position.x, aimTransform.position.y, 0), aimVector3, Quaternion.identity);

            // 2. Big effects
            // Note - reference to motor could be removed? Player firing event?
            GameObject muzzle = Instantiate(vfx_missileMuzzleFlare, aimTransform.position, aimTransform.rotation);
            Destroy(muzzle, 0.7f);
            motor.Cam.CameraShake(0.1f, 0.1f);

            // 3. Set Firing Delay
            firingDelay = config.FireRate;

            // 4. Check missile consumption
            if (config.Equals(missiles)) {
                IncrementCurrentMissileValue(-1);
                playerAudio.PlayClipOneShot("missile_fire");
            }

            // This is really crusty; We probably need to rework this script to better support audio and other effects on a per attack basis.
            if (config.Equals(airShot)) {
                playerAudio.PlayClipOneShot("air_blast");
            }
        }

        yield return new WaitForFixedUpdate();
        activeFiringCoroutine = null;
    }

    protected IEnumerator FireWeaponHitscan(HitscanAttackConfig config) {
        UpdateAimVector();

        if (firingDelay <= 0.0f) {
            
            // 1. Fire
            Vector3 aimVector3 = new Vector3(aimVector.x, aimVector.y, 0);

            config.Attack(this.gameObject, new Vector3(aimTransform.position.x, aimTransform.position.y, 0), aimVector3);

            // 2. Big effects
            // Note - reference to motor could be removed? Player firing event? 
            GameObject muzzle = Instantiate(vfx_blasterMuzzleFlare, aimTransform.position, aimTransform.rotation * Quaternion.Euler(0, 90, 0));
            Destroy(muzzle, 0.7f);
            
            motor.Cam.CameraShake(0.1f, 0.1f);
            playerAudio.PlayClipOneShot("blaster_fire");

            // 3. Set Firing Delay
            firingDelay = config.FireRate;
        }

        yield return new WaitForFixedUpdate();
        activeFiringCoroutine = null;
    }
}
