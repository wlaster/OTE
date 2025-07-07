// Этот интерфейс может быть реализован любым объектом, который может получать урон
public interface IDamageable
{
    // Любой, кто реализует этот интерфейс, ОБЯЗАН иметь публичный метод TakeDamage
    void TakeDamage(float damage);
}