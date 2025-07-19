using UnityEngine;

public class TouchDamage : MonoBehaviour
{
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private float damageCooldown = 1.0f;
    [SerializeField] private LayerMask targetLayer;

    [SerializeField] private Collider2D hitbox;
 

    private float lastDamageTime;

    private void OnTriggerStay2D(Collider2D hitbox)
    {
        // Проверяем, что столкнулись с нужным слоем
        if (targetLayer.value != (targetLayer.value | (1 << hitbox.gameObject.layer)))
        {
            return;
        }

        // Проверяем перезарядку
        if (Time.time < lastDamageTime + damageCooldown)
        {
            return;
        }

        if (hitbox.TryGetComponent<IDamageable>(out var damageableObject))
        {
            damageableObject.TakeDamage(damageAmount, transform.position);
            lastDamageTime = Time.time;
        }
    }
}