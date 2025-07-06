using UnityEngine;

// Объявляем, что мы выполняем контракт IMovable
public class EnemyMovement : MonoBehaviour, IMovable
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float chaseSpeedMultiplier = 1.5f; // Враг будет бежать быстрее за игроком

    [Header("Patrol Settings")]
    [SerializeField] private Transform patrolCheckPoint;
    [SerializeField] private float groundCheckVerticalOffset = 1f;
    [SerializeField] private float checkDistance = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Grounding Check")]
    [SerializeField] private Transform isGroundedCheck;
    [SerializeField] private float groundingRadius = 0.2f;

    private Rigidbody2D rb;
    private int facingDirection = 1;
    private bool isGrounded;
    private bool isPatrolling = true; // Флаг для управления режимом

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(isGroundedCheck.position, groundingRadius, groundLayer);

        if (isGrounded && isPatrolling)
        {
            // Патрулируем, только если включен соответствующий режим
            Patrol();
        }
    }

    private void Patrol()
    {
        if (ShouldTurnAround())
        {
            Flip();
        }
        rb.linearVelocity = new Vector2(facingDirection * moveSpeed, rb.linearVelocity.y);
    }
    
    // --- РЕАЛИЗАЦИЯ ИНТЕРФЕЙСА IMovable ---

    public void MoveTowards(Vector2 targetPosition)
    {
        if (!isGrounded) return; // Не двигаемся к цели, если в воздухе

        // Определяем направление к цели
        float directionToTarget = Mathf.Sign(targetPosition.x - transform.position.x);

        // Если нужно, разворачиваемся
        if (directionToTarget != facingDirection)
        {
            Flip();
        }

        // Двигаемся в сторону цели с увеличенной скоростью
        rb.linearVelocity = new Vector2(facingDirection * moveSpeed * chaseSpeedMultiplier, rb.linearVelocity.y);
    }

    public void StartPatrolling()
    {
        isPatrolling = true;
    }

    public void StopPatrolling()
    {
        isPatrolling = false;
        // Останавливаемся, чтобы не ехать по инерции
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }
    
    // --- Приватные методы ---

    private bool ShouldTurnAround()
    {
        bool isWallAhead = Physics2D.Raycast(patrolCheckPoint.position, new Vector2(facingDirection, 0), checkDistance, groundLayer);
        if (isWallAhead) return true;

        Vector2 groundCheckPos = (Vector2)patrolCheckPoint.position - new Vector2(0, groundCheckVerticalOffset);
        bool isGroundAhead = Physics2D.Raycast(groundCheckPos, new Vector2(facingDirection, 0), checkDistance, groundLayer);
        if (!isGroundAhead) return true;

        return false;
    }

    private void Flip()
    {
        facingDirection *= -1;
        transform.Rotate(0f, 180f, 0f);
    }
    
    private void OnDrawGizmos()
    {
        // Гизмо для патрулирования
        if (patrolCheckPoint != null)
        {
            // Рисуем верхний луч (проверка стены)
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(patrolCheckPoint.position, (Vector2)patrolCheckPoint.position + new Vector2(facingDirection * checkDistance, 0));

            // Рисуем нижний луч (проверка земли)
            Gizmos.color = Color.green;
            Vector2 groundCheckPos = (Vector2)patrolCheckPoint.position - new Vector2(0, groundCheckVerticalOffset);
            Gizmos.DrawLine(groundCheckPos, groundCheckPos + new Vector2(facingDirection * checkDistance, 0));
        }

        // Гизмо для проверки "на земле"
        if (isGroundedCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(isGroundedCheck.position, groundingRadius);
        }
    }
}