using UnityEngine;

public class HomingProjectile : Projectile
{
    [Header("Homing Settings")]
    [Tooltip("Как быстро снаряд поворачивает к цели (чем меньше, тем больше радиус поворота).")]
    [SerializeField] private float turnSpeed = 5f;
    [Tooltip("Время жизни снаряда в секундах.")]
    [SerializeField] private float lifetime = 5f;

    private Transform target;

    // Этот метод будет вызван магом при создании снаряда
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        // Уничтожаем снаряд через заданное время, если он никуда не попал
        Destroy(gameObject, lifetime);
    }

    // Реализуем абстрактный метод Move
    protected override void Move()
    {
        if (target == null)
        {
            // Если цель потеряна (например, игрок умер), просто летим вперед
            rb.linearVelocity = transform.right * moveSpeed;
            return;
        }

        // --- Логика самонаведения ---
        // 1. Определяем направление к цели
        Vector2 directionToTarget = (target.position - transform.position).normalized;

        // 2. Вычисляем угол, на который нужно повернуться
        float targetAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;

        // 3. Плавно поворачиваем текущий угол вращения к целевому углу
        float angle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, turnSpeed * Time.fixedDeltaTime);

        // 4. Применяем новый угол вращения
        rb.rotation = angle;

        // 5. Двигаемся вперед в новом направлении
        rb.linearVelocity = transform.right * moveSpeed;
    }
}