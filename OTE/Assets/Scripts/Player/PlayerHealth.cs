using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Events; 


[System.Serializable]
/// <summary>
/// Событие изменения здоровья: передает (текущееHealth, maxHealth).
/// </summary>
public class HealthChangedEvent : UnityEvent<float, float> { }

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerMovement))]
/// <summary>
/// Отвечает за здоровье игрока: получение урона, инвулнериальность, отбрасывание и смерть.
/// </summary>
public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;

    [Header("Hurt Feedback")]
    [SerializeField] private Color hurtColor = Color.white;
    [SerializeField] private float hurtFlashDuration = 0.1f;
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float invincibilityDuration = 0.5f;

    [Header("Death Settings")]
    [SerializeField] private float deathKnockbackForce = 15f;
    [SerializeField] private Vector2 deathColliderSize = new Vector2(1f, 0.5f);
    [SerializeField] private LayerMask layersToExcludeOnDeath;

    [Header("Knockback Settings")]
    [Tooltip("Угол отскока в градусах. 45 = вверх-вбок, 90 = строго вверх.")]
    [Range(0f, 90f)]
    [SerializeField] private float knockbackAngle = 45f;

    [Space]
    [Header("Events")]
    [Tooltip("Событие, которое вызывается при изменении здоровья. Передает (currentHealth, maxHealth).")]
    public HealthChangedEvent OnHealthChanged;

    
    private Animator animator;
    private Rigidbody2D rb;
    private PlayerController playerController;
    private PlayerMovement playerMovement;
    private SpriteRenderer spriteRenderer;
    private CapsuleCollider2D capsuleCollider;

    
    private float currentHealth;
    private bool isDead = false;
    private bool isInvincible = false;
    private Color originalColor;

    /// <summary>
    /// Инициализирует ссылки на компоненты и задает начальное значение здоровья.
    /// </summary>
    private void Awake()
    {
        if (!TryGetComponent(out animator)) Debug.LogError("Animator не найден на " + gameObject.name);
        if (!TryGetComponent(out rb)) Debug.LogError("Rigidbody2D не найден на " + gameObject.name);
        if (!TryGetComponent(out playerController)) Debug.LogError("PlayerController не найден на " + gameObject.name);
        if (!TryGetComponent(out playerMovement)) Debug.LogError("PlayerMovement не найден на " + gameObject.name);
        if (!TryGetComponent(out spriteRenderer)) Debug.LogError("SpriteRenderer не найден на " + gameObject.name);
        if (!TryGetComponent(out capsuleCollider)) Debug.LogError("CapsuleCollider2D не найден на " + gameObject.name);

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Вызывает событие изменения здоровья при старте (для инициализации UI и слушателей).
    /// </summary>
    private void Start()
    {
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    /// <summary>
    /// Получает урон, запускает эффект урона и проверяет на смерть.
    /// </summary>
    /// <param name="damageAmount">Количество урона.</param>
    /// <param name="knockbackSourcePosition">Позиция источника для расчета отбрасывания.</param>
    public void TakeDamage(float damageAmount, Vector2 knockbackSourcePosition)
    {
        if (isDead || isInvincible)
        {
            return;
        }

        currentHealth -= damageAmount;
        animator.SetTrigger("hurt");
        Debug.Log($"Получено {damageAmount} урона. ");

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        StartCoroutine(HurtSequence(knockbackSourcePosition));

        if (currentHealth <= 0)
        {
            Die(knockbackSourcePosition);
        }
    }

    /// <summary>
    /// Последовательность эффектов после получения урона: инвик и отбрасывание, миг цветом.
    /// </summary>
    private IEnumerator HurtSequence(Vector2 sourcePosition)
    {
        isInvincible = true;

        playerMovement.enabled = false;
        ApplyKnockback(sourcePosition, knockbackForce);

        if (spriteRenderer != null)
        {
            spriteRenderer.color = hurtColor;
            yield return new WaitForSeconds(hurtFlashDuration);
            spriteRenderer.color = originalColor;
        }

        float remainingInvincibility = invincibilityDuration - hurtFlashDuration;
        if (remainingInvincibility > 0)
        {
            yield return new WaitForSeconds(remainingInvincibility);
        }
        isInvincible = false;
        playerMovement.enabled = true;
    }

    /// <summary>
    /// Обрабатывает смерть игрока: отключает контролы, применяет отбрасывание и запускает рестарт уровня.
    /// </summary>
    private void Die(Vector2 sourcePosition)
    {
        isDead = true;
        isInvincible = true;

        playerController.enabled = false;
        playerMovement.enabled = false;

        gameObject.tag = "Untagged";
        gameObject.layer = 0; 

        if (capsuleCollider != null)
        {
            capsuleCollider.sharedMaterial = null;
            capsuleCollider.direction = CapsuleDirection2D.Horizontal;
            capsuleCollider.size = deathColliderSize;
        }

        ApplyKnockback(sourcePosition, deathKnockbackForce);

        if (animator != null)
        {
            animator.SetTrigger("death");
        }

        // StartCoroutine(FreezeAnimationOnDeath());
        // StartCoroutine(RestartLevel());
        StartCoroutine(ShowGameOverWithDelay());
    }

    private IEnumerator ShowGameOverWithDelay()
    {
        yield return new WaitForSeconds(1.5f); // Ждем 1.5 секунды анимации смерти
        LevelManager.Instance.GameOver();      // Показываем экран
    }

    
    /// <summary>
    /// Применяет отбрасывающую силу от заданного источника с учётом угла.
    /// </summary>
    /// <param name="sourcePosition">Позиция источника урона.</param>
    /// <param name="force">Сила отбрасывания.</param>
    private void ApplyKnockback(Vector2 sourcePosition, float force)
    {
        Vector2 directionFromSource = ((Vector2)transform.position - sourcePosition).normalized;

        if (directionFromSource == Vector2.zero)
        {
            directionFromSource = new Vector2(-transform.localScale.x, 0).normalized;
        }

        float directionX = Mathf.Sign(directionFromSource.x);

        float angleInRadians = knockbackAngle * Mathf.Deg2Rad;

        Vector2 knockbackVector = new Vector2(directionX, Mathf.Tan(angleInRadians)).normalized;

        rb.linearVelocity = Vector2.zero; 
        rb.AddForce(knockbackVector * force, ForceMode2D.Impulse);
    }

    
    /// <summary>
    /// Вспышка цвета при ранении (используется при необходимости).
    /// </summary>
    private IEnumerator FlashEffect()
    {
        if (spriteRenderer == null) yield break;

        spriteRenderer.color = hurtColor;
        yield return new WaitForSeconds(hurtFlashDuration);
        spriteRenderer.color = originalColor;
    }

    /// <summary>
    /// Останавливает анимацию через небольшую задержку после смерти для эффекта 'заморозки'.
    /// </summary>
    private IEnumerator FreezeAnimationOnDeath()
    {
        yield return new WaitForSeconds(1f);
        animator.speed = 0;
    }

    /// <summary>
    /// Перезапускает уровень через заданную задержку после смерти.
    /// </summary>
    private IEnumerator RestartLevel()
    {
        yield return new WaitForSeconds(9f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    /// <summary>
    /// Возвращает текущее здоровье игрока.
    /// </summary>
    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    /// <summary>
    /// Устанавливает текущее здоровье (с ограничением) и оповещает слушателей.
    /// </summary>
    public void SetCurrentHealth(float health)
    {
        currentHealth = Mathf.Clamp(health, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}