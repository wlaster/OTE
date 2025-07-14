using UnityEngine;

public class GroundEdgeDetector : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private Transform groundCheckPoint; // Точка для проверки земли под ногами
    [SerializeField] private Transform wallCheckPoint;  // Точка для проверки стены перед собой
    [SerializeField] private float checkDistance = 0.2f;  // Дистанция проверки
    [SerializeField] private LayerMask groundLayer;     // Слой, который считается землей/стенами

    // Публичное свойство, которое другие скрипты могут только читать
    public bool IsTouchingEdge { get; private set; }

    private void Update()
    {
        // Проверяем, есть ли земля под ногами впереди
        bool isGroundedAhead = Physics2D.Raycast(groundCheckPoint.position, Vector2.down, checkDistance, groundLayer);
        
        // Проверяем, есть ли стена перед нами
        bool isTouchingWall = Physics2D.Raycast(wallCheckPoint.position, transform.right, checkDistance, groundLayer);

        // Если впереди обрыв ИЛИ стена, значит, мы у края
        IsTouchingEdge = !isGroundedAhead || isTouchingWall;
    }

    // Визуализация для удобной настройки в редакторе
    private void OnDrawGizmos()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(groundCheckPoint.position, groundCheckPoint.position + Vector3.down * checkDistance);
        }
        if (wallCheckPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(wallCheckPoint.position, wallCheckPoint.position + (Vector3)transform.right * checkDistance);
        }
    }
}