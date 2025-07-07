public class PatrolState : IState
{
    private readonly EnemyController controller;

    public PatrolState(EnemyController enemyController)
    {
        controller = enemyController;
    }

    public void Enter()
    {
        // При входе в состояние патруля, устанавливаем соответствующее поведение
        controller.SetMovementBehavior(controller.PatrolMovement);
    }

    public void Execute()
    {
        // Выполняем движение, если оно есть
        controller.CurrentMovementBehavior?.Move(controller.RB, null);

        // Проверяем, есть ли у нас модуль зрения и модуль преследования,
        // и только тогда пытаемся перейти в состояние погони.
        if (controller.Vision != null && controller.Vision.IsPlayerDetected && controller.ChaseMovement != null)
        {
            controller.ChangeState(controller.chaseState);
        }
    }

    public void Exit()
    {
        // Ничего не делаем при выходе, смена поведения произойдет в Enter нового состояния
    }
}