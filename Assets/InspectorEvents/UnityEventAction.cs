using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnityEventAction : EventAction
{
    public UnityEvent events;
    protected override void onActionCalled() {
        events?.Invoke();
    }
}
