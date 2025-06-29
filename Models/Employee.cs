using RestaurantAuth.Models.Enums;

namespace RestaurantAuth.Models
{
    /// <summary>
    /// Базовый класс для всех сотрудников
    /// </summary>
    public class Employee : Person, IEmployee
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public int EfficiencyMetric { get; set; }
        
        /// <summary>
        /// Тип сотрудника
        /// </summary>
        public EmployeeType EmployeeType { get; set; }
    }
} 