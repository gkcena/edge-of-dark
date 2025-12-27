// 12/27/2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimationController : MonoBehaviour
{
    public Animator animator;
    public Rigidbody rb;

    public bool isGrounded = true;

    void Update()
    {
        Vector3 horizontalVelocity = rb.linearVelocity; // linearVelocity yerine velocity kullanılmalı

        float speed = horizontalVelocity.magnitude;
        animator.SetFloat("Speed", speed);

        animator.SetBool("IsGrounded", isGrounded);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            animator.SetTrigger("Jump");
        }
    }
}