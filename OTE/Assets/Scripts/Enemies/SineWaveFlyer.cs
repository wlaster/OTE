
using UnityEngine;

/// <summary>
/// Летающий враг, движущийся по синусоидальной траектории, ориентируется на игрока.
/// </summary>
public class SineWaveFlyer : Enemy
{

    [Header("Movement Settings")]
    [Tooltip("Амплитуда (высота) синусоидальной волны.")]
    [SerializeField] private float amplitude = 2f;
    [Tooltip("Частота (как часто будут волны) синусоидального движения.")]
    [SerializeField] private float frequency = 2f;
    [Tooltip("Максимальное расстояние по горизонтали от игрока, после которого враг развернется.")]
    [SerializeField] private float maxHorizontalDistance = 15f;

    [Header("Target")]
    [Tooltip("Перетащите сюда объект игрока со сцены.")]
    [SerializeField] private Transform playerTransform;

    [Header("Activation Settings")]
    [Tooltip("Максимальное расстояние до игрока для активации движения.")]
    [SerializeField] private float activationDistance = 20f;

    private float originalY;
    private float horizontalPosition;
    private float journeyTime = 0f;

    /// <summary>
    /// Инициализация: сохраняет исходную высоту и проверяет назначенную цель.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        rb.gravityScale = 0;

        if (playerTransform == null)
        {
            Debug.LogError("Цель (Player) не назначена в инспекторе для " + gameObject.name, this);
            enabled = false;
            return;
        }

        originalY = transform.position.y;
        horizontalPosition = transform.position.x;
    }

    /// <summary>
    /// Проверяет дистанцию до игрока и управляет передвижением, если цель в зоне активации.
    /// </summary>
    protected override void Update()
    {
        base.Update();
        if (playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= activationDistance)
        {
            HandleMovement();
            CheckForTurnaround();
        }
    }

    /// <summary>
    /// Обновляет положение по синусоидальной траектории и движется по горизонтали.
    /// </summary>
    private void HandleMovement()
    {
        journeyTime += Time.deltaTime;

        horizontalPosition += (isFacingRight ? 1 : -1) * moveSpeed * Time.deltaTime;

        float yOffset = Mathf.Sin(journeyTime * frequency) * amplitude;

        transform.position = new Vector2(horizontalPosition, originalY + yOffset);
    }

    /// <summary>
    /// Проверяет, не удаляется ли враг слишком далеко по горизонтали от игрока и при необходимости разворачивается.
    /// </summary>
    private void CheckForTurnaround()
    {
        float horizontalDistanceToPlayer = Mathf.Abs(transform.position.x - playerTransform.position.x);

        if (horizontalDistanceToPlayer > maxHorizontalDistance)
        {
            bool isMovingAway = (isFacingRight && transform.position.x > playerTransform.position.x) ||
                                (!isFacingRight && transform.position.x < playerTransform.position.x);

            if (isMovingAway)
            {
                Flip();
            }
        }
    }
    
}