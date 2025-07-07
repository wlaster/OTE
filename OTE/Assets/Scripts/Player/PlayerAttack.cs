using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackDamage = 35f;

    [Header("Attack Zone")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.8f;
    [SerializeField] private LayerMask enemyLayer;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Этот метод вызывается из PlayerController
    public void PerformAttack()
    {
        // Проверяем, находимся ли мы сейчас в одной из анимаций атаки
        // Мы используем IsTag("Attack"), чтобы отлавливать все состояния с этим тегом
        bool isAttacking = animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack");

        if (isAttacking)
        {
            // Если мы уже атакуем, то это нажатие - запрос на продолжение комбо
            animator.SetBool("continueCombo", true);
        }
        else
        {
            // Если мы не атакуем, начинаем комбо с первого удара
            animator.SetTrigger("attack");
        }
    }

    // --- МЕТОДЫ, ВЫЗЫВАЕМЫЕ ИЗ АНИМАЦИИ (Animation Events) ---

    // Event в НАЧАЛЕ КАЖДОГО удара комбо.
    public void ResetContinueCombo()
    {
        animator.SetBool("continueCombo", false);
    }
    
    // Event в КАДР УДАРА для каждой анимации.
    public void DealDamage()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange);
        foreach (Collider2D enemyCollider in hitEnemies)
        {
            // Ищем любой объект, который может получать урон
            enemyCollider.GetComponent<IDamageable>()?.TakeDamage(attackDamage);
        }
    }

    // Вспомогательная функция для визуализации
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}