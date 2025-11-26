using UnityEngine;

[RequireComponent(typeof(EnemyVision))]
/// <summary>
/// Ближний боец-страж: преследует игрока и использует ближнюю атаку на дистанции.
/// </summary>
public class Guardian : Enemy
{
    [Header("Guardian AI Settings")]
    [Tooltip("Дальность, на которой страж останавливается и начинает атаковать.")]
    [SerializeField] private float attackRange = 1.8f;

    private EnemyVision enemyVision;
    private EnemyMeleeAttack meleeAttack;

    /// <summary>
    /// Инициализация: получает ссылки на зрение и компонент ближней атаки.
    /// </summary>
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

    /// <summary>
    /// Выполняет выбор поведения каждое обновление (преследование/атака/простой).
    /// </summary>
    protected override void Update()
    {
        base.Update();
        HandleAIState();
    }

    
    /// <summary>
    /// Логика ИИ: если видит игрока — поворачивается к нему и выбирает между атакой и преследованием.
    /// </summary>
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

    
    /// <summary>
    /// Переходит в состояние простоя.
    /// </summary>
    private void Idle()
    {
        StopMoving();
    }

    
    /// <summary>
    /// Преследует игрока, двигаясь в его направлении.
    /// </summary>
    private void Chase()
    {
        rb.linearVelocity = new Vector2(isFacingRight ? moveSpeed : -moveSpeed, rb.linearVelocity.y);
        animator.SetBool("isWalking", true);
    }

    
    /// <summary>
    /// Останавливается и вызывает выполнение ближней атаки.
    /// </summary>
    private void Attack()
    {
        StopMoving();
        meleeAttack.PerformAttack();
    }

    
    /// <summary>
    /// Поворачивает стража в сторону игрока.
    /// </summary>
    private void FacePlayer(Transform player)
    {
        if ((player.position.x > transform.position.x && !isFacingRight) || 
            (player.position.x < transform.position.x && isFacingRight))
        {
            Flip();
        }
    }

    
    /// <summary>
    /// Останавливает движение и отключает анимацию ходьбы.
    /// </summary>
    private void StopMoving()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        animator.SetBool("isWalking", false);
    }

    /// <summary>
    /// Рисует гизмо диапазона атаки в редакторе.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
