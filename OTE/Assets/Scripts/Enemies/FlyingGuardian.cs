// FlyingGuardian.cs
using UnityEngine;

[RequireComponent(typeof(EnemyVision))]
public class FlyingGuardian : Enemy
{
    private EnemyVision enemyVision;
    private Vector2 startingPosition;

    private Vector2 direction;

    protected override void Awake()
    {
        base.Awake();
        enemyVision = GetComponent<EnemyVision>();
        startingPosition = transform.position;
        rb.gravityScale = 0;
    }

    protected override void Update()
    {
        base.Update();
        HandleMovement();
    }


    private void HandleMovement()
    {
        
        if (enemyVision.CanSeePlayer)
        {
            // Летим к игроку
            Transform player = enemyVision.Player;
            direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;
        }
        else
        {
            // Возвращаемся на исходную позицию
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