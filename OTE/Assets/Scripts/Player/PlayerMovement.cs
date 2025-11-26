using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
/// <summary>
/// Управляет движением игрока: горизонтальным перемещением, прыжком и анимациями.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 16f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    
    private Rigidbody2D rb;
    private Animator animator;

    
    private float moveInput;
    private bool isGrounded;
    private bool isFacingRight = true;

    /// <summary>
    /// Инициализация компонентов: кэширую Rigidbody2D и Animator.
    /// </summary>
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Ежемесячное обновление: проверка касания земли, обновление анимаций и ориентации спрайта.
    /// </summary>
    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        UpdateAnimationState();
        Flip();
    }

    /// <summary>
    /// Физическое обновление: применяет горизонтальную скорость к Rigidbody2D.
    /// </summary>
    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    
    /// <summary>
    /// Устанавливает направление движения от ввода (влево/вправо).
    /// </summary>
    /// <param name="direction">Значение направления: -1, 0 или 1.</param>
    public void SetDirectionalInput(float direction)
    {
        moveInput = direction;
    }

    
    /// <summary>
    /// Выполняет прыжок, если игрок стоит на земле.
    /// </summary>
    public void Jump()
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    /// <summary>
    /// Обновляет параметры аниматора в соответствии с состоянием движения и прыжка.
    /// </summary>
    private void UpdateAnimationState()
    {
        animator.SetBool("isRunning", moveInput != 0 && isGrounded);
        animator.SetBool("isJumping", !isGrounded);
    }

    /// <summary>
    /// Поворачивает спрайт персонажа при смене направления движения.
    /// </summary>
    private void Flip()
    {
        if ((moveInput > 0 && !isFacingRight) || (moveInput < 0 && isFacingRight))
        {
            isFacingRight = !isFacingRight;
            transform.Rotate(0f, 180f, 0f);
        }
    }
}