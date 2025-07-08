using UnityEngine;

public class RamAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 1f;

    private float lastAttackTime = -1f; // Инициализируем отрицательным значением, чтобы первая атака сработала сразу

    // Этот метод вызывается КАЖДЫЙ КАДР, пока коллайдеры соприкасаются
    private void OnCollisionStay2D(Collision2D collision)
    {
        // Проверяем, прошло ли достаточно времени с последней атаки
        if (Time.time < lastAttackTime + attackCooldown)
        {
            return; // Если нет, выходим
        }

        // Ищем на объекте, с которым соприкасаемся, компонент IDamageable
        IDamageable damageableObject = collision.gameObject.GetComponent<IDamageable>();

        if (damageableObject != null)
        {
            // Наносим урон
            damageableObject.TakeDamage(attackDamage);

            // Запоминаем время этой атаки, чтобы перезарядка началась
            lastAttackTime = Time.time;
        }
    }
}