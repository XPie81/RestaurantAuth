namespace RestaurantAuth.Models.Enums
{
    /// <summary>
    /// Тип пользователя в системе авторизации
    /// </summary>
    public enum UserType
    {
        /// <summary>
        /// Сотрудник (официант, повар, курьер, администратор)
        /// </summary>
        Employee,
        
        /// <summary>
        /// Клиент
        /// </summary>
        Client
    }
} 