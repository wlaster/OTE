using System.Collections.Generic;
using UnityEngine;

public class AI_Detection_Sight : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private LayerMask obstacleLayer; // Слой, который считается препятствием (стены, земля)
    
    // Список всех потенциальных целей, которые находятся внутри триггера
    private List<Transform> potentialTargets = new List<Transform>();
    
    // Главное свойство, которое сообщает, кого мы видим в данный момент
    public Transform DetectedTarget { get; private set; }

    private Transform parentTransform; // Ссылка на "тело" врага

    private void Awake()
    {
        // Получаем ссылку на родительский объект (самого врага)
        parentTransform = transform.parent;
    }

    private void Update()
    {
        // Каждый кадр проверяем, видим ли мы кого-то из потенциальных целей
        CheckLineOfSight();
    }

    private void CheckLineOfSight()
    {
        // Если в списке есть цели, проверяем первую из них
        if (potentialTargets.Count > 0)
        {
            Transform target = potentialTargets[0]; // Берем ближайшую цель (можно усложнить, но для одной цели - игрока - этого достаточно)
            Vector2 directionToTarget = (target.position - parentTransform.position).normalized;
            float distanceToTarget = Vector2.Distance(parentTransform.position, target.position);

            // Пускаем луч от "тела" врага к цели
            // Если луч НЕ столкнулся с препятствием, значит, мы видим цель
            if (!Physics2D.Raycast(parentTransform.position, directionToTarget, distanceToTarget, obstacleLayer))
            {
                DetectedTarget = target;
                return; // Выходим, цель найдена
            }
        }
        
        // Если мы дошли до сюда, значит, мы никого не видим
        DetectedTarget = null;
    }

    // Когда игрок входит в триггер, добавляем его в список потенциальных целей
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !potentialTargets.Contains(other.transform))
        {
            potentialTargets.Add(other.transform);
        }
    }

    // Когда игрок выходит из триггера, убираем его из списка
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            potentialTargets.Remove(other.transform);
        }
    }

    // Визуализация для отладки
    private void OnDrawGizmos()
    {
        if (parentTransform != null && DetectedTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(parentTransform.position, DetectedTarget.position);
        }
    }
}