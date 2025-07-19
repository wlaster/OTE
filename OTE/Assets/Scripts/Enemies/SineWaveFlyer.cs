// SineWaveFlyerAI.cs
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

    private Vector2 startPosition; // Точка, от которой начинается текущая волна
    private float journeyTime = 0f; // Время, прошедшее с начала текущего движения

    protected override void Awake()
    {
        base.Awake();
        rb.gravityScale = 0;
        
        // Проверяем, назначена ли цель в инспекторе
        if (playerTransform == null)
        {
            Debug.LogError("Цель (Player) не назначена в инспекторе для " + gameObject.name, this);
            enabled = false;
            return;
        }
        
        startPosition = transform.position;
    }

    protected override void Update()
    {
        base.Update();
        
        if (playerTransform == null) return;

        HandleMovement();
        CheckForTurnaround();
    }

    private void HandleMovement()
    {
        // Увеличиваем время "путешествия"
        journeyTime += Time.deltaTime;

        // Вычисляем смещение по X и Y
        // X - это линейное движение со скоростью moveSpeed
        float xOffset = (isFacingRight ? 1 : -1) * moveSpeed * journeyTime;
        // Y - это синусоидальное колебание
        float yOffset = Mathf.Sin(journeyTime * frequency) * amplitude;
        
        // Применяем смещение к стартовой позиции текущей волны
        transform.position = startPosition + new Vector2(xOffset, yOffset);
    }

    private void CheckForTurnaround()
    {
        // Вычисляем горизонтальное расстояние до игрока
        float horizontalDistanceToPlayer = Mathf.Abs(transform.position.x - playerTransform.position.x);

        // Если мы улетели слишком далеко...
        if (horizontalDistanceToPlayer > maxHorizontalDistance)
        {
            // ...и летим ОТ игрока...
            bool isMovingAway = (isFacingRight && transform.position.x > playerTransform.position.x) ||
                                (!isFacingRight && transform.position.x < playerTransform.position.x);
            
            if (isMovingAway)
            {
                // ...то разворачиваемся.
                FlipAndReset();
            }
        }
    }
    
    private void FlipAndReset()
    {
        Flip(); 
        
        // Сбрасываем таймер и запоминаем новую стартовую точку для следующей волны
        journeyTime = 0f;
        startPosition = transform.position;
    }
}