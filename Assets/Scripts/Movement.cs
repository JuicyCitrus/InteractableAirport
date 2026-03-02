using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Core Components")]
    public CharacterController controller;

    [Header("Movement")]
    public float moveSpeed = 6f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 100f;
    public Transform cameraPivot;

    private Controls controls;
    private float xRotation = 0f;
    private WorldSpaceButton currentButton;

    private void Awake()
    {
        controls = new Controls();
        controls.Enable();
        controller = GetComponent<CharacterController>();

        // Lock cursor for FPS control
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleMouseLook();
        HandleMovement();
    }

    private void HandleMouseLook()
    {
        float mouseX = controls.Player.Look.ReadValue<Vector2>().x * mouseSensitivity * Time.deltaTime;
        float mouseY = controls.Player.Look.ReadValue<Vector2>().y * mouseSensitivity * Time.deltaTime;

        // Vertical look (camera only)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Horizontal look (player body)
        controller.gameObject.transform.localRotation = Quaternion.Euler(0f, controller.gameObject.transform.localRotation.eulerAngles.y + mouseX, 0f);
    }

    private void HandleMovement()
    {
        float x = controls.Player.Move.ReadValue<Vector2>().x;
        float z = controls.Player.Move.ReadValue<Vector2>().y;

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * moveSpeed * Time.deltaTime);
    }
}
