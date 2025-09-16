using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerMovement))]
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

    // Ссылки на компоненты
    private Animator animator;
    private Rigidbody2D rb;
    private PlayerController playerController;
    private PlayerMovement playerMovement;
    private SpriteRenderer spriteRenderer;
    private CapsuleCollider2D capsuleCollider;

    // Состояние
    private float currentHealth;
    private bool isDead = false;
    private bool isInvincible = false;
    private Color originalColor;

    private void Awake()
    {
        // Безопасное получение компонентов с помощью TryGetComponent
        if (!TryGetComponent(out animator)) Debug.LogError("Animator не найден на " + gameObject.name);
        if (!TryGetComponent(out rb)) Debug.LogError("Rigidbody2D не найден на " + gameObject.name);
        if (!TryGetComponent(out playerController)) Debug.LogError("PlayerController не найден на " + gameObject.name);
        if (!TryGetComponent(out playerMovement)) Debug.LogError("PlayerMovement не найден на " + gameObject.name);
        if (!TryGetComponent(out spriteRenderer)) Debug.LogError("SpriteRenderer не найден на " + gameObject.name);
        if (!TryGetComponent(out capsuleCollider)) Debug.LogError("CapsuleCollider2D не найден на " + gameObject.name);

        // Сохраняем начальные значения
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        currentHealth = maxHealth;

    }

    public void TakeDamage(float damageAmount, Vector2 knockbackSourcePosition)
    {
        if (isDead || isInvincible)
        {
            return;
        }

        currentHealth -= damageAmount;
        animator.SetTrigger("hurt");
        Debug.Log($"Получено {damageAmount} урона. ");

        // Запускаем корутину, передавая в нее позицию источника урона
        StartCoroutine(HurtSequence(knockbackSourcePosition));

        if (currentHealth <= 0)
        {
            // Передаем позицию источника и в метод смерти для финального отскока
            Die(knockbackSourcePosition);
        }
    }

    private IEnumerator HurtSequence(Vector2 sourcePosition)
    {
        isInvincible = true;

        // 1. Применяем отскок
        playerMovement.enabled = false;
        ApplyKnockback(sourcePosition, knockbackForce);

        // 2. Визуальная обратная связь
        if (spriteRenderer != null)
        {
            spriteRenderer.color = hurtColor;
            yield return new WaitForSeconds(hurtFlashDuration);
            spriteRenderer.color = originalColor;
        }

        // 3. Оставшееся время неуязвимости
        // Убедимся, что не будет отрицательного ожидания
        float remainingInvincibility = invincibilityDuration - hurtFlashDuration;
        if (remainingInvincibility > 0)
        {
            yield return new WaitForSeconds(remainingInvincibility);
        }
        isInvincible = false;
        playerMovement.enabled = true;
    }

    private void Die(Vector2 sourcePosition)
    {
        isDead = true;
        isInvincible = true;

        playerController.enabled = false;
        playerMovement.enabled = false;

        gameObject.tag = "Untagged";
        gameObject.layer = 0; // Слой Default

        if (capsuleCollider != null)
        {
            capsuleCollider.sharedMaterial = null;
            capsuleCollider.direction = CapsuleDirection2D.Horizontal;
            capsuleCollider.size = deathColliderSize;
        }

        // Применяем финальный, более сильный отскок
        ApplyKnockback(sourcePosition, deathKnockbackForce);

        if (animator != null)
        {
            animator.SetTrigger("death");
        }

        StartCoroutine(FreezeAnimationOnDeath());
        StartCoroutine(RestartLevel());
    }

    /// Вычисляет направление и применяет силу отскока к Rigidbody.
    private void ApplyKnockback(Vector2 sourcePosition, float force)
    {
        // 1. Определяем направление ОТ источника К игроку
        Vector2 directionFromSource = ((Vector2)transform.position - sourcePosition).normalized;

        // Если источник урона находится прямо там же, где и игрок (например, урон по таймеру),
        // отталкиваем просто назад, чтобы избежать деления на ноль.
        if (directionFromSource == Vector2.zero)
        {
            directionFromSource = new Vector2(-transform.localScale.x, 0).normalized;
        }

        // 2. Определяем горизонтальное направление (1 или -1)
        float directionX = Mathf.Sign(directionFromSource.x);

        // 3. Конвертируем угол из градусов в радианы для тригонометрии
        float angleInRadians = knockbackAngle * Mathf.Deg2Rad;

        // 4. Вычисляем вектор отскока с заданным углом
        // Мы используем тангенс, чтобы получить нужную высоту (Y) относительно горизонтали (X)
        Vector2 knockbackVector = new Vector2(directionX, Mathf.Tan(angleInRadians)).normalized;

        // 5. Применяем силу
        rb.linearVelocity = Vector2.zero; // Обнуляем скорость для чистого импульса
        rb.AddForce(knockbackVector * force, ForceMode2D.Impulse);
    }

    /// Корутина для визуального эффекта мигания.
    private IEnumerator FlashEffect()
    {
        if (spriteRenderer == null) yield break;

        spriteRenderer.color = hurtColor;
        yield return new WaitForSeconds(hurtFlashDuration);
        spriteRenderer.color = originalColor;
    }

    private IEnumerator FreezeAnimationOnDeath()
    {
        yield return new WaitForSeconds(1f);
        animator.speed = 0;
    }

    private IEnumerator RestartLevel()
    {
        yield return new WaitForSeconds(9f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public float GetCurrentHealth()
    {
    return currentHealth;
    }

    public void SetCurrentHealth(float health)
    {
    currentHealth = health;
    // Здесь можно добавить обновление UI полоски здоровья
    }
}