using UnityEngine;

public class PatternAttack : MonoBehaviour, IAttackBehavior
{
    [Header("Attack Settings")]
    [Tooltip("Анимационный триггер, который будет запущен для этой атаки.")]
    [SerializeField] private string attackTriggerName = "attack";
    [Tooltip("Время перезарядки после этой атаки.")]
    [SerializeField] private float attackCooldown = 2f;

    private Animator anim;
    private float lastAttackTime;

    private void Awake()
    {
        // Ищем аниматор на родительском объекте
        anim = GetComponentInParent<Animator>();
    }

    // Реализация метода из интерфейса IAttackBehavior
    public void Attack(Transform target)
    {
        // Проверяем, прошла ли перезарядка
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            // Запускаем триггер анимации
            anim?.SetTrigger(attackTriggerName);
            
            // Запоминаем время атаки, чтобы запустить перезарядку
            lastAttackTime = Time.time;
        }
    }
}