using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float mouseSensitivity = 100f;
    private float xRotation = 0f;

    public Transform playerBody;
    public Camera cam;

    public bool controlEnabled = true;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (!controlEnabled) return;

        // Movimiento (WSAD)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        transform.Translate(move * speed * Time.deltaTime, Space.World);

        // Rotación (Mouse)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);

        // Click Izquierdo
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("¡Click detectado!");
        }
    }
}


