using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private bool _invincible = false;
    [SerializeField] private Color _damageColor = Color.red;
    
    [Header("Effects")]
    [SerializeField] private ParticleSystem _hitEffect;
    [SerializeField] private AudioClip _damageSound;
    
    private float _currentHealth;
    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;
    private Animator _animator;
    
    public delegate void HealthEvent(float amount);
    public event HealthEvent OnDamageTaken;
    public event HealthEvent OnDeath;
    
    private void Awake()
    {
        _currentHealth = _maxHealth;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer != null) _originalColor = _spriteRenderer.color;
        _animator = GetComponent<Animator>();
    }

    // Реализация интерфейса IDamageable
    public void TakeDamage(float damage)
    {
        if (_invincible || _currentHealth <= 0) return;

        _currentHealth -= damage;
        Mathf.Clamp(_currentHealth, 0, _maxHealth);
        
        // Визуальная обратная связь
        PlayDamageEffects();
        
        // Событие для UI/логики
        OnDamageTaken?.Invoke(damage);
        
        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void PlayDamageEffects()
    {
        // Эффект попадания
        if (_hitEffect != null) _hitEffect.Play();
        
        // Звук
        if (_damageSound != null) 
            AudioSource.PlayClipAtPoint(_damageSound, transform.position);
        
        // Анимация
        if (_animator != null) _animator.SetTrigger("Hit");
        
        // Мигание спрайтом
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = _damageColor;
            Invoke(nameof(ResetColor), 0.2f);
        }
    }

    private void ResetColor()
    {
        if (_spriteRenderer != null) _spriteRenderer.color = _originalColor;
    }

    private void Die()
    {
        // Событие смерти
        OnDeath?.Invoke(_maxHealth);
        
        // Анимация смерти
        if (_animator != null)
        {
            _animator.SetTrigger("Die");
            // Уничтожение после анимации
            Destroy(gameObject, _animator.GetCurrentAnimatorStateInfo(0).length);
        }
        else
        {
            Destroy(gameObject);
        }
        
        // Отключаем коллайдеры
        var colliders = GetComponents<Collider2D>();
        foreach (var col in colliders) col.enabled = false;
    }

    // Методы для лечения/изменения параметров
    public void Heal(float amount)
    {
        _currentHealth = Mathf.Min(_currentHealth + amount, _maxHealth);
    }

    public void SetInvincible(bool state)
    {
        _invincible = state;
    }

    // Для отображения в UI
    public float GetHealthPercentage()
    {
        return _currentHealth / _maxHealth;
    }
}