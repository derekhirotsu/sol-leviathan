using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMotor : MonoBehaviour {
    // -----
    // Adjustable fields
    // -----

    public enum LocomotionType {
        GROUNDED,
        AERIAL
    }

    [SerializeField] protected LocomotionType locomotion;

    // -----
    // Adjustable fields
    // -----

    [SerializeField] protected StatProfile enemyStats;
    public float BaseMoveSpeed { get { return enemyStats.MoveSpeed; } }
    [SerializeField] protected float offmeshTraversalSpeed = 20f;
    [SerializeField] protected float forceThreshold = 5f;
    public float ForceThreshold { get { return forceThreshold; } }

    // -----
    // Component references
    // -----

    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public PhysicsModule physics;
    // protected HealthController health;

    // -----
    // Motor state
    // -----

    protected bool usingOffMeshLink = false;
    public bool UsingOffMeshLink { get { return usingOffMeshLink; } }

    protected bool usingPhysics = false;
    public bool UsingPhysics { get { return usingPhysics; } } 

    protected List<float> speedModifiers;

    protected float MoveSpeed { get { return enemyStats.MoveSpeed * moveSpeedModifier; } }
    protected float moveSpeedModifier {
        get {
            float speedMod = 1f;
            foreach (float mod in speedModifiers) {
                speedMod += mod;
            }

            return Mathf.Clamp(speedMod, 0.1f, Mathf.Infinity);
        }
    }

    IEnumerator offLinkCoroutine;
    protected IEnumerator forceTranslationCoroutine;
    
    // -----
    // Unity Lifecycle methods
    // -----

    void Awake() {
        physics = GetComponent<PhysicsModule>();
        agent = GetComponent<NavMeshAgent>();
        // health = GetComponent<HealthController>();

        // When damage is taken, offmeshlink traversal should be interrupted.
        // health.OnTakeDamage += InterruptOffMeshLink;

        speedModifiers = new List<float>();
        physics.ApplyGravity = false;
        agent.speed = MoveSpeed;

    }

    void FixedUpdate() {
        usingOffMeshLink = agent.isOnOffMeshLink;
        if (usingOffMeshLink && offLinkCoroutine == null) {
            // if (offLinkCoroutine != null) {
            //     StopCoroutine(offLinkCoroutine);
            // }
            offLinkCoroutine = TraverseOffMeshLink(agent);
            StartCoroutine(offLinkCoroutine);
        } else if (offLinkCoroutine == null) {

            DeterminePrimaryMotor();
            CheckIfShouldApplyGravity();
        }
    }

    // -----
    // Motor API
    // -----

    public void ClearSpeedModifications() {
        speedModifiers.Clear();
        agent.speed = MoveSpeed;
    }
    
    public void AddSpeedModifier(float modification) {
        speedModifiers.Add(modification);
        agent.speed = MoveSpeed;
    }

    public void RemoveSpeedModifier(float modification) {
        speedModifiers.Remove(modification);
        agent.speed = MoveSpeed;
    }

    public void MoveTo(Vector3 destination) {
        if (agent.enabled) {
            agent.destination = destination;
        }
    }
    
    // A combination of physics force and a decaying translation coroutine.
    public void ForceTranslation(Vector3 forceVector) {
        if (forceTranslationCoroutine != null) {
            StopCoroutine(forceTranslationCoroutine);
        }

        InterruptOffMeshLink();

        forceVector.y *= enemyStats.WeightMod;
        forceVector.y *= (1 + (1 - enemyStats.WeightMod));

        physics.AddForce(forceVector);
    }

    // Finds a valid navmesh position based on an angle and cast distance.
    // Used to find positions above or below a target with some inner padding.
    public Vector3 GetNavmeshPositionFromAngle(Vector3 startPosition, Vector2 castDirection, float castAngle, float maxDistance, float minDistance = 0, float leniency = 1, int areaMask = 1) {
        Vector3 navmeshPosition = startPosition;

        // Visualize the angle bounds
        Vector3 viewAngleA = FieldOfView.DirFromAngleXY(-castAngle/2, castDirection);
        Vector3 viewAngleB = FieldOfView.DirFromAngleXY(castAngle/2, castDirection);

        // Debug.DrawLine(startPosition, startPosition + viewAngleA * maxDistance, Color.cyan, 1f);
        // Debug.DrawLine(startPosition, startPosition + viewAngleB * maxDistance, Color.cyan, 1f);

        // Setup the max iterations
        int maxIterations = 20;
        int currentIterations = 0;

        List<Vector3> validHits = new List<Vector3>();

        NavMeshHit navHit;
        while (currentIterations < maxIterations && validHits.Count < maxIterations) {
            
            // Get a random angle within the established bounds
            float randomAngle = Random.Range(-castAngle/2, castAngle/2);
            Vector3 randomAngleVector = FieldOfView.DirFromAngleXY(randomAngle, castDirection);

            // Starting from the minimum distance, get a random amount of padding, with the maxDistance as the ceiling.
            float randomEndRange = Random.Range(0, maxDistance - minDistance);

            // The end of our ray, cast out from that angle, will be our testPosition we poll on the NavMesh.
            Vector3 testPosition = startPosition + randomAngleVector * (minDistance + randomEndRange);

            if (NavMesh.SamplePosition(testPosition, out navHit, leniency, areaMask) ) {
                
                // Debug.DrawLine(startPosition, startPosition + randomAngleVector * minDistance, Color.red, 1f);
                // Debug.DrawLine(startPosition + randomAngleVector * minDistance, navHit.position, Color.green, 1f);
                // Debug.DrawLine(startPosition + randomAngleVector * minDistance, startPosition + randomAngleVector * (minDistance + randomEndRange), Color.green, 1f);

                validHits.Add(navHit.position);

            }

            currentIterations++;
        }

        // Check if any valid points were found.
        if (validHits.Count > 0) {
            Debug.Log(validHits.Count + " valid points were found. One is chosen at random.");

            int randomPoint = Random.Range(0, validHits.Count);
            return validHits[randomPoint];

        } else {
            Debug.LogWarning(validHits.Count + " valid points were found by " + this.name + " . Returning startPosition.");

            return startPosition;
        }
        
    }

    // This maybe could be moved to an enemy-specific controller script?
    public Vector3 GetAmbushPosition(Vector3 targetPosition, float radius, float leniency, int areaLayer, float innerPadding = 0) {
        Vector3 ambushLocation = targetPosition;
        bool validPositionFound = false;

        int maxIterations = 10;
        int currentIterations = 0;

        NavMeshHit navHit;
        while (!validPositionFound && currentIterations < maxIterations) {
            float potentialX = targetPosition.x + Random.Range(-radius, radius);
            float potentialY = targetPosition.y + Random.Range(-radius, radius);

            Vector3 testPosition = new Vector3(potentialX, potentialY, targetPosition.z);

            if (innerPadding > 0 && Vector3.Distance(testPosition, targetPosition) < innerPadding) {
                currentIterations++;
                continue;
            }

            int areaMask = 1 << areaLayer;

            Debug.Log("areaLayer: " + areaLayer + ", areaMask: " + areaMask);

            if (NavMesh.SamplePosition(testPosition, out navHit, leniency, areaMask)) {
                ambushLocation = navHit.position;
                Debug.DrawLine(transform.position, ambushLocation, Color.magenta, 2.5f);
                validPositionFound = true;
            }

            currentIterations++;
        }

        if (validPositionFound) {
            return ambushLocation;
        }

        return targetPosition;
    }
    
    // -----
    // Protected Methods
    // -----

    // testing manually traversing offmesh links
    // this is necessary as we need to override physics-based translations
    // while traversing off mesh links.
    IEnumerator TraverseOffMeshLink(NavMeshAgent agent)
    {
        agent.enabled = true;
        usingPhysics = false;

        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
        while (!Utils.WithinRangeSqr(agent.transform.position, endPos, 0.5f))
        {
            agent.transform.position = Vector3.MoveTowards(agent.transform.position, endPos, offmeshTraversalSpeed * Time.deltaTime);
            yield return null;
        }

        usingOffMeshLink = false;
        agent.CompleteOffMeshLink();
        offLinkCoroutine = null;
    }

    protected void InterruptOffMeshLink() {
        if (offLinkCoroutine != null) {
            StopCoroutine(offLinkCoroutine);
        }

        usingOffMeshLink = false;
        offLinkCoroutine = null;
    }

    // This is useful, since gravity is applied constantly.
    // We can disable gravity when grounded, to better control our SummedVelocity
    protected void CheckIfShouldApplyGravity() {
        if (locomotion.Equals(LocomotionType.AERIAL)) {
            
            physics.ApplyGravity = false;
        
        } else if (locomotion.Equals(LocomotionType.GROUNDED)) {

            if (!physics.Grounded && usingPhysics) {
                physics.ApplyGravity = true;
            } else {
                physics.ApplyGravity = false;
            }
        }
    }

    // If the SummedVelocity drops below a certain threshold, we can NeutralizeForce,
    // Allowing the navmesh agent to take over as opposed to the Physics Module
    protected void DeterminePrimaryMotor() {
        
        // 
        // if (variableMotor) {

        // Here, we let navmesh take over.
        if (physics.SummedVelocity.magnitude <= forceThreshold && forceTranslationCoroutine == null) {
            physics.NeutralizeAllForce();
            agent.enabled = true;
            usingPhysics = false;
        
        // Here we're letting physics take over
        } else {

            agent.enabled = false;
            usingPhysics = true;
        }

        // } else {
        //     // physics.enabled = false;
        //     agent.enabled = true;
        //     usingPhysics = false;
        // }
    }

    protected IEnumerator TranslateOverTime(Vector3 direction, float forceSpeed, float duration) {
        float timer = duration;

        while (timer > 0) {

            if (usingPhysics) {
                Vector2 forceTranslation = direction * (forceSpeed * (timer / duration));
                physics.Translate(forceTranslation);
            }
            

            timer -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        forceTranslationCoroutine = null;
        
    }
}
