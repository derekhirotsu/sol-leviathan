using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CollisionFlags {
public static string Grounded { get { return "GROUND_BELOW"; } }
    
    // WALL_LEFT,
    // WALL_RIGHT,
    // CEILING_ABOVE
}

// Requirements
// [RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(PhysicsCollisionDetection))]
public class PhysicsModule : MonoBehaviour
{
    // Component references
    private PhysicsCollisionDetection collision;

    [Header("Ground Vectors")]
    [SerializeField] protected Vector2 groundNormalVector;
    public Vector2 GroundNormalVector { get { return groundNormalVector; } }
    [SerializeField] protected Vector3 groundFacingVector;
    public Vector2 GroundFacingVector { get { return groundFacingVector; } }
    // public Vector3 wallNormalVector;
    // public float wallAngle;

    [Header("Collision Tuning")]
    // Tunable fields
    [SerializeField] protected float mass = 4f; // Base 1
    [SerializeField] protected float landingDampen = 0.5f; // Base 0.5f, how much velocity is dampened on collision enter;
    [SerializeField] float velocityDampenThreshold = 1f; // How low velocity needs to be before being considered 0;

    [Header("Vertical Collision Flags")]
    [SerializeField] protected bool grounded = false;
    public bool Grounded { get { return grounded; } }
    [SerializeField] protected bool ceilingAbove = false;
    public bool CeilingAbove { get { return ceilingAbove; } }

    [Header("Horizontal Collision Flags")]
    [SerializeField] protected bool wallLeft = false;
    public bool WallLeft { get { return wallLeft; } }
    [SerializeField] protected bool wallRight = false;
    public bool WallRight { get { return wallRight; } }
    
    [Header("Gravity")]
    [SerializeField] protected float forceOfGravity = 200f; // Base 9.81
    protected float gravityMod = 1f;
    protected float ForceOfGravity { get { return forceOfGravity * gravityMod; } }
    [SerializeField] protected bool applyGravity = true;
    public bool ApplyGravity {
        get {
            return applyGravity;
        }
        
        set {
            applyGravity = value;
            currentPlatform = null;
        }
    }

    [Header("Grav ")]
    protected bool zeroGravityArea = false;
    protected Collider currentPlatform;
    protected Vector3 jumpVector = Vector3.zero;
    public Vector3 JumpVector { get { return jumpVector; } }
    
    [Header("Drag")]
    [SerializeField] protected float coefficientOfDrag = 1.2f; // Base 1.2
    [SerializeField] protected float maxVelocityMagnitude = 10f;
    protected float dragModifier = 1f;
    public void ModifyDrag(float modifier) {
        dragModifier += modifier;
    }

    [Header("Velocity Decay Scalars")]

    [SerializeField] protected float groundForceFriction = 0.9f;
    [SerializeField] protected float airForceDrag = 1f;

    protected float velocityExponent = 1;

    // === PHYSICS SYSTEM === 
    protected List<Vector3> forceVectorList;
    protected List<Vector3> translationVectorList;
    protected Vector3 velocityVector;  // [m s^-1]
    protected Vector3 netForceVector; // N [kg m s^-2]
    protected Vector3 netTranslationVector;
    public Vector3 SummedVelocity { get { return netForceVector + netTranslationVector; } }

    void Awake() {
        // Get components
        collision = GetComponent<PhysicsCollisionDetection>();
    }

    // Start is called before the first frame update
    void Start() {
        // Init collections
        forceVectorList = new List<Vector3>();
        translationVectorList = new List<Vector3>();
    }

    void OnEnable() {
        collision.OnCeilingCollisionTrue += HandleCeilingCollisionTrue;
        collision.OnCeilingCollisionFalse += HandleCeilingCollisionFalse;

        collision.OnGroundCollisionTrue += HandleGroundCollisionTrue;
        collision.OnGroundCollisionFalse += HandleGroundCollisionFalse;

        collision.OnLeftCollisionTrue += HandleLeftCollisionTrue;
        collision.OnLeftCollisionFalse += HandleLeftCollisionFalse;

        collision.OnRightCollisionTrue += HandleRightCollisionTrue;
        collision.OnRightCollisionFalse += HandleRightCollisionFalse;
    }

    void OnDisable() {
        collision.OnCeilingCollisionTrue -= HandleCeilingCollisionTrue;
        collision.OnCeilingCollisionFalse -= HandleCeilingCollisionFalse;

        collision.OnGroundCollisionTrue -= HandleGroundCollisionTrue;
        collision.OnGroundCollisionFalse -= HandleGroundCollisionFalse;

        collision.OnLeftCollisionTrue -= HandleLeftCollisionTrue;
        collision.OnLeftCollisionFalse -= HandleLeftCollisionFalse;

        collision.OnRightCollisionTrue -= HandleRightCollisionTrue;
        collision.OnRightCollisionFalse -= HandleRightCollisionFalse;
    }

    // FixedUpdate for physics calculations
    void FixedUpdate() {
        UpdateGravityForce();

        ApplyTranslationForces();

        ApplyExternalForces();

        ApplyDrag();

        ApplyWeightDecay();

        CapVelocity();
    }

    public void CapVelocity() {
        if (velocityVector.magnitude > maxVelocityMagnitude) {
            velocityVector = velocityVector.normalized * maxVelocityMagnitude;
        }
    }

    public void NeutralizeAllForce() {
        velocityVector = Vector3.zero;
    }

    public void NeutralizeVerticalForce() {
        if (velocityVector.y < 0) {
            velocityVector.y = 0;
        }       
    }

    public void AddForce(Vector3 forceVector)
	{
		forceVectorList.Add(forceVector);
	}

    public void Translate(Vector3 translationVector) {
        translationVectorList.Add(translationVector);
    }

    protected void UpdateGravityForce() {
        jumpVector = this.transform.up;

        if (applyGravity) {
            AddForce(-this.transform.up * ForceOfGravity);
        }
    }

    protected void HandleCeilingCollisionTrue(List<RaycastHit> hits) {
        ceilingAbove = true;
    }

    protected void HandleCeilingCollisionFalse() { ceilingAbove = false; }

    protected void HandleGroundCollisionTrue(List<RaycastHit> hits) {
        grounded = true; 
        RaycastHit hitInfo = GetShortestRaycastHit(hits);

        groundNormalVector = hitInfo.normal.normalized;
        groundFacingVector = -Vector2.Perpendicular(groundNormalVector).normalized;
        transform.position = new Vector3(transform.position.x, hitInfo.point.y + 1f, transform.position.z);

        // Testing handling sliding down slopes that are too steep to traverse normally
        // float angle = Vector3.Angle(transform.up, groundFacingVector);
        // Debug.Log(angle - 90);
        // Debug.Log(groundFacingVector);

        // if (Mathf.Abs(angle - 90f) > 45f) {
        //     // grounded = false;
        //     AddForce(groundNormalVector * forceOfGravity);
        // } else {
        //     transform.position = new Vector3(transform.position.x, useHit.point.y + 1f ,transform.position.z);
        // }

        // grounded = true;
        // groundNormalVector = hitInfo.normal.normalized;
        // groundFacingVector = -Vector2.Perpendicular(groundNormalVector).normalized;
    }

    protected void HandleGroundCollisionFalse() {
        grounded = false;
        groundNormalVector = this.transform.up;
        groundFacingVector = -Vector2.Perpendicular(groundNormalVector).normalized;
    }

    protected void HandleLeftCollisionTrue(List<RaycastHit> hits) { 
        wallLeft = true;
        // RaycastHit hitInfo = GetShortestRaycastHit(hits);
        // wallNormalVector = hitInfo.normal;
        // wallAngle = Vector3.Angle(transform.up, hitInfo.normal);
    }

    protected void HandleLeftCollisionFalse() { wallLeft = false; }

    protected void HandleRightCollisionTrue(List<RaycastHit> hits) { 
        wallRight = true;
        // RaycastHit hitInfo = GetShortestRaycastHit(hits);
        // wallNormalVector = hitInfo.normal;
        // wallAngle = Vector3.Angle(transform.up, hitInfo.normal);
    }

    protected void HandleRightCollisionFalse() { wallRight = false; }

    protected RaycastHit GetShortestRaycastHit(List<RaycastHit> hits) {
        float shortestDist = Mathf.Infinity;
        RaycastHit useHit = hits[0];
        foreach (RaycastHit hit in hits) {
            if (hit.distance < shortestDist) {
                shortestDist = hit.distance;
                useHit = hit;
            }
        }

        return useHit;
    }

    void ApplyExternalForces() {

		// Sum the forces and clear the list
		netForceVector = Vector3.zero;
		foreach (Vector3 forceVector in forceVectorList)
		{
			netForceVector = netForceVector + forceVector;
		}
		forceVectorList.Clear();

		// Calculate position change due to net force
		Vector3 accelerationVector = netForceVector / mass;
		velocityVector += accelerationVector * Time.fixedDeltaTime;

        CheckForceCollisions();
        DampenVelocityJitter();
        
		transform.position += velocityVector * Time.fixedDeltaTime;
        
	}

    void DampenVelocityJitter() {
        if (velocityVector.x < velocityDampenThreshold && velocityVector.x > -velocityDampenThreshold) {
            velocityVector.x = 0;
        }

        if (velocityVector.y < velocityDampenThreshold && velocityVector.y > -velocityDampenThreshold) {
            velocityVector.y = 0;
        }
    }

    void ApplyTranslationForces() {
        netTranslationVector = Vector3.zero;
        foreach(Vector3 translation in translationVectorList) {
            netTranslationVector = netTranslationVector + translation;
        }
        
        translationVectorList.Clear();

        CheckTranslationCollisions();

        if (netTranslationVector.x > 0 || netTranslationVector.x < 0) {
            velocityVector.x *= 0.95f;
        }

        this.transform.position += netTranslationVector * Time.fixedDeltaTime;

    }

    // This is called before net translation is applied.
    //      Here we should check for adjacent walls, and prevent movement in that direction using netTranslationVector.
    void CheckTranslationCollisions() {
        // if (grounded && netTranslationVector.y < 0) {
        //     netTranslationVector.y = 0;
        // }

        if (ceilingAbove && netTranslationVector.y > 0) {
            netTranslationVector.y = 0;
        }

        if (wallLeft && netTranslationVector.x < 0) {
            netTranslationVector.x = 0;
        }

        if (wallRight && netTranslationVector.x > 0) {
            netTranslationVector.x = 0;
        }
    }

    void CheckForceCollisions() {
        if (grounded && velocityVector.y < 0) {
            velocityVector.y = 0;
        }

        if (ceilingAbove && velocityVector.y > 0) {
            velocityVector.y = 0;
        }

        if (wallLeft && velocityVector.x < 0) {
            velocityVector.x = 0;
        }

        if (wallRight && velocityVector.x > 0) {
            velocityVector.x = 0;
        }
    }

    // void OnCollisionEnter(Collision collision)
    // {

    //     // Lets keep a tab on our last platform we landed on.
    //     if (currentPlatform == null || currentPlatform != collision.collider) {
    //         currentPlatform = collision.collider;
    //     }
        
    //     foreach (ContactPoint contact in collision.contacts) {
    //         Debug.DrawRay(contact.point, contact.normal, Color.red);

    //         Vector3 contactVector;
    //         // if (zeroGravityArea) {
    //         contactVector = (this.transform.position - contact.point).normalized;
    //         // } else { 
    //         //     contactVector = Vector3.up;
    //         // }

    //         // AlignPlayerWithVector(-contactVector);
    //         AddForce(contactVector * velocityVector.magnitude);
            
    //         // playerToPointOfCollisionVector = (contact.point - this.transform.position).normalized;
    //     }

    //     // Heavily dampen velocity on landing
    //     velocityVector *= landingDampen;
    //     netTranslationVector *= landingDampen;
        
    //     CheckForceCollisions();
    // }

    protected void AlignPlayerWithVector(Vector3 alignVector) {
        this.transform.rotation = Quaternion.LookRotation(alignVector) * Quaternion.Euler(-90, 0, 0);
    }

    protected void UpdateZeroGravity() {
        Debug.DrawRay(this.transform.position, this.transform.up * 2f, Color.red);
        Debug.DrawRay(this.transform.position, -this.transform.up * 2f, Color.magenta);

        Vector3 surfaceNormalVector = Vector3.zero;
        RaycastHit hit;

        // 1. Check to see if the player is grounded
        if (Physics.Raycast(this.transform.position, -this.transform.up, out hit, 3, collision.WalkableLayerMask)) {
            grounded = true;
            surfaceNormalVector = hit.normal;
            Debug.DrawRay(this.transform.position, surfaceNormalVector * 3f, Color.green);
            
        } else {
            grounded = false;
        }

        // if (applyGravity && !zeroGravityArea) {

        //     AlignPlayerWithVector(-surfaceNormalVector);
        //     AddForce(-surfaceNormalVector * forceOfGravity);

        //     jumpVector = Vector3.up;

        // } else
        if (applyGravity && currentPlatform != null) {
            // Rotate player to match the normal of the platform.
            Vector3 playerToPlatformVector = (currentPlatform.transform.position - this.transform.position);

            // This is determining the surface normal vector of the area below the player
            Debug.DrawRay(this.transform.position, playerToPlatformVector, Color.cyan);
            if (Physics.Raycast(this.transform.position, -this.transform.up, out hit, playerToPlatformVector.magnitude, collision.WalkableLayerMask)) {
                surfaceNormalVector = hit.normal;
                Debug.DrawRay(this.transform.position, surfaceNormalVector * playerToPlatformVector.magnitude, Color.green);
            } else if (Physics.Raycast(this.transform.position, playerToPlatformVector, out hit, playerToPlatformVector.magnitude, collision.WalkableLayerMask)) {
                surfaceNormalVector = hit.normal;
                Debug.DrawRay(this.transform.position, surfaceNormalVector * playerToPlatformVector.magnitude, Color.green);
            }

            jumpVector = surfaceNormalVector;
            
            AlignPlayerWithVector(-surfaceNormalVector);
            AddForce(-surfaceNormalVector * ForceOfGravity);
            
            // Apply opposing force to gravity (normal force) if grounded
            if (grounded) {
                AddForce(surfaceNormalVector * ForceOfGravity);
            }

        } else if (applyGravity && currentPlatform != null) {
            this.transform.rotation = Quaternion.identity;
        }
    }

    protected void ApplyDrag() {

        float currentVelocityMagnitude = velocityVector.magnitude;
        
	    // Fdrag = dragConstant * v^(velocityExponent)
		float forceOfDrag = (coefficientOfDrag * dragModifier) * Mathf.Pow(currentVelocityMagnitude, 1);

        Vector3 dragVector = forceOfDrag * -velocityVector.normalized;

        AddForce(dragVector);
    }


    protected void ApplyWeightDecay() {
        if (grounded) {
            velocityVector.x *= groundForceFriction;
        } else {
            velocityVector *= airForceDrag;
        }
    }

    private void DrawGroundVectors() {
        Debug.DrawRay(this.transform.position, groundNormalVector * 5f, Color.blue);
        Debug.DrawRay(this.transform.position, groundFacingVector * 5f, Color.blue);
    }

    private void OnDrawGizmosSelected() {
        DrawGroundVectors();
    }
}
