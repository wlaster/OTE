using UnityEngine;

public class ContactDamage : MonoBehaviour, IAttackBehavior
{
    [Header("Settings")]
    [SerializeField] private float attackDamage = 10f;
    [Tooltip("Как часто враг может наносить урон касанием.")]
    [SerializeField] private float attackCooldown = 1f;

    private float lastAttackTime;

    // Этот метод по-прежнему остается пустым
    public void Attack(Transform target)
    {
        // No action needed for contact damage
    }

    // --- ИСПОЛЬЗУЕМ OnCollisionStay2D ВМЕСТО OnCollisionEnter2D ---
    // Этот метод вызывается КАЖДЫЙ КАДР, пока объекты соприкасаются
    private void OnCollisionStay2D(Collision2D other)
    {
        // Проверяем, прошло ли достаточно времени с последней атаки
        if (Time.time < lastAttackTime + attackCooldown)
        {
            return; // Если перезарядка не прошла, выходим
        }

        // Ищем на объекте, с которым соприкасаемся, компонент IDamageable
        IDamageable damageableObject = other.gameObject.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            // Наносим урон
            damageableObject.TakeDamage(attackDamage);
            
            // Сбрасываем таймер, запоминая время последней успешной атаки
            lastAttackTime = Time.time;
        }
    }
}