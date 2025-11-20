// GroundCrawler.cs
using UnityEngine;

public class GroundCrawler : Enemy
{
    [Header("Crawler Settings")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkDistance = 0.5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float rotationSpeed = 5f;

    protected override void Awake()
    {
        base.Awake();
        rb.gravityScale = 0;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = transform.right * moveSpeed;

        animator.SetBool("isWalking", true);

        // Проверка земли под собой
        RaycastHit2D groundInfo = Physics2D.Raycast(groundCheck.position, -transform.up, checkDistance, groundLayer);

        if (groundInfo.collider == false)
        {
            // Если земли нет, ищем ее, поворачиваясь
            transform.Rotate(0, 0, -rotationSpeed * Time.fixedDeltaTime * 10f * (isFacingRight ? 1 : -1));
        }
        else
        {
            // Прилипаем к поверхности
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, groundInfo.normal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
        }
    }
}