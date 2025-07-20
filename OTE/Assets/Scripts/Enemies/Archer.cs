using UnityEngine;

/// <summary>
/// Искусственный интеллект для врага-лучника.
/// Использует EnemyVision для обнаружения цели и принимает решения на основе этого.
/// </summary>
[RequireComponent(typeof(EnemyVision))]
public class ArcherAI : Enemy
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
        // Просто проверяем флаг из компонента зрения
        if (!enemyVision.CanSeePlayer)
        {
            // Состояние "ПОКОЙ"
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            animator.SetBool("isWalking", false);
            return;
        }

        // Если мы здесь, значит, мы видим игрока. Берем его из EnemyVision.
        Transform player = enemyVision.Player;
        if (player == null) return; // Дополнительная проверка на всякий случай

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Поворачиваемся в сторону игрока
        if ((player.position.x > transform.position.x && !isFacingRight) || (player.position.x < transform.position.x && isFacingRight))
        {
            Flip();
        }

        // Если игрок слишком близко
        if (distanceToPlayer < retreatDistance)
        {
            // Состояние "ПАНИКА/ОТСТУПЛЕНИЕ"
            float direction = -Mathf.Sign(player.position.x - transform.position.x);
            rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
            animator.SetBool("isWalking", true);
        }
        // Если игрок на идеальной дистанции для стрельбы
        else if (distanceToPlayer <= idealShootingRange)
        {
            // Состояние "СТРЕЛЬБА"
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            animator.SetBool("isWalking", false);
            TryToShoot();
        }
        else // Игрок виден, но слишком далеко (за пределами idealShootingRange)
        {
            // Состояние "СБЛИЖЕНИЕ"
            float direction = Mathf.Sign(player.position.x - transform.position.x);
            rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
            animator.SetBool("isWalking", true);
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
        if (arrowPrefab != null && firePoint != null)
        {
            Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        // Отрисовка зон для удобства
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, idealShootingRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, retreatDistance);
    }
}