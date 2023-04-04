using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PhysicsCollisionDetection : MonoBehaviour
{
    // -----
    // Adjustable fields
    // -----

    [SerializeField] protected LayerMask walkableLayerMask;
    public LayerMask WalkableLayerMask { get { return walkableLayerMask; } }

    [Header("Vertical Collision")]
    [Range(3,10)]
    [SerializeField] protected int verticalCollisionResolution = 3;
    [SerializeField] protected float verticalCheckLength = 1.2f;
    [SerializeField] protected float verticalCenterPadding = 0.5f;

    [Header("Horizontal Collision")]
    [Range(3,10)]
    [SerializeField] protected int horizontalCollisionResolution = 3;
    [SerializeField] protected float horizontalCheckLength = 1.2f;
    [SerializeField] protected float horizontalCenterPadding = 0.5f;

    // if a surface normal is greater than this treat it as a wall
    [SerializeField] protected float maxSlopeAngle;

    [SerializeField] protected bool drawCollisionGizmos = false;

    // -----
    // Component references
    // -----

    private BoxCollider physicsCollider;

    // -----
    // Collision Detection state
    // -----

    protected List<RaycastHit> verticalHits;
    protected List<RaycastHit> horizontalHits;

    public bool CeilingCollisionsEnabled = true;
    public bool GroundCollisionsEnabled = true;

    public bool LeftCollisionsEnabled = true;
    public bool RightCollisionsEnabled = true;

    // [Header("Ground Vectors")]
    // [SerializeField] protected Vector2 groundNormalVector;
    // public Vector2 GroundNormalVector { get { return groundNormalVector; } }
    // [SerializeField] protected Vector2 groundFacingVector;
    // public Vector2 GroundFacingVector { get { return groundFacingVector; } }

    // -----
    // Unity Lifecycle methods
    // -----

    void Awake() {
        physicsCollider = this.GetComponent<BoxCollider>();
        verticalHits = new List<RaycastHit>();
        horizontalHits = new List<RaycastHit>();
    }

    void FixedUpdate() {
        CheckCollisions();
    }

    void OnEnable() {
        CeilingCollisionsEnabled = true;
        GroundCollisionsEnabled = true;
        LeftCollisionsEnabled = true;
        RightCollisionsEnabled = true;
    }

    void OnDisable() {
        CeilingCollisionsEnabled = false;
        GroundCollisionsEnabled = false;
        LeftCollisionsEnabled = false;
        RightCollisionsEnabled = false;
    }

    // -----
    // Events
    // -----

    public event Action<List<RaycastHit>> OnCeilingCollisionTrue;
    public event Action OnCeilingCollisionFalse;

    public event Action<List<RaycastHit>> OnGroundCollisionTrue;
    public event Action OnGroundCollisionFalse;

    public event Action<List<RaycastHit>> OnLeftCollisionTrue;
    public event Action OnLeftCollisionFalse;

    public event Action<List<RaycastHit>> OnRightCollisionTrue;
    public event Action OnRightCollisionFalse;

    // -----
    // Protected Methods
    // -----

    private void CheckCollisions() {
        if (CeilingCollisionsEnabled) {
            CheckCeilingCollision();
        }

        if (GroundCollisionsEnabled) {
            CheckGroundCollision();
        }

        if (LeftCollisionsEnabled) {
            CheckLeftCollision();
        }

        if (RightCollisionsEnabled) {
            CheckRightCollision();
        }
    }

    private void CheckCeilingCollision() {
        CheckVerticalCollision(true, OnCeilingCollisionTrue, OnCeilingCollisionFalse);
    }

    private void CheckGroundCollision() {
        CheckVerticalCollision(false, OnGroundCollisionTrue, OnGroundCollisionFalse);
    }

    private void CheckLeftCollision() {
        CheckHorizontalCollision(false, OnLeftCollisionTrue, OnLeftCollisionFalse);
    }

    private void CheckRightCollision() {
        CheckHorizontalCollision(true, OnRightCollisionTrue, OnRightCollisionFalse);
    }

    private void CheckVerticalCollision(bool facingUp, Action<List<RaycastHit>> OnCollisionTrue, Action OnCollisionFalse) {
        RaycastHit hitInfo;
        Vector3 direction = facingUp ? transform.up : -transform.up;
        float currentY = GetVerticalOffset(facingUp);

        verticalHits.Clear();

        for (int i = 0; i < verticalCollisionResolution; i++) {
            float currentX = GetVerticalCastX(i);
    
            if (Physics.Raycast(CreateCastOrigin(currentX, currentY), direction, out hitInfo, verticalCheckLength, walkableLayerMask)) { 
                verticalHits.Add(hitInfo);
            }
        }

        if (verticalHits.Count > 0) {
            OnCollisionTrue?.Invoke(verticalHits);
        } else {
            OnCollisionFalse?.Invoke();
        }
    }

    private void CheckHorizontalCollision(bool facingRight, Action<List<RaycastHit>> OnCollisionTrue, Action OnCollisionFalse) {
        RaycastHit hitInfo;
        Vector3 direction = facingRight ? transform.right : -transform.right;
        float currentX = GetHorizontalOffset(facingRight);

        horizontalHits.Clear();

        for (int i = 0; i < horizontalCollisionResolution; i++) {
            float currentY = GetHorizontalCastY(i);

            if (Physics.Raycast(CreateCastOrigin(currentX, currentY), direction, out hitInfo, horizontalCheckLength, walkableLayerMask)) {
                float wallAngle = Vector3.Angle(transform.up, hitInfo.normal);
                if (Mathf.Abs(wallAngle) > maxSlopeAngle) {
                    horizontalHits.Add(hitInfo);
                }
            }
        }

        if (horizontalHits.Count > 0) {
            OnCollisionTrue?.Invoke(horizontalHits);
        } else {
            OnCollisionFalse?.Invoke();
        }
    }

    private float GetVerticalOffset(bool facingUp) {
        float offset = facingUp ? verticalCenterPadding : -verticalCenterPadding;
        return physicsCollider.center.y + offset;
    }

    private float GetHorizontalOffset(bool facingRight) {
        float offset = facingRight ? horizontalCenterPadding : -horizontalCenterPadding;
        return physicsCollider.center.x + offset;
    }

    private float GetSizeBasedVerticalOffset(bool facingUp) {
        float offset = GetVerticalOffset(facingUp);
        float extent = physicsCollider.size.y / 2;

        return facingUp ? offset + extent : offset - extent;
    }

    private float GetSizeBasedHorizontalOffset(bool facingRight) {
        float offset = GetHorizontalOffset(facingRight);
        float extent = physicsCollider.size.x / 2;

        return facingRight ? offset + extent : offset - extent;
    }

    private float GetVerticalCastX(int interval) {
        float interpolation  = (float) interval / (float) (verticalCollisionResolution - 1);
        float xExtent = physicsCollider.size.x / 2;
        float startValue = physicsCollider.center.x - xExtent;
        float endValue = physicsCollider.center.x + xExtent;

        return Mathf.Lerp(startValue, endValue, interpolation);
    }

    private float GetHorizontalCastY(int interval) {
        float interpolation  = (float) interval / (float) (horizontalCollisionResolution - 1);
        float yExtent = physicsCollider.size.y / 2;
        float startValue = physicsCollider.center.y - yExtent;
        float endValue = physicsCollider.center.y + yExtent;

        return Mathf.Lerp(startValue, endValue, interpolation);
    }

    private Vector3 CreateCastOrigin(float x, float y, float z = 0) {
        return transform.position + new Vector3(x, y, z);
    }

    // -----
    // Debugging
    // -----

    private void OnDrawGizmosSelected() {
        if (physicsCollider != null) {
            if (drawCollisionGizmos) {
                if (CeilingCollisionsEnabled) {
                    DrawCeilingCollision();
                }

                if (GroundCollisionsEnabled) {
                    DrawGroundCollision();
                }

                if (LeftCollisionsEnabled) {
                    DrawLeftCollision();
                }

                if (RightCollisionsEnabled) {
                    DrawRightCollision();
                }
            }
        }
    }

    private void DrawCeilingCollision() {
        DrawVerticalCollision(true);
    }

    private void DrawGroundCollision() {
        DrawVerticalCollision(false);
    }

    private void DrawLeftCollision() {
        DrawHorizontalCollision(false);
    }

    private void DrawRightCollision() {
        DrawHorizontalCollision(true);
    }

    // Postive vertical direction being up; Negative being down.
    private void DrawVerticalCollision(bool facingUp) {
        RaycastHit hitInfo;
        Vector3 direction = facingUp ? transform.up : -transform.up;
        float currentY = GetVerticalOffset(facingUp);

        for (int i = 0; i < verticalCollisionResolution; i++) {
            float currentX = GetVerticalCastX(i);

            if (Physics.Raycast(CreateCastOrigin(currentX, currentY), direction, out hitInfo, verticalCheckLength, walkableLayerMask)) { 
                Debug.DrawLine(CreateCastOrigin(currentX, currentY), hitInfo.point, Color.green);
            } else {
                Debug.DrawRay(CreateCastOrigin(currentX, currentY), direction * verticalCheckLength, Color.red);
            }
        }
    }

    // Postive horizontal direction being right; Negative being left.
    private void DrawHorizontalCollision(bool facingRight) {
        RaycastHit hitInfo;
        Vector3 direction = facingRight ? transform.right : -transform.right;
        float currentX = GetHorizontalOffset(facingRight);

        for (int i = 0; i < horizontalCollisionResolution; i++) {
            float currentY = GetHorizontalCastY(i);

            if (Physics.Raycast(CreateCastOrigin(currentX, currentY), direction, out hitInfo, horizontalCheckLength, walkableLayerMask)) {
                float wallAngle = Vector3.Angle(transform.up, hitInfo.normal);
                if (Mathf.Abs(wallAngle) > maxSlopeAngle) {
                    Debug.DrawLine(CreateCastOrigin(currentX, currentY), hitInfo.point, Color.green);
                } else {
                    Debug.DrawLine(CreateCastOrigin(currentX, currentY), hitInfo.point, Color.yellow);
                }
            } else {
                Debug.DrawRay(CreateCastOrigin(currentX, currentY), direction * horizontalCheckLength, Color.red);
            }
        }
    }
}
