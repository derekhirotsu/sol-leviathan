using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpherePhysics : MonoBehaviour {
    // Limits our maximum travel to keep the complexity down.
    public float maxSpeed = 15f;

    public float radius = 0.5f;

    public Vector3 distanceToTravel;

    public Vector3 gravity;

    [SerializeField]
    InputActionAsset actionAsset;
    InputActionMap actions;
    Vector2 movementVector = new Vector2();

    void Awake() {
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

    public void OnMovement(InputAction.CallbackContext value) {
        movementVector = value.ReadValue<Vector2>();
    }

    void FixedUpdate() {
        // -----
        distanceToTravel = movementVector * maxSpeed;
        distanceToTravel += gravity;
        // -----

        Vector3 offset = distanceToTravel;
        Vector3 direction = offset.normalized;
        float maxDistance = Mathf.Min(offset.magnitude, maxSpeed * Time.fixedDeltaTime);

        Vector3 targetPosition = transform.position + direction * maxDistance;

        // Currently set to do 2 passes:
        // 1: Beeline toward the mouse until you hit something.
        // 2: Use any remaining movement to move perpendicular to the obstacle.
        // In my tests this was enough, but you can increase this
        // if you have more complex arrangements or a faster cursor.
        int limit = 2;
        for (int i = 0; i < limit; i++) {

            // Check for a collision in the direction we're trying to move.
            RaycastHit hit;
            if (Physics.SphereCast(transform.position, radius, direction, out hit, maxDistance)) {
                Debug.DrawRay(hit.point, hit.normal * 2f, Color.red);

                // Back up to closest non-intersecting point.
                // (Plus a small fudge factor for stability).
                offset = hit.point + (radius * hit.normal) - transform.position;
                float distance = Vector3.Dot(offset, direction) - 0.0001f;
                targetPosition = transform.position + direction * distance;

                transform.position = targetPosition;

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
            } else {
                // No collision! Complete the move.
                transform.position = targetPosition;
                return;                
            }
        }
    }
}
