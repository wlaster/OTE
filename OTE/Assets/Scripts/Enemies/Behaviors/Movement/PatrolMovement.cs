using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PatrolMovement : MonoBehaviour, IMovementBehavior
{
    [Header("Patrol Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [Tooltip("Дистанция, на которую враг отходит от стартовой точки. Если 0, патрулирует от стены до стены.")]
    [SerializeField] private float patrolDistance = 5f;
    [Tooltip("Время ожидания в конечной точке патруля.")]
    [SerializeField] private float waitTime = 1f;

    [Header("Detection Points")]
    [Tooltip("Точка для проверки стен и обрывов. Должна быть дочерним объектом, расположенным перед врагом.")]
    [SerializeField] private Transform patrolCheck;
    [Tooltip("Слой, на котором находятся земля и стены.")]
    [SerializeField] private LayerMask groundLayer;
    [Tooltip("Длина луча для проверки стены.")]
    [SerializeField] private float wallCheckDistance = 0.1f;
    [Tooltip("Длина луча для проверки обрыва.")]
    [SerializeField] private float edgeCheckDistance = 1f;

    // --- Приватные переменные ---
    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 startPosition;
    private bool isWaiting = false;
    private float waitTimer;
    private bool isDynamicPatrol;

    private EnemyController controller;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        startPosition = rb.position;
        isDynamicPatrol = patrolDistance > 0;
        controller = GetComponent<EnemyController>();
    }

    public void Move(Rigidbody2D rigidbody, Transform target)
    {
        Patrol();
    }

    private void Patrol()
    {
        if (isWaiting)
        {   
            waitTimer -= Time.deltaTime;
            rb.linearVelocity = Vector2.zero;
            if (waitTimer <= 0)
            {
                isWaiting = false;
                controller.Flip();
            }
            return;
        }

        if (ShouldTurn())
        {
            isWaiting = true;
            waitTimer = waitTime;
            return;
        }
        float direction = controller.IsFacingRight ? 1 : -1;
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
    }

    private bool ShouldTurn()
    {
        bool atObstacle = IsNearWall() || IsAtEdge();
        if (isDynamicPatrol)
        {
            bool atPatrolLimit = false;
            float currentDirection = controller.IsFacingRight ? 1 : -1;

            if (currentDirection > 0) // Если идем вправо
            {
                // Проверяем, не зашли ли мы за правую границу
                atPatrolLimit = rb.position.x >= startPosition.x + patrolDistance;
            }
            else // Если идем влево
            {
                // Проверяем, не зашли ли мы за левую границу
                atPatrolLimit = rb.position.x <= startPosition.x - patrolDistance;
            }

            return atPatrolLimit || atObstacle;
        }
        else
        {
            return atObstacle;
        }
    }

    // Проверка на стену: пускаем короткий луч ВПЕРЕД из точки patrolCheck
    private bool IsNearWall()
    {
        // Используем направление из контроллера
        float direction = controller.IsFacingRight ? 1 : -1;
        return Physics2D.Raycast(patrolCheck.position, Vector2.right * direction, wallCheckDistance, groundLayer);
    }

    // Проверка на обрыв: пускаем луч ВНИЗ из точки patrolCheck
    private bool IsAtEdge()
    {
        float direction = controller.IsFacingRight ? 1 : -1;
        Vector2 checkPosition = new Vector2(patrolCheck.position.x + (wallCheckDistance * direction), patrolCheck.position.y);
        return !Physics2D.Raycast(checkPosition, Vector2.down, edgeCheckDistance, groundLayer);
    }

    // Визуализация лучей для удобной настройки в редакторе
    private void OnDrawGizmos()
    {
        if (patrolCheck == null)
            return;

        float gizmoDirection = 1;

        if (Application.isPlaying && controller != null)
        {
            gizmoDirection = controller.IsFacingRight ? 1 : -1;
        }
        else
        {
            if (transform.lossyScale.x < 0)
            {
                gizmoDirection = -1;
            }
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(patrolCheck.position, (Vector2)patrolCheck.position + Vector2.right * gizmoDirection * wallCheckDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(patrolCheck.position, (Vector2)patrolCheck.position + Vector2.down * edgeCheckDistance);
    }
}