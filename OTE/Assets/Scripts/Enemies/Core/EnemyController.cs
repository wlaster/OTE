using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(EnemyHealth))]
public class EnemyController : MonoBehaviour
{
    [Header("State Machine")]
    private IState currentState;
    // Экземпляры всех возможных состояний
    public IdleState idleState;
    public PatrolState patrolState;
    public ChaseState chaseState;
    public AttackState attackState;

    [Header("Attack Settings (for transitions)")]
    [Tooltip("Дистанция, на которой враг переходит в состояние атаки.")]
    [SerializeField] private float attackRange = 1.5f;
    // Перезарядку мы убрали отсюда, так как она теперь настраивается в самом модуле атаки

    [Header("Core Components")]
    public Rigidbody2D RB { get; private set; }
    public Animator Anim { get; private set; }
    public EnemyVision Vision { get; private set; }

    [Header("Behaviors (found automatically)")]
    // Ссылки на все возможные поведения, которые могут быть у врага
    public IMovementBehavior PatrolMovement { get; private set; }
    public IMovementBehavior ChaseMovement { get; private set; }
    public IAttackBehavior AttackBehavior { get; private set; } // Теперь у нас одно общее поле для атаки

    // Ссылка на ТЕКУЩЕЕ активное поведение движения
    public IMovementBehavior CurrentMovementBehavior { get; private set; }

    private void Awake()
    {
        // --- Получаем основные компоненты ---
        RB = GetComponent<Rigidbody2D>();
        Anim = GetComponentInChildren<Animator>();
        Vision = GetComponent<EnemyVision>();

        // --- Ищем все доступные поведения ---
        // Движение
        var movementBehaviors = GetComponents<IMovementBehavior>();
        foreach (var behavior in movementBehaviors)
        {
            if (behavior is PatrolMovement) PatrolMovement = behavior;
            if (behavior is ChaseMovement) ChaseMovement = behavior;
            (behavior as MonoBehaviour).enabled = false; // Выключаем все на старте
        }

        // Атака (теперь ищем любой компонент, реализующий интерфейс)
        AttackBehavior = GetComponent<IAttackBehavior>();
        if (AttackBehavior != null)
        {
            // Для атак типа ContactDamage не нужно выключать компонент,
            // но для PatternAttack или RangedAttack это может быть полезно, если они имеют свою логику в Update.
            // Пока оставляем включенным для универсальности.
        }

        // --- Инициализируем состояния ---
        idleState = new IdleState(this);
        patrolState = new PatrolState(this);
        chaseState = new ChaseState(this);
        // Передаем в AttackState настройки из контроллера
        attackState = new AttackState(this, attackRange);
    }

    private void Start()
    {
        // --- Устанавливаем начальное состояние ---
        if (PatrolMovement != null)
        {
            ChangeState(patrolState);
        }
        else
        {
            ChangeState(idleState);
        }
    }

    private void Update()
    {
        currentState?.Execute();
    }

    public void ChangeState(IState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void SetMovementBehavior(IMovementBehavior newBehavior)
    {
        if (CurrentMovementBehavior != null)
        {
            (CurrentMovementBehavior as MonoBehaviour).enabled = false;
        }
        CurrentMovementBehavior = newBehavior;
        if (CurrentMovementBehavior != null)
        {
            (CurrentMovementBehavior as MonoBehaviour).enabled = true;
        }
    }

    public bool IsFacingRight { get; private set; } = true;
    public void Flip()
    {
        IsFacingRight = !IsFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    // Публичное свойство для получения attackRange из состояний
    public float GetAttackRange()
    {
        return attackRange;
    }
}