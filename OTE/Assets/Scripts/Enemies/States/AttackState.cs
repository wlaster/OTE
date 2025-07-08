using UnityEngine;

public class AttackState : IState
{
    private readonly EnemyController controller;
    private readonly float attackRange;
    private readonly float attackCooldown; // Можно будет задать в будущем

    public AttackState(EnemyController enemyController, float range)
{
    controller = enemyController;
    attackRange = range;
}

    public void Enter()
    {
        // При входе в состояние атаки, останавливаем движение
        controller.RB.linearVelocity = Vector2.zero;
        // Можно также выключить модуль движения для надежности
        controller.SetMovementBehavior(null);
    }

    public void Execute()
    {
        // Поворачиваемся в сторону игрока, если он еще виден
        if (controller.Vision != null && controller.Vision.IsPlayerDetected)
        {
            var playerPosition = controller.Vision.PlayerTarget.position;
            var selfPosition = controller.transform.position;
            
            bool shouldFaceRight = playerPosition.x > selfPosition.x;
            if (shouldFaceRight != controller.IsFacingRight)
            {
                controller.Flip();
            }
        }

        // Выполняем атаку
        controller.AttackBehavior?.Attack(controller.Vision?.PlayerTarget);

        // Проверяем, не пора ли вернуться к преследованию
        // Если игрок вышел из зоны атаки, но все еще виден
        float distanceToPlayer = Vector2.Distance(controller.transform.position, controller.Vision.PlayerTarget.position);
        if (distanceToPlayer > attackRange)
        {
            controller.ChangeState(controller.chaseState);
        }
        // Если игрок вообще пропал из вида
        else if (controller.Vision != null && !controller.Vision.IsPlayerDetected && controller.PatrolMovement != null)
        {
            controller.ChangeState(controller.patrolState);
        }
    }

    public void Exit()
    {
        // Ничего особенного делать не нужно
    }
}