using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestPhysics : MonoBehaviour {
    BoxCollider boxCollider;

    [SerializeField]
    Vector3 TestVelocity;

    RaycastHit hitInfo;
    bool didHit;
    Vector3 direction;
    float maxDistance;

    public float maxSpeed = 15f;

    void Awake() {
        boxCollider = GetComponent<BoxCollider>();
        actions = actionAsset.FindActionMap("Player");
        actions.Enable();      
    }

    void OnEnable() {
        actions["Move"].performed += OnMovement;
        actions["Move"].canceled += OnMovement;
    }

    void OnDisable() {
        actions["Move"].performed -= OnMovement;
        actions["Move"].canceled -= OnMovement;
    }

    public Vector3 distanceToTravel;

    [SerializeField]
    InputActionAsset actionAsset;
    InputActionMap actions;
    Vector2 movementVector = new Vector2();

    public void OnMovement(InputAction.CallbackContext value) {
        movementVector = value.ReadValue<Vector2>();
    }

    void FixedUpdate() {
        distanceToTravel = movementVector * maxSpeed;
        Vector3 offset = distanceToTravel;
        direction = offset.normalized;
        maxDistance = Mathf.Min(offset.magnitude, maxSpeed * Time.fixedDeltaTime);
        

        DistanceToBounds(direction);

        Vector3 targetPosition = transform.position + direction * maxDistance;
        int limit = 1;
        for (int i = 0; i < limit; i++) {

        didHit = Physics.BoxCast(transform.position, boxCollider.bounds.extents, direction, out hitInfo, transform.rotation, maxDistance);

        if (didHit) {


            offset = hitInfo.point + (DistanceToBounds(direction) * hitInfo.normal) - transform.position;
            // float newDistance = hitInfo.distance - DistanceToBounds(direction) - 0.001f;
            float newDistance = Vector3.Dot(offset, direction) - 0.001f;
            targetPosition = transform.position + direction * newDistance;
            
            if (i + 1 == limit) {
                return;
            }

            maxDistance = Mathf.Max(maxDistance - newDistance, 0f);

            offset = distanceToTravel;
            
            // Make sure we don't overshoot the closest point on this line
            // (prevents vibrating when close-but-not-quite).
            newDistance = offset.sqrMagnitude;
            float forbidden = Vector3.Dot(offset, hitInfo.normal);
            newDistance -= forbidden * forbidden - DistanceToBounds(hitInfo.normal);
            
            // Shouldn't happen with infinite-precision real numbers, 
            // but rounding errors happen in practice.
            if (newDistance < 0f) {
                return;
            }

            maxDistance = Mathf.Min(maxDistance, Mathf.Sqrt(newDistance));

            offset -= forbidden * hitInfo.normal;
            offset.z = 0f;
            direction = offset.normalized;

            targetPosition = transform.position + direction * maxDistance;
            Debug.DrawLine(transform.position, targetPosition, Color.black, 1.5f);
        } else {
            // No collision! Complete the move.
            transform.position = targetPosition;
            return;                
        }

        transform.position = targetPosition;
        }
    }

    float DistanceToBounds(Vector3 direction) {
        var absoluteDirection = new Vector3(Mathf.Abs(direction.x), Mathf.Abs(direction.y), 0).normalized;
        var boundsTemp = new Vector3(boxCollider.bounds.extents.x, boxCollider.bounds.extents.y, 0).normalized;
        var degreesFromUp = Vector3.Angle(Vector3.up, absoluteDirection);
        var degreesToBoundsCorner = Vector3.Angle(Vector3.up, boundsTemp);

        Debug.Log("degrees to corner of bounds: " + degreesToBoundsCorner);

        if (degreesFromUp == 0f) {
            Debug.Log("We are pointing directly up (or down). Use the height extents for distance: " + boxCollider.bounds.extents.y);
            return boxCollider.bounds.extents.y;
        } 

        if (degreesFromUp == 90f) {
            Debug.Log("We are pointing directly right (or left). Use the width extents for distance: " + boxCollider.bounds.extents.x);
            return boxCollider.bounds.extents.x;            
        }

        var angle = degreesFromUp;
        var adjacent = boxCollider.bounds.extents.y;
        if (degreesFromUp > degreesToBoundsCorner) {
            angle = 90 - degreesFromUp;
            adjacent = boxCollider.bounds.extents.x;
        }

        var hypotenuse = adjacent / Mathf.Cos(Mathf.Deg2Rad * angle);

        // Debug.DrawRay(transform.position, absoluteDirection * hypotenuse, Color.red);

        return hypotenuse;

        // var temp = new Vector3(Mathf.Abs(direction.x), Mathf.Abs(direction.y), 0).normalized;

        // Debug.DrawRay(transform.position, direction * direction.magnitude, Color.black);
        // Debug.DrawRay(transform.position, temp * direction.magnitude, Color.red);
        // Debug.DrawRay(transform.position, boxCollider.bounds.extents, Color.green);
        // // Debug.Log(boxCollider.bounds.extents);
        // var boundsTemp = new Vector3(boxCollider.bounds.extents.x, boxCollider.bounds.extents.y, 0).normalized;
        // var boundsProjection = Vector3.Dot(boxCollider.bounds.extents, temp);
        // Debug.Log(Vector3.Dot(temp, Vector3.up));
        // Debug.Log(Vector3.Angle(Vector3.up, temp));
        // Debug.DrawRay(transform.position, temp * boundsProjection, Color.cyan);
    }

    void OnDrawGizmos() {
        if (didHit) {
            Gizmos.DrawRay(transform.position, direction * hitInfo.distance);
            Gizmos.DrawWireCube(transform.position + direction * hitInfo.distance, boxCollider.size);
        } else {
            //Draw a Ray forward from GameObject toward the maximum distance
            Gizmos.DrawRay(transform.position, direction * maxDistance);
            //Draw a cube at the maximum distance
            Gizmos.DrawWireCube(transform.position + direction * maxDistance, boxCollider.size);
        }
    }
}
