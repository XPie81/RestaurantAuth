using System.Collections.Generic;

namespace RestaurantAuth.Models
{
    /// <summary>
    /// Базовый класс для всех людей в системе
    /// </summary>
    public class Person : IDomainObject
    {
        public int Id { get; set; }
        
        /// <summary>
        /// Имя человека
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Номер телефона
        /// </summary>
        public string Phone { get; set; }
        
        /// <summary>
        /// Список ID заказов
        /// </summary>
        public List<int> OrderIds { get; set; } = new List<int>();
    }
} 