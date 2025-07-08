using UnityEngine;

// Перечисление для состояний ИИ
public enum AIState
{
    Patrolling,
    Chasing,
    Attacking
}

// Перечисление для режимов патрулирования
public enum PatrolMode
{
    PatrolUntilWall,
    PatrolInArea
}

[RequireComponent(typeof(EnemyMovement))]
public class EnemyAIController : MonoBehaviour
{
    [Header("Core Components")]
    [SerializeField] private EnemyMovement movementModule;
    // public EnemyAttack attackModule; // Раскомментируйте, когда будет готов модуль атаки
    [SerializeField] private Animator animator;

    [Header("Behavior Toggles")]
    [SerializeField] private bool canPatrol = true;
    [SerializeField] private bool canChase = false;

    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float loseSightRadius = 15f;
    [SerializeField] private LayerMask playerLayer;
    private Transform player;

    [Header("Patrol Settings")]
    [SerializeField] private PatrolMode patrolMode;
    [SerializeField] private float patrolAreaRadius = 5f;
    [SerializeField] private Transform groundWallCheckPoint; // НАША НОВАЯ ТОЧКА
    [SerializeField] private float groundCheckDistance = 1f;
    [SerializeField] private float wallCheckDistance = 0.5f;
    [SerializeField] private LayerMask groundLayer;

    // --- Состояние ИИ ---
    private AIState currentState;

    private void Awake()
    {
        if (movementModule == null) movementModule = GetComponent<EnemyMovement>();
        if (animator == null) animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform; // Находим игрока по тегу
    }

    private void Start()
    {
        currentState = AIState.Patrolling;
    }

    private void Update()
    {
        // Главный цикл конечного автомата
        switch (currentState)
        {
            case AIState.Patrolling:
                HandlePatrollingState();
                break;
            case AIState.Chasing:
                HandleChasingState();
                break;
            case AIState.Attacking:
                HandleAttackingState();
                break;
        }
        
        UpdateAnimations();
    }

    // --- Обработчики состояний ---

    private void HandlePatrollingState()
    {
        // Переход в состояние преследования
        if (canChase && IsPlayerInSight())
        {
            currentState = AIState.Chasing;
            return;
        }

        if (canPatrol)
        {
            if (movementModule.ShouldFlip(patrolMode, patrolAreaRadius, groundWallCheckPoint, wallCheckDistance, groundCheckDistance, groundLayer))
            {
                movementModule.Flip();
            }
            movementModule.MoveInCurrentDirection();
        }
        else
        {
            movementModule.Stop();
        }
    }

    private void HandleChasingState()
    {
        // Переход обратно в патрулирование, если игрок ушел
        if (!IsPlayerInSight(loseSightRadius))
        {
            currentState = AIState.Patrolling;
            return;
        }
        
        // Переход в атаку, если игрок близко
        // if (IsPlayerInAttackRange()) { currentState = AIState.Attacking; return; }

        movementModule.MoveTowards(player.position);
    }

    private void HandleAttackingState()
    {
        movementModule.Stop();
        // attackModule.Attack(player);
        // После атаки нужно решить, куда переходить - в преследование или патруль
        // if (!IsPlayerInAttackRange()) { currentState = AIState.Chasing; }
    }
    
    // --- Вспомогательные функции ---

    private bool IsPlayerInSight(float radius = 0)
    {
        if (player == null) return false;
        
        float currentRadius = (radius == 0) ? detectionRadius : radius;
        return Vector2.Distance(transform.position, player.position) < currentRadius;
    }

    private void UpdateAnimations()
    {
        // Управляем анимацией движения
        // animator.SetBool("isMoving", currentState == AIState.Patrolling || currentState == AIState.Chasing);
    }

    // --- Визуализация в редакторе ---
    private void OnDrawGizmosSelected()
    {
        // Рисуем радиус обнаружения
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Рисуем радиус потери цели
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, loseSightRadius);
        
        // Рисуем лучи для проверки стен и обрывов
        if (groundWallCheckPoint != null)
        {
            int direction = Application.isPlaying ? movementModule.GetDirection() : 1;
            
            // Луч для стены
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(groundWallCheckPoint.position, groundWallCheckPoint.position + (Vector3.right * direction * wallCheckDistance));
            
            // Луч для обрыва
            Gizmos.color = Color.green;
            Gizmos.DrawLine(groundWallCheckPoint.position, groundWallCheckPoint.position + (Vector3.down * groundCheckDistance));
        }
    }
}