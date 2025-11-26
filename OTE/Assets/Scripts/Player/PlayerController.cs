using UnityEngine;

[RequireComponent(typeof(PlayerMovement), typeof(PlayerAttack))]
/// <summary>
/// Координует ввод игрока: передаёт ввод в `PlayerMovement` и команды атаки в `PlayerAttack`.
/// </summary>
public class PlayerController : MonoBehaviour
{
    
    private PlayerMovement playerMovement;
    private PlayerAttack playerAttack;

    /// <summary>
    /// Инициализация: получает ссылки на компоненты движения и атаки.
    /// </summary>
    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerAttack = GetComponent<PlayerAttack>();
    }

    /// <summary>
    /// Обрабатывает ввод игрока: движение, прыжок и атаки; пропускает обработку если игра на паузе.
    /// </summary>
    private void Update()
    {
        if (PauseMenuManager.IsGamePaused)
        {
            return;
        }

        float moveDirection = Input.GetAxisRaw("Horizontal");
        playerMovement.SetDirectionalInput(moveDirection);

        if (Input.GetButtonDown("Jump"))
        {
            playerMovement.Jump();
        }

        if (Input.GetButtonDown("Fire1"))
        {
            playerAttack.PerformAttack();
        }
    }
}