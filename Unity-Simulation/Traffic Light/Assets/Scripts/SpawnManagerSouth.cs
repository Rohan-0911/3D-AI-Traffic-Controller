using Unity.VisualScripting;
using UnityEngine;

public class SpawnManagerSouth : MonoBehaviour
{
    public GameObject[] vehiclePrefabs;
    private int vehicleIndex;

    private int spawnPosZ = -160;

    private float startDelay = 2.5f;
    private float spawnInterval = 8.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating("SpawnRandomCar", startDelay, spawnInterval);
    }

    // Update is called once per frame````
    void Update()
    {

    }
    private float[] laneXPositions = { -3.4f, -9.4f };

    private void SpawnRandomCar()
    {
        vehicleIndex = Random.Range(0, vehiclePrefabs.Length);

        // Pick a random lane index from your array
        int randomLane = Random.Range(0, laneXPositions.Length);
        float chosenX = laneXPositions[randomLane];

        Vector3 spawnPos = new Vector3(chosenX, 0 ,spawnPosZ);
        Instantiate(vehiclePrefabs[vehicleIndex], spawnPos, vehiclePrefabs[vehicleIndex].transform.rotation);
    }

    public void SpawnEmergencyVehicle(GameObject emergencyPrefab)
    {
        int randomLane = Random.Range(0, laneXPositions.Length);
        float chosenX = laneXPositions[randomLane];

        Vector3 spawnPos = new Vector3(chosenX, 0, spawnPosZ);
        Instantiate(emergencyPrefab, spawnPos, emergencyPrefab.transform.rotation);
    }
}