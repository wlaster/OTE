using UnityEngine;

public class TouchDamage : MonoBehaviour
{
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private float damageCooldown = 1.0f;
    [SerializeField] private LayerMask targetLayer;

    [SerializeField] private Collider2D hitbox;
 

    private float lastDamageTime;

    private void OnTriggerStay2D(Collider2D other)
    {
        // Проверяем, что столкнулись с нужным слоем. Эта проверка более стандартная и понятная.
        if ((targetLayer.value & (1 << other.gameObject.layer)) == 0)
        {
            return;
        }

        // Проверяем перезарядку
        if (Time.time < lastDamageTime + damageCooldown)
        {
            return;
        }

        if (other.TryGetComponent<IDamageable>(out var damageableObject))
        {
            damageableObject.TakeDamage(damageAmount, transform.position);
            lastDamageTime = Time.time;
        }
    }
}
