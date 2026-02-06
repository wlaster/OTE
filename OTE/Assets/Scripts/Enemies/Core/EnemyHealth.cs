using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
/// <summary>
/// Управляет здоровьем врага, эффектами при попадании и поведением при смерти.
/// </summary>
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

    
    [Space]
    [Header("Events")]
    [Tooltip("Срабатывает в момент получения урона.")]
    public UnityEvent OnHit;
    [Tooltip("Срабатывает в момент смерти.")]
    public UnityEvent OnDeath;

    
    private float currentHealth;
    private bool isDead = false;

    
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Collider2D mainCollider;
    private Enemy baseEnemyScript;
    private Animator animator;

    /// <summary>
    /// Инициализация: кэширует компоненты и задаёт стартовое здоровье.
    /// </summary>
    private void Awake()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        mainCollider = GetComponent<Collider2D>();
        baseEnemyScript = GetComponent<Enemy>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    
    /// <summary>
    /// Получает урон и запускает эффект попадания; вызывает смерть при достижении 0 HP.
    /// </summary>
    public void TakeDamage(float damageAmount, Vector2 knockbackSourcePosition)
    {
        if (isDead)
        {
            return;
        }

        currentHealth -= damageAmount;
        Debug.Log($"[EnemyDamage] {gameObject.name} получил {damageAmount} урона. ");

        OnHit?.Invoke();

        StartCoroutine(HurtFlashCoroutine());
        animator.SetTrigger("hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Обрабатывает смерть врага: отключает поведение, запускает анимацию и уничтожает объект.
    /// </summary>
    private void Die()
    {
        isDead = true;

        if (baseEnemyScript != null)
        {
            baseEnemyScript.enabled = false;
        }

        transform.localScale = new Vector3(1.5f,0.5f,1.5f);
        animator.speed = 0;

        if (TryGetComponent<TouchDamage>(out var touchDamage))
        {
            Destroy(touchDamage);
        }

        if (TryGetComponent<CapsuleCollider2D>(out var capsuleCollider))
        {
            capsuleCollider.size /= 2;
        }

        if (TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        OnDeath?.Invoke();

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnEnemyDied();
        }

        Destroy(gameObject, 2f);
    }

    
    /// <summary>
    /// Короткая корутина мигания спрайта при попадании.
    /// </summary>
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