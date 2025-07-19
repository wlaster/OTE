using UnityEngine;

[RequireComponent(typeof(EnemyVision))]
public class Guardian : Enemy
{
    [Header("Guardian AI Settings")]
    [Tooltip("Дальность, на которой страж останавливается и начинает атаковать.")]
    [SerializeField] private float attackRange = 1.8f;

    private EnemyVision enemyVision;
    private EnemyMeleeAttack meleeAttack;

    protected override void Awake()
    {
        base.Awake();
        enemyVision = GetComponent<EnemyVision>();
        meleeAttack = GetComponent<EnemyMeleeAttack>();

        if (meleeAttack == null)
        {
            Debug.LogError("Компонент EnemyMeleeAttack не найден на " + gameObject.name);
        }
    }

    protected override void Update()
    {
        base.Update();
        HandleAIState();
    }

    /// Основная логика принятия решений (машина состояний).
    private void HandleAIState()
    {
        // Если мы не видим игрока, переходим в состояние покоя
        if (!enemyVision.CanSeePlayer)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            animator.SetBool("isWalking", false);
            return;
        }

        // Если мы здесь, значит, игрок в поле зрения
        Transform player = enemyVision.Player;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Поворачиваемся в сторону игрока
        if ((player.position.x > transform.position.x && !isFacingRight) || (player.position.x < transform.position.x && isFacingRight))
        {
            Flip();
        }

        // Проверяем, не проигрывается ли уже анимация атаки
        bool isCurrentlyAttacking = animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack");

        // Если игрок в зоне атаки и мы не атакуем...
        if (distanceToPlayer <= attackRange)
        {
            // ...переходим в состояние АТАКИ
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            animator.SetBool("isWalking", false);
            
            // Отдаем команду на атаку
            meleeAttack.PerformAttack();
        }
        else if (!isCurrentlyAttacking) // Если игрок далеко И мы не атакуем...
        {
            // ...переходим в состояние ПРЕСЛЕДОВАНИЯ
            rb.linearVelocity = new Vector2(isFacingRight ? moveSpeed : -moveSpeed, rb.linearVelocity.y);
            animator.SetBool("isWalking", true);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}