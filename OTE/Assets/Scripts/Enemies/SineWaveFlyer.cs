// SineWaveFlyerAI.cs
using UnityEngine;

public class SineWaveFlyer
 : Enemy
{
    [Header("Sine Wave Settings")]
    [SerializeField] private float amplitude = 2f; // Высота волны
    [SerializeField] private float frequency = 2f; // Частота волны

    private Vector2 startPosition;
    private float timeAlive;

    protected override void Awake()
    {
        base.Awake();
        rb.gravityScale = 0;
        startPosition = transform.position;
    }

    protected override void Update()
    {
        base.Update();
        timeAlive += Time.deltaTime;

        float x = (isFacingRight ? 1 : -1) * moveSpeed * timeAlive;
        float y = Mathf.Sin(timeAlive * frequency) * amplitude;

        transform.position = startPosition + new Vector2(x, y);

        // Логика разворота (например, по таймеру или за экраном)
        if (timeAlive > 10f) // Разворот через 10 секунд
        {
            Flip();
            timeAlive = 0;
            startPosition = transform.position;
        }
    }
}