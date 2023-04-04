using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnRegion : MonoBehaviour
{
    [SerializeField] protected ActiveSpawnRegions SpawnRegionLookup;
    protected BoxCollider regionBounds;

    public void Start() {
        regionBounds = this.GetComponent<BoxCollider>();
    }

    public Vector3 GetRandomPosInBounds() {
        Vector3 newRegionPoint = GetNavmeshPosition(regionBounds.size.magnitude);
        return newRegionPoint;
    }

    public Vector3 GetNavmeshPosition(float maxDistance, float leniency = 5, int areaMask = 1) {
 
        // Visualize the angle bounds
        float castAngle = 360;

        Vector3 viewAngleA = FieldOfView.DirFromAngleXY(-castAngle/2, Vector2.up);
        Vector3 viewAngleB = FieldOfView.DirFromAngleXY(castAngle/2, Vector2.up);

        Debug.DrawLine(this.transform.position, this.transform.position + viewAngleA * maxDistance, Color.cyan, 1f);
        Debug.DrawLine(this.transform.position, this.transform.position + viewAngleB * maxDistance, Color.cyan, 1f);

        // Setup the max iterations
        NavMeshHit navHit;
            
        // Get a random angle within the established bounds
        float randomAngle = Random.Range(-castAngle/2, castAngle/2);
        Vector3 randomAngleVector = FieldOfView.DirFromAngleXY(randomAngle, Vector2.up);

        // Starting from the minimum distance, get a random amount of padding, with the maxDistance as the ceiling.
        float randomEndRange = Random.Range(0, maxDistance);

        // The end of our ray, cast out from that angle, will be our testPosition we poll on the NavMesh.
        Vector3 testPosition = this.transform.position + randomAngleVector * randomEndRange;

        if (NavMesh.SamplePosition(testPosition, out navHit, leniency, areaMask) ) {
            
            Debug.DrawLine(this.transform.position, this.transform.position + randomAngleVector, Color.red, 1f);
            Debug.DrawLine(this.transform.position + randomAngleVector, navHit.position, Color.green, 1f);
            // Debug.DrawLine(startPosition + randomAngleVector * minDistance, startPosition + randomAngleVector * (minDistance + randomEndRange), Color.green, 1f);

            return navHit.position;

        } else {
            return transform.TransformPoint(regionBounds.center);
        }
        
    }

    void OnEnable() {
        if (SpawnRegionLookup != null) {
            SpawnRegionLookup.AddItem(this);
        }
    }

    void OnDisable() {
        if (SpawnRegionLookup != null) {
            SpawnRegionLookup.RemoveItem(this);
        }
    }
}
