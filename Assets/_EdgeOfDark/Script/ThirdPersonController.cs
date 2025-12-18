using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterController))]
public class ThirdPersonMoveAndCamera_NewInput : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Main Camera transform (drag Main Camera here).")]
    public Transform cameraTransform;

    [Header("Movement")]
    public float moveSpeed = 5.5f;
    public float rotationSpeed = 12f;      // character turning speed
    public float gravity = -20f;

    [Header("Camera")]
    public Vector3 cameraOffset = new Vector3(0f, 2.2f, -4.5f);
    public float mouseSensitivity = 0.12f;
    public float minPitch = -25f;
    public float maxPitch = 70f;
    public float cameraSmooth = 14f;

    private CharacterController cc;
    private Vector3 verticalVelocity;

    private float yaw;
    private float pitch;

    // Input Actions (no asset needed)
    private InputAction moveAction;
    private InputAction lookAction;

    void Awake()
    {
        cc = GetComponent<CharacterController>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        // Create actions
        moveAction = new InputAction("Move", InputActionType.Value);
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        lookAction = new InputAction("Look", InputActionType.Value, "<Mouse>/delta");
    }

    void OnEnable()
    {
        moveAction.Enable();
        lookAction.Enable();

        // optional: lock cursor for TPS feel
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
    }

    void Start()
    {
        // initialize yaw from current facing
        yaw = transform.eulerAngles.y;
        pitch = 15f;
    }

    void Update()
    {
        if (cameraTransform == null) return;

        HandleLook();
        HandleMovement();
        HandleCameraFollow();
    }

    void HandleLook()
    {
        Vector2 look = lookAction.ReadValue<Vector2>();

        yaw += look.x * mouseSensitivity;
        pitch -= look.y * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    void HandleMovement()
    {
        Vector2 move = moveAction.ReadValue<Vector2>();
        Vector3 input = new Vector3(move.x, 0f, move.y);
        input = Vector3.ClampMagnitude(input, 1f);

        // camera-relative directions (based on yaw)
        Quaternion yawRot = Quaternion.Euler(0f, yaw, 0f);
        Vector3 moveDir = yawRot * input; // WASD relative to camera yaw

        // gravity
        if (cc.isGrounded && verticalVelocity.y < 0f)
            verticalVelocity.y = -2f;
        verticalVelocity.y += gravity * Time.deltaTime;

        // move
        Vector3 velocity = moveDir * moveSpeed + Vector3.up * verticalVelocity.y;
        cc.Move(velocity * Time.deltaTime);

        // rotate character towards move direction (only if moving)
        if (moveDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }

    void HandleCameraFollow()
    {
        // camera pivot: player position + rotated offset
        Quaternion camRot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 targetPos = transform.position + camRot * cameraOffset;

        // smooth position
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPos, cameraSmooth * Time.deltaTime);

        // look at player (chest height)
        Vector3 lookTarget = transform.position + Vector3.up * 1.5f;
        Quaternion targetLook = Quaternion.LookRotation(lookTarget - cameraTransform.position, Vector3.up);
        cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, targetLook, cameraSmooth * Time.deltaTime);
    }
}
