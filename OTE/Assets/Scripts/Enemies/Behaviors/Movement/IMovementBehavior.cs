using UnityEngine;

public interface IMovementBehavior
{
    // Метод для выполнения движения
    void Move(Rigidbody2D rigidbody, Transform target);
}