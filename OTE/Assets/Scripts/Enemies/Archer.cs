using UnityEngine;

/// Искусственный интеллект для врага-лучника.
/// Использует EnemyVision для обнаружения цели и принимает решения на основе этого.
[RequireComponent(typeof(EnemyVision))]
public class Archer : Enemy
{
    [Header("AI Behavior")]
    [Tooltip("Дистанция, на которой лучник перестает убегать и начинает стрелять.")]
    [SerializeField] private float idealShootingRange = 10f;
    [Tooltip("Дистанция, на которой лучник начинает паниковать и убегать от игрока.")]
    [SerializeField] private float retreatDistance = 4f;

    [Header("Combat")]
    [Tooltip("Как быстро лучник стреляет (выстрелов в секунду).")]
    [SerializeField] private float fireRate = 0.5f;
    [Tooltip("Префаб стрелы, которую будет выпускать лучник.")]
    [SerializeField] private GameObject arrowPrefab;
    [Tooltip("Точка, из которой вылетает стрела.")]
    [SerializeField] private Transform firePoint;
    
    // Ссылки
    private EnemyVision enemyVision;
    
    // Состояние
    private float nextFireTime = 0f;

    protected override void Awake()
    {
        base.Awake();
        enemyVision = GetComponent<EnemyVision>();
        if (enemyVision == null)
        {
            Debug.LogError("Компонент EnemyVision не найден на " + gameObject.name, this);
            enabled = false;
        }
    }

    protected override void Update()
    {
        base.Update();
        HandleAIState();
    }

    private void HandleAIState()
    {
        if (!enemyVision.CanSeePlayer || enemyVision.Player == null)
        {
            PerformIdle();
            return;
        }

        Transform player = enemyVision.Player;
        FacePlayer(player);

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < retreatDistance)
        {
            PerformRetreat(player);
        }
        else if (distanceToPlayer <= idealShootingRange)
        {
            PerformAttack();
        }
        else
        {
            PerformChase(player);
        }
    }

    private void PerformIdle()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        animator.SetBool("isWalking", false);
    }

    private void PerformChase(Transform player)
    {
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
        animator.SetBool("isWalking", true);
    }

    private void PerformRetreat(Transform player)
    {
        float direction = -Mathf.Sign(player.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
        animator.SetBool("isWalking", true);
    }

    private void PerformAttack()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        animator.SetBool("isWalking", false);
        TryToShoot();
    }

    private void FacePlayer(Transform player)
    {
        bool shouldFlip = (player.position.x > transform.position.x && !isFacingRight) || 
                          (player.position.x < transform.position.x && isFacingRight);
        if (shouldFlip)
        {
            Flip();
        }
    }

    private void TryToShoot()
    {
        if (Time.time > nextFireTime)
        {
            nextFireTime = Time.time + 1f / fireRate;
            animator.SetTrigger("attack");
        }
    }

    // Этот метод вызывается из Animation Event
    public void FireArrow()
    {
        if (arrowPrefab != null && firePoint != null && enemyVision.Player != null)
        {
            GameObject arrowGO = Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);
            Arrow arrow = arrowGO.GetComponent<Arrow>();
            if (arrow != null)
            {
                arrow.Initialize(enemyVision.Player);
            }
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, idealShootingRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, retreatDistance);
    }
}
