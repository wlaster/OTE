using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float attackDuration = 0.2f;

    [Header("References")]
    [Tooltip("Коллайдер-триггер, отвечающий за зону нанесения урона.")]
    [SerializeField] private Collider2D attackHitbox; // <<--- УКАЗЫВАЕМ ЯВНО
    [SerializeField] private LayerMask playerLayer;

    public UnityEvent OnAttackFinished;

    private float nextAttackTime = 0f;

    private void Awake()
    {
        if (attackHitbox == null)
        {
            Debug.LogError("Attack Hitbox не назначен в инспекторе для " + gameObject.name, this);
            this.enabled = false;
            return;
        }
        // Убеждаемся, что хитбокс выключен на старте
        attackHitbox.gameObject.SetActive(false);
    }

    public void PerformAttack()
    {
        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;
            // GetComponent<Animator>().SetTrigger("attack");
            StartCoroutine(AttackCoroutine());
        }
    }

    private IEnumerator AttackCoroutine()
    {
        attackHitbox.gameObject.SetActive(true);
        yield return new WaitForSeconds(attackDuration);
        attackHitbox.gameObject.SetActive(false);
        
        OnAttackFinished?.Invoke();
    }

    // Этот метод будет вызван, когда дочерний триггер attackHitbox с чем-то столкнется
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!attackHitbox.gameObject.activeInHierarchy) return;
        
        // Проверяем, что столкнулись с игроком
        if ((playerLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            // --- ИСПРАВЛЕНИЕ ЗДЕСЬ ---
            // Ищем на игроке компонент PlayerHealth
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                // Используем нашу переменную attackDamage для нанесения урона
                playerHealth.TakeDamage(attackDamage);
            }

            // Выключаем хитбокс после первого попадания, чтобы не бить дважды за взмах
            attackHitbox.gameObject.SetActive(false);
        }
    }
}