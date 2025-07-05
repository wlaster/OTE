using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 16f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    // Компоненты
    private Rigidbody2D rb;
    private Animator animator;

    // Состояние
    private float moveInput;
    private bool isGrounded;
    private bool isFacingRight = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Проверяем состояние "на земле" каждый кадр для быстрой реакции анимации
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        
        UpdateAnimationState();
        Flip();
    }

    private void FixedUpdate()
    {
        // Применяем физику движения в FixedUpdate
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    // Этот метод вызывается из PlayerController
    public void SetDirectionalInput(float direction)
    {
        moveInput = direction;
    }

    // Этот метод вызывается из PlayerController
    public void Jump()
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void UpdateAnimationState()
    {
        animator.SetBool("isRunning", moveInput != 0 && isGrounded);
        animator.SetBool("isJumping", !isGrounded);
    }

    private void Flip()
    {
        if ((moveInput > 0 && !isFacingRight) || (moveInput < 0 && isFacingRight))
        {
            isFacingRight = !isFacingRight;
            transform.Rotate(0f, 180f, 0f);
        }
    }
}