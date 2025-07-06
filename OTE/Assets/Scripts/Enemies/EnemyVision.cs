using UnityEngine;
using UnityEngine.Events;

public class EnemyVision : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("Коллайдер-триггер, отвечающий за зону видимости.")]
    [SerializeField] private Collider2D visionTrigger;

    [Header("Detection Layers")]
    [Tooltip("Слой(и), на котором находятся цели для обнаружения (например, Player).")]
    [SerializeField] private LayerMask targetLayer; // <<--- Переименовали для ясности
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Settings")]
    [Tooltip("Включить проверку на наличие стен между врагом и целью")]
    [SerializeField] private bool checkLineOfSight = true;

    // --- СОБЫТИЯ ---
    public UnityEvent<Transform> OnTargetSpotted;
    public UnityEvent OnTargetLost;

    private Transform currentTarget = null;
    private bool wasTargetVisibleLastFrame = false;

    private void Awake()
    {
        if (visionTrigger == null)
        {
            Debug.LogError("Vision Trigger не назначен в инспекторе для " + gameObject.name, this);
            this.enabled = false;
        }
    }

    private void Update()
    {
        bool isTargetCurrentlyVisible = CheckForTarget();

        if (isTargetCurrentlyVisible && !wasTargetVisibleLastFrame)
        {
            OnTargetSpotted?.Invoke(currentTarget);
        }
        else if (!isTargetCurrentlyVisible && wasTargetVisibleLastFrame)
        {
            OnTargetLost?.Invoke();
        }

        wasTargetVisibleLastFrame = isTargetCurrentlyVisible;
    }

    private bool CheckForTarget()
    {
        // 1. Ищем КОЛЛАЙДЕРЫ целей внутри нашего триггера
        Collider2D[] targetsInZone = new Collider2D[1]; // Ищем только одну цель для простоты
        ContactFilter2D filter = new ContactFilter2D().NoFilter();
        filter.SetLayerMask(targetLayer);

        int targetCount = visionTrigger.Overlap(filter, targetsInZone);

        if (targetCount == 0)
        {
            currentTarget = null;
            return false; // Если целей в зоне нет, выходим
        }

        // Берем первую найденную цель
        currentTarget = targetsInZone[0].transform;

        // 2. Если включена проверка на стены, делаем ее
        if (checkLineOfSight)
        {
            Vector2 directionToTarget = (currentTarget.position - transform.position).normalized;
            float distanceToTarget = Vector2.Distance(transform.position, currentTarget.position);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleLayer);

            if (hit.collider != null)
            {
                // Цель за стеной
                return false;
            }
        }

        // Если все проверки пройдены, цель видима
        return true;
    }
    
    public bool IsTargetVisible()
    {
        // Просто возвращаем состояние из прошлого кадра
        return wasTargetVisibleLastFrame;
    }   
}