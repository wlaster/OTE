using UnityEngine;

public class StraightProjectile : Projectile
{
    private Vector2 initialDirection;

    // Этот метод будет вызван лучником при создании снаряда
    public void SetDirection(Vector2 direction)
    {
        initialDirection = direction.normalized;
        
        // Поворачиваем спрайт стрелы в сторону полета
        float angle = Mathf.Atan2(initialDirection.y, initialDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    // Реализуем абстрактный метод Move
    protected override void Move()
    {
        // Просто летим вперед с постоянной скоростью
        rb.linearVelocity = initialDirection * moveSpeed;
    }
}