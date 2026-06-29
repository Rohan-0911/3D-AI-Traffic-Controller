using Unity.VisualScripting;
using UnityEngine;

public class SpawnManagerEast : MonoBehaviour
{
    public GameObject[] vehiclePrefabs;
    private int vehicleIndex;

    private int spawnPosX = 160;

    private float startDelay = 4.5f;
    private float spawnInterval = 9.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating("SpawnRandomCar", startDelay, spawnInterval);
    }

    // Update is called once per frame````
    void Update()
    {

    }

    // Define the exact center Z-coordinate for each lane
    private float[] laneZPositions = { -3.4f, -9.4f};

    private void SpawnRandomCar()
    {
        vehicleIndex = Random.Range(0, vehiclePrefabs.Length);

        // Pick a random lane index from your array
        int randomLane = Random.Range(0, laneZPositions.Length);
        float chosenZ = laneZPositions[randomLane];

        Vector3 spawnPos = new Vector3(spawnPosX, 0, chosenZ);

        Instantiate(vehiclePrefabs[vehicleIndex], spawnPos, Quaternion.Euler(0, 270, 0));
    }

    // ADD THIS TO SpawnManagerEast.cs

    public void SpawnEmergencyVehicle(GameObject emergencyPrefab)
    {
        // Pick a random lane index from your array
        int randomLane = Random.Range(0, laneZPositions.Length);
        float chosenZ = laneZPositions[randomLane];

        Vector3 spawnPos = new Vector3(spawnPosX, 0, chosenZ);

        // Uses your exact existing rotation!
        Instantiate(emergencyPrefab, spawnPos, Quaternion.Euler(0, 270, 0));
    }
}