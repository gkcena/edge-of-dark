
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] float walkSpeed = 4.5f;
    [SerializeField] float runSpeed = 8.5f;
    [SerializeField] float rotationSpeed = 10f;

    [Header("Jump")]
    [SerializeField] float jumpForce = 6f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundCheckDistance = 0.25f;

    public AudioClip jumpSound;

    Rigidbody rb;
    Animator animator;
    bool isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        GroundCheck();
        Move();
        Jump();
    }

    void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Transform cam = Camera.main.transform;

        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;
        camForward.y = 0;
        camRight.y = 0;

        Vector3 moveDir = (camForward.normalized * v + camRight.normalized * h).normalized;

        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        Vector3 velocity = moveDir * speed;
        velocity.y = rb.linearVelocity.y; // Corrected from rb.linearVelocity to rb.velocity
        rb.linearVelocity = velocity;

        animator.SetFloat("Speed", moveDir.magnitude);

        if (moveDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z); // Corrected from rb.linearVelocity to rb.velocity
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            animator.SetTrigger("Jump");
            SFXManager.Instance.PlaySFX(jumpSound);
        }
    }

    void GroundCheck()
    {
        isGrounded = Physics.Raycast(
            transform.position + Vector3.up * 0.05f,
            Vector3.down,
            groundCheckDistance,
            groundLayer
        );

        animator.SetBool("IsGrounded", isGrounded);
    }
}