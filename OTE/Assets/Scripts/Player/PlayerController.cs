using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // === КОМПОНЕНТЫ И НАСТРОЙКИ ===
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 14f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;

    // Ссылки на компоненты
    private Rigidbody2D rb;
    private Animator anim;
    private PlayerAttack playerAttack; // НОВОЕ: Ссылка на наш новый скрипт атаки

    // Переменные для отслеживания состояния
    private float moveInput;
    private bool isGrounded;
    private bool isFacingRight = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerAttack = GetComponent<PlayerAttack>(); // НОВОЕ: Получаем компонент при старте
    }

    void Update()
    {
        // 1. Ввод
        moveInput = Input.GetAxisRaw("Horizontal");

        // 2. Проверка земли
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);

        // 3. Прыжок
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // 4. Атака (ИЗМЕНЕНО)
        if (Input.GetMouseButtonDown(0)) // Левая кнопка мыши
        {
            // Мы больше не выполняем логику здесь, а просто просим
            // компонент PlayerAttack сделать свою работу.
            playerAttack.TryToAttack();
        }

        // 5. Анимации и поворот
        UpdateAnimations();
        Flip();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }
    
    private void UpdateAnimations()
    {
        anim.SetBool("isRunning", moveInput != 0 && isGrounded);
        anim.SetBool("isJumping", !isGrounded);
    }
    
    private void Flip()
    {
        if (moveInput > 0 && !isFacingRight)
        {
            transform.localScale = Vector3.one;
            isFacingRight = true;
        }
        else if (moveInput < 0 && isFacingRight)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            isFacingRight = false;
        }
    }
}