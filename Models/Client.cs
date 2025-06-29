using RestaurantAuth.Models;

namespace RestaurantAuth.Models
{
    /// <summary>
    /// Класс для клиентов ресторана
    /// </summary>
    public class Client : Person
    {
        /// <summary>
        /// Адрес клиента
        /// </summary>
        public string Address { get; set; }
        
        /// <summary>
        /// Улица
        /// </summary>
        public string Street { get; set; }
        
        /// <summary>
        /// Дом
        /// </summary>
        public string House { get; set; }
        
        /// <summary>
        /// Квартира
        /// </summary>
        public string Apartment { get; set; }
    }
} 