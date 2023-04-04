using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPathing : MonoBehaviour {
    public List<Transform> Nodes;
    public float Speed = 1.0f;

    protected Transform NextNode;
    public bool Stopped = false;
    public int NextNodeIndex = 0;
    public float WaitAtNodeTime = 0f;

    // [SerializeField]
    // protected TargetPathingLookup activeMovingTargets;

    protected float remainingWaitTime;

    protected IEnumerator waitCoroutine;

    void Start() {
        if (Nodes.Count <= 1) {
            enabled = false;
            return;
        }

        transform.position = Nodes[NextNodeIndex].position;
        remainingWaitTime = WaitAtNodeTime;

        SetNextNode();
    }

    // void OnEnable() {
    //     if (activeMovingTargets != null) {
    //         activeMovingTargets.AddItem(this);
    //     }
    // }

    // void OnDisable() {
    //     if (activeMovingTargets != null) {
    //         activeMovingTargets.RemoveItem(this);
    //     }
    // }

    void Update() {
        if (Stopped) {
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, NextNode.position, Speed * Time.deltaTime);

        if (transform.position == NextNode.position) {
            waitCoroutine = WaitAtNode();
            StartCoroutine(waitCoroutine);
            SetNextNode();
        }
    }

    void SetNextNode() {
        if (NextNodeIndex >= Nodes.Count - 1) {
            NextNodeIndex = 0;
        } else {
            NextNodeIndex++;
        }

        NextNode = Nodes[NextNodeIndex];
    }

    IEnumerator WaitAtNode() {
        Stopped = true;

        while (remainingWaitTime > 0f) {
            remainingWaitTime -= Time.deltaTime;
            yield return null;
        }

        Stopped = false;

        remainingWaitTime = WaitAtNodeTime;
        waitCoroutine = null;
    }

    void OnDrawGizmos() {
        foreach (var node in Nodes) {

            if (node != null) {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(node.position, 1f);
            }
        }
    }

    public void StopMoving() {
        Stopped = true;

        if (waitCoroutine != null) {
            StopCoroutine(waitCoroutine);
        }
    }

    public void StartMoving() {
        if (waitCoroutine != null) {
            StartCoroutine(waitCoroutine);
        } else {
            Stopped = false;
        }
    }
} 
