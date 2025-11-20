// EnemyMeleeAttack.cs 
using UnityEngine;

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

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (attackPoint == null)
        {
            Debug.LogError("Attack Point не назначен в инспекторе для " + gameObject.name);
        }
    }

    // Этот метод по-прежнему вызывается из ИИ (GuardianAI)
    public void PerformAttack()
    {
        bool isAlreadyAttacking = animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack");
        if (!isAlreadyAttacking)
        {
            animator.SetTrigger("attack");
        }
    }

    /// Наносит урон всем целям в зоне атаки. Вызывается из Animation Event в нужный кадр.
    public void DealDamage()
    {
        if (attackPoint == null) return;

        // 1. Создаем зону атаки в указанной точке НА ОДИН КАДР
        Collider2D[] hitTargets = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, hittableLayers);

        // 2. Проходим по всем, кого нашли
        foreach (Collider2D targetCollider in hitTargets)
        {
            // 3. Наносим урон
            if (targetCollider.TryGetComponent<IDamageable>(out var damageableObject))
            {
                damageableObject.TakeDamage(attackDamage, transform.position);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}