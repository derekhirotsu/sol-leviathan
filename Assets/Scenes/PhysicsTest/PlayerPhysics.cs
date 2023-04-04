using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPhysics : MonoBehaviour {
    // Limits our maximum travel to keep the complexity down.
    public float maxSpeed = 15f;

    public float radius = 0.5f;

    public Vector3 distanceToTravel;

    public Vector3 gravity;

    [SerializeField]
    InputActionAsset actionAsset;
    InputActionMap actions;
    Vector2 movementVector = new Vector2();

    BoxCollider boxCollider;

    float scalarThing;

    void Awake() {
        boxCollider = GetComponent<BoxCollider>();
        scalarThing = Vector3.Dot(Vector3.right, boxCollider.bounds.extents);
        Debug.Log(scalarThing);
        actions = actionAsset.FindActionMap("Player");
        actions.Enable();        
        // Application.targetFrameRate = 15;
    }

    void OnEnable() {
        actions["Move"].performed += OnMovement;
        actions["Move"].canceled += OnMovement;
    }

    void OnDisable() {
        actions["Move"].performed -= OnMovement;
        actions["Move"].canceled -= OnMovement;
    }

    public void OnMovement(InputAction.CallbackContext value) {
        movementVector = value.ReadValue<Vector2>();
    }

    bool canmove = true;
    float maxDistance;
    RaycastHit hit;
    Vector3 direction;
    bool didHit;

    void FixederUpdate() {

    }

    void FixedUpdate() {
        if (!canmove) { return;} 
        // -----
        distanceToTravel = movementVector * maxSpeed;
        distanceToTravel += gravity;
        // -----

        Vector3 offset = distanceToTravel;
        direction = offset.normalized;
        maxDistance = Mathf.Min(offset.magnitude, maxSpeed * Time.fixedDeltaTime);

        Vector3 targetPosition = transform.position + direction * maxDistance;
        
        var test = Vector3.Dot(direction, boxCollider.bounds.extents.normalized);
        Debug.Log(test);
        Debug.DrawRay(transform.position, direction * test, Color.blue);
        // Currently set to do 2 passes:
        // 1: Beeline toward the mouse until you hit something.
        // 2: Use any remaining movement to move perpendicular to the obstacle.
        // In my tests this was enough, but you can increase this
        // if you have more complex arrangements or a faster cursor.
        int limit = 2;
        for (int i = 0; i < limit; i++) {

            // Check for a collision in the direction we're trying to move.
            
            // if (Physics.SphereCast(transform.position, radius, direction, out hit, maxDistance)) {
            didHit = Physics.SphereCast(transform.position, radius, direction, out hit, maxDistance);
            if (didHit) {
                // Debug.DrawLine(hit.point, transform.position, Color.red);
                var dist = DistanceToBounds(hit.normal);
                // Back up to closest non-intersecting point.
                // (Plus a small fudge factor for stability).
                offset = hit.point + (dist * 1.1f * hit.normal) - transform.position;
                // offset = hit.point + (radius * hit.normal) - transform.position;
                Debug.Log(radius * hit.normal);
                Debug.Log(dist * hit.normal);
                float distance = Vector3.Dot(offset, direction) - 0.001f;
                targetPosition = transform.position + direction * distance;
                
                transform.position = targetPosition;
                // canmove = false;    

                if (i + 1 == limit) {
                    return;
                }

                maxDistance = Mathf.Max(maxDistance - distance, 0f);

                offset = distanceToTravel;
                
                // Make sure we don't overshoot the closest point on this line
                // (prevents vibrating when close-but-not-quite).
                distance = offset.sqrMagnitude;
                float forbidden = Vector3.Dot(offset, hit.normal);
                distance -= forbidden * forbidden;
                Debug.DrawRay(hit.point, hit.normal * forbidden, Color.magenta);
                // Shouldn't happen with infinite-precision real numbers, 
                // but rounding errors happen in practice.
                if (distance < 0f) {
                    return;
                }

                maxDistance = Mathf.Min(maxDistance, Mathf.Sqrt(distance));

                offset -= forbidden * hit.normal;
                offset.z = 0f;
                direction = offset.normalized;

                targetPosition = transform.position + direction * maxDistance;
                Debug.DrawLine(transform.position, targetPosition, Color.black, 1.5f);
            } else {
                // No collision! Complete the move.
                transform.position = targetPosition;
                return;                
            }
        }
    } 

    public float DistanceToBounds(Vector3 direction) {
        var angle = Vector3.Angle(Vector3.up, direction);

        if (direction == Vector3.up || direction == Vector3.down) {
            Debug.Log("up/down contact; use box hight extents " + boxCollider.bounds.extents.y);
            Debug.Log("Angle - " + angle);
            Debug.DrawRay(transform.position, direction * boxCollider.bounds.extents.y, Color.red);
            return boxCollider.bounds.extents.y;
        } else if (direction == Vector3.left || direction == Vector3.right) {
            Debug.Log("left/right contact; use box hight extents " + boxCollider.bounds.extents.x);
            Debug.Log("Angle - " + angle);
            // Debug.DrawRay(transform.position, direction * boxCollider.bounds.extents.x, Color.red);
            return boxCollider.bounds.extents.x;
        }

        float sideLength = boxCollider.bounds.extents.y;
        
        if (angle < 45) {
            sideLength = boxCollider.bounds.extents.y;
        }
        if (angle >= 45 && angle < 135) {
            sideLength = boxCollider.bounds.extents.x;
        }
        
        
        // var shorterSide = Mathf.Min(boxCollider.bounds.extents.x, boxCollider.bounds.extents.y);
        // var hypotenuse = shorterSide / Mathf.Sin(Mathf.Deg2Rad * angle);
        Debug.Log("-----");
        Debug.Log("Angle - " + angle);
        Debug.Log("Sine - " + Mathf.Sin(Mathf.Deg2Rad * angle));
        var hypotenuse = sideLength / Mathf.Sin(Mathf.Deg2Rad * angle);
        Debug.Log("hypotenuse " + hypotenuse);
        Debug.DrawRay(transform.position, direction * hypotenuse, Color.red, 1.5f);
        return hypotenuse;

        // var angle = Vector3.Angle(Vector3.up, direction);
        // Debug.Log(direction);
        // Debug.Log(Mathf.Tan(Mathf.Deg2Rad * angle));
    }


    //Draw the BoxCast as a gizmo to show where it currently is testing. Click the Gizmos button to see this
    // void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;

    //     //Check if there has been a hit yet
    //     if (didHit)
    //     {
    //         //Draw a Ray forward from GameObject toward the hit
    //         Gizmos.DrawRay(transform.position, direction * hit.distance);
    //         //Draw a cube that extends to where the hit exists
    //         Gizmos.DrawWireCube(transform.position + direction * hit.distance, transform.localScale);
    //     }
    //     //If there hasn't been a hit yet, draw the ray at the maximum distance
    //     else
    //     {
    //         //Draw a Ray forward from GameObject toward the maximum distance
    //         Gizmos.DrawRay(transform.position, direction * maxDistance);
    //         //Draw a cube at the maximum distance
    //         Gizmos.DrawWireCube(transform.position + direction * maxDistance, transform.localScale);
    //     }
    // }
}
