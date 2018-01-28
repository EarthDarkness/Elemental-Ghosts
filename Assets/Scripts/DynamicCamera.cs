using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCamera : MonoBehaviour
{
    ////DEBUG pourpouses!
    //GameObject[] marker = new GameObject[11];

    /// <summary>
    /// Reference to main camera
    /// </summary>
    Transform cam;

    /// <summary>
    /// List of tracking objects
    /// </summary>
    public List<Transform> trackList = new List<Transform>();

    /// <summary>
    /// Minimum distance of the camera from center
    /// </summary>
    public float cameraMinimumDistance = 5.0f;

    /// <summary>
    /// Camera margin from tracked objects
    /// </summary>
    public float cameraMargin = 0.3f;

    /// <summary>
    /// Camera inclination from XZ
    /// (horizontal) plane in degrees
    /// </summary>
    public float cameraInclination = 45.0f;

    /// <summary>
    /// Camera direction, in degrees,
    /// around Y (vertical) axis
    /// </summary>
    public float cameraDirection = 0.0f;

    /// <summary>
    /// Direction from center towards the camera
    /// </summary>
    Vector3 camDirection;

    /// <summary>
    /// Minimum and maximum corners that envolve all tracked objects
    /// </summary>
    Vector3 minCorner, maxCorner;

    /// <summary>
    /// Camera Plane Vectors
    /// </summary>
    Vector3 camPlaneNormal;
    Vector3 camPlaneTangent;
    Vector3 camPlaneBiTangent;

    /// <summary>
    /// The time it takes to interpolate camera position
    /// </summary>
    [SerializeField]
    float interpolationTime = 0.3f;


    /// <summary>
    /// Current camera speed
    /// </summary>
    Vector3 camSpeed;
    Vector3 camRotSpeed;

    // Use this for initialization
    void Start()
    {
        ////DEBUG pourpouses!
        //for (int i = 0; i < 8; i++)
        //{
        //    marker[i] = GameObject.CreatePrimitive(PrimitiveType.Quad);
        //    marker[i].GetComponent<Collider>().enabled = false;
        //}
        //for (int i = 0; i < 3; i++)
        //{
        //    marker[8+i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //    marker[8+i].GetComponent<Collider>().enabled = false;
        //}
        //camDirection = Camera.main.transform.position.normalized;
    }

    //Update plane parallel to cam far plane
    void UpdateCamPlane()
    {
        camPlaneNormal = cam.transform.rotation * Vector3.forward;
        camPlaneTangent = cam.transform.rotation * Vector3.right;
        camPlaneBiTangent = cam.transform.rotation * Vector3.up;
    }

    void GetCameraTransform(out Vector3 position, out Vector3 direction, out Vector3 center)
    {
        //Update camera reference
        cam = Camera.main.transform;

        //Reset corners to first element (or zero if no elements tracked)
        minCorner = maxCorner = (trackList.Count > 0) ? trackList[0].position : Vector3.zero;

        //Get corner positions
        for (int i = 0; i < trackList.Count; i++)
        {
            //Compare X
            if (trackList[i].position.x < minCorner.x)
                minCorner.x = trackList[i].position.x;
            else if (trackList[i].position.x > maxCorner.x)
                maxCorner.x = trackList[i].position.x;

            //Compare Y
            if (trackList[i].position.y < minCorner.y)
                minCorner.y = trackList[i].position.y;
            else if (trackList[i].position.y > maxCorner.y)
                maxCorner.y = trackList[i].position.y;

            //Compare Z
            if (trackList[i].position.z < minCorner.z)
                minCorner.z = trackList[i].position.z;
            else if (trackList[i].position.z > maxCorner.z)
                maxCorner.z = trackList[i].position.z;
        }

        //Average
        Vector3 cubeCenter = (minCorner + maxCorner) * 0.5f;

        //Update cam plane
        UpdateCamPlane();

        //To be able to use trigonometric relations to find the camera distance from the center
        //and still keep every tracking object in view, regardless of the camera inclination,
        //we have to project the objects into a plane parallel to the camera far/near plane.
        //By doing so, we can compute the (cam plane space) horizontal and vertical distances
        //from the center and use trigonometric relations (using the tangent of half FOV angle)
        //to find which is the proper distance to place the camera. References:
        //https://en.wikipedia.org/wiki/Line%E2%80%93plane_intersection
        //https://en.wikipedia.org/wiki/Invertible_matrix#Methods_of_matrix_inversion

        //First, let us define what we need:

        // - The corners of our bounding box whose min and max corners (in world coords) we know
        //and we will use an auxiliar vector for getting the distances right for each axis
        Vector3 auxVector = maxCorner - minCorner;
        Vector3[] corners = new Vector3[8]
        {
            minCorner,
            minCorner + auxVector.x * Vector3.right,
            minCorner + auxVector.y * Vector3.up,
            minCorner + auxVector.z * Vector3.forward,
            maxCorner,
            maxCorner + auxVector.x * Vector3.left,
            maxCorner + auxVector.y * Vector3.down,
            maxCorner + auxVector.z * Vector3.back
        };

        //Distance Vector (for taking distances from center)
        Vector2 distVector, maxDistVector, minDistVector;
        distVector = maxDistVector = minDistVector = new Vector2();

        // We need to project each corner
        for (int i = 0; i < 8; i++)
        {
            //For this projection we must solve a system of equations defined by the intersection
            //of a vector, from the tracking object to the camera's position, and the given plane
            Vector3 trackingVector = (cam.position - corners[i]);

            // - The Determinant of an matrix given by the plane's normal and the tracking vector
            float det = Vector3.Dot(trackingVector, camPlaneNormal);

            // - A vector comming from the plane's center to the corner possition
            Vector3 center_to_corner = (corners[i] - cubeCenter);

            //Now we solve "t" for the trackingVector ("t" like in P = P0 + V*t)
            float t = -Vector3.Dot(camPlaneNormal / det, center_to_corner);

            //We can solve the "r" and the "s" for the plane, but "t" gives us a position already;
            //"r" and "s" are required to get horizontal and vertical distances from center. As our
            //plane is an perfect view-oriented plane but still in world-space the variables mean 
            //the horizontal and vertical distances from the center!

            //Record maximum horizontal distance (absolute function because distance cannot be negative)
            float r = Vector3.Dot(Vector3.Cross(camPlaneBiTangent, trackingVector) / det, center_to_corner);
            if (Mathf.Abs(r) > distVector.x)
                distVector.x = Mathf.Abs(r);

            //Update maximum and minimum horizontal vectors
            if (r > maxDistVector.x)
                maxDistVector.x = r;
            if (r < minDistVector.x)
                minDistVector.x = r;

            //Record maximum vertical distance
            float s = Vector3.Dot(Vector3.Cross(trackingVector, camPlaneTangent) / det, center_to_corner);
            if (Mathf.Abs(s) > distVector.y)
                distVector.y = Mathf.Abs(s);

            //Update maximum and minimum vertical vectors
            if (s > maxDistVector.y)
                maxDistVector.y = s;
            if (s < minDistVector.y)
                minDistVector.y = s;

            //And replace corner variable with the position using the corner, trackingVector and t
            //or using center, planeTangent, r, planeBiTangent and s
            corners[i] = corners[i] + trackingVector * t;

            ////DEBUG pourpouses!
            //marker[i].transform.position = corners[i];
            //marker[i].transform.rotation = Quaternion.LookRotation(camPlaneNormal, camPlaneBiTangent);
        }

        ////DEBUG pourpouses!
        //Debug.Log("Min:" + minDistVector + "; Max:" + maxDistVector);
        //marker[8].transform.position = position;
        //marker[9].transform.position = center + minDistVector.x * camPlaneTangent + minDistVector.y * camPlaneBiTangent;
        //marker[10].transform.position = center + maxDistVector.x * camPlaneTangent + maxDistVector.y * camPlaneBiTangent;

        //To define the proper camera position we must define the middle of our corner projections
        //In order to do so, we must use our maximum and minimum positions and define the half way between them
        position = cubeCenter + (minDistVector.x + (maxDistVector.x - minDistVector.x) * 0.5f) * camPlaneTangent
            + (minDistVector.y + (maxDistVector.y - minDistVector.y) * 0.5f) * camPlaneBiTangent;

        //We will now use trigonometric relations to define the proper distance for the camera to fit all targets

        //Get tangent of half FOV angle
        float halfFOVTangent = Mathf.Tan(Camera.main.fieldOfView * Mathf.Deg2Rad * 0.5f);

        //Trigonometry relation between the two triangles should give us the proper distance (on each local axis)
        float yDis = distVector.y / halfFOVTangent;
        float xDis = distVector.x / (halfFOVTangent * Camera.main.aspect);

        //Set distance as the highest distance definition
        float distance = Mathf.Max(Mathf.Max(xDis, yDis) * (1.0f + cameraMargin), cameraMinimumDistance);

        //Find camera final direction from inclination and direction variables
        camDirection = Quaternion.AngleAxis(-cameraDirection, Vector3.up) * (Quaternion.AngleAxis(-cameraInclination, Vector3.left) * Vector3.back);

        //Set final camera center, position and rotation
        center = position;
        position += camDirection * distance;
        direction = Quaternion.AngleAxis(-cameraDirection, Vector3.up) * Quaternion.AngleAxis(-cameraInclination, Vector3.left) * Vector3.forward;
    }

    // Update is called once per frame
    void Update()
    {
        //Get new camera transform and hold it's values
        Vector3 newCamPos;
        Vector3 newCamDir;
        Vector3 newCamCenter;
        GetCameraTransform(out newCamPos, out newCamDir, out newCamCenter);

        //Update interpolated transforms
        cam.position = Vector3.SmoothDamp(cam.position, newCamPos, ref camSpeed, interpolationTime);
        Vector3 camDirection = Vector3.SmoothDamp(cam.rotation * Vector3.forward, newCamDir, ref camRotSpeed, interpolationTime);
        cam.rotation = Quaternion.LookRotation(camDirection, Vector3.up);
    }
}
