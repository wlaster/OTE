// SineWaveFlyer.cs
using UnityEngine;

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

    private bool isActivated = false;

    private float originalY;
    private float horizontalPosition;
    private float journeyTime = 0f;

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

    private void HandleMovement()
    {
        journeyTime += Time.deltaTime;

        horizontalPosition += (isFacingRight ? 1 : -1) * moveSpeed * Time.deltaTime;
        
        // Вычисляем смещение по Y от ОРИГИНАЛЬНОЙ высоты
        float yOffset = Mathf.Sin(journeyTime * frequency) * amplitude;
        
        transform.position = new Vector2(horizontalPosition, originalY + yOffset);
    }

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