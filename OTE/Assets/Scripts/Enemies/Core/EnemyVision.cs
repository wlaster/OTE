using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class EnemyVision : MonoBehaviour
{
    [Header("Vision Settings")]
    [Tooltip("Дальность, на которой враг может заметить игрока.")]
    [SerializeField] private float detectionRange = 10f;
    [Tooltip("Время в секундах, через которое враг теряет игрока, если тот вне зоны видимости.")]
    [SerializeField] private float loseSightTime = 3f;
    [Tooltip("Как часто (в секундах) враг будет проверять наличие игрока. Меньшее значение = выше реакция, но больше нагрузка.")]
    [SerializeField] private float detectionInterval = 0.2f;
    [Tooltip("Слой, на котором находится игрок.")]
    [SerializeField] private LayerMask playerLayer;
    [Tooltip("Слои, которые блокируют зрение (стены, земля).")]
    [SerializeField] private LayerMask obstacleLayer;

    // --- ПУБЛИЧНЫЕ СВОЙСТВА ---
    public bool CanSeePlayer { get; private set; }
    public Transform Player { get; private set; }

    // --- СОБЫТИЯ ---
    [Space]
    [Header("Events")]
    public UnityEvent OnPlayerDetected;
    public UnityEvent OnPlayerLost;
    
    // --- Приватные переменные ---
    private Coroutine _detectionCoroutine;
    private float _timeSincePlayerSeen;

    private void Start()
    {
        _detectionCoroutine = StartCoroutine(DetectionRoutine());
    }

    private void OnDisable()
    {
        if (_detectionCoroutine != null)
        {
            StopCoroutine(_detectionCoroutine);
        }
    }

    private IEnumerator DetectionRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(detectionInterval);
        while (true)
        {
            HandleDetection();
            yield return wait;
        }
    }

    private void HandleDetection()
    {
        bool isPlayerVisible = IsPlayerInLineOfSight();

        if (CanSeePlayer && !isPlayerVisible)
        {
            // Игрок был виден, но теперь пропал из виду. Начинаем отсчет времени.
            _timeSincePlayerSeen += detectionInterval;
            if (_timeSincePlayerSeen >= loseSightTime)
            {
                LosePlayer();
            }
        }
        else if (isPlayerVisible)
        {
            // Игрок в поле зрения.
            DetectPlayer();
        }
    }

    private bool IsPlayerInLineOfSight()
    {
        Collider2D playerInDetectionZone = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);

        if (playerInDetectionZone == null)
        {
            return false;
        }

        Player = playerInDetectionZone.transform;
        Vector2 directionToPlayer = (Player.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, Player.position);
        
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleLayer);

        // Если луч ни во что не врезался (кроме самого игрока), значит, игрок в прямой видимости.
        return hit.collider == null;
    }

    private void DetectPlayer()
    {
        _timeSincePlayerSeen = 0f;
        if (!CanSeePlayer)
        {
            CanSeePlayer = true;
            OnPlayerDetected?.Invoke();
        }
    }

    private void LosePlayer()
    {
        CanSeePlayer = false;
        Player = null;
        OnPlayerLost?.Invoke();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        if (CanSeePlayer)
        {
            Gizmos.color = Color.green;
            if (Player != null)
            {
                Gizmos.DrawLine(transform.position, Player.position);
            }
        }
    }
}
