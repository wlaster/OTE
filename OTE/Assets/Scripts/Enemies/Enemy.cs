// Enemy.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(EnemyHealth))]
public abstract class Enemy : MonoBehaviour
{
    [Header("Base Enemy Settings")]
    [SerializeField] protected float moveSpeed = 2f;

    protected Rigidbody2D rb;
    protected Animator animator;
    protected bool isFacingRight = true;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        
    }

    protected void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }
}