using UnityEngine;

// Требуем, чтобы на объекте были эти компоненты
[RequireComponent(typeof(Rigidbody2D), typeof(GroundEdgeDetector))]
public class AI_Patrol_Ground : MonoBehaviour
{
    // Перечисление для выбора режима в инспекторе
    public enum PatrolMode { PatrolUntilEdge, PatrolShortDistance }

    [Header("Patrol Settings")]
    [SerializeField] private PatrolMode mode = PatrolMode.PatrolUntilEdge;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float shortPatrolDistance = 3f; // Дистанция для короткого патруля

    // Компоненты
    private Rigidbody2D rb;
    private GroundEdgeDetector edgeDetector;

    // Состояние
    private int moveDirection = 1; // 1 = вправо, -1 = влево
    private Vector3 startPosition;
    private bool isWaiting = false; // Флаг для ожидания на краю короткого патруля

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        edgeDetector = GetComponent<GroundEdgeDetector>();
        startPosition = transform.position;
    }

    private void Update()
    {
        if (isWaiting) return; // Если ждем, ничего не делаем

        // Выбираем логику в зависимости от режима
        switch (mode)
        {
            case PatrolMode.PatrolUntilEdge:
                if (edgeDetector.IsTouchingEdge)
                {
                    Flip();
                }
                break;
            
            case PatrolMode.PatrolShortDistance:
                if (Mathf.Abs(transform.position.x - startPosition.x) >= shortPatrolDistance)
                {
                    // Достигли края короткого патруля, начинаем ожидание
                    StartCoroutine(WaitAndFlip());
                }
                // Дополнительно проверяем край, чтобы не упасть
                else if (edgeDetector.IsTouchingEdge)
                {
                    Flip();
                }
                break;
        }
    }

    private void FixedUpdate()
    {
        if (isWaiting)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Останавливаемся во время ожидания
            return;
        }
        
        // Двигаем врага
        rb.linearVelocity = new Vector2(moveSpeed * moveDirection, rb.linearVelocity.y);
    }

    private void Flip()
    {
        moveDirection *= -1; // Меняем направление
        transform.Rotate(0f, 180f, 0f); // Поворачиваем спрайт
        
        // Если это короткий патруль, обновляем стартовую позицию после разворота
        if (mode == PatrolMode.PatrolShortDistance)
        {
            startPosition = transform.position;
        }
    }
    
    // Корутина для ожидания на краю короткого патруля
    private System.Collections.IEnumerator WaitAndFlip()
    {
        isWaiting = true;
        yield return new WaitForSeconds(1.5f); // Ждем 1.5 секунды
        Flip();
        isWaiting = false;
    }
}