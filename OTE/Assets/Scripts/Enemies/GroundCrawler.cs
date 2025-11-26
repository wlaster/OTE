
using UnityEngine;

/// <summary>
/// Враг-ползун по поверхности: прикрепляется к поверхности и перемещается вдоль неё.
/// </summary>
public class GroundCrawler : Enemy
{
    [Header("Crawler Settings")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkDistance = 0.5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float rotationSpeed = 5f;

    /// <summary>
    /// Инициализация: отменяет гравитацию, чтобы ползунок держался на поверхности.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        rb.gravityScale = 0;
    }

    /// <summary>
    /// Фиксированное обновление: двигается вдоль поверхности, проверяет землю и корректирует ориентацию.
    /// </summary>
    private void FixedUpdate()
    {
        rb.linearVelocity = transform.right * moveSpeed;

        animator.SetBool("isWalking", true);

        RaycastHit2D groundInfo = Physics2D.Raycast(groundCheck.position, -transform.up, checkDistance, groundLayer);

        if (groundInfo.collider == false)
        {
            transform.Rotate(0, 0, -rotationSpeed * Time.fixedDeltaTime * 10f * (isFacingRight ? 1 : -1));
        }
        else
        {
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, groundInfo.normal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
        }
    }
}