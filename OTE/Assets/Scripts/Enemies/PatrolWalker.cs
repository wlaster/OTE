
using UnityEngine;

/// <summary>
/// Враг-патрульный: ходит по платформе, разворачиваясь у края или при столкновении со стеной.
/// </summary>
public class PatrolWalker : Enemy
{
    [Header("Patrol Settings")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float checkRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    private bool isTouchingWall;
    private bool isGrounded;

    /// <summary>
    /// Проверяет край платформы и стены, обновляет анимацию и разворачивает при необходимости.
    /// </summary>
    protected override void Update()
    {
        base.Update();

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, checkRadius, groundLayer);
        animator.SetBool("isWalking", true);

        if (!isGrounded || isTouchingWall)
        {
            Flip();
        }
    }

    /// <summary>
    /// Применяет скорость патруля в FixedUpdate.
    /// </summary>
    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(isFacingRight ? moveSpeed : -moveSpeed, rb.linearVelocity.y);
    }

    /// <summary>
    /// Рисует зоны проверки пола и стены в редакторе.
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        Gizmos.DrawWireSphere(wallCheck.position, checkRadius);
    }
}