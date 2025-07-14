using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(GroundEdgeDetector))]
public class AI_Chase_Ground : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4f;

    // Ссылка на цель, которую нужно преследовать
    public Transform target;

    private Rigidbody2D rb;
    private GroundEdgeDetector edgeDetector;
    private int moveDirection = 1;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        edgeDetector = GetComponent<GroundEdgeDetector>();
    }

    private void Update()
    {
        if (target == null) return;

        // Определяем направление к цели
        if (target.position.x > transform.position.x)
        {
            moveDirection = 1;
        }
        else
        {
            moveDirection = -1;
        }

        // Поворачиваем врага лицом к цели
        if ((moveDirection > 0 && transform.right.x < 0) || (moveDirection < 0 && transform.right.x > 0))
        {
            transform.Rotate(0f, 180f, 0f);
        }
    }

    private void FixedUpdate()
    {
        if (target == null || edgeDetector.IsTouchingEdge)
        {
            // Если цели нет или впереди обрыв/стена - стоим на месте
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        rb.linearVelocity = new Vector2(moveSpeed * moveDirection, rb.linearVelocity.y);
    }
}