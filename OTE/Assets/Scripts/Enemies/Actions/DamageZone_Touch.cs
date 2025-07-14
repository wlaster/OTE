using UnityEngine;

public class DamageZone_Touch : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private float touchDamage = 10f;
    [SerializeField] private float cooldown = 1.0f;

    private float nextDamageTime = 0f;

    // Используем OnTriggerStay2D, так как наш коллайдер теперь триггер
    private void OnTriggerStay2D(Collider2D other)
    {
        // Проверяем, что время перезарядки прошло
        if (Time.time < nextDamageTime) return;

        // Проверяем, что в триггер вошел игрок
        if (other.CompareTag("Player"))
        {
            // Ищем компонент здоровья у игрока
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(touchDamage);
                // Устанавливаем время следующего возможного урона
                nextDamageTime = Time.time + cooldown;
            }
        }
    }
}