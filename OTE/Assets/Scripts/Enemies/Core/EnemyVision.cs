using UnityEngine;
using UnityEngine.Events;
using System.Collections;

/// <summary>
/// Отвечает за обнаружение игрока врагом: проверяет видимость и бросает события при обнаружении/потере.
/// </summary>
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

    
    public bool CanSeePlayer { get; private set; }
    public Transform Player { get; private set; }

    
    [Space]
    [Header("Events")]
    public UnityEvent OnPlayerDetected;
    public UnityEvent OnPlayerLost;
    
    
    private Coroutine _detectionCoroutine;
    private float _timeSincePlayerSeen;

    /// <summary>
    /// Запускает корутину периодической проверки зрения.
    /// </summary>
    private void Start()
    {
        _detectionCoroutine = StartCoroutine(DetectionRoutine());
    }

    /// <summary>
    /// Останавливает корутину при отключении компонента.
    /// </summary>
    private void OnDisable()
    {
        if (_detectionCoroutine != null)
        {
            StopCoroutine(_detectionCoroutine);
        }
    }

    /// <summary>
    /// Периодически вызывает проверку видимости игрока через заданный интервал.
    /// </summary>
    private IEnumerator DetectionRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(detectionInterval);
        while (true)
        {
            HandleDetection();
            yield return wait;
        }
    }

    /// <summary>
    /// Обрабатывает результат проверки видимости: учитывает таймер потери цели и вызывает события.
    /// </summary>
    private void HandleDetection()
    {
        bool isPlayerVisible = IsPlayerInLineOfSight();

        if (CanSeePlayer && !isPlayerVisible)
        {
            _timeSincePlayerSeen += detectionInterval;
            if (_timeSincePlayerSeen >= loseSightTime)
            {
                LosePlayer();
            }
        }
        else if (isPlayerVisible)
        {
            DetectPlayer();
        }
    }

    /// <summary>
    /// Проверяет, находится ли игрок в зоне обнаружения и не закрыт ли он препятствием.
    /// </summary>
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

        return hit.collider == null;
    }

    /// <summary>
    /// Обрабатывает обнаружение игрока: сбрасывает таймер и вызывает событие один раз при входе в зону.
    /// </summary>
    private void DetectPlayer()
    {
        _timeSincePlayerSeen = 0f;
        if (!CanSeePlayer)
        {
            CanSeePlayer = true;
            OnPlayerDetected?.Invoke();
        }
    }

    /// <summary>
    /// Обрабатывает потерю игрока из поля зрения и вызывает соответствующее событие.
    /// </summary>
    private void LosePlayer()
    {
        CanSeePlayer = false;
        Player = null;
        OnPlayerLost?.Invoke();
    }

    /// <summary>
    /// Отрисовка гизмо зоны обнаружения и линии на игрока при детекции (редактор).
    /// </summary>
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
