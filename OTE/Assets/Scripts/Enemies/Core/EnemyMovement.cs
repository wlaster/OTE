using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;

    // --- Приватные переменные ---
    private Vector2 startingPosition; // Для режима патрулирования в области
    private int moveDirection = 1; // 1 = вправо, -1 = влево

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        startingPosition = transform.position;
    }

    // Главный метод, который вызывается из AI контроллера
    public void MoveInCurrentDirection()
    {
        rb.linearVelocity = new Vector2(moveSpeed * moveDirection, rb.linearVelocity.y);
    }

    // Метод для остановки
    public void Stop()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    // Метод для преследования цели
    public void MoveTowards(Vector2 targetPosition)
    {
        float directionToTarget = Mathf.Sign(targetPosition.x - transform.position.x);
        
        // Поворачиваемся, если нужно
        if (directionToTarget != moveDirection && directionToTarget != 0)
        {
            Flip();
        }
        
        MoveInCurrentDirection();
    }

    // Проверяем, нужно ли развернуться
    public bool ShouldFlip(PatrolMode mode, float areaRadius, Transform checkPoint, float wallCheckDist, float groundCheckDist, LayerMask groundLayer)
    {
        bool isNearWall = Physics2D.Raycast(checkPoint.position, Vector2.right * moveDirection, wallCheckDist, groundLayer);
        bool isNearLedge = !Physics2D.Raycast(checkPoint.position, Vector2.down, groundCheckDist, groundLayer);
        
        if (isNearWall || isNearLedge)
        {
            return true;
        }

        if (mode == PatrolMode.PatrolInArea)
        {
            if (Mathf.Abs(transform.position.x - startingPosition.x) > areaRadius)
            {
                // Разворачиваемся, только если движемся ОТ центра зоны
                if (Mathf.Sign(transform.position.x - startingPosition.x) == moveDirection)
                {
                    return true;
                }
            }
        }
        
        return false;
    }

    // Разворот
    public void Flip()
    {
        moveDirection *= -1;
        transform.Rotate(0f, 180f, 0f);
    }
    
    // Публичный метод, чтобы AI мог узнать направление
    public int GetDirection()
    {
        return moveDirection;
    }
}