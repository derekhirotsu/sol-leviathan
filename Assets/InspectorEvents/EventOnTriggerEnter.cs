using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventOnTriggerEnter : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // trigger any actions attached to game object
        EventAction[] actions = GetComponents<EventAction>();
        for (int i = 0; i < actions.Length; i++) {
            actions[i].CallAction();
        }
    }
}
