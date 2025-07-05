using UnityEngine;

// Убеждаемся, что все нужные компоненты есть на объекте
[RequireComponent(typeof(PlayerMovement), typeof(PlayerAttack))]
public class PlayerController : MonoBehaviour
{
    // Ссылки на компоненты-исполнители
    private PlayerMovement playerMovement;
    private PlayerAttack playerAttack; // Добавили ссылку

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerAttack = GetComponent<PlayerAttack>(); // Получили компонент
    }

    private void Update()
    {
        // --- ДВИЖЕНИЕ ---
        float moveDirection = Input.GetAxisRaw("Horizontal");
        playerMovement.SetDirectionalInput(moveDirection);

        // --- ПРЫЖОК ---
        if (Input.GetButtonDown("Jump"))
        {
            playerMovement.Jump();
        }

        // --- АТАКА ---
        if (Input.GetButtonDown("Fire1")) // Fire1 - это левая кнопка мыши
        {
            playerAttack.PerformAttack(); // Отдаем команду на атаку
        }
    }
}