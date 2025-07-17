// FlyingGuardianAI.cs
using UnityEngine;

public class FlyingGuardian : Enemy
{
    [Header("Flying Guardian Settings")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private LayerMask playerLayer;

    private Transform player;
    private Vector2 startingPosition;

    protected override void Awake()
    {
        base.Awake();
        startingPosition = transform.position;
        rb.gravityScale = 0; // Летающие враги не подвержены гравитации
    }
    
    protected override void Update()
    {
        base.Update();
        DetectPlayer();
        Vector2 direction;

        if (player != null)
        {
            // Летим к игроку
            direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;
        }
        else
        {
            // Возвращаемся на исходную позицию, если потеряли игрока
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

    private void DetectPlayer()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);
        player = (playerCollider != null) ? playerCollider.transform : null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}