
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// Класс Player_PlayModeTests: Короткое описание.
/// </summary>
public class Player_PlayModeTests
{
    private GameObject playerInstance;
    private PlayerMovement playerMovement;
    private PlayerHealth playerHealth;
    private PlayerAttack playerAttack;
    private Rigidbody2D playerRb;
    private Animator playerAnimator;

    
    [SetUp]
    public void Setup()
    {
        var playerPrefab = Resources.Load<GameObject>("Test_Player");
        if (playerPrefab == null)
        {
            Assert.Fail("Префаб 'Test_Player' не найден в папке 'Assets/Resources'.");
        }
        
        playerInstance = Object.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        
        playerMovement = playerInstance.GetComponent<PlayerMovement>();
        playerHealth = playerInstance.GetComponent<PlayerHealth>();
        playerAttack = playerInstance.GetComponent<PlayerAttack>();
        playerRb = playerInstance.GetComponent<Rigidbody2D>();
        playerAnimator = playerInstance.GetComponent<Animator>();
    }

    
    [TearDown]
    public void Teardown()
    {
        Object.Destroy(playerInstance);
    }
    
    
    [UnityTest]
    public IEnumerator B01_And_B04_PlayerAttack_TriggersAttackAnimation()
    {
        
        Assert.IsNotNull(playerAnimator, "У игрока должен быть компонент Animator.");
        Assert.IsNotNull(playerAnimator.runtimeAnimatorController, "У Animator'а должен быть назначен Animator Controller в префабе.");

        
        playerAttack.PerformAttack();
        
        
        float timeout = 2f; 
        float timer = 0f;
        bool attackStateReached = false;

        while (timer < timeout)
        {
            
            if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
            {
                attackStateReached = true;
                break; 
            }
            
            timer += Time.deltaTime;
            yield return null; 
        }
        

        
        Assert.IsTrue(attackStateReached, 
            "Аниматор не перешел в состояние с тегом 'Attack' за " + timeout + " сек. " +
            "Проверьте настройки Animator Controller: наличие состояния с тегом 'Attack' и переход в него по триггеру 'attack'.");
    }
    
    [UnityTest]
    public IEnumerator B02_PlayerMovement_MovesWhenInputIsSet()
    {
        
        var playerController = playerInstance.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }
        
        playerMovement.SetDirectionalInput(-1.0f);

        
        yield return new WaitForFixedUpdate();

        
        Assert.Less(playerRb.linearVelocity.x, 0f, "Скорость по оси X должна быть отрицательной (движение влево).");
    }

    
    
    [Test]
    public void B03_PlayerHealth_ReducesHealth_OnTakeDamage()
    {
        
        float initialHealth = playerHealth.GetCurrentHealth();
        
        
        playerHealth.TakeDamage(20, Vector2.zero);

        
        Assert.AreEqual(initialHealth - 20, playerHealth.GetCurrentHealth(), "Здоровье должно было уменьшиться на 20.");
    }
}