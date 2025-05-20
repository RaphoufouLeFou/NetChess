using UnityEngine;

public class PlayerMovements : MonoBehaviour
{
    public float speed = 5f; // Speed of the player movement
    public GameObject Camera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // lock the cursor to the center of the screen and make it invisible
        Cursor.lockState = CursorLockMode.Locked;

        Cursor.visible = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        // Get the horizontal and vertical input axes
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Create a new Vector3 for movement direction
        Vector3 movement = new Vector3(horizontal, 0, vertical);

        // Normalize the movement vector to prevent faster diagonal movement
        if (movement.magnitude > 1)
        {
            movement.Normalize();
        }

        // Move the player in the specified direction
        transform.Translate(movement * Time.deltaTime * speed);

        // rotate the player with the mouse
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Rotate the player around the Y axis based on mouse movement
        transform.Rotate(0, mouseX * 5f, 0);

        // Rotate the camera around the X axis based on mouse movement
        Camera.transform.Rotate(-mouseY * 5f, 0, 0);

        // Clamp the camera rotation to prevent flipping
        Vector3 cameraRotation = Camera.transform.localEulerAngles;
        if (cameraRotation.x > 180)
        {
            cameraRotation.x -= 360;
        }
        cameraRotation.x = Mathf.Clamp(cameraRotation.x, -60, 60); // Limit the camera rotation
        Camera.transform.localEulerAngles = cameraRotation;


    }
}
