using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBound : MonoBehaviour {
    [SerializeField]
    Transform resetPosition;

    void OnTriggerEnter(Collider entity) {
        if (entity.gameObject.layer != LayerMask.NameToLayer("Player")) {
            return;
        }

        PhysicsModule physics = entity.gameObject.GetComponent<PhysicsModule>();

        if (physics != null) {
            physics.NeutralizeAllForce();
        }
        
        entity.transform.position = resetPosition.position;
    }
}
