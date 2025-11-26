using UnityEngine;
using UnityEngine.UI;
using TMPro; 

/// <summary>
/// Обновляет визуальное отображение здоровья игрока: заливку и текстовое значение.
/// </summary>
public class HealthBar : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Компонент Image, который является заполняемой полоской.")]
    [SerializeField] private Image healthBarFill;
    [Tooltip("Компонент TextMeshPro для отображения числового значения здоровья.")]
    [SerializeField] private TextMeshProUGUI hpText;

    
    
    
    
    
    /// <summary>
    /// Обновляет полоску и текст здоровья в соответствии с текущим и максимальным значением.
    /// </summary>
    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        if (healthBarFill != null)
        {
            float fillAmount = currentHealth / maxHealth;
            healthBarFill.fillAmount = fillAmount;
        }

        if (hpText != null)
        {
            hpText.text = $"{Mathf.CeilToInt(currentHealth)} / {Mathf.CeilToInt(maxHealth)}";
        }
    }
    
}