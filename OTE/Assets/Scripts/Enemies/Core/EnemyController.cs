using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(EnemyHealth))]
public class EnemyController : MonoBehaviour
{
    [Header("State Machine")]
    private IState currentState;
    public PatrolState patrolState;
    public ChaseState chaseState;
    public IdleState idleState;

    [Header("Core Components")]
    public Rigidbody2D RB { get; private set; }
    public Animator Anim { get; private set; }
    public EnemyVision Vision { get; private set; }

    [Header("Behaviors")]
    // Ссылки на все возможные поведения, которые могут быть у врага
    public IMovementBehavior PatrolMovement { get; private set; }
    public IMovementBehavior ChaseMovement { get; private set; }
    public IAttackBehavior ContactAttack { get; private set; }
    // public IAttackBehavior RangedAttack { get; private set; }

    public IMovementBehavior CurrentMovementBehavior { get; private set; }

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        Anim = GetComponentInChildren<Animator>();
        Vision = GetComponent<EnemyVision>();

        // Используем GetComponents, чтобы найти все модули, которые реализуют интерфейс
        var movementBehaviors = GetComponents<IMovementBehavior>();
        foreach (var behavior in movementBehaviors)
        {
            if (behavior is PatrolMovement) PatrolMovement = behavior;
            if (behavior is ChaseMovement) ChaseMovement = behavior;

            (behavior as MonoBehaviour).enabled = false;
        }

        var attackBehaviors = GetComponents<IAttackBehavior>();
        foreach (var behavior in attackBehaviors)
        {
            if (behavior is ContactDamage) ContactAttack = behavior;
            // (behavior as MonoBehaviour).enabled = false; // Атака касанием работает через физику, ее не надо выключать
        }

        // Инициализируем состояния
        patrolState = new PatrolState(this);
        chaseState = new ChaseState(this);
        idleState = new IdleState(this);
    }

    private void Start()
    {
        // Устанавливаем начальное состояние.
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

    // Метод для смены состояний
    public void ChangeState(IState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    // метод для установки активного поведения движения
    public void SetMovementBehavior(IMovementBehavior newBehavior)
    {
        // Выключаем текущее активное поведение
        if (CurrentMovementBehavior != null)
        {
            (CurrentMovementBehavior as MonoBehaviour).enabled = false;
        }

        // Устанавливаем и включаем новое
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
}