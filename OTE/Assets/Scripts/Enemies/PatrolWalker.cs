// PatrolWalker.cs
using UnityEngine;

public class PatrolWalker : Enemy
{
    [Header("Patrol Settings")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float checkRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    private bool isTouchingWall;
    private bool isGrounded;

    protected override void Update()
    {
        base.Update(); // Вызываем базовый Update, если там есть логика

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, checkRadius, groundLayer);
        animator.SetBool("isWalking", true);

        if (!isGrounded || isTouchingWall)
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(isFacingRight ? moveSpeed : -moveSpeed, rb.linearVelocity.y);
    }

    // Визуализация для настройки
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        Gizmos.DrawWireSphere(wallCheck.position, checkRadius);
    }
}