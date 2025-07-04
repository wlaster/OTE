using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackDamage = 25f;
    [SerializeField] private float attackCooldown = 0.5f;

    [Header("Attack Area")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.6f;
    [SerializeField] private LayerMask enemyLayer;

    private Animator animator;
    private float nextAttackTime = 0f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Публичный метод, который будет вызывать PlayerController
    public void TryToAttack()
    {
        if (Time.time >= nextAttackTime)
        {
            animator.SetTrigger("attack");
            nextAttackTime = Time.time + attackCooldown;
            // Урон будет наноситься через Animation Event, как мы настраивали ранее.
            // Это самый надежный способ.
        }
    }

    // Этот метод вызывается из анимации через Event
    public void DealDamageEvent()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
        foreach (Collider2D enemyCollider in hitEnemies)
        {
            EnemyHealth enemyHealth = enemyCollider.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
            }
        }
    }
    
    // Визуализация для редактора
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}