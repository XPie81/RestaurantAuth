namespace RestaurantAuth.Models
{
    /// <summary>
    /// Интерфейс для сотрудников
    /// </summary>
    public interface IEmployee
    {
        /// <summary>
        /// Логин сотрудника
        /// </summary>
        string Login { get; set; }
        
        /// <summary>
        /// Пароль сотрудника (зашифрованный)
        /// </summary>
        string Password { get; set; }
        
        /// <summary>
        /// Метрика эффективности (время просрочки/перевыполнения в минутах)
        /// </summary>
        int EfficiencyMetric { get; set; }
    }
} 