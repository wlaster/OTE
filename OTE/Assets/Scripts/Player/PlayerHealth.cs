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
    [SerializeField] private Vector2 hurtKnockback = new Vector2(5f, 5f);
    [SerializeField] private float invincibilityDuration = 0.5f;

    [Header("Death Settings")]
    [SerializeField] private Vector2 deathKnockback = new Vector2(7f, 10f);
    [SerializeField] private Vector2 deathColliderSize = new Vector2(1f, 0.5f);
    [SerializeField] private LayerMask layersToExclude;

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
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();
        playerMovement = GetComponent<PlayerMovement>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();

        originalColor = spriteRenderer.color;
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

        float knockbackDirection = transform.localScale.x > 0 ? -1 : 1; // Отбрасывает в сторону, противоположную взгляду
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(hurtKnockback.x * knockbackDirection, hurtKnockback.y), ForceMode2D.Impulse);

        spriteRenderer.color = hurtColor;
        yield return new WaitForSeconds(hurtFlashDuration);
        spriteRenderer.color = originalColor;

        yield return new WaitForSeconds(invincibilityDuration - hurtFlashDuration);
        isInvincible = false;
    }

    private void Die()
    {
        isDead = true;
        isInvincible = true;

        gameObject.tag = "Untagged";
        gameObject.layer = 0;

        playerController.enabled = false;
        playerMovement.enabled = false;
        rb.excludeLayers = layersToExclude;

        float knockbackDirection = transform.localScale.x > 0 ? -1 : 1;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(deathKnockback.x * knockbackDirection, deathKnockback.y), ForceMode2D.Impulse);

        if (capsuleCollider != null)
        {
            capsuleCollider.direction = CapsuleDirection2D.Horizontal;
            capsuleCollider.size = deathColliderSize;
            capsuleCollider.sharedMaterial = null;
        }

        animator.SetTrigger("death");
        StartCoroutine(FreezeAnimationOnDeath());
        StartCoroutine(RestartLevel());

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