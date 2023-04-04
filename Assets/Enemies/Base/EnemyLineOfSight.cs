using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyLineOfSight : MonoBehaviour {

    // -----
    // Entity Lookups
    // -----
    [SerializeField] protected EntityLookup ActivePlayers;

    // -----
    // Adjustable fields
    // -----
    [SerializeField] protected float viewRadius;
    public float ViewRadius { get { return viewRadius; } }

    [SerializeField] protected float findTargetInterval;
    public float FindTargetInterval { get { return findTargetInterval; } }

    [SerializeField] protected float lineOfSightInterval;
    public float LineOfSightInterval { get { return lineOfSightInterval; } }

    [SerializeField] protected LayerMask targetMask;
    public LayerMask TargetMask { get { return targetMask; } }
    
    [SerializeField] protected LayerMask obstacleMask;
    public LayerMask ObstacleMask { get { return obstacleMask; } }

    // -----
    // Line Of Sight state
    // -----

    protected Transform target;
    public Transform Target { get { return target; } }

    protected bool hasLineOfSight;
    public bool HasLineOfSight { get { return hasLineOfSight; } }

    protected IEnumerator getTargetCoroutine;
    protected IEnumerator getLineOfSightCoroutine;

    // -----
    // Events
    // -----

    // public event Action OnTargetFound;

    // -----
    // Unity Lifecycle methods
    // -----

    void OnEnable() {
        if (target == null) {
            StartSearchingForTarget();
        } else {
            StartCheckingLineOfSight();
        }
    }

    void OnDisable() {
        StopSearchingForTarget();
        StopCheckingLineOfSight();
    }

    // -----
    // Line Of Sight API
    // -----

    public void StartSearchingForTarget() {
        if (getTargetCoroutine != null) {
            StopCoroutine(getTargetCoroutine);
        }

        getTargetCoroutine = SearchForTarget();
        StartCoroutine(getTargetCoroutine);
    }

    public void StopSearchingForTarget() {
        if (getTargetCoroutine != null) {
            StopCoroutine(getTargetCoroutine);
        }
    }

    public void StartCheckingLineOfSight() {
        if (getLineOfSightCoroutine != null) {
            StopCoroutine(getLineOfSightCoroutine);
        }

        getLineOfSightCoroutine = CheckLineOfSight();
        StartCoroutine(getLineOfSightCoroutine);
    }

    public void StopCheckingLineOfSight() {
        if (getLineOfSightCoroutine != null) {
            StopCoroutine(getLineOfSightCoroutine);
        }
    }

    public void SetTarget(Transform newTarget) {
        StopSearchingForTarget();
        StopCheckingLineOfSight();

        target = newTarget;
        StartCheckingLineOfSight();
    }

    public void SetTargetToPlayer() {
        if (ActivePlayers.Items.Count <= 0) {
            return;
        }
        
        StopSearchingForTarget();
        StopCheckingLineOfSight();

        target = ActivePlayers.Items[0].transform;
        StartCheckingLineOfSight();
    }

    // -----
    // Protected Methods
    // -----

    IEnumerator SearchForTarget() {
        while (target == null) {
            FindTarget();
            yield return new WaitForSeconds(findTargetInterval);
        }
    }

    IEnumerator CheckLineOfSight() {
        while (true) {
            GetLineOfSight();
            yield return new WaitForSeconds(lineOfSightInterval);
        }
    }


    // Used to find a valid target when enemy has no target.
    protected void FindTarget() {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        if (targetsInViewRadius.Length == 0) {
           return;
        }

        // For now, only player is being tracked so there will only ever be one entity found
        // and so we only ever need to get the first index.
        target = targetsInViewRadius[0].transform;

        // OnTargetFound?.Invoke();

        StartCheckingLineOfSight();
    }

    // Used to check line of sight to the enemy's target.
    protected void GetLineOfSight() {
        if (target == null) {
            return;
        }

        Vector3 origin = transform.position;
        Vector3 direction = (target.position - origin).normalized;
        float distance = (target.position - origin).magnitude;

        hasLineOfSight = !Physics.Raycast(origin, direction, distance, obstacleMask);
    }

    // -----
    // Debugging
    // -----

    void OnDrawGizmos() {
        if (target == null) {
            return;
        }

        Gizmos.color =  hasLineOfSight ? Color.blue : Color.grey;
        Gizmos.DrawLine(transform.position, target.position);
    }
}
