
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// Класс EnemyCombat_PlayModeTests: Короткое описание.
/// </summary>
public class EnemyCombat_PlayModeTests
{
    
    [UnityTest]
    public IEnumerator B13_Arrow_InitializesWithVelocityTowardsTarget()
    {
        
        
        var arrowPrefab = Resources.Load<GameObject>("Arrow"); 
        Assert.IsNotNull(arrowPrefab, "Префаб 'Arrow' не найден в Assets/Resources.");
        
        var arrowInstance = Object.Instantiate(arrowPrefab, Vector3.zero, Quaternion.identity);
        var arrowScript = arrowInstance.GetComponent<Arrow>();
        var arrowRb = arrowInstance.GetComponent<Rigidbody2D>();

        
        var target = new GameObject("Target").transform;
        target.position = new Vector3(10, 5, 0);

        
        arrowScript.Initialize(target);
        yield return new WaitForFixedUpdate(); 

        
        Assert.Greater(arrowRb.linearVelocity.magnitude, 0, "Стрела должна иметь скорость после инициализации.");
        
        Vector2 directionToTarget = (target.position - arrowInstance.transform.position).normalized;
        Assert.AreEqual(directionToTarget.x, arrowRb.linearVelocity.normalized.x, 0.1f, "Направление по X неверное.");
        Assert.AreEqual(directionToTarget.y, arrowRb.linearVelocity.normalized.y, 0.1f, "Направление по Y неверное.");

        
        Object.Destroy(arrowInstance);
        Object.Destroy(target.gameObject);
    }

    [UnityTest]
    public IEnumerator B14_EnemyMeleeAttack_DealDamage_HitsPlayer()
    {
        
        var enemyPrefab = Resources.Load<GameObject>("Test_Guardian"); 
        Assert.IsNotNull(enemyPrefab, "Префаб 'Test_Guardian' не найден.");
        var enemyInstance = Object.Instantiate(enemyPrefab, Vector3.zero, Quaternion.identity);
        var enemyAttack = enemyInstance.GetComponent<EnemyMeleeAttack>();
        Assert.IsNotNull(enemyAttack, "У префаба врага нет компонента EnemyMeleeAttack.");

        
        var attackPoint = GetPrivateField<Transform>(enemyAttack, "attackPoint");
        Assert.IsNotNull(attackPoint, "У префаба врага в компоненте EnemyMeleeAttack не назначен 'attackPoint'.");
        Vector3 attackPosition = attackPoint.position;

        var playerPrefab = Resources.Load<GameObject>("Test_Player");
        Assert.IsNotNull(playerPrefab, "Префаб 'Test_Player' не найден.");
        
        
        var playerInstance = Object.Instantiate(playerPrefab, attackPosition, Quaternion.identity);
        var playerHealth = playerInstance.GetComponent<PlayerHealth>();
        float initialPlayerHealth = playerHealth.GetCurrentHealth();
        
        
        int playerLayer = LayerMask.NameToLayer("Player");
        Assert.AreNotEqual(-1, playerLayer, "Слой 'Player' не настроен!");
        Assert.AreEqual(playerLayer, playerInstance.layer, "У префаба игрока не установлен слой 'Player'.");
        LayerMask hittableLayers = GetPrivateField<LayerMask>(enemyAttack, "hittableLayers");
        Assert.IsTrue((hittableLayers.value & (1 << playerLayer)) > 0, "В EnemyMeleeAttack не выбран слой 'Player'.");

        
        enemyAttack.DealDamage();
        yield return null;

        
        Assert.Less(playerHealth.GetCurrentHealth(), initialPlayerHealth, "Здоровье игрока должно было уменьшиться.");

        
        Object.Destroy(enemyInstance);
        Object.Destroy(playerInstance);
    }

    
    [UnityTest]
    public IEnumerator B15_TouchDamage_DealsDamage_OnTriggerStay()
    {
        
        var enemyPrefab = Resources.Load<GameObject>("Test_PatrolWalker");
        Assert.IsNotNull(enemyPrefab, "Префаб 'Test_PatrolWalker' не найден.");
        var enemyInstance = Object.Instantiate(enemyPrefab, Vector3.zero, Quaternion.identity);
        var touchDamage = enemyInstance.GetComponent<TouchDamage>();

        
        
        var lastDamageTimeField = typeof(TouchDamage).GetField("lastDamageTime", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        lastDamageTimeField.SetValue(touchDamage, -100f);
        
        var enemyCollider = enemyInstance.GetComponent<Collider2D>();
        enemyCollider.isTrigger = true; 
        if (enemyInstance.GetComponent<Rigidbody2D>() == null)
            enemyInstance.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

        var playerPrefab = Resources.Load<GameObject>("Test_Player");
        Assert.IsNotNull(playerPrefab, "Префаб 'Test_Player' не найден.");
        var playerInstance = Object.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        var playerHealth = playerInstance.GetComponent<PlayerHealth>();
        if (playerInstance.GetComponent<Rigidbody2D>() == null)
            playerInstance.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

        float initialPlayerHealth = playerHealth.GetCurrentHealth();

        
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        
        Assert.Less(playerHealth.GetCurrentHealth(), initialPlayerHealth, "Игрок должен был получить урон от прикосновения.");

        
        Object.Destroy(enemyInstance);
        Object.Destroy(playerInstance);
    }


    
    private T GetPrivateField<T>(object obj, string fieldName)
    {
        var field = obj.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field == null) Assert.Fail($"Приватное поле '{fieldName}' не найдено в классе '{obj.GetType().Name}'.");
        return (T)field.GetValue(obj);
    }
}