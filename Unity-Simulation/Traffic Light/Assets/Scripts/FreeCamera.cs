using UnityEngine;

public class FreeCamera : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Normal flying speed (WASD)")]
    public float normalSpeed = 20f;
    [Tooltip("Sprint speed when holding Shift")]
    public float sprintSpeed = 50f;
    [Tooltip("Speed when moving straight up/down (E and Q)")]
    public float verticalSpeed = 15f;

    [Header("Look Settings")]
    public float mouseSensitivity = 2f;

    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start()
    {
        // Lock the mouse cursor to the game window so it doesn't wander onto your other monitors
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Grab the initial rotation so the camera doesn't snap when you hit play
        Vector3 rot = transform.localRotation.eulerAngles;
        rotationY = rot.y;
        rotationX = rot.x;
    }

    void Update()
    {
        // -----------------------------
        // 1. LOOK AROUND (Mouse)
        // -----------------------------
        rotationY += Input.GetAxis("Mouse X") * mouseSensitivity;
        rotationX -= Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Clamp the up/down looking so you don't accidentally do backflips
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);

        // -----------------------------
        // 2. FLY AROUND (Keyboard)
        // -----------------------------
        // Check if we are holding Shift to sprint
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : normalSpeed;

        float horizontal = Input.GetAxis("Horizontal"); // A and D keys
        float vertical = Input.GetAxis("Vertical");     // W and S keys

        // Calculate direction relative to where the camera is currently looking
        Vector3 movement = (transform.right * horizontal) + (transform.forward * vertical);

        // Fly straight up (E) or straight down (Q)
        if (Input.GetKey(KeyCode.E)) movement += Vector3.up * (verticalSpeed / currentSpeed);
        if (Input.GetKey(KeyCode.Q)) movement -= Vector3.up * (verticalSpeed / currentSpeed);

        // Apply the math to actually move the camera
        transform.position += movement * currentSpeed * Time.deltaTime;

        // -----------------------------
        // 3. UNLOCK CURSOR (Escape)
        // -----------------------------
        // If you need to click a button or stop the game, press Escape!
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}