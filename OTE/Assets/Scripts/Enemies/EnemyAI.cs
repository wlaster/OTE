using System.Collections; // Обязательно для использования корутин
using UnityEngine;

[RequireComponent(typeof(EnemyVision))]
public class EnemyAI : MonoBehaviour
{
    public enum State { Patrolling, Chasing }
    
    [Header("AI Settings")]
    [SerializeField] private State currentState = State.Patrolling;
    [Tooltip("Как долго враг будет искать игрока после потери из виду (в секундах).")]
    [SerializeField] private float chaseLostTime = 2f;

    private IMovable movement;
    private EnemyVision vision;

    private Transform currentTarget;
    private bool isTargetVisible = false;
    private Coroutine loseTargetCoroutine; // Ссылка на нашу корутину-таймер

    private void Awake()
    {
        movement = GetComponent<IMovable>();
        vision = GetComponent<EnemyVision>();

        if (movement == null) { /* ... проверка ... */ }

        if (vision != null)
        {
            vision.OnTargetSpotted.AddListener(HandleTargetSpotted);
            vision.OnTargetLost.AddListener(HandleTargetLost);
        }
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Patrolling:
                // Логика патрулирования (пока пустая, т.к. EnemyMovement делает все сам)
                break;
            case State.Chasing:
                UpdateChasingState();
                break;
        }
    }

    private void UpdateChasingState()
    {
        if (currentTarget == null)
        {
            // Если цели по какой-то причине нет, переходим в патруль
            SwitchState(State.Patrolling);
            return;
        }

        // Двигаемся к цели
        movement.MoveTowards(currentTarget.position);
    }

    private void SwitchState(State newState)
    {
        if (currentState == newState) return;
        currentState = newState;
        Debug.Log("Переключил состояние на: " + newState);

        switch (currentState)
        {
            case State.Patrolling:
                movement.StartPatrolling();
                break;
            case State.Chasing:
                movement.StopPatrolling();
                break;
        }
    }

    // --- ОБРАБОТЧИКИ СОБЫТИЙ С НОВОЙ ЛОГИКОЙ ---

    private void HandleTargetSpotted(Transform spottedTarget)
    {
        // Если мы уже преследуем цель, просто обновляем ее позицию
        currentTarget = spottedTarget;

        // Если мы были в процессе потери цели, отменяем этот процесс
        if (loseTargetCoroutine != null)
        {
            StopCoroutine(loseTargetCoroutine);
            loseTargetCoroutine = null;
            Debug.Log("Цель снова замечена, отменяю потерю.");
        }

        // Если мы не преследовали цель, начинаем преследование
        if (!isTargetVisible)
        {
            isTargetVisible = true;
            Debug.Log("ЦЕЛЬ ЗАМЕЧЕНА! Имя цели: " + spottedTarget.name);
            SwitchState(State.Chasing);
        }
    }

    private void HandleTargetLost()
    {
        // Запускаем таймер на потерю цели, только если мы ее действительно видели
        if (isTargetVisible)
        {
            Debug.Log("Цель пропала из виду, запускаю таймер на " + chaseLostTime + " сек.");
            loseTargetCoroutine = StartCoroutine(LoseTargetCoroutine());
        }
    }

    // --- НАША НОВАЯ КОРУТИНА-ТАЙМЕР ---

    private IEnumerator LoseTargetCoroutine()
    {
        // Ждем указанное количество секунд
        yield return new WaitForSeconds(chaseLostTime);

        // Если мы дождались, и цель так и не появилась,
        // то окончательно ее теряем.
        isTargetVisible = false;
        currentTarget = null;
        Debug.Log("Время вышло, цель окончательно потеряна.");
        SwitchState(State.Patrolling);
        
        // Сбрасываем ссылку на корутину
        loseTargetCoroutine = null;
    }

    private void OnDestroy()
    {
        if (vision != null)
        {
            vision.OnTargetSpotted.RemoveListener(HandleTargetSpotted);
            vision.OnTargetLost.RemoveListener(HandleTargetLost);
        }
    }
}