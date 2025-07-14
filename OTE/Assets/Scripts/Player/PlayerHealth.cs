using UnityEngine;
using System.Collections;
using UnityEditor.U2D.Aseprite;
using NUnit.Framework.Constraints;
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
        
        // SpriteRenderer может быть на дочернем объекте
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null) Debug.LogError("SpriteRenderer не найден на " + gameObject.name + " или его дочерних объектах");
        
        if (!TryGetComponent(out capsuleCollider)) Debug.LogError("CapsuleCollider2D не найден на " + gameObject.name);

        // Сохраняем начальные значения
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damageAmount)
    {
        if (isDead || isInvincible)
        {
            return;
        }

        currentHealth -= damageAmount;
        animator.SetTrigger("hurt");
        StartCoroutine(HurtSequence());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator HurtSequence()
    {
        isInvincible = true;

        // 1. Определяем, куда смотрит игрок.
        // Мы используем transform.localScale.x, так как наш PlayerMovement меняет его для поворота.
        // Если scale.x > 0, смотрит вправо (направление 1).
        // Если scale.x < 0, смотрит влево (направление -1).
        float playerDirection = Mathf.Sign(transform.localScale.x);

        // 2. Направление отскока - В ПРОТИВОПОЛОЖНУЮ сторону от взгляда игрока.
        float knockbackDirectionX = -playerDirection;

        // 3. Создаем вектор отскока под 45 градусов вверх
        Vector2 knockbackDirection = new Vector2(knockbackDirectionX, 1f).normalized;

        // 4. Применяем силу
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

        // 5. Визуальная обратная связь
        spriteRenderer.color = hurtColor;
        yield return new WaitForSeconds(hurtFlashDuration);
        spriteRenderer.color = originalColor;

        // 6. Оставшееся время неуязвимости
        yield return new WaitForSeconds(invincibilityDuration - hurtFlashDuration);
        isInvincible = false;
    }

    private void Die()
    {
        isDead = true;
        isInvincible = true;

        // Отключаем управление и меняем физические свойства
        playerController.enabled = false;
        playerMovement.enabled = false;
        rb.excludeLayers = layersToExcludeOnDeath;
        gameObject.tag = "Untagged"; // Чтобы враги перестали нацеливаться

        // Применяем отскок при смерти
        ApplyKnockback(deathKnockbackForce);

        // Изменяем коллайдер, чтобы игрок "упал" на землю
        if (capsuleCollider != null)
        {
            capsuleCollider.direction = CapsuleDirection2D.Horizontal;
            capsuleCollider.size = deathColliderSize;
        }

        if (animator != null)
        {
            animator.SetTrigger("death");
        }
        
        StartCoroutine(RestartLevelAfterDelay(2f));
    }

    /// <summary>
    /// Вычисляет направление и применяет силу отскока к Rigidbody.
    /// </summary>
    /// <param name="force">Сила отскока.</param>
    private void ApplyKnockback(float force)
    {
        // 1. Определяем направление, в котором смотрит игрок (1 = вправо, -1 = влево)
        // Это зависит от того, как реализован поворот в PlayerMovement.
        // Если через transform.Rotate, то localScale.x не меняется.
        // Если через изменение localScale.x, то этот метод работает.
        // Давайте сделаем более надежный способ, который не зависит от реализации поворота.
        // Мы возьмем его из PlayerMovement, если он есть.
        float playerFacingDirection = 1f;
        if (playerMovement != null)
        {
            // Предполагаем, что в PlayerMovement есть публичный метод или свойство для получения направления
            // Если его нет, нужно будет добавить. А пока используем transform.localScale.x
            playerFacingDirection = Mathf.Sign(transform.localScale.x);
        }

        // 2. Направление отскока - В ПРОТИВОПОЛОЖНУЮ сторону от взгляда
        float knockbackDirectionX = -playerFacingDirection;

        // 3. Создаем вектор отскока под 45 градусов вверх.
        // Вектор (1, 1) или (-1, 1) - это и есть направление под 45 градусов.
        // Нормализация делает его длину равной 1, сохраняя направление.
        Vector2 knockbackVector = new Vector2(knockbackDirectionX, 1f).normalized;
        
        // --- ОТЛАДКА ---
        // Выведем в консоль, чтобы видеть, что происходит.
        // Debug.Log($"Отскок: Направление игрока = {playerFacingDirection}, Вектор отскока = {knockbackVector}, Сила = {force}");

        // 4. Применяем силу
        rb.linearVelocity = Vector2.zero; // Обнуляем скорость для чистого импульса
        rb.AddForce(knockbackVector * force, ForceMode2D.Impulse);
    }

    /// <summary>
    /// Корутина для визуального эффекта мигания.
    /// </summary>
    private IEnumerator FlashEffect()
    {
        if (spriteRenderer == null) yield break;

        spriteRenderer.color = hurtColor;
        yield return new WaitForSeconds(hurtFlashDuration);
        spriteRenderer.color = originalColor;
    }

    /// <summary>
    /// Корутина для перезапуска уровня с задержкой.
    /// </summary>
    /// <param name="delay">Задержка в секундах перед перезапуском.</param>
    private IEnumerator RestartLevelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator FreezeAnimationOnDeath()
    {
        yield return new WaitForSeconds(1f);
        animator.speed = 0;
    }
    
    private IEnumerator RestartLevel()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}