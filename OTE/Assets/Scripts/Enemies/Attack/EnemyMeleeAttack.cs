
using UnityEngine;

/// <summary>
/// Компонент ближней атаки врага: триггерит анимацию и наносит урон в зоне удара.
/// </summary>
public class EnemyMeleeAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackDamage = 20f;

    [Header("Attack Zone")]
    [Tooltip("Пустой дочерний объект, чья позиция используется как центр атаки.")]
    [SerializeField] private Transform attackPoint;
    [Tooltip("Радиус круга атаки.")]
    [SerializeField] private float attackRange = 0.8f;
    [Tooltip("Слой, на котором находится игрок.")]
    [SerializeField] private LayerMask hittableLayers;

    private Animator animator;

    /// <summary>
    /// Кэширует `Animator` и проверяет, назначена ли точка атаки.
    /// </summary>
    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (attackPoint == null)
        {
            Debug.LogError("Attack Point не назначен в инспекторе для " + gameObject.name);
        }
    }

    
    /// <summary>
    /// Инициирует атаку если сейчас не воспроизводится анимация атаки.
    /// </summary>
    public void PerformAttack()
    {
        bool isAlreadyAttacking = animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack");
        if (!isAlreadyAttacking)
        {
            animator.SetTrigger("attack");
        }
    }

    
    /// <summary>
    /// Наносит урон всем целям в радиусе `attackRange` от `attackPoint`.
    /// </summary>
    public void DealDamage()
    {
        if (attackPoint == null) return;

        Collider2D[] hitTargets = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, hittableLayers);

        foreach (Collider2D targetCollider in hitTargets)
        {
            if (targetCollider.TryGetComponent<IDamageable>(out var damageableObject))
            {
                damageableObject.TakeDamage(attackDamage, transform.position);
            }
        }
    }

    /// <summary>
    /// Отрисовка гизмо зоны атаки в редакторе.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}