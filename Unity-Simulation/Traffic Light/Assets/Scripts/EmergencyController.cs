using UnityEngine;
using TMPro; // Required for modern Unity Text UI

public class EmergencyController : MonoBehaviour
{
    public enum SpawnDirection { East, South, West, North }
    public enum EmergencyType { Ambulance, Firetruck, Police }

    [Header("UI Reference")]
    public TextMeshProUGUI statusText;

    [Header("Emergency Prefabs")]
    public GameObject ambulancePrefab;
    public GameObject firetruckPrefab;
    public GameObject policePrefab;

    [Header("Connected Spawn Managers")]
    public SpawnManagerEast eastManager;
    // Note: Change these class names if your scripts are named differently (e.g., SpawnManager_South)
    public SpawnManagerSouth southManager;
    public SpawnManagerWest westManager;
    public SpawnManagerNorth northManager;

    private SpawnDirection currentDirection = SpawnDirection.East;
    private EmergencyType currentVehicle = EmergencyType.Ambulance;

    void Start()
    {
        UpdateUIText();
    }

    void Update()
    {
        // 1. Cycle Direction (Lane)
        if (Input.GetKeyDown(KeyCode.L))
        {
            // Cycles 0 -> 1 -> 2 -> 3 -> 0
            currentDirection = (SpawnDirection)(((int)currentDirection + 1) % 4);
            UpdateUIText();
        }

        // 2. Select Vehicle Type
        if (Input.GetKeyDown(KeyCode.A)) { currentVehicle = EmergencyType.Ambulance; UpdateUIText(); }
        if (Input.GetKeyDown(KeyCode.F)) { currentVehicle = EmergencyType.Firetruck; UpdateUIText(); }
        if (Input.GetKeyDown(KeyCode.P)) { currentVehicle = EmergencyType.Police; UpdateUIText(); }

        // 3. Execute Spawn
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SpawnEmergency();
        }
    }

    void UpdateUIText()
    {
        if (statusText != null)
        {
            statusText.text = $"SELECTED LANE: <color=#00FF00>{currentDirection}</color>  |  VEHICLE: <color=#FFA500>{currentVehicle}</color>\n<size=50%>Press L to cycle lane. Press A, F, P to select. Press Enter to spawn.</size>";
        }
    }

    void SpawnEmergency()
    {
        GameObject prefabToSpawn = null;

        // Assign correct prefab
        switch (currentVehicle)
        {
            case EmergencyType.Ambulance: prefabToSpawn = ambulancePrefab; break;
            case EmergencyType.Firetruck: prefabToSpawn = firetruckPrefab; break;
            case EmergencyType.Police: prefabToSpawn = policePrefab; break;
        }

        // Send to the correct existing spawner
        switch (currentDirection)
        {
            case SpawnDirection.East:
                if (eastManager != null) eastManager.SpawnEmergencyVehicle(prefabToSpawn);
                break;
            case SpawnDirection.South:
                if (southManager != null) southManager.SpawnEmergencyVehicle(prefabToSpawn);
                break;
            case SpawnDirection.West:
                if (westManager != null) westManager.SpawnEmergencyVehicle(prefabToSpawn);
                break;
            case SpawnDirection.North:
                if (northManager != null) northManager.SpawnEmergencyVehicle(prefabToSpawn);
                break;
        }

        Debug.Log($"Emergency Controller: Deployed {currentVehicle} from {currentDirection} Spawner.");
    }
}