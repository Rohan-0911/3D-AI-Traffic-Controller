using Unity.VisualScripting;
using UnityEngine;

public class SpawnManagerWest : MonoBehaviour
{
    public GameObject[] vehiclePrefabs;
    private int vehicleIndex;

    private int spawnPosX = -160;

    private float startDelay = 2.5f;
    private float spawnInterval = 7.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating("SpawnRandomCar", startDelay, spawnInterval);
    }

    // Update is called once per frame````
    void Update()
    {

    }
    private float[] laneZPositions = { 3.4f, 9.4f };

    private void SpawnRandomCar()
    {
        vehicleIndex = Random.Range(0, vehiclePrefabs.Length);

        // Pick a random lane index from your array
        int randomLane = Random.Range(0, laneZPositions.Length);
        float chosenZ = laneZPositions[randomLane];

        Vector3 spawnPos = new Vector3(spawnPosX, 0, chosenZ);
        Instantiate(vehiclePrefabs[vehicleIndex], spawnPos, Quaternion.Euler(0, 90, 0));
    }

    public void SpawnEmergencyVehicle(GameObject emergencyPrefab)
    {
        int randomLane = Random.Range(0, laneZPositions.Length);
        float chosenZ = laneZPositions[randomLane];

        Vector3 spawnPos = new Vector3(spawnPosX, 0, chosenZ);
        Instantiate(emergencyPrefab, spawnPos, Quaternion.Euler(0, 90, 0));
    }
}