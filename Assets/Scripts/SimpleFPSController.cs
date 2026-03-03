using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class SimpleFPSController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float acceleration = 20f;
    public float airControl = 0.3f;
    public float jumpForce = 5f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 0.1f;
    public float maxLookAngle = 80f;

    [Header("Ground Check")]
    public float groundCheckDistance = 1f;
    public LayerMask groundMask;

    [Header("UI / Cursor")]
    [Tooltip("If true, movement + look are disabled and cursor is unlocked/visible.")]
    [SerializeField] private bool uiOpen = false;

    private Rigidbody rb;
    private Camera cam;

    private float xRotation;
    private bool isGrounded;

    private Vector2 moveInput;
    private Vector2 lookInput;

    private Controls playerControls;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cam = GetComponentInChildren<Camera>();
        playerControls = new Controls();
    }

    private void OnEnable()
    {
        playerControls.Enable();

        playerControls.Player.Move.performed += OnMove;
        playerControls.Player.Move.canceled += OnMove;

        playerControls.Player.Look.performed += OnLook;
        playerControls.Player.Look.canceled += OnLook;

        playerControls.Player.Jump.started += OnJumpStarted;
    }

    private void OnDisable()
    {
        playerControls.Player.Move.performed -= OnMove;
        playerControls.Player.Move.canceled -= OnMove;

        playerControls.Player.Look.performed -= OnLook;
        playerControls.Player.Look.canceled -= OnLook;

        playerControls.Player.Jump.started -= OnJumpStarted;

        playerControls.Disable();
    }

    private void Start()
    {
        ApplyCursorState(uiOpen);
    }

    private void Update()
    {
        if (uiOpen) return;
        HandleMouseLook();
    }

    private void FixedUpdate()
    {
        if (uiOpen)
        {
            // Optional: stop drift while UI is open.
            var v = rb.linearVelocity;
            rb.linearVelocity = new Vector3(0f, v.y, 0f);
            return;
        }

        CheckGround();
        HandleMovement();
    }

    // =========================
    // PUBLIC API (call from Win/Lose UI)
    // =========================
    public void SetUIOpen(bool open)
    {
        uiOpen = open;

        // Clear inputs so you don't "snap" when closing UI
        moveInput = Vector2.zero;
        lookInput = Vector2.zero;

        ApplyCursorState(uiOpen);
    }

    private void ApplyCursorState(bool open)
    {
        if (open)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // =========================
    // INPUT CALLBACKS
    // =========================
    private void OnMove(InputAction.CallbackContext ctx)
    {
        if (uiOpen) { moveInput = Vector2.zero; return; }
        moveInput = ctx.ReadValue<Vector2>();
    }

    private void OnLook(InputAction.CallbackContext ctx)
    {
        if (uiOpen) { lookInput = Vector2.zero; return; }
        lookInput = ctx.ReadValue<Vector2>();
    }

    private void OnJumpStarted(InputAction.CallbackContext ctx)
    {
        if (uiOpen) return;
        HandleJump();
    }

    // =========================
    // LOOK
    // =========================
    private void HandleMouseLook()
    {
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    // =========================
    // MOVEMENT
    // =========================
    private void HandleMovement()
    {
        Vector3 moveDir = transform.right * moveInput.x + transform.forward * moveInput.y;
        moveDir.Normalize();

        Vector3 targetVelocity = moveDir * moveSpeed;

        Vector3 velocity = rb.linearVelocity;
        Vector3 velocityChange = targetVelocity - new Vector3(velocity.x, 0f, velocity.z);

        float control = isGrounded ? 1f : airControl;
        velocityChange *= control;

        rb.AddForce(velocityChange * acceleration, ForceMode.Acceleration);
    }

    private void HandleJump()
    {
        if (!isGrounded) return;
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    // =========================
    // GROUND CHECK
    // =========================
    private void CheckGround()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        isGrounded = Physics.Raycast(ray, groundCheckDistance + 0.1f, groundMask);
    }
}