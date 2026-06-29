using UnityEngine;

public class MoveForward : MonoBehaviour
{
    [Header("Car Settings")]
    public int priority;
    [Tooltip("Set the individual car's speed here in the Inspector")]
    public float speed = 1.0f;
    private float defaultSpeed;

    // ADD THIS NEW VARIABLE
    [Header("Priority Settings")]
    [Tooltip("Regular Car = 1, Police = 5, Firetruck = 10, Ambulance = 15")]
    public int vehicleWeight = 1;

    // ... rest of your code remains the same ...

    [Header("Intersection Settings")]
    public Vector3 intersectionCenter = Vector3.zero;
    public float intersectionRadius = 18f;
    public float stopZoneSize = 10f;

    private bool isClear = true;
    private IntersectionManager intersectionManager;
    private CarAI carBrain;

    private IntersectionManager.LightPhase myDrivingDirection;

    void Start()
    {
        intersectionManager = GameObject.FindFirstObjectByType<IntersectionManager>();
        carBrain = GetComponent<CarAI>();

        // Figure out exactly which way this car is facing on the grid
        if (transform.forward.x > 0.5f)
            myDrivingDirection = IntersectionManager.LightPhase.PositiveX;
        else if (transform.forward.x < -0.5f)
            myDrivingDirection = IntersectionManager.LightPhase.NegativeX;
        else if (transform.forward.z > 0.5f)
            myDrivingDirection = IntersectionManager.LightPhase.PositiveZ;
        else
            myDrivingDirection = IntersectionManager.LightPhase.NegativeZ;

        // FIXED: Memorize the speed you set in the Unity Inspector as our "Go" speed
        defaultSpeed = speed;
    }

    void Update()
    {
        if (carBrain != null)
        {
            isClear = !carBrain.shouldStop;
        }

        Vector3 vectorToCenter = intersectionCenter - transform.position;
        float distanceToCenter = Vector3.Dot(vectorToCenter, transform.forward);

        // ZONE 1: Inside the intersection OR already passed it
        if (distanceToCenter < intersectionRadius)
        {
            if (isClear) speed = defaultSpeed;
            else speed = 0;
        }
        // ZONE 2: Approaching the Stop Line
        else if (distanceToCenter >= intersectionRadius && distanceToCenter <= intersectionRadius + stopZoneSize)
        {
            bool isMyLightGreen = false;

            if (!intersectionManager.isAllRed && intersectionManager.currentPhase == myDrivingDirection)
            {
                isMyLightGreen = true;
            }

            if (isMyLightGreen && isClear) speed = defaultSpeed;
            else speed = 0; // Red light, Transition delay, or Blocked! Stop!
        }
        // ZONE 3: Far away on the road
        else
        {
            if (isClear) speed = defaultSpeed;
            else speed = 0;
        }

        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }
}