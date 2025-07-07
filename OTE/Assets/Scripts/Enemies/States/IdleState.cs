public class IdleState : IState
{
    private readonly EnemyController controller;

    public IdleState(EnemyController enemyController)
    {
        controller = enemyController;
    }

    public void Enter()
    {
        // При входе в состояние ожидания, выключаем любое движение
        controller.SetMovementBehavior(null);
    }

    public void Execute()
    {
        // Постоянно проверяем, не увидел ли враг игрока
        // Если у врага есть "глаза" и "модуль преследования"...
        if (controller.Vision != null && controller.Vision.IsPlayerDetected && controller.ChaseMovement != null)
        {
            // ...то переходим в состояние погони
            controller.ChangeState(controller.chaseState);
        }
    }

    public void Exit()
    {
        // Ничего особенного делать не нужно
    }
}