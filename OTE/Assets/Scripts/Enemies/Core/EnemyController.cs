using UnityEngine;

// Убеждаемся, что на враге есть все необходимые компоненты
[RequireComponent(typeof(Rigidbody2D), typeof(EnemyHealth))]
public class EnemyController : MonoBehaviour
{
    [Header("State Machine")]
    private IState currentState;
    // Здесь мы будем создавать экземпляры наших состояний
    public PatrolState patrolState;
    // public ChaseState chaseState;
    // ...

    [Header("Core Components")]
    public Rigidbody2D RB { get; private set; }
    public Animator Anim { get; private set; }
    public EnemyVision Vision { get; private set; }

    public IMovementBehavior MovementBehavior { get; private set; }
    public IAttackBehavior AttackBehavior { get; private set; }

    private void Awake()
    {
        // Получаем ссылки на основные компоненты
        RB = GetComponent<Rigidbody2D>();
        Anim = GetComponentInChildren<Animator>();
        Vision = GetComponentInChildren<EnemyVision>(); // Vision будет на дочернем объекте

        // Проверяем и присваиваем поведения
        MovementBehavior = GetComponent<IMovementBehavior>();
        if (MovementBehavior == null)
        {
            Debug.LogError("На объекте отсутствует компонент, реализующий IMovementBehavior!", this);
        }

        AttackBehavior = GetComponent<IAttackBehavior>();
        if (AttackBehavior == null)
        {
            Debug.LogError("На объекте отсутствует компонент, реализующий IAttackBehavior!", this);
        }
    }

    private void Start()
    {
        // Инициализация состояний и установка начального состояния
        // Например:
        patrolState = new PatrolState(this);
        // chaseState = new ChaseState(this);
        ChangeState(patrolState);
    }

    private void Update()
    {
        // Каждый кадр выполняем логику текущего состояния
        currentState?.Execute();
    }

    // Метод для смены состояний
    public void ChangeState(IState newState)
    {
        // Выходим из старого состояния
        currentState?.Exit();
        
        // Устанавливаем новое состояние
        currentState = newState;
        
        // Входим в новое состояние
        currentState.Enter();
    }
}