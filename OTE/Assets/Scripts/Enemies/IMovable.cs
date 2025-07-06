using UnityEngine;

// Это интерфейс, а не класс. Он описывает "что можно делать", а не "как это делать".
public interface IMovable 
{
    // Любой, кто реализует этот интерфейс, ДОЛЖЕН иметь эти методы.

    /// <summary>
    /// Двигаться в сторону указанной позиции.
    /// </summary>
    void MoveTowards(Vector2 targetPosition);

    /// <summary>
    /// Включить режим автоматического патрулирования.
    /// </summary>
    void StartPatrolling();

    /// <summary>
    /// Выключить режим патрулирования и остановиться.
    /// </summary>
    void StopPatrolling();
}