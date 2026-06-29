using UnityEngine;

public class DestroyOutOfBounds : MonoBehaviour
{

    private float verticalBound = 160f;
    private float horizontalBound = 160f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.z > verticalBound)
        {
            Destroy(gameObject);
        }
        else if (transform.position.z < -verticalBound)
        {
            Destroy(gameObject);
        }
        else if (transform.position.x > horizontalBound)
        {
            Destroy(gameObject);
        }
        else if (transform.position.x < -horizontalBound)
        {
            Destroy(gameObject);
        }
    }
}