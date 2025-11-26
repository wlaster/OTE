
using UnityEngine;

/// <summary>
/// Интерфейс для объектов, которые могут получать урон.
/// </summary>
public interface IDamageable
{
    /// <summary>
    /// Вызывается при получении урона.
    /// </summary>
    /// <param name="damage">Сумма урона.</param>
    /// <param name="knockbackSourcePosition">Позиция источника удара для расчёта отбрасывания.</param>
    void TakeDamage(float damage, Vector2 knockbackSourcePosition);
}