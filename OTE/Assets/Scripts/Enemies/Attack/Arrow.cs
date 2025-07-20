using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float lifetime = 5f; // Время жизни стрелы в секундах
    [SerializeField] private LayerMask hittableLayers; // Слои, которым стрела наносит урон (Player)
    [SerializeField] private LayerMask obstacleLayers; // Слои, об которые стрела ломается (Ground, Walls)

    private void Start()
    {
        // Задаем начальную скорость
        GetComponent<Rigidbody2D>().linearVelocity = transform.right * speed;
        // Устанавливаем таймер на самоуничтожение
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, попали ли мы в цель, которой можно нанести урон
        if ((hittableLayers.value & (1 << other.gameObject.layer)) > 0)
        {
            if (other.TryGetComponent<IDamageable>(out var damageableObject))
            {
                damageableObject.TakeDamage(damage, transform.position);
            }
            Destroy(gameObject); // Уничтожаем стрелу при попадании в цель
            return;
        }

        // Проверяем, столкнулись ли мы с препятствием
        if ((obstacleLayers.value & (1 << other.gameObject.layer)) > 0)
        {
            Destroy(gameObject); // Уничтожаем стрелу при столкновении со стеной
        }
    }
}