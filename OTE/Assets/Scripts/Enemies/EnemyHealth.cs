using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [Tooltip("Максимальное количество здоровья врага.")]
    [SerializeField] private float maxHealth = 100f;

    [Header("Feedback on Hit")]
    [Tooltip("Цвет, в который окрасится спрайт при получении урона.")]
    [SerializeField] private Color hurtColor = Color.red;
    [Tooltip("Длительность эффекта мигания в секундах.")]
    [SerializeField] private float hurtFlashDuration = 0.15f;

    // --- СОБЫТИЯ (EVENTS) ---
    [Space]
    [Header("Events")]
    [Tooltip("Срабатывает в момент получения урона.")]
    public UnityEvent OnHit;
    [Tooltip("Срабатывает в момент смерти.")]
    public UnityEvent OnDeath;

    // --- ПРИВАТНЫЕ ПЕРЕМЕННЫЕ ---
    private float currentHealth;
    private bool isDead = false;

    // Ссылки на другие компоненты для управления
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Collider2D mainCollider;
    private Enemy baseEnemyScript;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        mainCollider = GetComponent<Collider2D>();
        baseEnemyScript = GetComponent<Enemy>();

        // Сохраняем оригинальный цвет спрайта
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    // Главный публичный метод, который будут вызывать другие объекты (например, хитбокс игрока)
    public void TakeDamage(float damageAmount, Vector2 knockbackSourcePosition)
    {
        // "Guard Clause" - Защитное условие. Нельзя нанести урон, если враг уже мертв.
        if (isDead)
        {
            return;
        }

        // Уменьшаем здоровье
        currentHealth -= damageAmount;
        Debug.Log($"[EnemyDamage] {gameObject.name} получил {damageAmount} урона. ");

        OnHit?.Invoke();

        // Запускаем визуальный эффект "мигания"
        StartCoroutine(HurtFlashCoroutine());
        animator.SetTrigger("hurt");

        // Проверяем, не достигло ли здоровье нуля
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        // Отключаем ИИ, чтобы враг перестал двигаться и атаковать
        if (baseEnemyScript != null)
        {
            baseEnemyScript.enabled = false;
        }

        // Отключаем коллайдер, чтобы игрок мог пройти сквозь "труп"
        mainCollider.enabled = false;
        
        // Отключаем физику, чтобы тело не падало и не каталось
        if (TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        // Вызываем событие OnDeath. Сюда можно подключить систему выпадения лута, счетчик убийств и т.д.
        OnDeath?.Invoke();

        // Запускаем анимацию смерти, если она есть
        if (TryGetComponent<Animator>(out var animator))
        {
            animator.SetTrigger("death");
        }

        Destroy(gameObject, 2f);
    }

    // Корутина, отвечающая за эффект мигания
    private System.Collections.IEnumerator HurtFlashCoroutine()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = hurtColor;
            yield return new WaitForSeconds(hurtFlashDuration);
            spriteRenderer.color = originalColor;
        }
    }
}