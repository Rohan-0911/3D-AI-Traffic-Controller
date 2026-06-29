using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class TrafficAgent : MonoBehaviour
{
    [Header("Server Connection")]
    public string serverUrl = "http://127.0.0.1:5000/predict_light";
    [Tooltip("How often Unity asks the AI for a decision")]
    public float aiDecisionInterval = 3.0f;

    [Header("Traffic Detection")]
    [Tooltip("How far down the road to scan for waiting cars")]
    public float detectionRadius = 150f;

    private IntersectionManager intersectionManager;
    private float timer;
    private bool isTransitioning = false;

    // These classes match the exact JSON structure Python expects
    [System.Serializable]
    public class TrafficState
    {
        // 1. AI Scores (Used by Neural Network)
        public int positive_x; public int negative_x; public int positive_z; public int negative_z;

        // 2. Physical Counts (Used by Dashboard)
        public int count_px; public int count_nx; public int count_pz; public int count_nz;

        // 3. Specific Emergency Trackers
        public int amb_px; public int fire_px; public int pol_px;
        public int amb_nx; public int fire_nx; public int pol_nx;
        public int amb_pz; public int fire_pz; public int pol_pz;
        public int amb_nz; public int fire_nz; public int pol_nz;
    }

    [System.Serializable]
    public class TrafficResponse
    {
        public int action;
    }

    void Start()
    {
        intersectionManager = GameObject.FindFirstObjectByType<IntersectionManager>();
    }

    void Update()
    {
        // Only bother the AI if we aren't currently switching lights
        if (!isTransitioning && !intersectionManager.isAllRed)
        {
            timer += Time.deltaTime;
            if (timer >= aiDecisionInterval)
            {
                timer = 0f;
                StartCoroutine(AskPythonBrain());
            }
        }
    }

    IEnumerator AskPythonBrain()
    {
        // 1. Scan the environment and count waiting cars
        TrafficState currentState = CountCars();
        string jsonData = JsonUtility.ToJson(currentState);

        // 2. Send the HTTP POST request to Flask
        using (UnityWebRequest request = new UnityWebRequest(serverUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // 3. Read the AI's decision
                TrafficResponse response = JsonUtility.FromJson<TrafficResponse>(request.downloadHandler.text);
                ApplyAction(response.action);
            }
            else
            {
                Debug.LogError("Failed to reach Python AI: " + request.error);
            }
        }
    }

    TrafficState CountCars()
    {
        TrafficState state = new TrafficState();
        MoveForward[] allCars = GameObject.FindObjectsByType<MoveForward>(FindObjectsSortMode.None);

        foreach (MoveForward car in allCars)
        {
            Vector3 vectorToCenter = intersectionManager.transform.position - car.transform.position;
            float distanceToCenter = Vector3.Dot(vectorToCenter, car.transform.forward);

            if (distanceToCenter > 10f && distanceToCenter <= detectionRadius)
            {
                // Detect specific vehicles based on the weights you set
                bool isAmb = car.vehicleWeight == 15;
                bool isFire = car.vehicleWeight == 10;
                bool isPol = car.vehicleWeight == 5;

                if (car.transform.forward.x > 0.5f)
                {
                    state.positive_x += car.vehicleWeight; // AI Score
                    state.count_px++; // Actual physical vehicle
                    if (isAmb) state.amb_px++; else if (isFire) state.fire_px++; else if (isPol) state.pol_px++;
                }
                else if (car.transform.forward.x < -0.5f)
                {
                    state.negative_x += car.vehicleWeight;
                    state.count_nx++;
                    if (isAmb) state.amb_nx++; else if (isFire) state.fire_nx++; else if (isPol) state.pol_nx++;
                }
                else if (car.transform.forward.z > 0.5f)
                {
                    state.positive_z += car.vehicleWeight;
                    state.count_pz++;
                    if (isAmb) state.amb_pz++; else if (isFire) state.fire_pz++; else if (isPol) state.pol_pz++;
                }
                else
                {
                    state.negative_z += car.vehicleWeight;
                    state.count_nz++;
                    if (isAmb) state.amb_nz++; else if (isFire) state.fire_nz++; else if (isPol) state.pol_nz++;
                }
            }
        }
        return state;
    }

    void ApplyAction(int action)
    {
        IntersectionManager.LightPhase decidedPhase = intersectionManager.currentPhase;

        // FIXED: This now perfectly matches the [Positive_X, Negative_X, Positive_Z, Negative_Z] array order!
        if (action == 0) decidedPhase = IntersectionManager.LightPhase.PositiveX;
        else if (action == 1) decidedPhase = IntersectionManager.LightPhase.NegativeX;
        else if (action == 2) decidedPhase = IntersectionManager.LightPhase.PositiveZ;
        else if (action == 3) decidedPhase = IntersectionManager.LightPhase.NegativeZ;

        // If the AI wants to change the light, run the safety transition!
        if (intersectionManager.currentPhase != decidedPhase)
        {
            StartCoroutine(ChangeLightSafely(decidedPhase));
        }
    }

    IEnumerator ChangeLightSafely(IntersectionManager.LightPhase newPhase)
    {
        isTransitioning = true;

        // 1. Turn all lights red
        intersectionManager.isAllRed = true;

        // 2. Wait 2 seconds for intersection to clear
        yield return new WaitForSeconds(2.0f);

        // 3. Apply the new green light phase
        intersectionManager.currentPhase = newPhase;
        intersectionManager.isAllRed = false;

        isTransitioning = false;
    }
}