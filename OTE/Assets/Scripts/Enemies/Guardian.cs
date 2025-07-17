// GuardianAI.cs
using UnityEngine;

public class Guardian : Enemy
{
    [Header("Guardian Settings")]
    [SerializeField] private float detectionRange = 7f; // Дальность обнаружения игрока
    [SerializeField] private float attackRange = 1.5f;  // Дальность атаки
    [SerializeField] private LayerMask playerLayer;

    // Ссылка на компонент атаки, чтобы его можно было выключить
    [SerializeField] private GuardianAttack guardianAttack; 

    private Transform player;
    private bool isPlayerDetected = false;

    protected override void Awake()
    {
        base.Awake();
        // На старте отключаем компонент атаки
        if (guardianAttack != null) guardianAttack.enabled = false;
    }

    protected override void Update()
    {
        base.Update();
        DetectPlayer();

        if (isPlayerDetected && player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            // Если игрок в зоне атаки, останавливаемся и атакуем
            if (distanceToPlayer <= attackRange)
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Останавливаемся
                // animator.SetBool("isWalking", false);
                if (guardianAttack != null) guardianAttack.enabled = true; // Включаем атаку
            }
            // Если игрок обнаружен, но далеко, преследуем его
            else
            {
                if (guardianAttack != null) guardianAttack.enabled = false; // Выключаем атаку
                MoveTowardsPlayer();
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Стоим на месте
            // animator.SetBool("isWalking", false);
        }
    }

    private void DetectPlayer()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);
        if (playerCollider != null)
        {
            player = playerCollider.transform;
            isPlayerDetected = true;
        }
        else
        {
            isPlayerDetected = false;
        }
    }

    private void MoveTowardsPlayer()
    {
        float direction = player.position.x > transform.position.x ? 1 : -1;
        rb.linearVelocity = new Vector2(moveSpeed * direction, rb.linearVelocity.y);
        // animator.SetBool("isWalking", true);

        if ((direction > 0 && !isFacingRight) || (direction < 0 && isFacingRight))
        {
            Flip();
        }
    }

    // Визуализация
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}