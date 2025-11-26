
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(EnemyHealth))]
/// <summary>
/// Базовый абстрактный класс для врагов: содержит общую логику движения и ориентации.
/// </summary>
public abstract class Enemy : MonoBehaviour
{
    [Header("Base Enemy Settings")]
    [SerializeField] protected float moveSpeed = 2f;

    protected Rigidbody2D rb;
    protected Animator animator;
    protected bool isFacingRight = true;

    /// <summary>
    /// Инициализация общих компонентов врага (Rigidbody2D, Animator).
    /// </summary>
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Виртуальное обновление для дочерних классов (поддержка поведения врага).
    /// </summary>
    protected virtual void Update()
    {
    }

    /// <summary>
    /// Поворачивает врага на 180 градусов (инвертирует направление взгляда).
    /// </summary>
    protected void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }
}