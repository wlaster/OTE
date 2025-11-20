using UnityEngine;
using UnityEngine.UI;
using TMPro; // Обязательно для работы с TextMeshPro

public class HealthBar : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Компонент Image, который является заполняемой полоской.")]
    [SerializeField] private Image healthBarFill;
    [Tooltip("Компонент TextMeshPro для отображения числового значения здоровья.")]
    [SerializeField] private TextMeshProUGUI hpText;

    /// <summary>
    /// Публичный метод для обновления полоски здоровья.
    /// </summary>
    /// <param name="currentHealth">Текущее количество здоровья.</param>
    /// <param name="maxHealth">Максимальное количество здоровья.</param>
    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        if (healthBarFill != null)
        {
            // Рассчитываем долю здоровья (значение от 0 до 1)
            float fillAmount = currentHealth / maxHealth;
            healthBarFill.fillAmount = fillAmount;
        }

        if (hpText != null)
        {
            // Обновляем текст, округляя значения до целых чисел
            hpText.text = $"{Mathf.CeilToInt(currentHealth)} / {Mathf.CeilToInt(maxHealth)}";
        }
    }
}