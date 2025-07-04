using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    // Обратите внимание, что метод принимает float, как и в вашем скрипте атаки
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " получил урон " + damage + ". Осталось здоровья: " + currentHealth);

        // Тут можно добавить анимацию получения урона

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " умер!");
        // Тут будет анимация смерти и т.д.
        Destroy(gameObject);
    }
}