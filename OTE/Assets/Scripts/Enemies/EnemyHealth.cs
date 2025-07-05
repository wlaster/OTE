using UnityEngine;
using UnityEngine.Events; // Обязательно для использования UnityEvents

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    
    [Header("Feedback on Hit")]
    [SerializeField] private Color hurtColor = Color.red; // Каким цветом мигнет враг
    [SerializeField] private float hurtDuration = 0.1f;   // Как долго он будет мигать

    // --- СОБЫТИЯ ---
    // На эти события можно будет подписать другие скрипты прямо в инспекторе
    [Space]
    [Header("Events")]
    public UnityEvent OnHit;
    public UnityEvent OnDeath;

    // --- Приватные переменные ---
    private float currentHealth;
    private bool isDead = false;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Collider2D enemyCollider;
    // private EnemyAI enemyAI; // Раскомментируйте, когда у нас появится скрипт ИИ

    private void Awake()
    {
        // Получаем ссылки на компоненты один раз
        spriteRenderer = GetComponentInChildren<SpriteRenderer>(); // GetComponentInChildren найдет спрайт даже на дочернем объекте
        enemyCollider = GetComponent<Collider2D>();
        // enemyAI = GetComponent<EnemyAI>(); // Раскомментируйте, когда у нас появится скрипт ИИ

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        currentHealth = maxHealth;
    }

    // Главный публичный метод для нанесения урона
    public void TakeDamage(float damage)
    {
        // Нельзя нанести урон мертвому или неуязвимому врагу
        if (isDead)
        {
            return;
        }
        
        // Уменьшаем здоровье
        currentHealth -= damage;
        
        // Вызываем событие OnHit. Все, кто на него подписан, сработают.
        OnHit?.Invoke();
        
        // Запускаем визуальный эффект
        StartCoroutine(HurtFlash());

        // Проверяем, не умер ли враг
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        // Выключаем физику и ИИ, чтобы враг перестал быть угрозой
        enemyCollider.enabled = false;
        // if (enemyAI != null) enemyAI.enabled = false; // Раскомментируйте, когда у нас появится скрипт ИИ
        
        // Вызываем событие смерти. На это может быть подписан счетчик убийств, система лута и т.д.
        OnDeath?.Invoke();

        // Тут можно запустить триггер анимации смерти
        // GetComponent<Animator>().SetTrigger("death");

        // Уничтожаем объект с задержкой, чтобы успела проиграться анимация
        Destroy(gameObject, 2f); // Например, уничтожить через 2 секунды
    }

    // Корутина для визуального эффекта "мигания"
    private System.Collections.IEnumerator HurtFlash()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = hurtColor;
            yield return new WaitForSeconds(hurtDuration);
            spriteRenderer.color = originalColor;
        }
    }
}