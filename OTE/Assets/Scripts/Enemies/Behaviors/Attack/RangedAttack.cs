using UnityEngine;

public class RangedAttack : MonoBehaviour, IAttackBehavior
{
    [Header("Ranged Attack Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float attackCooldown = 1.5f;

    private float lastAttackTime;

    public void Attack(Transform target)
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            if (projectilePrefab != null && firePoint != null && target != null)
            {
                GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

                // --- УМНАЯ ЛОГИКА ВЫЗОВА ---
                // Проверяем, есть ли на снаряде компонент StraightProjectile
                StraightProjectile straightProjectile = projectileGO.GetComponent<StraightProjectile>();
                if (straightProjectile != null)
                {
                    // Если это стрела, задаем ей направление
                    Vector2 direction = (target.position - firePoint.position).normalized;
                    straightProjectile.SetDirection(direction);
                }

                // Проверяем, есть ли на снаряде компонент HomingProjectile
                HomingProjectile homingProjectile = projectileGO.GetComponent<HomingProjectile>();
                if (homingProjectile != null)
                {
                    // Если это самонаводящийся снаряд, задаем ему цель
                    homingProjectile.SetTarget(target);
                }
            }
            lastAttackTime = Time.time;
        }
    }
}