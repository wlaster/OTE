using UnityEngine;

[RequireComponent(typeof(PlayerMovement), typeof(PlayerAttack))]
public class PlayerController : MonoBehaviour
{
    // Ссылки на компоненты-исполнители
    private PlayerMovement playerMovement;
    private PlayerAttack playerAttack;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerAttack = GetComponent<PlayerAttack>();
    }

    private void Update()
    {

        // Если игра на паузе, мы ничего не делаем.
        if (PauseMenuManager.IsGamePaused)
        {
        return;
        }
        
        // --- ДВИЖЕНИЕ ---
        float moveDirection = Input.GetAxisRaw("Horizontal");
        playerMovement.SetDirectionalInput(moveDirection);

        // --- ПРЫЖОК ---
        if (Input.GetButtonDown("Jump"))
        {
            playerMovement.Jump();
        }

        // --- АТАКА ---
        if (Input.GetButtonDown("Fire1"))
        {
            playerAttack.PerformAttack();
        }
    }
}