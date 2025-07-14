using UnityEngine;

public class AI_Attack_Melee : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackDamage = 15f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1.5f;

    [Header("Attack Point")]
    [SerializeField] private Transform attackPoint; // Точка, из которой будет исходить атака

    private float nextAttackTime = 0f;
    // private Animator animator;

    private void Awake()
    {
        // animator = GetComponent<Animator>();
    }

    // Метод для проверки, находится ли цель в зоне досягаемости
    public bool IsTargetInRange(Transform target)
    {
        if (target == null) return false;
        return Vector2.Distance(transform.position, target.position) <= attackRange;
    }

    // Главный метод атаки, вызываемый из "мозга"
    public void PerformAttack()
    {
        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;
            // animator?.SetTrigger("attack");

            // --- ВСЯ ЛОГИКА УДАРА В ОДНОМ МЕСТЕ ---
            // Создаем невидимый круг в точке атаки
            Collider2D[] hitTargets = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, LayerMask.GetMask("Player"));

            // Проходим по всем, кто попал в круг
            foreach (Collider2D targetCollider in hitTargets)
            {
                // Ищем на цели компонент здоровья игрока
                PlayerHealth playerHealth = targetCollider.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(attackDamage);
                    // Выходим из цикла после первого попадания, чтобы не ударить дважды
                    break; 
                }
            }
        }
    }

    // Визуализация для настройки в редакторе
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}