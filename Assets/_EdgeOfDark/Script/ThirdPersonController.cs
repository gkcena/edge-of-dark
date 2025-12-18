using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Camera")]
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float cameraDistance = 4f;
    [SerializeField] private float cameraHeight = 2f;

    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float cameraPitch;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    /* ===== INPUT SYSTEM EVENTS ===== */

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    /* ===== MOVEMENT ===== */

    private void FixedUpdate()
    {
        Vector3 camForward = cameraPivot.forward;
        Vector3 camRight = cameraPivot.right;

        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDirection =
            camForward * moveInput.y +
            camRight * moveInput.x;

        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(moveDirection);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            );
        }

        rb.MovePosition(
            rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime
        );
    }

    /* ===== CAMERA ===== */

    private void LateUpdate()
    {
        cameraPitch -= lookInput.y * mouseSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -35f, 60f);

        cameraPivot.localRotation =
            Quaternion.Euler(cameraPitch, 0f, 0f);

        cameraPivot.Rotate(Vector3.up * lookInput.x * mouseSensitivity);

        Vector3 cameraOffset =
            -cameraPivot.forward * cameraDistance +
            Vector3.up * cameraHeight;

        Camera.main.transform.position =
            cameraPivot.position + cameraOffset;

        Camera.main.transform.LookAt(
            cameraPivot.position + Vector3.up * 1.5f
        );
    }
}
