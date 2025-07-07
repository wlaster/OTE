using UnityEngine;


[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class ChaseMovement : MonoBehaviour, IMovementBehavior
{
    [Header("Chase Settings")]
    [SerializeField] private float moveSpeed = 3.5f;

    private Rigidbody2D rb;
    private Animator anim;
    private Transform currentTarget;
    private EnemyController controller;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        controller = GetComponent<EnemyController>();
    }

    // Реализация метода из интерфейса IMovementBehavior
    public void Move(Rigidbody2D rigidbody, Transform target)
    {
        // Если цели нет, ничего не делаем
        if (target == null)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            // anim?.SetBool("isRunning", false);
            return;
        }

        currentTarget = target;
        Chase();
    }

    private void Chase()
    {
        // Определяем, куда нужно двигаться
        float targetDirection = currentTarget.position.x > rb.position.x ? 1 : -1;
        
        // Движение
        rb.linearVelocity = new Vector2(targetDirection * moveSpeed, rb.linearVelocity.y);

        // Поворот в сторону цели через контроллер
        bool isFacingTarget = (controller.IsFacingRight && targetDirection > 0) || (!controller.IsFacingRight && targetDirection < 0);
        if (!isFacingTarget)
        {
            controller.Flip();
        }
    }
}