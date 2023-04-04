using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EventAction : MonoBehaviour
{
    public bool oneShot = true;
    bool triggered = false;
    public void CallAction() {
        if (!oneShot || !triggered) {
            onActionCalled();
            triggered = true;
        }
    }

    protected abstract void onActionCalled();
}
