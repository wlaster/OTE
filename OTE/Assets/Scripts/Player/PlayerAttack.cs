using UnityEngine;

[RequireComponent(typeof(Animator))]
/// <summary>
/// Управляет атаками игрока: воспроизведение анимаций атаки и нанесение урона зоне атаки.
/// </summary>
public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackDamage = 35f;

    [Header("Attack Zone")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.8f;

    [SerializeField] private LayerMask hittableLayers; 

    private Animator animator;

    /// <summary>
    /// Кэширует ссылку на Animator.
    /// </summary>
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    
    /// <summary>
    /// Запускает атаку: либо триггерит новую анимацию, либо продолжает комбо.
    /// </summary>
    public void PerformAttack()
    {
        bool isAttacking = animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack");

        if (isAttacking)
        {
            animator.SetBool("continueCombo", true);
        }
        else
        {
            animator.SetTrigger("attack");
        }
    }

    
    /// <summary>
    /// Сбрасывает флаг продолжения комбо (вызывается из анимации).
    /// </summary>
    public void ResetContinueCombo()
    {
        animator.SetBool("continueCombo", false);
    }
    
    
    /// <summary>
    /// Наносит урон всем объектам в пределах зоны атаки, реализующим `IDamageable`.
    /// </summary>
    public void DealDamage()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, hittableLayers);
        foreach (Collider2D enemyCollider in hitEnemies)
        {
            enemyCollider.GetComponent<IDamageable>()?.TakeDamage(attackDamage, transform.position);
        }
    }

    /// <summary>
    /// Отрисовка Gizmo зоны атаки в редакторе для удобства настройки.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}