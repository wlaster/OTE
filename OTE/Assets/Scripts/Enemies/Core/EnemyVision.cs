// EnemyVision.cs
using UnityEngine;
using UnityEngine.Events;

public class EnemyVision : MonoBehaviour
{
    [Header("Vision Settings")]
    [Tooltip("Дальность, на которой враг может заметить игрока.")]
    [SerializeField] private float detectionRange = 10f;
    [Tooltip("Время в секундах, через которое враг теряет игрока, если тот вне зоны видимости.")]
    [SerializeField] private float loseSightTime = 3f;
    [Tooltip("Слой, на котором находится игрок.")]
    [SerializeField] private LayerMask playerLayer;
    [Tooltip("Слои, которые блокируют зрение (стены, земля).")]
    [SerializeField] private LayerMask obstacleLayer;

    // --- ПУБЛИЧНЫЕ СВОЙСТВА ДЛЯ ДРУГИХ СКРИПТОВ ---
    public bool CanSeePlayer { get; private set; }
    public Transform Player { get; private set; }

    // --- СОБЫТИЯ ---
    [Space]
    [Header("Events")]
    public UnityEvent OnPlayerDetected;
    public UnityEvent OnPlayerLost;
    
    // Приватные переменные
    private float timeSincePlayerSeen = 0f;

    private void Update()
    {
        HandleDetection();
    }

    private void HandleDetection()
    {
        bool playerWasSeen = CanSeePlayer; // Запоминаем, видели ли мы игрока в прошлом кадре

        Collider2D playerInDetectionZone = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);

        if (playerInDetectionZone != null)
        {
            // Проверяем наличие прямой видимости (нет стен на пути)
            Vector2 directionToPlayer = (playerInDetectionZone.transform.position - transform.position).normalized;
            float distanceToPlayer = Vector2.Distance(transform.position, playerInDetectionZone.transform.position);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleLayer);

            if (hit.collider == null) // Если луч ни во что не врезался
            {
                // Мы видим игрока
                Player = playerInDetectionZone.transform;
                CanSeePlayer = true;
                timeSincePlayerSeen = 0f;

                // Если мы только что его заметили, вызываем событие
                if (!playerWasSeen)
                {
                    OnPlayerDetected?.Invoke();
                }
                return; // Выходим из функции, так как цель найдена
            }
        }
        
        // Если мы дошли до сюда, значит, прямой видимости нет
        if (CanSeePlayer)
        {
            timeSincePlayerSeen += Time.deltaTime;
            if (timeSincePlayerSeen >= loseSightTime)
            {
                // Теряем цель
                Player = null;
                CanSeePlayer = false;

                // Вызываем событие потери цели
                OnPlayerLost?.Invoke();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = CanSeePlayer ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}