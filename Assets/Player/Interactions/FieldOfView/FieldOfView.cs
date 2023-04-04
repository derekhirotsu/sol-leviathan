using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [Header ("View Parameters")]
    [SerializeField] protected bool viewExpandsClockwise = false;
    public bool ViewExpandsClockwise { get { return viewExpandsClockwise; } }
    [SerializeField] protected float viewRadius;
    public float ViewRadius { get { return viewRadius; } set { viewRadius = value; } }

    [Range(0, 360)]
    [SerializeField] protected float viewAngle;
    public float ViewAngle { get { return viewAngle; } set { viewAngle = value; } }

    [Header("Mask Parameters")]
    [SerializeField] protected LayerMask targetMask;
    public LayerMask TargetMask { get { return targetMask; } set { targetMask = value; } }
    [SerializeField] protected LayerMask obstacleMask;

    [SerializeField] protected List<Transform> visibleTargets = new List<Transform>();
    public List<Transform> VisibleTargets { get { return visibleTargets; } }

    [Header("Rendering Parameters")]
    public MeshFilter viewMeshFilter;
    private Mesh viewMesh;
    [SerializeField] protected float meshResolution = 0.1f;

    void Awake() {
        
        if (viewMeshFilter != null) {
            viewMesh = new Mesh();
            viewMesh.name = "View Mesh";
            viewMeshFilter.mesh = viewMesh;
        }
        
    }

    void Update() {
        FindVisibleTargets();
    }

    // Generate a mesh to visualize FoV by casting rays at intervals defined by the meshResolution
    public void DrawFieldOfView() {
        if (viewMeshFilter == null) {
            return;
        }

        // Calculate the resolution and the size of each angle.
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / stepCount;

        // Keep track of a list of view points
        List<Vector3> viewPoints = new List<Vector3>();

        // Loop through each step count, cast a ray outward in that direction.
        for (int i = 0; i <= stepCount; ++i) {

            // Calculate the current angle being cast forward.
            float curAngle;
            if (!viewExpandsClockwise) {
                curAngle = transform.eulerAngles.y - viewAngle/2 + (stepAngleSize * i);
            } else {
                curAngle = transform.eulerAngles.y + (stepAngleSize * i);
            }
            
            // Debug.DrawLine(transform.position, transform.position + DirFromAngle(curAngle, true) * viewRadius, Color.red);

            // Construct a new ViewCastInfo to store the data found at the current angle.
            ViewCastInfo newViewCast = ViewCast(curAngle);
            viewPoints.Add(newViewCast.point);

        }

        // Construct a mesh from the collected Vector3 points.
        int vertexCount = viewPoints.Count + 1;

         // Must have at least 3 vertices to draw a Triangle.
        if (vertexCount >= 3) {
            Vector3[] vertices = new Vector3[vertexCount];
            int[] triangles = new int[(vertexCount-2) * 3];

            // Assign vertex positions. First vertex will be at Vector3.zero
            vertices[0] = Vector3.zero;
            for (int i = 0; i < vertexCount - 1; ++i) {
                vertices[i+1] = transform.InverseTransformPoint(viewPoints[i]);

                // Ensure the vertex counter remains in range.
                if (i < vertexCount - 2) {
                    triangles[i * 3] = 0;
                    triangles[i * 3 + 1] = i + 1;
                    triangles[i * 3 + 2] = i + 2;
                }
                
            }

            viewMesh.Clear();
            viewMesh.vertices = vertices;
            viewMesh.triangles = triangles;
            viewMesh.RecalculateNormals();
        }
    }

    public void ClearViewMesh() {
        viewMesh.Clear();
    }

    

    // Taking the visible targets of many FOV components and appending unique transforms into a single list
    public static List<Transform> AppendVisibleTargets(List<FieldOfView> fovList) {
        // Find the list index with the highest visible targets.
        foreach(FieldOfView fov in fovList) {
            fov.FindVisibleTargets();
        }

        List<Transform> largestList = fovList[0].VisibleTargets;

        for (int i = 1; i < fovList.Count; ++i) {
            largestList = largestList.Union<Transform>(fovList[i].VisibleTargets).ToList<Transform>();
        }
        
        return largestList;
    }

    public List<Transform> FindNearbyTargets(float distance, LayerMask layerMask) {
        List<Transform> nearbyTargets = new List<Transform>();

        UnityEngine.Collider[] targetsInViewRadius = Physics.OverlapSphere (transform.position, distance, layerMask);

        foreach(UnityEngine.Collider collider in targetsInViewRadius) {
            nearbyTargets.Add (collider.transform);
        }

        return nearbyTargets;
    }

    public void FindVisibleTargets() {
        // Clear the current list of visible targets. This is the simplest way to remove targets that leave the radius.
        visibleTargets.Clear();

        UnityEngine.Collider[] targetsInViewRadius = Physics.OverlapSphere (transform.position, viewRadius, targetMask);

        foreach(UnityEngine.Collider collider in targetsInViewRadius) {
            Transform target = collider.transform;

            // Debug.Log(target.name + " within range of " + this.name);

            if (WithinAngle(target.position, transform.forward, viewAngle, target.localScale.x)) {
                visibleTargets.Add (target);
            }
        }
    }

    public bool WithinAngle(Vector3 targetPosition, Vector3 angleDirection, float angleInDegrees = 0f, float colliderRadius = 0f) {
        Vector3 dirToTarget = (targetPosition - transform.position).normalized;

        // If not angle is provided, default to the viewing angle.
        if (angleInDegrees <= 0) {
            angleInDegrees = viewAngle;
        }

        // If the angle between the provided direction and target direction is within the supplied angle...
        float angleBetweenTarget = Mathf.FloorToInt(Vector3.Angle(angleDirection, dirToTarget));
        if (angleBetweenTarget <= (angleInDegrees / 2)) {

            // Get the distance between this position and the collider
            float distToTarget = Vector3.Distance ( transform.position, targetPosition );

            // Raycast towards the target and check for collisions. If none exist, target is within the angle.
            if (!Physics.Raycast (transform.position, dirToTarget, distToTarget, obstacleMask)) {
                return true;
            }

        } else {
            // Debug.Log(angleBetweenTarget + " is not less than " + (angleInDegrees / 2) );
        }

        return false;
    }

    public bool SourceWithinAngle(Vector3 sourcePosition, Vector3 targetPosition, Vector3 angleDirection, float angleInDegrees = 0f) {
        Vector3 dirToTarget = (targetPosition - sourcePosition).normalized;
        // If not angle is provided, default to the viewing angle.
        if (angleInDegrees <= 0) {
            angleInDegrees = viewAngle;
        }

        // If the angle between the provided direction and target direction is within the supplied angle...
        if (Vector3.Angle(angleDirection, dirToTarget) < angleInDegrees / 2) {

            // Get the distance between this position and the collider
            float distToTarget = Vector3.Distance ( sourcePosition, targetPosition );

            // Raycast towards the target and check for collisions.
            if (!Physics.Raycast (sourcePosition, dirToTarget, distToTarget, obstacleMask)) {
                return true;
            }
        }

        return false;
    }

    public bool WithinRadius(Vector3 targetPosition, float radius = 0f) {
        Vector3 dirToTarget = (targetPosition - transform.position).normalized;
        float distToTarget = Vector3.Distance ( transform.position, targetPosition );

        if (distToTarget <= radius && !Physics.Raycast (transform.position, dirToTarget, distToTarget, obstacleMask)) {
            return true;
        }

        return false;
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal) {
        if (!angleIsGlobal) {
            angleInDegrees += transform.eulerAngles.y;
        }
        
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), 0);
    }

    public static Vector3 DirFromAngleXY(float angleInDegrees, Vector2 castDirection) {
        // ---
        // TEMP FOR TESTING calculating angle from two vectors - should be ~ -90 degrees
        // ---

        // Vector2 testDirA = new Vector2(0.74f, 0.74f).normalized;
        // Vector2 testDirB = new Vector2(-0.74f, -0.74f).normalized;
        
        // float calcCastDirection = AngleBetweenInDegrees(testDirA, testDirB);
        // Debug.Log("DirFromAngleXY : The direction between " + testDirA + " and " + testDirB + " is " + calcCastDirection);

        // Calculate the Vector given an angle and it's direction.
        float castAngle = AngleFromVector(castDirection);

        angleInDegrees += castAngle;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), 0);
    }

    public static float AngleFromVector(Vector2 vector) {
        float angle = AngleBetweenInDegrees(Vector2.up, vector);

        if (vector.x < 0) {
            angle = 180 - angle;
            angle *= -1f;
        }

        return angle; 
    }

    public static float AngleBetweenInDegrees(Vector3 a, Vector3 b) {
        a = a.normalized;
        b = b.normalized;

        // 1. Calculate alpha
        float alpha;
        if (a.x == 0) {
            alpha = (a.y > 0 ? 90f : 180f);
        } else {
            alpha = Mathf.Atan( a.y / a.x ) * Mathf.Rad2Deg;
        }
        // Debug.Log("     Alpha angle for Vector " + a + " is " + alpha);

        // 2. Calculate beta
        float beta;
        if (b.x == 0) {
            beta = (b.y > 0 ? 90f : 180f);
        } else {
            beta = Mathf.Atan( b.y / b.x ) * Mathf.Rad2Deg;
        }

        // Debug.Log("     Beta angle for Vector " + b + " is " + beta);

        // float angleBetweenAB = Mathf.Atan(Vector3.Cross(a, b).magnitude / Vector3.Dot(a, b));
        // return (angleBetweenAB * Mathf.Rad2Deg);

        return ( alpha - beta );
    }

    // The mesh cone will cast outward, and must return into regarding what it hit.
    public struct ViewCastInfo {
        public bool hit;
        public Vector3 point;
        public float distance;
        public float angle;
        public ViewCastInfo(bool _hit, Vector3 _point, float _dist, float _angle) {
            hit = _hit;
            point = _point;
            distance = _dist;
            angle = _angle;
        }
    }

    private ViewCastInfo ViewCast(float targetAngle) {
        Vector3 dir = DirFromAngle(targetAngle, true);

        // RaycastHit hit;
        return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, targetAngle);

        // // If an obstacle was struck
        // if (Physics.Raycast(transform.position, dir, out hit, viewRadius)) {
        //     Debug.Log("Obstruction hit");
        //     return new ViewCastInfo(true, hit.point, hit.distance, targetAngle);
        // } else {
        //     Debug.Log("Straight Line");
            
        // }
    }

    public void SetColor(Color newColor) {
        viewMeshFilter.GetComponent<Renderer>().material.color = newColor;
    }

}
