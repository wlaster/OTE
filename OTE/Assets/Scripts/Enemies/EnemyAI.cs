using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    // === СОСТОЯНИЯ AI ===
    private enum State
    {
        Patrolling,
        Chasing,
        Attacking
    }

    [Header("AI Settings")]
    [SerializeField] private float detectionRange = 5f; // Дистанция обнаружения игрока
    [SerializeField] private float attackRange = 1f;    // Дистанция для атаки
    [SerializeField] private float chaseSpeedMultiplier = 1.5f; // Насколько быстрее враг бежит за игроком

    [Header("References")]
    [SerializeField] private LayerMask playerLayer; // Слой, на котором находится игрок

    // Ссылки на другие компоненты врага
    private EnemyPatrol patrol;
    // private EnemyAttack attack; // Раскомментируем, когда создадим скрипт атаки

    private State currentState;
    private Transform playerTransform;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        patrol = GetComponent<EnemyPatrol>();
        // attack = GetComponent<EnemyAttack>();

        // Находим игрока один раз при старте
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform; // ВАЖНО: У игрока должен быть тег "Player"
    }

    private void Start()
    {
        // Начальное состояние - патрулирование
        currentState = State.Patrolling;
        patrol.enabled = true;
    }

    private void Update()
    {
        // В зависимости от текущего состояния, выполняем разную логику
        switch (currentState)
        {
            case State.Patrolling:
                UpdatePatrollingState();
                break;
            case State.Chasing:
                UpdateChasingState();
                break;
            case State.Attacking:
                UpdateAttackingState();
                break;
        }
    }

    // --- ЛОГИКА СОСТОЯНИЙ ---

    private void UpdatePatrollingState()
    {
        // Если игрок в зоне видимости, переключаемся на преследование
        if (IsPlayerInDetectionRange())
        {
            SwitchState(State.Chasing);
        }
    }

    private void UpdateChasingState()
    {
        // Если игрок вышел из зоны видимости, возвращаемся к патрулированию
        if (!IsPlayerInDetectionRange())
        {
            SwitchState(State.Patrolling);
            return;
        }

        // Если игрок в зоне атаки, переключаемся на атаку
        if (IsPlayerInAttackRange())
        {
            SwitchState(State.Attacking);
            return;
        }

        // Логика преследования: двигаемся в сторону игрока
        // (Реализуем это чуть позже, изменив EnemyPatrol)
    }

    private void UpdateAttackingState()
    {
        // Если игрок вышел из зоны атаки, но все еще виден, возвращаемся к преследованию
        if (!IsPlayerInAttackRange())
        {
            SwitchState(State.Chasing);
            return;
        }

        // Выполняем атаку
        // attack.PerformAttack();
    }

    // --- ПЕРЕКЛЮЧЕНИЕ СОСТОЯНИЙ ---

    private void SwitchState(State newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case State.Patrolling:
                patrol.enabled = true;
                patrol.SetChasing(false); // Говорим патрулю вернуться к обычному режиму
                break;
            case State.Chasing:
                patrol.enabled = true;
                patrol.SetChasing(true); // Говорим патрулю преследовать игрока
                break;
            case State.Attacking:
                patrol.enabled = false; // Останавливаемся для атаки
                rb.linearVelocity = Vector2.zero; // Обнуляем скорость
                break;
        }
    }

    // --- ВСПОМОГАТЕЛЬНЫЕ ФУНКЦИИ ---

    private bool IsPlayerInDetectionRange()
    {
        if (playerTransform == null) return false;
        return Vector2.Distance(transform.position, playerTransform.position) < detectionRange;
    }

    private bool IsPlayerInAttackRange()
    {
        if (playerTransform == null) return false;
        return Vector2.Distance(transform.position, playerTransform.position) < attackRange;
    }

    // Визуализация для настройки
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}