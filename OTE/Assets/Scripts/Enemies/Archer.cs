// ArcherAI.cs
using UnityEngine;

public class Archer : Enemy
{
    [Header("Archer Settings")]
    [SerializeField] private float shootingRange = 12f;
    [SerializeField] private float retreatDistance = 4f; // Дистанция, на которой начинает убегать
    [SerializeField] private float fireRate = 2f; // Выстрелов в секунду
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private LayerMask playerLayer;

    private Transform player;
    private float nextFireTime;

    protected override void Update()
    {
        base.Update();
        DetectPlayer();

        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            Vector2 directionToPlayer = (player.position - transform.position).normalized;

            // Поворачиваемся к игроку
            if ((directionToPlayer.x > 0 && !isFacingRight) || (directionToPlayer.x < 0 && isFacingRight))
            {
                Flip();
            }

            // Если игрок слишком близко - убегаем
            if (distanceToPlayer < retreatDistance)
            {
                rb.linearVelocity = new Vector2(-directionToPlayer.x * moveSpeed, rb.linearVelocity.y);
            }
            // Если игрок в зоне досягаемости - стреляем
            else if (distanceToPlayer <= shootingRange)
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Стоим и стреляем
                if (Time.time >= nextFireTime)
                {
                    Shoot();
                    nextFireTime = Time.time + 1f / fireRate;
                }
            }
            else
            {
                 rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
        }
    }

    private void DetectPlayer()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, shootingRange, playerLayer);
        player = (playerCollider != null) ? playerCollider.transform : null;
    }

    private void Shoot()
    {
        // Логика создания стрелы
        Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);
    }

    // ... Gizmos для отрисовки ...
}