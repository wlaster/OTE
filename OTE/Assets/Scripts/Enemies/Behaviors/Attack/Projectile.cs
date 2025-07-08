using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public abstract class Projectile : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] protected float moveSpeed = 10f;
    [SerializeField] protected float damage = 15f;
    [SerializeField] protected LayerMask obstacleLayer; // Слой для стен и земли

    protected Rigidbody2D rb;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // Делаем снаряд невосприимчивым к гравитации
        rb.gravityScale = 0;
    }

    // Этот метод будет реализован в дочерних классах
    protected abstract void Move();

    private void FixedUpdate()
    {
        // Каждый кадр физики двигаем снаряд
        Move();
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, не столкнулись ли мы с препятствием
        // Используем побитовую операцию, чтобы проверить, входит ли слой объекта в нашу маску
        if ((obstacleLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            // Попали в стену - уничтожаемся
            Destroy(gameObject);
            return;
        }

        // Ищем на цели компонент, который может получать урон
        IDamageable damageableObject = other.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            // Наносим урон и уничтожаемся
            damageableObject.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}