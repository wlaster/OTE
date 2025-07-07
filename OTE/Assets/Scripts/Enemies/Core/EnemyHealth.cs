using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    
    [Header("Feedback")]
    [SerializeField] private Color hurtColor = Color.red;
    [SerializeField] private float hurtDuration = 0.1f;

    [Header("Events")]
    public UnityEvent OnHit;
    public UnityEvent OnDeath;

    private float currentHealth;
    private bool isDead = false;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Collider2D enemyCollider;
    private EnemyController enemyController; // Ссылка на мозг

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        enemyCollider = GetComponent<Collider2D>();
        enemyController = GetComponent<EnemyController>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        OnHit?.Invoke();
        StartCoroutine(HurtFlash());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        enemyCollider.enabled = false;
        if (enemyController != null) enemyController.enabled = false; // Отключаем мозг
        
        OnDeath?.Invoke();
        Destroy(gameObject, 2f); // Уничтожаем с задержкой
    }

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