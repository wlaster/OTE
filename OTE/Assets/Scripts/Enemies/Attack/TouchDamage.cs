using UnityEngine;

/// <summary>
/// Наносит урон при контакте (например, шипы или тело врага), с учётом отката по времени.
/// </summary>
public class TouchDamage : MonoBehaviour
{
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private float damageCooldown = 1.0f;
    [SerializeField] private LayerMask targetLayer;

    [SerializeField] private Collider2D hitbox;
 

    private float lastDamageTime;

    /// <summary>
    /// При нахождении объекта в триггере наносит урон, если прошёл кулдаун.
    /// </summary>
    private void OnTriggerStay2D(Collider2D other)
    {
        if ((targetLayer.value & (1 << other.gameObject.layer)) == 0)
        {
            return;
        }

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
