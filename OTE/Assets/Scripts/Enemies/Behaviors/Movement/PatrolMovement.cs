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
    private int direction = 1; // 1 = вправо, -1 = влево
    private bool isDynamicPatrol;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        startPosition = rb.position;
        isDynamicPatrol = patrolDistance > 0;
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
                Flip();
            }
            return;
        }

        if (ShouldTurn())
        {
            isWaiting = true;
            waitTimer = waitTime;
            return;
        }
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
    }

    private bool ShouldTurn()
    {
        bool atObstacle = IsNearWall() || IsAtEdge();
        if (isDynamicPatrol)
        {
            float distanceFromStart = Mathf.Abs(rb.position.x - startPosition.x);
            return distanceFromStart >= patrolDistance || atObstacle;
        }
        else
        {
            return atObstacle;
        }
    }

    // --- НОВАЯ ЛОГИКА ПРОВЕРКИ ---

    // Проверка на стену: пускаем короткий луч ВПЕРЕД из точки patrolCheck
    private bool IsNearWall()
    {
        return Physics2D.Raycast(patrolCheck.position, Vector2.right * direction, wallCheckDistance, groundLayer);
    }

    // Проверка на обрыв: пускаем луч ВНИЗ из точки patrolCheck
    private bool IsAtEdge()
    {
        return !Physics2D.Raycast(patrolCheck.position, Vector2.down, edgeCheckDistance, groundLayer);
    }

    private void Flip()
    {
        direction *= -1;
        transform.Rotate(0f, 180f, 0f);
    }

    // Визуализация лучей для удобной настройки в редакторе
    private void OnDrawGizmos()
    {
        if (patrolCheck != null)
        {
            // Рисуем луч для проверки стены
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(patrolCheck.position, (Vector2)patrolCheck.position + Vector2.right * direction * wallCheckDistance);

            // Рисуем луч для проверки обрыва
            Gizmos.color = Color.green;
            Gizmos.DrawLine(patrolCheck.position, (Vector2)patrolCheck.position + Vector2.down * edgeCheckDistance);
        }
    }
}