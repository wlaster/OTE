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

    /// Основная логика принятия решений.
    private void HandleAIState()
    {
        if (!enemyVision.CanSeePlayer)
        {
            Idle();
            return;
        }

        Transform player = enemyVision.Player;
        FacePlayer(player);

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        bool isCurrentlyAttacking = animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack");

        if (distanceToPlayer <= attackRange)
        {
            Attack();
        }
        else if (!isCurrentlyAttacking)
        {
            Chase();
        }
    }

    /// Состояние покоя: враг останавливается.
    private void Idle()
    {
        StopMoving();
    }

    /// Состояние преследования: враг движется к игроку.
    private void Chase()
    {
        rb.linearVelocity = new Vector2(isFacingRight ? moveSpeed : -moveSpeed, rb.linearVelocity.y);
        animator.SetBool("isWalking", true);
    }

    /// Состояние атаки: враг останавливается и атакует.
    private void Attack()
    {
        StopMoving();
        meleeAttack.PerformAttack();
    }

    /// Поворачивает врага лицом к игроку.
    private void FacePlayer(Transform player)
    {
        if ((player.position.x > transform.position.x && !isFacingRight) || 
            (player.position.x < transform.position.x && isFacingRight))
        {
            Flip();
        }
    }

    /// Останавливает горизонтальное движение.
    private void StopMoving()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        animator.SetBool("isWalking", false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
