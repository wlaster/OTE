using UnityEngine;

public class ChaseState : IState
{
    private readonly EnemyController controller;
    private Transform playerTarget;

    public ChaseState(EnemyController enemyController)
    {
        controller = enemyController;
    }

    public void Enter()
    {
        // Устанавливаем поведение преследования
        controller.SetMovementBehavior(controller.ChaseMovement);
        playerTarget = controller.Vision?.PlayerTarget;
    }

    public void Execute()
    {
        // Если враг может потерять цель ИЛИ у него нет модуля зрения,
        // и цель исчезла, возвращаемся в патруль (если он есть).
        if (controller.Vision != null && !controller.Vision.IsPlayerDetected)
        {
            if (controller.PatrolMovement != null)
            {
                controller.ChangeState(controller.patrolState);
            }
            // Если патруля нет, можно перейти в IdleState
            return;
        }

        // Выполняем движение к цели
        controller.CurrentMovementBehavior?.Move(controller.RB, playerTarget);
    }

    public void Exit()
    {
        controller.RB.linearVelocity = Vector2.zero;
    }
}