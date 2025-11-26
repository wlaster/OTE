using UnityEngine;

/// <summary>
/// Префаб стрелы: движется в направлении цели и наносит урон при попадании.
/// </summary>
public class Arrow : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float lifetime = 5f; 
    [SerializeField] private LayerMask hittableLayers;
    [SerializeField] private LayerMask obstacleLayers;

    private Rigidbody2D rb;

    /// <summary>
    /// Кэширует Rigidbody2D.
    /// </summary>
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Планирует уничтожение стрелы через `lifetime` секунд.
    /// </summary>
    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    /// <summary>
    /// Инициализирует траекторию стрелы в направлении цели и задаёт скорость.
    /// </summary>
    public void Initialize(Transform target)
    {
        Vector2 direction = (target.position - transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        rb.linearVelocity = direction * speed;
    }

    /// <summary>
    /// Обработка столкновений: наносит урон по слоям и уничтожает себя при попадании в препятствие.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((hittableLayers.value & (1 << other.gameObject.layer)) > 0)
        {
            if (other.TryGetComponent<IDamageable>(out var damageableObject))
            {
                damageableObject.TakeDamage(damage, transform.position);
            }
            Destroy(gameObject);
            return;
        }

        if ((obstacleLayers.value & (1 << other.gameObject.layer)) > 0)
        {
            Destroy(gameObject);
        }
    }
}
