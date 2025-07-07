using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [Header("Detection Settings")]
    [Tooltip("Коллайдер-триггер, который определяет зону видимости. Должен быть на дочернем объекте.")]
    [SerializeField] private Collider2D detectionCollider;
    [Tooltip("Включить проверку на наличие стен между врагом и игроком.")]
    [SerializeField] private bool checkForWalls = true;
    [Tooltip("Слой, на котором находятся препятствия (стены, земля).")]
    [SerializeField] private LayerMask wallLayer;

    // --- Публичные свойства для доступа из других скриптов ---
    public bool IsPlayerDetected { get; private set; }
    public Transform PlayerTarget { get; private set; }

    private Transform selfTransform; // Кэшируем transform для оптимизации

    private void Awake()
    {
        selfTransform = transform;
        if (detectionCollider == null)
        {
            Debug.LogError("Detection Collider не назначен в EnemyVision!", this);
        }
    }

    // Этот метод вызывается автоматически, когда что-то входит в триггер
    private void OnTriggerStay2D(Collider2D other)
    {
        // Проверяем, является ли вошедший объект игроком (по тегу)
        if (other.CompareTag("Player"))
        {
            // Если проверка на стены включена, делаем Raycast
            if (checkForWalls)
            {
                Vector2 directionToPlayer = (other.transform.position - selfTransform.position).normalized;
                float distanceToPlayer = Vector2.Distance(selfTransform.position, other.transform.position);

                // Пускаем луч от врага к игроку
                RaycastHit2D hit = Physics2D.Raycast(selfTransform.position, directionToPlayer, distanceToPlayer, wallLayer);

                // Если луч ни во что не врезался (hit == null), значит, стен нет
                if (!hit.collider)
                {
                    DetectPlayer(other.transform);
                }
                else // Если луч во что-то врезался, значит, есть стена
                {
                    LosePlayer();
                }
            }
            else // Если проверка на стены выключена, просто обнаруживаем игрока
            {
                DetectPlayer(other.transform);
            }
        }
    }

    // Этот метод вызывается, когда объект покидает триггер
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            LosePlayer();
        }
    }

    private void DetectPlayer(Transform playerTransform)
    {
        IsPlayerDetected = true;
        PlayerTarget = playerTransform;
    }

    private void LosePlayer()
    {
        IsPlayerDetected = false;
        PlayerTarget = null;
    }
}