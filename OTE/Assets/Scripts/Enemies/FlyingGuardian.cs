
using UnityEngine;

[RequireComponent(typeof(EnemyVision))]
/// <summary>
/// Летающий страж: патрулирует вокруг стартовой позиции и преследует игрока при обнаружении.
/// </summary>
public class FlyingGuardian : Enemy
{
    private EnemyVision enemyVision;
    private Vector2 startingPosition;

    private Vector2 direction;

    /// <summary>
    /// Инициализация: кэширует `EnemyVision`, сохраняет стартовую позицию и отключает гравитацию.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        enemyVision = GetComponent<EnemyVision>();
        startingPosition = transform.position;
        rb.gravityScale = 0;
    }

    /// <summary>
    /// Обновление: выполняет логику движения каждый кадр.
    /// </summary>
    protected override void Update()
    {
        base.Update();
        HandleMovement();
    }


    /// <summary>
    /// Управляет перемещением: преследует игрока или возвращается к стартовой позиции.
    /// </summary>
    private void HandleMovement()
    {
        if (enemyVision.CanSeePlayer)
        {
            Transform player = enemyVision.Player;
            direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;
        }
        else
        {
            direction = (startingPosition - (Vector2)transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;

            if (Vector2.Distance(transform.position, startingPosition) < 0.1f)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }

        if ((direction.x > 0 && !isFacingRight) || (direction.x < 0 && isFacingRight))
        {
            Flip();
        }
    }
}