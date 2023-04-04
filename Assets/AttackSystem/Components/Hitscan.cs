using UnityEngine;

public class Hitscan : MonoBehaviour {
    protected HitscanAttackConfig config;
    public void SetStats(HitscanAttackConfig newConfig) {
        config = newConfig;
        Destroy(this.gameObject, config.TimeToLive);
        
        configured = true;
    }
    protected bool configured = false;
    protected bool directionSet = false;
    protected bool destinationSet = false;

    Vector3 direction;
    public void SetDirection(Vector3 newDirection) {
        lr.SetPosition(0, newDirection);
        direction = newDirection;
        directionSet = true;
    }
    Vector3 destination;
    public void SetDestination(Vector3 newDestination) {
        lr.SetPosition(2, newDestination);
        lr.SetPosition(1, direction + 0.75f * (newDestination - direction));
        destination = newDestination;
        destinationSet = true;
    }

    LineRenderer lr;

    Vector3 point0;

    void Awake() {
        lr = GetComponent<LineRenderer>();
        // Debug.Log("lr " + lr);
        // point0 = this.transform.position;
    }

    void FixedUpdate() {
        if (!configured) {
            return;
        }
    }

    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(destination, 0.3f);
    }
}
