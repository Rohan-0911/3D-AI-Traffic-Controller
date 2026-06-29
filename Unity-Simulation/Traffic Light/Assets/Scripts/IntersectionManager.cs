using UnityEngine;

public class IntersectionManager : MonoBehaviour
{
    public enum LightPhase { PositiveX, NegativeX, PositiveZ, NegativeZ }

    [Header("Current Status (Controlled by AI)")]
    public LightPhase currentPhase = LightPhase.PositiveX;
    public bool isAllRed = false;
}



// THIS IS THE OLD CODE for four way signal
// +++++=====++++++
//

//using UnityEngine;

//public class IntersectionManager : MonoBehaviour
//{
//    [Header("Signal Timings")]
//    [Tooltip("How many seconds a specific direction gets a green light")]
//    public float greenDuration = 10.0f;

//    [Tooltip("The All-Red delay between light changes so cars can clear out")]
//    public float transitionDelay = 2.0f;

//    // We define the 4 possible directions
//    public enum LightPhase { PositiveX, NegativeX, PositiveZ, NegativeZ }

//    [Header("Current Status")]
//    public LightPhase currentPhase = LightPhase.PositiveX;
//    public bool isAllRed = false; // True during the 2-second delay

//    private float timer;

//    void Update()
//    {
//        timer += Time.deltaTime;

//        // If a green light is currently active...
//        if (!isAllRed && timer >= greenDuration)
//        {
//            isAllRed = true; // Turn all lights red!
//            timer = 0; // Reset timer for the delay
//        }
//        // If we are currently in the 2-second red delay...
//        else if (isAllRed && timer >= transitionDelay)
//        {
//            currentPhase = GetNextPhase(currentPhase); // Switch to the next direction
//            isAllRed = false; // Turn the new direction green!
//            timer = 0; // Reset timer for the green light
//        }
//    }

//    // This helper smoothly cycles through the 4 directions in order
//    LightPhase GetNextPhase(LightPhase current)
//    {
//        if (current == LightPhase.PositiveX) return LightPhase.NegativeX;
//        if (current == LightPhase.NegativeX) return LightPhase.PositiveZ;
//        if (current == LightPhase.PositiveZ) return LightPhase.NegativeZ;

//        return LightPhase.PositiveX; // Loop back to the start
//    }
//}