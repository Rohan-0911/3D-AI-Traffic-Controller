using UnityEngine;

public class CarAI : MonoBehaviour
{
    [Header("Sensor Settings")]
    [Tooltip("Drag the SensorOrigin empty object here from your Hierarchy")]
    public Transform sensorOrigin;

    [Tooltip("How far the sensor shoots forward")]
    public float safeDistance = 5f;

    [Tooltip("How THICK the laser is. Set this to the width of your car!")]
    public float sensorRadius = 1f; // NEW: This makes the laser a thick tube!

    [Header("Tags")]
    public string carTag = "Car";

    public bool shouldStop = false;

    void Update()
    {
        CheckForwardClearance();
    }

    void CheckForwardClearance()
    {
        if (sensorOrigin == null) return;

        RaycastHit hit;

        // NEW: We use SphereCast to shoot a thick tube instead of a thin line
        if (Physics.SphereCast(sensorOrigin.position, sensorRadius, sensorOrigin.forward, out hit, safeDistance))
        {
            if (hit.collider.CompareTag(carTag))
            {
                shouldStop = true; // Hit the brakes!
                return;
            }
        }

        // If the thick laser hits nothing, or hits something that isn't a car
        shouldStop = false; // Release the brakes
    }

    // NEW: This draws the 3D SphereCast in your Scene view so you can see how thick it is!
    void OnDrawGizmos()
    {
        if (sensorOrigin != null)
        {
            // Color it Red if stopping, Green if safe
            Gizmos.color = shouldStop ? Color.red : Color.green;

            // Draw the starting sphere at the bumper
            Gizmos.DrawWireSphere(sensorOrigin.position, sensorRadius);

            // Draw the ending sphere down the road
            Vector3 endPosition = sensorOrigin.position + (sensorOrigin.forward * safeDistance);
            Gizmos.DrawWireSphere(endPosition, sensorRadius);

            // Draw a line connecting them
            Gizmos.DrawLine(sensorOrigin.position, endPosition);
        }
    }
}