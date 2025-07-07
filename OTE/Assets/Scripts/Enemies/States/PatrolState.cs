using System;

public class PatrolState : IState
{
    private readonly EnemyController controller;

    // Конструктор, который принимает ссылку на главный контроллер
    public PatrolState(EnemyController enemyController)
    {
        controller = enemyController;
    }

    public void Enter()
    {
        // Логика при входе в состояние (например, включить анимацию патруля)
    }

    public void Execute()
    {
        // Каждый кадр вызываем метод движения
        controller.MovementBehavior?.Move(controller.RB, null); // target здесь не нужен
        // Проверяем, не увидел ли враг игрока
        if (controller.Vision != null && controller.Vision.IsPlayerDetected)
        {
            // Если увидел, переключаемся в состояние преследования
            // controller.ChangeState(controller.chaseState); // Эту логику добавим позже
        }
    }

    public void Exit()
    {
        // Логика при выходе из состояния (например, остановить движение)
        controller.RB.linearVelocity = UnityEngine.Vector2.zero;
    }
}