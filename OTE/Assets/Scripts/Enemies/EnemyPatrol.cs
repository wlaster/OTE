using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("Patrol Points")]
    [SerializeField] private Transform leftPatrolPoint;
    [SerializeField] private Transform rightPatrolPoint;

    [Header("Ground & Wall Check")]
    [SerializeField] private Transform groundCheckPoint; // Точка чуть впереди и у ног врага
    [SerializeField] private float groundCheckDistance = 1.5f; // Длина "усов"
    [SerializeField] private LayerMask whatIsGround; // Указываем, что считать землей

    private Rigidbody2D rb;
    private bool movingRight = true;
    private Transform currentTarget;

    private bool isChasingPlayer = false;
    private Transform playerTransform;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentTarget = rightPatrolPoint;
        // Находим игрока
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void SetChasing(bool chasing)
    {
        isChasingPlayer = chasing;
    }

    private void FixedUpdate()
    {
        if (isChasingPlayer)
        {
            // --- РЕЖИМ ПРЕСЛЕДОВАНИЯ ---
            if (playerTransform.position.x > transform.position.x && !movingRight) Flip();
            else if (playerTransform.position.x < transform.position.x && movingRight) Flip();
        }
        else
        {
            // --- РЕЖИМ ПАТРУЛИРОВАНИЯ (старая логика) ---
            RaycastHit2D groundInfo = Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckDistance, whatIsGround);
            if (groundInfo.collider == null || IsAtPatrolPoint())
            {
                Flip();
            }
        }

        // Движение (работает для обоих режимов)
        float moveDirection = movingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(moveDirection * moveSpeed, rb.linearVelocity.y);
    }

    // Проверяем, достиг ли враг одной из точек патрулирования
    private bool IsAtPatrolPoint()
    {
        // Если движемся вправо и зашли за правую точку
        if (movingRight && transform.position.x >= rightPatrolPoint.position.x)
        {
            return true;
        }
        // Если движемся влево и зашли за левую точку
        if (!movingRight && transform.position.x <= leftPatrolPoint.position.x)
        {
            return true;
        }
        return false;
    }

    private void Flip()
    {
        movingRight = !movingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    // Визуализация для удобной настройки в редакторе
    private void OnDrawGizmos()
    {
        // Рисуем линию "усов"
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(groundCheckPoint.position, groundCheckPoint.position + Vector3.down * groundCheckDistance);
        }

        // Рисуем линии между точками патрулирования
        if (leftPatrolPoint != null && rightPatrolPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(leftPatrolPoint.position, rightPatrolPoint.position);
        }
    }
}