using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemyVision), typeof(EnemyAttack))]
public class EnemyAI : MonoBehaviour
{
    // Определяем возможные состояния врага
    public enum State { Patrolling, Chasing, Attacking }
    
    [Header("AI Settings")]
    [SerializeField] private State currentState = State.Patrolling;
    [SerializeField] private float chaseLostTime = 2f; // Задержка перед потерей цели
    [SerializeField] private float attackRange = 1f;   // Дистанция для начала атаки

    // Ссылки на модули
    private IMovable movement;
    private EnemyVision vision;
    private EnemyAttack attack;

    // Переменные состояния
    private Transform currentTarget;
    private Coroutine loseTargetCoroutine;

    private void Awake()
    {
        // Получаем ссылки на компоненты
        movement = GetComponent<IMovable>();
        vision = GetComponent<EnemyVision>();
        attack = GetComponent<EnemyAttack>();

        if (movement == null)
        {
            Debug.LogError("На враге отсутствует компонент, реализующий IMovable.", this);
            this.enabled = false;
        }

        // Подписываемся на события от других модулей
        vision.OnTargetSpotted.AddListener(HandleTargetSpotted);
        vision.OnTargetLost.AddListener(HandleTargetLost);
        attack.OnAttackFinished.AddListener(HandleAttackFinished);
    }

    private void Update()
    {
        // Главная машина состояний
        switch (currentState)
        {
            case State.Patrolling:
                // Патрулирование управляется модулем EnemyMovement
                break;
            case State.Chasing:
                UpdateChasingState();
                break;
            case State.Attacking:
                // В состоянии атаки просто ждем ее завершения
                break;
        }
    }

    // Логика поведения в состоянии преследования
    private void UpdateChasingState()
    {
        if (currentTarget == null)
        {
            SwitchState(State.Patrolling);
            return;
        }

        float distanceToTarget = Vector2.Distance(transform.position, currentTarget.position);
        if (distanceToTarget <= attackRange)
        {
            SwitchState(State.Attacking); // Если цель близко, атакуем
        }
        else
        {
            movement.MoveTowards(currentTarget.position); // Иначе продолжаем преследование
        }
    }

    // Центральный метод для смены состояний
    private void SwitchState(State newState)
    {
        if (currentState == newState) return;
        currentState = newState;

        switch (currentState)
        {
            case State.Patrolling:
                movement.StartPatrolling();
                break;
            case State.Chasing:
                movement.StopPatrolling();
                break;
            case State.Attacking:
                movement.StopPatrolling(); // Останавливаемся перед атакой
                attack.PerformAttack();   // Отдаем команду на атаку
                break;
        }
    }

    // Вызывается, когда цель замечена
    private void HandleTargetSpotted(Transform spottedTarget)
    {
        currentTarget = spottedTarget;
        if (loseTargetCoroutine != null)
        {
            StopCoroutine(loseTargetCoroutine);
            loseTargetCoroutine = null;
        }
        
        if (currentState != State.Attacking)
        {
            SwitchState(State.Chasing);
        }
    }

    // Вызывается, когда цель пропала из виду
    private void HandleTargetLost()
    {
        if (currentState == State.Attacking) return;
        
        loseTargetCoroutine = StartCoroutine(LoseTargetCoroutine());
    }
    
    // Вызывается, когда модуль атаки закончил свою работу
    private void HandleAttackFinished()
    {
        // Решаем, что делать после атаки
        if (currentTarget != null && vision.IsTargetVisible())
        {
            SwitchState(State.Chasing); // Если цель еще видна, снова преследуем
        }
        else
        {
            SwitchState(State.Patrolling); // Если цель потеряна, патрулируем
        }
    }

    // Таймер для задержки перед окончательной потерей цели
    private IEnumerator LoseTargetCoroutine()
    {
        yield return new WaitForSeconds(chaseLostTime);

        currentTarget = null;
        SwitchState(State.Patrolling);
        loseTargetCoroutine = null;
    }

    // Отписываемся от событий при уничтожении объекта, чтобы избежать ошибок
    private void OnDestroy()
    {
        if (vision != null)
        {
            vision.OnTargetSpotted.RemoveListener(HandleTargetSpotted);
            vision.OnTargetLost.RemoveListener(HandleTargetLost);
        }
        if (attack != null)
        {
            attack.OnAttackFinished.RemoveListener(HandleAttackFinished);
        }
    }
}