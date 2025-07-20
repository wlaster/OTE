using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float lifetime = 5f; // Время жизни стрелы в секундах
    [SerializeField] private LayerMask hittableLayers;
    [SerializeField] private LayerMask obstacleLayers;

    private void Start()
    {
        Vector2 direction = transform.right;
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            direction = ((Vector2)playerObj.transform.position - (Vector2)transform.position).normalized;
            // Поворачиваем стрелу по направлению
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        GetComponent<Rigidbody2D>().linearVelocity = direction * speed;
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
            Destroy(gameObject);
            return;
        }

        // Проверяем, столкнулись ли мы с препятствием
        if ((obstacleLayers.value & (1 << other.gameObject.layer)) > 0)
        {
            Destroy(gameObject);
        }
    }
}