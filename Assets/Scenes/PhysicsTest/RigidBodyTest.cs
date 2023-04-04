using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RigidBodyTest : MonoBehaviour {
    [SerializeField]
    InputActionAsset actionAsset;
    InputActionMap actions;
    Vector2 movementVector = new Vector2();

    public float speed;

    Rigidbody m_rigidbody;

    void Awake() {
        m_rigidbody = GetComponent<Rigidbody>();
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
        m_rigidbody.AddForce(new Vector3(movementVector.x, movementVector.y, 0) * speed);
    }

    void OnCollisionEnter(Collision collision) {
        Debug.Log(collision);
    }

}
