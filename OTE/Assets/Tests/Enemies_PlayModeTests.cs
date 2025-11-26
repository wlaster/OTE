
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// Класс Enemies_PlayModeTests: Короткое описание.
/// </summary>
public class Enemies_PlayModeTests
{
    
    [UnityTest]
    public IEnumerator B05_EnemyHealth_ReducesHealth_OnTakeDamage()
    {
        
        var enemyPrefab = Resources.Load<GameObject>("Test_PatrolWalker"); 
        var enemyInstance = Object.Instantiate(enemyPrefab);
        var enemyHealth = enemyInstance.GetComponent<EnemyHealth>();
        float initialHealth = GetPrivateField<float>(enemyHealth, "currentHealth");

        
        enemyHealth.TakeDamage(20, Vector2.zero);
        yield return null;

        
        float finalHealth = GetPrivateField<float>(enemyHealth, "currentHealth");
        Assert.AreEqual(initialHealth - 20, finalHealth, "Здоровье врага должно было уменьшиться на 20.");

        
        Object.Destroy(enemyInstance);
    }

    
    [UnityTest]
    public IEnumerator B06_EnemyVision_SeesPlayer()
    {
        
        var enemyPrefab = Resources.Load<GameObject>("Test_Archer"); 
        var enemyInstance = Object.Instantiate(enemyPrefab, Vector3.zero, Quaternion.identity);
        var vision = enemyInstance.GetComponent<EnemyVision>();
        
        var playerPrefab = Resources.Load<GameObject>("Test_Player");
        var playerInstance = Object.Instantiate(playerPrefab, new Vector3(5, 0, 0), Quaternion.identity);

        
        yield return new WaitForSeconds(0.3f); 

        
        Assert.IsTrue(vision.CanSeePlayer, "Враг должен был увидеть игрока.");
        
        
        Object.Destroy(enemyInstance);
        Object.Destroy(playerInstance);
    }
    
    
    [UnityTest]
    public IEnumerator B07_Archer_Retreats_WhenPlayerIsClose()
    {
        
        var archerPrefab = Resources.Load<GameObject>("Test_Archer");
        var archerInstance = Object.Instantiate(archerPrefab, Vector3.zero, Quaternion.identity);
        var archerRb = archerInstance.GetComponent<Rigidbody2D>();
        
        var playerPrefab = Resources.Load<GameObject>("Test_Player");
        
        var playerInstance = Object.Instantiate(playerPrefab, new Vector3(1, 0, 0), Quaternion.identity);

        
        yield return new WaitForSeconds(0.3f); 
        yield return new WaitForFixedUpdate(); 

        
        Assert.Less(archerRb.linearVelocity.x, 0, "Лучник должен двигаться влево (отступать от игрока).");
        
        
        Object.Destroy(archerInstance);
        Object.Destroy(playerInstance);
    }

    
    [UnityTest]
    public IEnumerator B08_FlyingGuardian_MovesTowardsPlayer_WhenSeen()
    {
        
        var guardianPrefab = Resources.Load<GameObject>("Test_FlyingGuardian");
        var guardianInstance = Object.Instantiate(guardianPrefab, Vector3.zero, Quaternion.identity);
        var guardianRb = guardianInstance.GetComponent<Rigidbody2D>();

        var playerPrefab = Resources.Load<GameObject>("Test_Player");
        var playerInstance = Object.Instantiate(playerPrefab, new Vector3(10, 5, 0), Quaternion.identity);

        
        yield return new WaitForSeconds(0.3f); 
        yield return new WaitForFixedUpdate();

        
        Vector2 directionToPlayer = (playerInstance.transform.position - guardianInstance.transform.position).normalized;
        
        Assert.AreEqual(directionToPlayer.x, guardianRb.linearVelocity.normalized.x, 0.1f);
        Assert.AreEqual(directionToPlayer.y, guardianRb.linearVelocity.normalized.y, 0.1f);

        
        Object.Destroy(guardianInstance);
        Object.Destroy(playerInstance);
    }
    
    
    [UnityTest]
    public IEnumerator B09_GroundCrawler_Rotates_WhenGroundIsMissing()
    {
        
        var crawlerPrefab = Resources.Load<GameObject>("Test_GroundCrawler");
        
        var crawlerInstance = Object.Instantiate(crawlerPrefab, Vector3.zero, Quaternion.identity);
        Quaternion initialRotation = crawlerInstance.transform.rotation;

        
        yield return new WaitForSeconds(0.2f); 

        
        Assert.AreNotEqual(initialRotation, crawlerInstance.transform.rotation, "Враг должен был изменить вращение.");
        
        
        Object.Destroy(crawlerInstance);
    }

    
    [UnityTest]
    public IEnumerator B10_Guardian_Attacks_WhenPlayerInRange()
    {
        
        var guardianPrefab = Resources.Load<GameObject>("Test_Guardian");
        var guardianInstance = Object.Instantiate(guardianPrefab, Vector3.zero, Quaternion.identity);
        var animator = guardianInstance.GetComponent<Animator>();
        
        var playerPrefab = Resources.Load<GameObject>("Test_Player");
        
        var playerInstance = Object.Instantiate(playerPrefab, new Vector3(1, 0, 0), Quaternion.identity);

        
        yield return new WaitForSeconds(0.3f); 

        
        Assert.IsTrue(animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"), 
            "Страж должен был перейти в состояние атаки. Проверьте тег 'Attack' в его Animator Controller.");
        
        
        Object.Destroy(guardianInstance);
        Object.Destroy(playerInstance);
    }

    
    [UnityTest]
    public IEnumerator B11_PatrolWalker_FlipsAtWall()
    {
        
        var walkerPrefab = Resources.Load<GameObject>("Test_PatrolWalker");
        Assert.IsNotNull(walkerPrefab, "Префаб 'Test_PatrolWalker' не найден.");
        var walkerInstance = Object.Instantiate(walkerPrefab, Vector3.zero, Quaternion.identity);
        var walker = walkerInstance.GetComponent<PatrolWalker>();
        var walkerRb = walkerInstance.GetComponent<Rigidbody2D>();
        
        
        SetPrivateField(walker, "moveSpeed", 2f);
        bool initialFacing = GetPrivateField<bool>(walker, "isFacingRight");
        Assert.IsTrue(initialFacing, "Для этого теста враг должен изначально смотреть вправо.");

        
        var wall = new GameObject("Wall");
        int groundLayer = LayerMask.NameToLayer("Obstacle");
        if(groundLayer == -1) Assert.Ignore("Слой 'Obstacle' не настроен.");
        wall.layer = groundLayer;
        wall.transform.position = new Vector3(1, 0, 0); 
        var wallCollider = wall.AddComponent<BoxCollider2D>();
        
        
        wallCollider.isTrigger = true;
        

        
        var ground = new GameObject("Obstacle");
        ground.layer = groundLayer;
        ground.transform.position = new Vector3(0, -1, 0);
        ground.transform.localScale = new Vector3(5, 1, 1);
        ground.AddComponent<BoxCollider2D>();

        
        
        yield return new WaitForSeconds(0.6f); 
        
        
        bool finalFacing = GetPrivateField<bool>(walker, "isFacingRight");
        Assert.AreNotEqual(initialFacing, finalFacing, "Патрульный должен был развернуться у стены.");
        Assert.Less(walkerRb.linearVelocity.x, 0, "После разворота скорость по X должна быть отрицательной.");
        
        
        Object.Destroy(walkerInstance);
        Object.Destroy(wall);
        Object.Destroy(ground);
    }

    
    [UnityTest]
    public IEnumerator B12_SineWaveFlyer_MovesInSineWave()
    {
        
        var flyerPrefab = Resources.Load<GameObject>("Test_SineWaveFlyer");
        if (flyerPrefab == null) Assert.Fail("Префаб 'Test_SineWaveFlyer' не найден в Resources.");
        
        
        flyerPrefab.SetActive(false);
        var flyerInstance = Object.Instantiate(flyerPrefab, Vector3.zero, Quaternion.identity);
        var flyerScript = flyerInstance.GetComponent<SineWaveFlyer>();
        
        
        var playerInstance = new GameObject("TestPlayer");
        
        
        SetPrivateField(flyerScript, "playerTransform", playerInstance.transform);

        
        flyerInstance.SetActive(true);
        flyerPrefab.SetActive(true); 
        

        float initialY = flyerInstance.transform.position.y;
        
        
        yield return new WaitForSeconds(0.5f);

        
        Assert.AreNotEqual(initialY, flyerInstance.transform.position.y, "Позиция Y должна была измениться из-за движения по синусоиде.");

        
        Object.Destroy(flyerInstance);
        Object.Destroy(playerInstance);
    }

    
    private T GetPrivateField<T>(object obj, string fieldName)
    {
        var field = obj.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return (T)field?.GetValue(obj);
    }
    
    
    private void SetPrivateField<T>(object obj, string fieldName, T value)
    {
        var field = obj.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(obj, value);
    }
}