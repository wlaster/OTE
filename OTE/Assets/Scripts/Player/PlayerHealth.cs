using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    public UnityEvent OnPlayerDeath;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Игрок получил урон: " + damage + ". Осталось здоровья: " + currentHealth);

        // Здесь можно добавить эффект получения урона (мигание, звук)

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Игрок умер!");
        OnPlayerDeath?.Invoke();
        
        // Здесь будет логика экрана смерти, перезапуска и т.д.
        // Пока просто выключим объект игрока для наглядности.
        gameObject.SetActive(false);
    }
}